using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;

namespace Market.PresentationLayer.Controllers.SalePolicy
{
    public class BasicPolicyController : Controller
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
            return View("~/PresentationLayer/Views/SalePolicy/BasicPolicy.cshtml", user);
        }
        public IActionResult Create(UserModel user, string description,
            string typeList, string applyon, int param1, int param2,
            int param3, string param4,int param5)
        {
            int? newKind1 = param1 == 0 ? null : param1;
            int? newKind2 = param2 == 0 ? null : param2;
            int? newKind3 = param3 == 0 ? null : param3;
            int? newKind5 = param5 == 0 ? null : param5;
            Response<int> res = Service.GetService().ShopService.AddSalePolicyRestriction(user.id, user.shop, description, typeList, applyon, newKind1, newKind2, newKind3,param4, newKind5);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Add policy successfuly!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index", "SalePolicy", user);
            }
            return RedirectToAction("Index", "BasicPolicy", user);
        }
    }
}
