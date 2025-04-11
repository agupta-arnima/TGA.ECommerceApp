using Capstone.ECommerceApp.Order.Application.Dto;
using Capstone.ECommerceApp.Order.Application.Interfaces;
using Newtonsoft.Json;
using System.Text;

namespace Capstone.ECommerceApp.Order.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public InventoryService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<bool> ReserveInventory(OrderHeaderDto order, string token)
    {

        try
        {
            var client = _httpClientFactory.CreateClient("Inventory");
            var request = new HttpRequestMessage(HttpMethod.Post, "api/inventory/reserveinventory");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode(); // Throws an exception if the status code is not 2xx
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            return resp?.IsSuccess ?? false;
        }
        catch (HttpRequestException ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"Request error: {ex.Message}");
            return false;
        }

    }


    public async Task<bool> ReleaseInventory(OrderHeaderDto order, string token)
    {
        var client = _httpClientFactory.CreateClient("Inventory");
        var content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("api/inventory/releaseinventory", content);
        var apiContent = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
        return resp?.IsSuccess ?? false;
    }
}
