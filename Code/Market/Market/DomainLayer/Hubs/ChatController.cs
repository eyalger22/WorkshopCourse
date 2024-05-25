using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Market.DomainLayer.Hubs
{
    public class ChatController : Controller
    {
        public IHubContext<ChatHub> ChatHubContext { get; }

        public ChatController(IHubContext<ChatHub> chatHubContext)
        {
            ChatHubContext = chatHubContext;
        }

        public async Task SendMessage(string user, string message)
        {
            await ChatHubContext.Clients.All.SendAsync(user, message);
        }

        public async Task SendPrivateMessage(string user, string message)
        {
            await ChatHubContext.Clients.User(user).SendAsync("ReceiveMessageToUser", message);
        }

        public void SendChatMessage(string who, string message)
        {
            //string name = "msg";
            //Context.User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

            foreach (var connectionId in ChatHub._connections.GetConnections(who))
            {
                _ = ChatHubContext.Clients.Client(connectionId).SendAsync("ReceiveMessageToUser", who, connectionId, message);
            }
        }
        //d51f3XE8izypdxD1f

    }
}
