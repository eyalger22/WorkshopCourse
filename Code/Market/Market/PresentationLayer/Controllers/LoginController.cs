using Market.DataObject;
using Market.DomainLayer.Hubs;
using Market.DomainLayer.Users;
using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Net.Http.Headers;
using NuGet.Packaging.Signing;
using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNet.SignalR.Client;
using Market.DomainLayer.Market;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace Market.Pages.Controllers
{
    public class LoginController : Controller
    {
        private HubConnection connection;
        public IActionResult Index(UserModel user)
        {
            return View("~/PresentationLayer/Views/Login/Index.cshtml",user);
        }

        public IActionResult Login(UserModel user, string username,string password)
        {
            //Hub
            //connection = new HubConnectionBuilder()
            //    .WithUrl("https://localhost:7294//chatHub").WithAutomaticReconnect()
            //    .Build();
            //using (var hubConnection = new HubConnection("https://localhost:7294//"))
            //{
            //    IHubProxy chatHub = hubConnection.CreateHubProxy("chatHub");
            //    chatHub.On("ReceiveMessage", message => Console.WriteLine(message));
            //    hubConnection.Start();
            //}
            HttpContext.Response.Cookies.Append("username", username);

            Service.GetService().UserService.Exit(user.id);
            Response<int> LoginSuccess = Service.GetService().UserService.Login(username, password);
            bool success = !LoginSuccess.HasError;
            if (success)
            {
                //connection.StartAsync();
                //Service.GetService().UserService.UpdateConnectionId(LoginSuccess.Value, connection.ConnectionId);
                if(username == "admin")
                {
                    user.mode = 3;
                    user.id = LoginSuccess.Value;
                    return RedirectToAction("index", "MarketPage", user);
                }
                //TODO: must check if this is a system maneger first
                user.mode = 2;
                user.id = LoginSuccess.Value;
                Response<List<string>> alerts = Service.GetService().UserService.ReadAlert(user.id);
                if (alerts.Value != null || alerts.Value.Count > 0)
                {
                    //alerts.Value.Add("Read Your Alerts");
                    string serializedAlerts = JsonConvert.SerializeObject(alerts.Value);
                    if (!string.IsNullOrEmpty(serializedAlerts))
                    {
                        TempData["serializedAlerts"] = serializedAlerts;
                    }
                }   
                return RedirectToAction("index", "MarketPage", user);
            }
            else
            {
                TempData["Message"] = LoginSuccess.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("index", "Login", user);
        }

        public IActionResult Logout(UserModel user)
        {
            Response<bool> res = Service.GetService().UserService.Logout(user.id);
            Response<int> newGuset = Service.GetService().UserService.EnterGuest();
            user.mode = 1;
            user.id = newGuset.Value;
            user.product = -1;
            user.shop = -1;
            return RedirectToAction("index", "MarketPage", user);
        }
    }
}
