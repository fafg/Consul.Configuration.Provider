// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//https://github.com/aspnet/Configuration/blob/master/src/Config.Json/JsonConfigurationProvider.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Consul.Configuration.Gateway;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Consul.Configuration.Provider
{
    /// <summary>
    /// A Consul configuration based on JSON file based <see cref="JsonConfigurationProvider"/>.
    /// </summary>
    public class ConsulConfigurationProvider : FileConfigurationProvider
    {
        private string _consulUri;
        private string _consulDatacenter;
        private string _consulToken;
        private string _variablePattern = "(\\{\\$.*?\\})"; 
        private Regex _regex;

        public IConsulGateway ConsulGateway { get; set; }

        /// <summary>
        /// Initializes a new instance with the specified source.
        /// </summary>
        /// <param name="source">The source settings.</param>
        public ConsulConfigurationProvider(ConsulConfigurationSource source, string uri = "", 
            string dataCenter = "", string token = "", IConsulGateway consulGatewayInstance = null) : base(source) 
        {
            _consulUri = uri;
            _consulDatacenter = dataCenter;
            _consulToken = token;
            
            _regex = new Regex(_variablePattern, RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.Singleline);

            ConsulGateway = consulGatewayInstance;
        }

        /// <summary>
        /// Loads the JSON data from a stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        public override void Load(Stream stream)
        {
            try
            {
                Data = JsonConfigurationFileParser.Parse(stream);

                CheckOrLoadConsulParameters();
                Task.Run(() => LoadDataFromConsul());
            }
            catch (JsonReaderException e)
            {
                string errorLine = string.Empty;
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    IEnumerable<string> fileContent;
                    using (var streamReader = new StreamReader(stream))
                    {
                        fileContent = ReadLines(streamReader);
                        errorLine = RetrieveErrorContext(e, fileContent);
                    }
                }

                throw new FormatException($"Json parser error. Line-Number: {e.LineNumber}, Error-Line: {errorLine}");
            }
        }

        private static string RetrieveErrorContext(JsonReaderException e, IEnumerable<string> fileContent)
        {
            string errorLine = null;
            if (e.LineNumber >= 2)
            {
                var errorContext = fileContent.Skip(e.LineNumber - 2).Take(2).ToList();
                // Handle situations when the line number reported is out of bounds
                if (errorContext.Count() >= 2)
                {
                    errorLine = errorContext[0].Trim() + Environment.NewLine + errorContext[1].Trim();
                }
            }
            if (string.IsNullOrEmpty(errorLine))
            {
                var possibleLineContent = fileContent.Skip(e.LineNumber - 1).FirstOrDefault();
                errorLine = possibleLineContent ?? string.Empty;
            }
            return errorLine;
        }

        private static IEnumerable<string> ReadLines(StreamReader streamReader)
        {
            string line;
            do
            {
                line = streamReader.ReadLine();
                yield return line;
            } while (line != null);
        }

        private void CheckOrLoadConsulParameters()
        {
            if (string.IsNullOrWhiteSpace(_consulUri))
                _consulUri = Data["ConsulConfigurationProvider:Uri"];

            if (string.IsNullOrWhiteSpace(_consulDatacenter))
                _consulUri = Data["ConsulConfigurationProvider:DataCenter"];

            if (string.IsNullOrWhiteSpace(_consulToken))
                _consulUri = Data["ConsulConfigurationProvider:Token"];
        }

        private async Task LoadDataFromConsul()
        {
            if (ConsulGateway == null)
                ConsulGateway = new ConsulGateway();

            ConsulGateway.Init(_consulUri, _consulDatacenter, _consulToken);

            string key = string.Empty;
            Match match = null;
            foreach (string item in Data.Keys)
            {
                match = _regex.Match(item);
                if (match.Success)
                {
                    key = item.Substring(2, item.Length - 3);
                    if (!string.IsNullOrWhiteSpace(key))
                        Data[item] = await ConsulGateway.GetValueAsync(key);
                }
            }

            ConsulGateway.Destroy();
        }
    }
}