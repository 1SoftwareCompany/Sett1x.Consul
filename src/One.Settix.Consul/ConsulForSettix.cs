using One.Settix.Consul.Consul;
using One.Settix.Consul.Consul.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace One.Settix
{
    public class ConsulForSettix : IConfigurationRepository, IGlobalConfigurationRepository
    {
        public const string RootFolder = "settix";

        private readonly ConsulClient _client;
        public ISettixContext GlobalContext { get; private set; }

        public ConsulForSettix(Uri address = null)
        {
            _client = new ConsulClient(address);

            string globalApp = EnvVar.GetGlobalApplication();
            if (string.IsNullOrEmpty(globalApp) == false)
                GlobalContext = new GlobalContext(globalApp);
        }

        public Task<bool> ExistsAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));

            string normalizedKey = key.ToLower().ToConsulKey();

            return _client.ExistKeyValueAsync(normalizedKey);
        }

        public async Task DeleteAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));

            string normalizedKey = key.ToLower().ToConsulKey();

            bool result = await _client.DeleteKeyValueAsync(normalizedKey).ConfigureAwait(false);
            if (result == false)
                throw new KeyNotFoundException("Unable to delete key/value with key: " + normalizedKey);
        }

        public async Task<string> GetAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));
            string normalizedKey = key.ToLower().ToConsulKey();

            ReadKeyValueResponse result = await _client.ReadKeyValueAsync(normalizedKey).ConfigureAwait(false);
            if (result is null)
                throw new KeyNotFoundException("Unable to find value for key: " + normalizedKey);

            byte[] data = Convert.FromBase64String(result.Value);

            var value = Encoding.UTF8.GetString(data);

            return value;
        }

        public IEnumerable<DeployedSetting> GetAll(ISettixContext context)
        {
            string settixApplication = context.ToApplicationKeyPrefix();
            Console.WriteLine($"Refreshing {settixApplication} configuration from Consul - {Thread.CurrentThread.ManagedThreadId}");

            IEnumerable<ReadKeyValueResponse> response = _client.ReadAllKeyValuesAsync(settixApplication).GetAwaiter().GetResult();

            // Filters out empty values, if we don't do this we will get an exception when we try to create DeployedSetting with an empty value.
            // And we lose all settings instead of skipping only the broken ones.
            IEnumerable<ReadKeyValueResponse> nonEmptyResponses = response.Where(x => string.IsNullOrEmpty(x.Value) == false);
            List<DeployedSetting> newSettings = nonEmptyResponses.Select(x => new DeployedSetting(x.Key.FromConsulKey(), Encoding.UTF8.GetString(Convert.FromBase64String(x.Value)))).ToList();

            Console.WriteLine($"Refreshing {settixApplication} configuration from Consul completed - {Thread.CurrentThread.ManagedThreadId}");

            return newSettings;
        }

        public async Task SetAsync(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));

            if (string.IsNullOrEmpty(value) == false)
            {
                string normalizedKey = key.ToLower().ToConsulKey();

                var result = await _client.CreateKeyValueAsync(normalizedKey, value).ConfigureAwait(false);
                if (result == false)
                    throw new KeyNotFoundException("Unable to store key/value: " + normalizedKey + "  " + value);
            }
        }

        public async Task SetGlobalAsync(string key, string value)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));
            if (GlobalContext is null) throw new InvalidOperationException("Global context is not configured. Set the settix_global_application environment variable.");

            string prefixedKey = key.ToGlobalKey().ToConsulKey(GlobalContext).ToLower();
            var result = await _client.CreateKeyValueAsync(prefixedKey, value).ConfigureAwait(false);
            if (result == false)
                throw new KeyNotFoundException("Unable to store key/value: " + prefixedKey + "  " + value);
        }

        public async Task<string> GetGlobalAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));
            if (GlobalContext is null) throw new InvalidOperationException("Global context is not configured. Set the settix_global_application environment variable.");

            string prefixedKey = key.ToGlobalKey().ToConsulKey(GlobalContext).ToLower();
            ReadKeyValueResponse result = await _client.ReadKeyValueAsync(prefixedKey).ConfigureAwait(false);
            if (result is null)
                throw new KeyNotFoundException("Unable to find value for key: " + prefixedKey);

            byte[] data = Convert.FromBase64String(result.Value);
            return Encoding.UTF8.GetString(data);
        }

        public Task<bool> ExistsGlobalAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));
            if (GlobalContext is null) throw new InvalidOperationException("Global context is not configured. Set the settix_global_application environment variable.");

            string prefixedKey = key.ToGlobalKey().ToConsulKey(GlobalContext).ToLower();
            return _client.ExistKeyValueAsync(prefixedKey);
        }

        public async Task DeleteGlobalAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException(nameof(key));
            if (GlobalContext is null) throw new InvalidOperationException("Global context is not configured. Set the settix_global_application environment variable.");

            string prefixedKey = key.ToGlobalKey().ToConsulKey(GlobalContext).ToLower();
            bool result = await _client.DeleteKeyValueAsync(prefixedKey).ConfigureAwait(false);
            if (result == false)
                throw new KeyNotFoundException("Unable to delete key/value with key: " + prefixedKey);
        }

        public IEnumerable<DeployedSetting> GetAllGlobal()
        {
            if (GlobalContext is null) throw new InvalidOperationException("Global context is not configured. Set the settix_global_application environment variable.");

            return GetAll(GlobalContext);
        }
    }
}
