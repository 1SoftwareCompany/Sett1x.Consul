using System;
using System.Threading.Tasks;
using One.Settix.Consul.Consul;
using Microsoft.Extensions.Configuration;

namespace One.Settix
{
    public class SettixConsulConfigurationSource : SettixConfigurationSource
    {
        private const string ConsulDefaultAddress = "http://consul.local.com:8500";

        private readonly Uri consulHost;

        /// <summary>
        /// Initializes SettixConsulConfigurationSource
        /// </summary>
        /// <param name="consulHost">The consul host. Ex: http://consul.local.com:8500</param>
        public SettixConsulConfigurationSource(string consulHost = null)
        {
            this.consulHost = new Uri(consulHost ?? ConsulDefaultAddress);
            ReloadDelay = TimeSpan.FromMinutes(5);
        }

        public override ISettixWatcher ReloadWatcher { get; set; }

        public override IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            ISettixContext context = new ApplicationContext();

            ConsulForSettix repository = new ConsulForSettix(consulHost);
            Settix = new Settix(context, repository);
            ReloadWatcher = new ConsulRefresher(Settix, new ConsulClient(consulHost), ReloadDelay);
            ChangeTokenConsumer = (provider) => Task.Factory.StartNew(() => provider.Load());

            return new SettixConfigurationProvider(this);
        }
    }
}
