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
        private readonly Task getGlobalTask;

        private IChangeToken changeToken;
        private CancellationTokenSource consulApplicationConfigurationTokenSource;
        private CancellationTokenSource consulGlobalConfigurationTokenSource;
        private CancellationTokenSource linkedCts;

        public ConsulRefresher(Settix settix, ConsulClient consul, TimeSpan refreshInterval)
        {
            this.settix = settix;
            this.consul = consul;
            this.refreshInterval = refreshInterval;
            getTask = Task.Factory.StartNew(RefreshAsync);
            getGlobalTask = Task.Factory.StartNew(RefreshGlobalAsync);
        }

        public IChangeToken Watch()
        {
            consulApplicationConfigurationTokenSource = new CancellationTokenSource();
            consulGlobalConfigurationTokenSource = new CancellationTokenSource();

            linkedCts = CancellationTokenSource.CreateLinkedTokenSource(consulApplicationConfigurationTokenSource.Token, consulGlobalConfigurationTokenSource.Token);

            changeToken = new CancellationChangeToken(linkedCts.Token);

            return changeToken;
        }

        public void Dispose()
        {
            getTask?.Dispose();
            getGlobalTask?.Dispose();
            consulApplicationConfigurationTokenSource?.Dispose();
            consulGlobalConfigurationTokenSource?.Dispose();
            linkedCts?.Dispose();
        }

        private Task RefreshAsync()
        {
            string prefix = settix.ApplicationContext.ToApplicationKeyPrefix();
            return WatchPrefixAsync(prefix, () => consulApplicationConfigurationTokenSource);
        }

        private Task RefreshGlobalAsync()
        {
            if (settix.GlobalContext is null)
                return Task.CompletedTask;

            string prefix = settix.GlobalContext.ToApplicationKeyPrefix();
            return WatchPrefixAsync(prefix, () => consulGlobalConfigurationTokenSource);
        }

        private async Task WatchPrefixAsync(string keyPrefix, Func<CancellationTokenSource> getTokenSource)
        {
            ulong lastIndex = 0;
            bool reloadOnRecovery = false;

            while (true)
            {
                try
                {
                    if (lastIndex == 0)
                    {
                        lastIndex = await GetConsulIndexAsync(keyPrefix, lastIndex).ConfigureAwait(false);
                        if (reloadOnRecovery)
                        {
                            getTokenSource()?.Cancel();
                            reloadOnRecovery = false;
                        }
                    }

                    ulong nextIndex = await GetConsulIndexAsync(keyPrefix, lastIndex).ConfigureAwait(false);
                    if (lastIndex != nextIndex)
                    {
                        lastIndex = nextIndex;
                        getTokenSource()?.Cancel();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"There was an error while getting configuration from consul. Retrying in 10 seconds...{Environment.NewLine}{ex.Message}");
                    lastIndex = 0;
                    reloadOnRecovery = true;
                    await Task.Delay(10_000).ConfigureAwait(false);
                }
            }
        }

        private async Task<ulong> GetConsulIndexAsync(string keyPrefix, ulong lastIndex)
        {
            var response = await consul.ReadAllKeyValuesAndMonitorAsync(keyPrefix, refreshInterval, lastIndex).ConfigureAwait(false);

            return response.lastIndex;
        }
    }
}
