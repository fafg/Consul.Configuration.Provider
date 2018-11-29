// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

//https://github.com/aspnet/Configuration/blob/master/src/Config.Json/JsonConfigurationSource.cs

using System;
using Microsoft.Extensions.Configuration;

namespace Consul.Configuration.Provider
{
    /// <summary>
    /// Represents a JSON file as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class ConsulConfigurationSource : FileConfigurationSource
    {
        public ConsulConfigurationSource()
        {
        }

        /// <summary>
        /// Builds the <see cref="ConsulConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ConsulConfigurationProvider"/></returns>
        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);

            return new ConsulConfigurationProvider(this);
        }

        /// <summary>
        /// Builds the <see cref="ConsulConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="ConsulConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder, string uri = "", string dataCenter = "", string token = "")
        {
            EnsureDefaults(builder);

            return new ConsulConfigurationProvider(this, uri, dataCenter, token);
        }
    }
}