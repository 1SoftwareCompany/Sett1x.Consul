using System;
using System.Threading;
using System.Threading.Tasks;
using One.Settix.Consul.Consul;
using Microsoft.Extensions.Primitives;

namespace One.Settix
{
    internal class ConsulRefresher : ISettixWatcher
    {
        private readonly Settix settix;
        private readonly ConsulClient consul;
        private readonly TimeSpan refreshInterval;
        private IChangeToken changeToken;
        private CancellationTokenSource consulConfigurationTokenSource;
        private readonly Task getTask;

        public ConsulRefresher(Settix settix, ConsulClient consul, TimeSpan refreshInterval)
        {
            this.settix = settix;
            this.consul = consul;
            this.refreshInterval = refreshInterval;
            getTask = Task.Factory.StartNew(RefreshAsync);
        }

        ulong consulIndex = 0;

        private async Task RefreshAsync()
        {
            while (true)
            {
                try
                {
                    if (consulIndex == 0)
                        consulIndex = await GetConsulIndexAsync().ConfigureAwait(false);

                    var theIndex = await GetConsulIndexAsync().ConfigureAwait(false);

                    if (consulIndex != theIndex)
                    {
                        consulIndex = theIndex;
                        consulConfigurationTokenSource?.Cancel();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"There was an error while getting configuration from consul. Retrying in 10 seconds...{Environment.NewLine}{ex.Message}");
                    consulIndex = 0;
                    await Task.Delay(10_000).ConfigureAwait(false);
                }
            }
        }

        private async Task<ulong> GetConsulIndexAsync()
        {
            string settixApplication = settix.ApplicationContext.ToApplicationKeyPrefix();
            var response = await consul.ReadAllKeyValuesAndMonitorAsync(settixApplication, refreshInterval, consulIndex).ConfigureAwait(false);

            return response.lastIndex;
        }

        public IChangeToken Watch()
        {
            consulConfigurationTokenSource = new CancellationTokenSource();
            changeToken = new CancellationChangeToken(consulConfigurationTokenSource.Token);

            return changeToken;
        }

        public void Dispose()
        {
            getTask?.Dispose();
            consulConfigurationTokenSource?.Dispose();
        }
    }
}
