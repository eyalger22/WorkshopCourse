using Market.DataObject;
using Market.Pages.Modles;
using Market.PresentationLayer.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Market.PresentationLayer.Controllers
{
    public class MyShopController : Controller
    {
        public IActionResult Index(MyShopsModel user)
        {
            Response<List<Shop>> res = Service.GetService().UserService.GetUserShops(user.id);
            if(res!= null || res.HasError)
            {
                user.shop_list = res.Value;
                if (user.shop_list.Count == 0)
                {
                    TempData["shops"] = "you dont have shops yet";
                }
            }
            return View("~/PresentationLayer/Views/MyShop/Index.cshtml", user);
        }
        public IActionResult EnterShop(MyShopsModel user) {
            UserModel model = new UserModel();
            model.mode = user.mode;
            model.id = user.id;
            model.shop = user.shop;
            model.product = user.product;
            return RedirectToAction("Index","ShopPage", model);
        }


        public IActionResult Open_shop(MyShopsModel user)
        {
            UserModel model = new UserModel();
            model.mode = user.mode;
            model.id = user.id;
            model.shop = user.shop;
            model.product = user.product;
            return View("~/PresentationLayer/Views/MyShop/Open_shop.cshtml", model);
        }

        public IActionResult open(UserModel user, string shopname, string address, string bank) {
            Response<Shop> openSuccess = Service.GetService().ShopService.OpenShop(user.id, shopname, address, bank);
            bool success = !openSuccess.HasError;
            if (success)
            {
                TempData["Message"] = "open new shop successfully!";
                return RedirectToAction("My_shops", "Transfer", user);
            }
            else
            {
                TempData["Message"] = openSuccess.ErrorMsg;
            }
            return RedirectToAction("Open_shop", "MyShop", user);
        }
    }
}
