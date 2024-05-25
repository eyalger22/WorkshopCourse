using Market.ServiceLayer;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR.Client;
using NuGet.Protocol.Plugins;


namespace Market.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public Service service;
        private HubConnection connection;
        public LoginModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            service = Service.GetService();
       
            
        }
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }


        public void OnGet()
        {
            // Do nothing on page load
            //var id = connection.ConnectionId;
            //Console.WriteLine(id);
        }

        public IActionResult OnPost()
        {
            if(Username is null) Username = string.Empty;
            if(Password is null) Password = string.Empty;
            string username = Username.Trim();
            string password = Password.Trim();

            ErrorMessage = "Invalid username or password.";
            return Page();
        }

        public void Login(string username, string password)
        {
            // connection = new HubConnectionBuilder()
            //.WithUrl("https://localhost:7294//chatHub", options =>
            //{
            //    options.AccessTokenProvider = async () =>
            //    {
            //        return await Task.FromResult(Username);
            //    };
            //})
            //.Build();
            Console.WriteLine("Login!!!");
        }
    }
}
