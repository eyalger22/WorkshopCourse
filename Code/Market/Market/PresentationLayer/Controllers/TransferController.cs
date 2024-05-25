using Market.Pages.Modles;
using Market.PresentationLayer.Modles;
using Microsoft.AspNetCore.Mvc;

namespace Market.PresentationLayer.Controllers
{
    public class TransferController : Controller
    {
        public IActionResult My_shops(UserModel user)
        {
            MyShopsModel shops = new MyShopsModel();
            shops.product = user.product;
            shops.id = user.id;
            shops.mode = user.mode;
            shops.shop = user.shop;
            return RedirectToAction("Index", "MyShop", shops);
        }

        public IActionResult Manage_items(UserModel user)
        {
            ShopProductsModel shops = new ShopProductsModel();
            shops.product = user.product;
            shops.id = user.id;
            shops.mode = user.mode;
            shops.shop = user.shop;
            return RedirectToAction("Index", "ManageItems", shops);
        }



    }
}
