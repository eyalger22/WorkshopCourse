using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;

namespace Market.PresentationLayer.Controllers.DiscountPolicies
{
    public class AddPredicatsController : Controller
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
            return View("~/PresentationLayer/Views/DiscountPolicy/AddPredicats.cshtml", user);
        }

        public ActionResult Create(UserModel user,string typeList,int id1,int id2) 
        {
            int? newid2 = id2 == 0 ? null : id2;
            Response<int> res = Service.GetService().ShopService.BuildDiscountPredicate(user.id, user.shop, typeList, id1, id2);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Add predicat successfuly!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index", "DiscountPolicy", user);
            }
            return RedirectToAction("Index", "AddPredicats", user);
        }
    }
}
