using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
using Microsoft.CodeAnalysis;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp;

namespace Market.PresentationLayer.Controllers.DiscountPolicies
{
    public class ConditionalDiscountController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<string> res = Service.GetService().ShopService.GetDiscountPolicies(user.id, user.shop);
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
            return View("~/PresentationLayer/Views/DiscountPolicy/ConditionalDiscount.cshtml", user);
        }

        public IActionResult Create(UserModel user, string description, int discountid, int predicateId) {
            Response<int> res = Service.GetService().ShopService.AddDiscountPolicyConditional(user.id, user.shop, description, discountid, predicateId);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Add policy successfuly!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index", "DiscountPolicy", user);
            }
            return RedirectToAction("Index", "ConditionalDiscount", user);
        }
    }
}
