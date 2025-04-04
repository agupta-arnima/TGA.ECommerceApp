using Newtonsoft.Json;
using Polly.CircuitBreaker;
using Capstone.ECommerceApp.ShoppingCart.Application.Dto;
using Capstone.ECommerceApp.ShoppingCart.Application.Interfaces;

namespace Capstone.ECommerceApp.ShoppingCart.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("Product"); //this is the name of the client we have defined in the program.cs and pick base address from there
                var response = await client.GetAsync("api/product");
                var apiContent = await response.Content.ReadAsStringAsync();
                var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                if (resp.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
                }
            }
            catch (BrokenCircuitException ex)
            {
                Console.WriteLine($"Request failed due to opened circuit: {ex.Message}");
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Request failed. StatusCode={httpEx.StatusCode} Message={httpEx.Message}");
            }
            return new List<ProductDto>();
        }
    }
}
