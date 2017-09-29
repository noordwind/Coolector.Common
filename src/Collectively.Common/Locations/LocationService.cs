using System;
using System.Net.Http;
using System.Threading.Tasks;
using Collectively.Common.Extensions;
using Collectively.Common.Types;
using Newtonsoft.Json;
using Polly;

namespace Collectively.Common.Locations
{
    public class LocationService : ILocationService
    {
        private static readonly Uri ApiUrl = new Uri("https://maps.googleapis.com/maps/api/geocode/json");
        private readonly HttpClient _client;
        private readonly LocationSettings _settings;
        
        public LocationService(LocationSettings settings)
        {
            _settings = settings;
            _client = new HttpClient
            {
                BaseAddress = ApiUrl
            };
        }

        public async Task<Maybe<LocationResponse>> GetAsync(string address)
        => await GetAsync(address, 0, 0);

        public async Task<Maybe<LocationResponse>> GetAsync(double latitude, double longitude)
        => await GetAsync(string.Empty, latitude, longitude);

        public async Task<Maybe<LocationResponse>> GetAsync(string address, double latitude, double longitude)
        {
            var retries = _settings.RetriesCount <= 0 ? 0 : _settings.RetriesCount;
            var retriesDelay = _settings.RetriesDelay <= 0 ? 500 : _settings.RetriesDelay;
            var retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryAsync<HttpResponseMessage>(retries, 
                    retryAttempt => TimeSpan.FromMilliseconds(retriesDelay));
            var response = await retryPolicy.ExecuteAsync(async () => 
            {
                var query = address.Empty() ? $"latlng={latitude},{longitude}" : $"address={address}";
                var queryWithKey = _settings.ApiKey.Empty() ? query : $"{query}&key={_settings.ApiKey}";

                return await _client.GetAsync($"?{queryWithKey}");
            });
            var content = await response.Content.ReadAsStringAsync();
            if(!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<LocationResponse>(content));
        }
    }
}