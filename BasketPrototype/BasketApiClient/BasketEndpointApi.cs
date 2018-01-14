using BasketApiClient.DTO;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BasketApiClient {
    public sealed class BasketEndpointApi {
        private readonly HttpClient _httpClient;
        private const string _CONTENT_TYPE = "application/json";

        public BasketEndpointApi() {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(_CONTENT_TYPE));
        }

        public async Task<Uri> CreateBasket(Uri uri, string customerId) {
            var content = JsonConvert.SerializeObject(new { CustomerId = customerId });
            var result = await _httpClient
                .PostAsync(uri, new StringContent(content, Encoding.UTF8, _CONTENT_TYPE))
                .ConfigureAwait(false);

            if (result.StatusCode != HttpStatusCode.Created) {
                throw new Exception(result.StatusCode.ToString());
            }
            return result.Headers.Location;
        }


        public async Task<BasketDto> FindOpenBasketByCustomerId(Uri uri, string customerId) {
            // TODO:
            throw new NotImplementedException();
        }

        public async Task<BasketDto> GetBasket(Uri uri) {
            var result = await _httpClient.GetAsync(uri);

            if (result.StatusCode != HttpStatusCode.OK) {
                throw new Exception(result.StatusCode.ToString());
            }

            return JsonConvert.DeserializeObject<BasketDto>(
                await result.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task AddOrUpdateBasketItem(Uri uri, int quantity) {
            var content = JsonConvert.SerializeObject(new { Quantity = quantity });
            var result = await _httpClient
                .PutAsync(uri, new StringContent(content, Encoding.UTF8, _CONTENT_TYPE))
                .ConfigureAwait(false);

            if (result.StatusCode != HttpStatusCode.OK) {
                throw new Exception(result.StatusCode.ToString());
            }
        }

        public async Task<PaginatedCollectionDto<StockItemDto>> GetCatalogueItems(Uri uri) {
            var result = await _httpClient.GetAsync(uri).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<PaginatedCollectionDto<StockItemDto>>(
                await result.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        public async Task<CollectionDto<BasketItemDto>> GetContents(Uri uri) {
            var result = await _httpClient.GetAsync(uri).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<CollectionDto<BasketItemDto>>(
                await result.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Remove a basket item, a basket or all basket items
        /// </summary>
        /// <param name="uri"></param>
        public async Task Remove(Uri uri) {
            var result = await _httpClient.DeleteAsync(uri);

            if (result.StatusCode != HttpStatusCode.OK) {
                throw new Exception(result.StatusCode.ToString());
            }
        }
    }
}
