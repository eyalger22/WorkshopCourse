using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
using Microsoft.AspNet.SignalR.Hubs;

namespace Market.PresentationLayer.Controllers.DiscountPolicies
{
    public class BasicDiscountController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<string> res = Service.GetService().ShopService.GetDiscountPolicies(user.id, user.shop);
            if(res == null || res.HasError)
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
            return View("~/PresentationLayer/Views/DiscountPolicy/BasicDiscount.cshtml", user);
        }
        public IActionResult Create(UserModel user, string description,string typeList, string kind1,int kind2 ,double discount) 
        {
            if(discount<=0 || discount >= 1) {
                TempData["Message"] = "discount must be between 0 to 1";
                TempData["MessageType"] = "error";
            }
            else
            {
                int? newKind2 = kind2 == 0 ? null : kind2;
                Response<int> res = Service.GetService().ShopService.AddDiscountPolicyBasic(user.id, user.shop, description, discount, typeList, newKind2, kind1);
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
            }
            return RedirectToAction("Index", "BasicDiscount", user);
        }


    }
}
