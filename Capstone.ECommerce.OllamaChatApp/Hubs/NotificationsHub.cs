using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;

namespace Capstone.ECommerce.OllamaChatApp.Hubs
{
    public class NotificationsHub:Hub
    {
        IChatClient chatClient;
        List<ChatMessage> chatHistory = new();
        public NotificationsHub()
        {
            chatClient = new OllamaChatClient(new Uri("http://localhost:11434/"), "orca-mini");
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
