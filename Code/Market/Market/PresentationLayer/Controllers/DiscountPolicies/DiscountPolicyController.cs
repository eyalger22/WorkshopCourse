using Market.Pages.Modles;
using Microsoft.AspNetCore.Mvc;

namespace Market.PresentationLayer.Controllers.DiscountPolicies
{
    public class DiscountPolicyController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            return View("~/PresentationLayer/Views/DiscountPolicy/Index.cshtml", user);
        }
    }
}
