using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using One.Settix.Consul.Consul;

namespace One.Settix
{
    internal class ConsulRefresher : ISettixWatcher
    {
        private readonly Settix settix;
        private readonly ConsulClient consul;
        private readonly TimeSpan refreshInterval;
        private readonly Task getTask;

        private IChangeToken changeToken;
        private CancellationTokenSource consulApplicationConfigurationTokenSource;
        private CancellationTokenSource consulGlobalConfigurationTokenSource;

        public ConsulRefresher(Settix settix, ConsulClient consul, TimeSpan refreshInterval)
        {
            this.settix = settix;
            this.consul = consul;
            this.refreshInterval = refreshInterval;
            getTask = Task.Factory.StartNew(RefreshAsync);
        }

        ulong consulApplicationIndex = 0;
        ulong consulGlobalIndex = 0;

        public IChangeToken Watch()
        {
            consulApplicationConfigurationTokenSource = new CancellationTokenSource();
            consulGlobalConfigurationTokenSource = new CancellationTokenSource();

            CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(consulApplicationConfigurationTokenSource.Token, consulGlobalConfigurationTokenSource.Token);

            changeToken = new CancellationChangeToken(linkedCts.Token);

            return changeToken;
        }

        public void Dispose()
        {
            getTask?.Dispose();
            consulApplicationConfigurationTokenSource?.Dispose();
            consulGlobalConfigurationTokenSource?.Dispose();
        }

        private async Task RefreshAsync()
        {
            while (true)
            {
                try
                {
                    if (consulApplicationIndex == 0)
                        consulApplicationIndex = await GetApplicationConsulIndexAsync().ConfigureAwait(false);

                    ulong theApplicationIndex = await GetApplicationConsulIndexAsync().ConfigureAwait(false);
                    if (consulApplicationIndex != theApplicationIndex)
                    {
                        consulApplicationIndex = theApplicationIndex;
                        consulApplicationConfigurationTokenSource?.Cancel();
                    }
                    if (settix.GlobalContext is not null)
                    {
                        await RefreshGlobalAsync().ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"There was an error while getting configuration from consul. Retrying in 10 seconds...{Environment.NewLine}{ex.Message}");
                    consulApplicationIndex = 0;
                    consulGlobalIndex = 0;
                    await Task.Delay(10_000).ConfigureAwait(false);
                }
            }
        }

        private async Task RefreshGlobalAsync()
        {
            if (consulGlobalIndex == 0)
                consulGlobalIndex = await GetGlobalConsulIndexAsync().ConfigureAwait(false);

            ulong theGlobalIndex = await GetGlobalConsulIndexAsync().ConfigureAwait(false);
            if (consulGlobalIndex != theGlobalIndex)
            {
                consulGlobalIndex = theGlobalIndex;
                consulGlobalConfigurationTokenSource?.Cancel();
            }
        }

        private async Task<ulong> GetApplicationConsulIndexAsync()
        {
            string settixApplication = settix.ApplicationContext.ToApplicationKeyPrefix();
            var response = await consul.ReadAllKeyValuesAndMonitorAsync(settixApplication, refreshInterval, consulApplicationIndex).ConfigureAwait(false);

            return response.lastIndex;
        }

        private async Task<ulong> GetGlobalConsulIndexAsync()
        {
            string settixApplication = settix.GlobalContext.ToApplicationKeyPrefix();
            var response = await consul.ReadAllKeyValuesAndMonitorAsync(settixApplication, refreshInterval, consulGlobalIndex).ConfigureAwait(false);

            return response.lastIndex;
        }
    }
}
