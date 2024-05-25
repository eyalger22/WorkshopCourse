using Market.Pages.Modles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Construction;
using Microsoft.CodeAnalysis;
using System.Diagnostics;
using ErrorViewModel = Market.Pages.Modles.ErrorViewModel;
using Market.DataObject;
using Market.ServiceLayer;

namespace Market.Pages.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UserModel user = null;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if(user == null)
            {
                user = new UserModel();
                Response<int> res = Service.GetService().UserService.EnterGuest();
                if (!res.HasError)
                {
                    user.id = res.Value;
                    user.mode = 1;
                    user.product = -1;
                    user.shop = -1;
                }
            }
            return RedirectToAction("Index", "MarketPage", user);
        }




        public IActionResult Privacy(UserModel userModle)
        {
            return View("~/PresentationLayer/Views/Home/Privacy.cshtml",userModle);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
