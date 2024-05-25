using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
using Market.DomainLayer.Users;

namespace Market.PresentationLayer.Controllers
{
    public class SystemManagerController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            return View("~/PresentationLayer/Views/SystemManager/Index.cshtml", user);
        }

        public IActionResult Remove_member(UserModel user,string member)
        {
            Response<bool> res = Service.GetService().UserService.RemoveMember(user.id, member);
            if(res == null || res.HasError || !res.Value)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["message"] = "The user has been removed";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "SystemManager", user);

        }
        public IActionResult Close_Shop_forever(UserModel user,string shopname)
        {
            {
                Response<Shop> shop = Service.GetService().ShopService.GetShopByName(shopname);
                if (shop == null || shop.HasError)
                {
                    TempData["Message"] = shop.ErrorMsg;
                    TempData["MessageType"] = "error";
                }
                else{
                    Response<bool> res = Service.GetService().UserService.CloseShopForever(user.id, shop.Value.ShopId);
                    if (res == null || res.HasError || !res.Value)
                    {
                        TempData["Message"] = res.ErrorMsg;
                        TempData["MessageType"] = "error";
                    }
                    else
                    {
                        TempData["message"] = "The shop has been removed";
                        TempData["MessageType"] = "success";
                    }
                }
                return RedirectToAction("Index", "SystemManager", user);

            }
        }
    }
}
