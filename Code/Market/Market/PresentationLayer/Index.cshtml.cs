using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Client;

namespace Market.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public Service service;
        private HubConnection connection;


        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            service = Service.GetService();
        }

        public void OnGet()
        {
            //service.UserService.EnterGuest();
            //connection = new HubConnectionBuilder()
            //    .WithUrl("https://localhost:7294//chatHub")
            //    .Build();
            //var id = connection.ConnectionId;
            //Console.WriteLine(id);
        }
        public void OnPost()
        {
            // some code here
        }
    }
}