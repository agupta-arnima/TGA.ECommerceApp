using Capstone.ECommerceApp.Infra.Common;
using Newtonsoft.Json;
using System.Text;

public class Program
{
    private static readonly HttpClient client = new HttpClient();
    private static readonly Dictionary<string, string> userTokens = new Dictionary<string, string>();
    private static readonly string baseUrl = "https://localhost:7777/api/auth";

    static async Task Main(string[] args)
    {

        while (true)
        {

            Console.WriteLine(" ☺ Register User (1)"); // Symbol for Register User
            Console.WriteLine(" ✉ Login (2)"); // Symbol for Login
            Console.WriteLine(" ✈ Place Order (3)"); // Symbol for Place Order
            Console.WriteLine(" ✔ Add to Cart (4)"); // Symbol for Add to Cart
            Console.WriteLine(" ✕ CheckOut (5)"); // Symbol for CheckOut
            Console.WriteLine(" ⏏ Exit (6)"); // Symbol for Exit


            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await RegisterUsers();
                    break;
                case "2":
                    await LoginUsers();
                    break;
                case "3":
                     await PlaceOrders();
                    break;
                case "4":
                    // Implement Add to Cart logic
                    Console.WriteLine("Add to Cart functionality is not implemented yet.");
                    break;
                case "5":
                    // Implement CheckOut logic
                    Console.WriteLine("CheckOut functionality is not implemented yet.");
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }


    private static async Task RegisterUsers()
    {
        var tasks = new List<Task>();

        // Register 10 users
        for (int i = 1; i <= 10; i++)
        {
            var user = new RegistrationRequestDto
            {
                Email = $"user{i}@loadtest.com",
                Name = $"user{i}",
                PhoneNumber = "9654663775",
                Password = "Test@1234",
                Role = "admin"
            };
            tasks.Add(RegisterUser(user));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("User registration completed.");
    }


    private static async Task LoginUsers()
    {
        var tasks = new List<Task>();

        for (int i = 1; i <= 10; i++)
        {
            var user = new LoginRequestDto
            {
                UserName = $"user{i}@loadtest.com",
                Password = "Test@1234"
            };
            tasks.Add(LoginUser(user));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("User login completed.");
    }

    private static async Task RegisterUser(RegistrationRequestDto user)
    {
        var json = JsonConvert.SerializeObject(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{baseUrl}/register", content);
        var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Register Response: {responseString}");
    }

    private static async Task LoginUser(LoginRequestDto user)
    {
        var json = JsonConvert.SerializeObject(user);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync($"{baseUrl}/login", content);
        var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Login Response: {responseString}");

        dynamic result = JsonConvert.DeserializeObject(responseString);
        string token = result?.accessToken;

        if (!string.IsNullOrEmpty(token))
        {
            userTokens[user.UserName] = token;
        }

    }


    private static async Task PlaceOrders()
    {
        //var tasks = new List<Task>();

        //for (int i = 1; i <= 10; i++)
        //{
        //    string email = $"user{i}@loadtest.com";
        //    if (userTokens.TryGetValue(email, out string token))
        //    {
        //        var cart = new CartDto
        //        {
        //            UserId = email,
        //            CartItems = new List<CartItemDto>
        //                 {
        //                 new CartItemDto { ProductId = 1, Quantity = 2 },
        //                 new CartItemDto { ProductId = 2, Quantity = 1 }
        //                 }
        //                                };

        //        tasks.Add(CreateOrder(cart, token));
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Token not found for {email}. Make sure the user is logged in.");
        //    }
        //}

        //await Task.WhenAll(tasks);
        Console.WriteLine("Order placement completed.");
    }


    private static async Task CreateOrder(CartDto cart, string token)
    {
        var json = JsonConvert.SerializeObject(cart);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:7777/api/order/createOrder");
        request.Content = content;
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.SendAsync(request);
        var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Order Response for {cart.CartHeader.UserId}: {responseString}");
    }

}
