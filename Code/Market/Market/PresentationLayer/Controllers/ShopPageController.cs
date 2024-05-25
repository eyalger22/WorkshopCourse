using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Market.DataObject;

namespace Market.PresentationLayer.Controllers
{
    public class ShopPageController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<bool> res = Service.GetService().UserService.Can_close_shop(user.id, user.shop);
            if (!res.HasError && res.Value)
            {
                TempData["close"] = "can";
            }
            Response<bool> res2 = Service.GetService().UserService.Can_open_shop(user.id, user.shop);
            if (!res2.HasError && res2.Value)
            {
                TempData["open"] = "can";
            }
            return View("~/PresentationLayer/Views/ShopPage/Index.cshtml",user);
        }

        public IActionResult ManageItems(UserModel user)
        {
            return RedirectToAction("index", "ManageItems", user);
        }

        public IActionResult Close(UserModel user)
        {
            Response<int> res = Service.GetService().ShopService.CloseShop(user.id,user.shop);
            if(!res.HasError)
            {
                TempData["Message"] = "Close shop successful!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index", "ShopPage", user);
              
            }
            else
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
                return RedirectToAction("Index", "ShopPage", user);
            }
        }
        public IActionResult Reopen(UserModel user)
        {
            Response<int> res = Service.GetService().ShopService.OpenClosedShop(user.id, user.shop);
            if (!res.HasError)
            {
                TempData["Message"] = "Reopen shop successful!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index", "ShopPage", user);

            }
            else
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
                return RedirectToAction("Index", "ShopPage", user);
            }
        }
    }
}
