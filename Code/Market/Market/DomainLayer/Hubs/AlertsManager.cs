using Azure;
using Market.DomainLayer.Users;
using Market.ORM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;

namespace Market.DomainLayer.Hubs
{
    public class AlertsManager
    {
        private ChatHub chatHub;
        public static ChatController chatController;
        private static AlertsManager instance = null;
        public static AlertsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AlertsManager();
                }
                return instance;
            }
        }

        public AlertsManager() { }
        //public AlertsManager(ChatController chatController) 
        //{
        //    this.chatController = chatController;
        //}
        //public static HubConnection connection = new HubConnectionBuilder()
        //    .WithUrl("https://localhost:7294//chatHub")
        //    .Build();

        public async void SendAlert(Member user, string message)
        {
            if (user.SessionId != -1 && chatController != null)    //logged in
            {

                chatController.SendChatMessage(user.Name, message);
                //_ = connection.InvokeAsync("SendChatMessage", user.Name, message);
            }
            else
            {
                Alert a = new Alert(message, user.Name);
                using var transaction = MarketContext.Instance.Database.BeginTransaction();
                try
                {
                    FormattableString sql = $"SET IDENTITY_INSERT Alerts ON";
                    MarketContext.Instance.Database.ExecuteSqlInterpolated(sql);
                    user._alerts.AddItem(a, a.alertId);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw new Exception("Failed to add alert to user", e);
                }

            }
        }



    }
}
