using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
namespace Market.PresentationLayer.Controllers.SalePolicy
{
    public class SalePolicyController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<string> res = Service.GetService().ShopService.GetSalePolicies(user.id, user.shop);
            if (res == null || res.HasError)
            {
                TempData["Message"] = "Error getting all policies";
                TempData["MessageType"] = "error";
            }
            else
            {
                char[] del = { '\n' };
                List<string> strings = res.Value.Split(del).ToList();
                TempData["policy"] = strings;
            }
            return View("~/PresentationLayer/Views/SalePolicy/Index.cshtml", user);
        }
    }
}
