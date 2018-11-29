using System;
using System.IO;

using Consul.Configuration.Provider;
using Consul.Configuration.Provider.Extensions;

using Microsoft.Extensions.Configuration;

using Xunit;

namespace Consul.Configuration.Provider.Tests
{
    public class ConsulConfigurationProviderTests
    {
        [Fact]
        public void with_validjson_testconsulprovider_readkey_properly()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                                                .SetBasePath(Directory.GetCurrentDirectory())
                                                .AddConsulJsonFile("appsettings-c1.json");

            IConfigurationRoot configuration = builder.Build();
            
            Assert.True(configuration["MySettingsInConsul:MyKey"] == "{$MyKeyAtConsul}");
            Assert.Matches("(\\{\\$.*?\\})", configuration["MySettingsInConsul:MyKey"]);
        }
    }
}
