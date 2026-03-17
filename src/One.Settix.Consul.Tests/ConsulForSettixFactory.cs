using System;
using Microsoft.Extensions.Configuration;
using One.Settix.Box;

namespace One.Settix.Consul.Tests
{
    public class ConsulForSettixFactory : ConsulForSettix
    {
        public const string GlobalApplicationEnvironmentVariable = "settix_global_application";

        public static ConsulForSettix Create()
        {
            IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

            string consulUrl = config["consulHost"];

            return new ConsulForSettix(new Uri(consulUrl));
        }

        public static ConsulForSettix CreateWithGlobalContext(string globalApplication = "one.settix.consul.tests.global")
        {
            Environment.SetEnvironmentVariable(GlobalApplicationEnvironmentVariable, globalApplication);
            return Create();
        }
    }

    public static class TestConsulSettixKeyExtensions
    {
        public static string CreateSettixRawKey(this string settingKey)
        {
            return NameBuilder.GetSettingName("One.Settix.Consul.Tests", "test", Box.Machine.NotSpecified, settingKey);
        }
    }

    public class TestContext : ApplicationContext
    {
        public TestContext() : base("One.Settix.Consul.Tests")
        {

        }
    }
}
