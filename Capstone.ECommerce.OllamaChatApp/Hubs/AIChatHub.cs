using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;

namespace Capstone.ECommerce.OllamaChatApp.Hubs
{
    public class AIChatHub : Hub
    {
        private readonly IChatClient chatClient;
        List<ChatMessage> chatHistory = new();
        public AIChatHub(IConfiguration configuration)
        {
            var ollamaBaseUrl = configuration["OllamaSettings:BaseUrl"];
            var ollamaModelName = configuration["OllamaSettings:ModelName"] ?? "orca-mini";
            if (string.IsNullOrEmpty(ollamaBaseUrl))
            {
                throw new InvalidOperationException("OllamaSettings:BaseUrl is not configured in appsettings.json.");
            }
            if (ollamaBaseUrl.StartsWith("http://http://"))
            {
                ollamaBaseUrl = ollamaBaseUrl.Replace("http://http://", "http://");
                Console.WriteLine($"Warning: Corrected Ollama BaseUrl typo. Using: {ollamaBaseUrl}");
            }
            chatClient = new OllamaChatClient(new Uri(ollamaBaseUrl), ollamaModelName);
        }

        //Client will call SendNotification function
        public async Task SendNotification(string notification)
        {
            var userPrompt = notification;
            chatHistory.Add(new ChatMessage(ChatRole.User, userPrompt));
            var response = "";

            await foreach (var item in chatClient.GetStreamingResponseAsync(chatHistory))
            {
                //Server will be invoking ReceiveNotification function on client side.
                await Clients.All.SendAsync("ReceiveNotification", item.Text);
                response += item.Text;
            }
            chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
        }
    }
}
