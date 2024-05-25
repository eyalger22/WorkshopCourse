using Market.ORM;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Bson;
using Microsoft.AspNet.SignalR.Hubs;
//using Microsoft.AspNet.SignalR;

namespace Market.DomainLayer.Hubs
{
    public class ChatHub : Hub
    {
        //public ChatHub() 
        //{

        //}
        //private readonly IConnectionManager _connections;

        //public ChatHub(IConnectionManager connections)
        //{
        //    _connections = connections;
        //    Clients = Clients.All;
        //}

        //protected IHubContext<ChatHub> _hubContext;
        //public ChatHub(IHubContext<ChatHub> hubContext)
        //{
        //    _hubContext = hubContext;
        //}
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToUser(string user, string targetConnectionId, string message)
        {
            Console.WriteLine($"{Context.ConnectionId}-{targetConnectionId}-{message}");
            await Clients.Client(targetConnectionId).SendAsync("ReceiveMessageToUser", user, targetConnectionId, message);
        }

        public async Task SendMessageToUserByName(string user, string usernameTosend, string message)
        {
            Console.WriteLine($"{Context.ConnectionId}-{usernameTosend}-{message}");
            await Clients.User(usernameTosend).SendAsync("ReceiveMessageToUser", user, usernameTosend, message);
        }

        //start basic chat
        public readonly static ConnectionMapping<string> _connections =
            new ConnectionMapping<string>();

        public async void SendChatMessage(string who, string message)
        {
            //string name = "msg";
            //Context.User.Claims.FirstOrDefault(c => c.Type == "username")?.Value;

            foreach (var connectionId in _connections.GetConnections(who))
            {
                Clients.Client(connectionId).SendAsync("ReceiveMessageToUser", who, connectionId, message);
                //_ = Clients.Client(connectionId).SendAsync("ReceiveMessageToUser", who, connectionId, message);
            }
        }

        public override Task OnConnectedAsync()
        {
            //string name = Context.User.Identity.Name;
            //string name = Context.QueryString["username"];
            string name = Context.GetHttpContext().Request.Cookies["username"];
            //string name = Context.Request.Cookies["username"].Value;
            if (name == null)
            {
                name = "moshe";
            }
            _connections.Add(name, Context.ConnectionId);
            //SendChatMessage(name, "Connected");
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //string name = Context.QueryString["username"];
            //string name = Context.User.Identity.Name;
            string name = Context.GetHttpContext().Request.Cookies["username"];
            //string name = Context.Request.Cookies["username"].Value;

            if (name == null)
            {
                name = "moshe";
            }
            _connections.Remove(name, Context.ConnectionId);
            
            return base.OnDisconnectedAsync(exception);
        }


        //public override Task OnReconnected()
        //{
        //    //string name = Context.QueryString["username"];
        //    //string name = Context.User.Identity.Name;
        //    string name = Context.Request.Cookies["username"].Value;

        //    if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
        //    {
        //        _connections.Add(name, Context.ConnectionId);
        //    }

        //    return base.OnReconnected();
        //}
    }
}
