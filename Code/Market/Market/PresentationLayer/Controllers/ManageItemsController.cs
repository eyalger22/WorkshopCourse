using Market.DataObject;
using Market.Pages.Modles;
using Market.PresentationLayer.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;

namespace Market.PresentationLayer.Controllers
{
    public class ManageItemsController : Controller
    {
        public IActionResult Index(ShopProductsModel user)
        {
            user.products = Service.GetService().ShopService.GetShopProducts(user.shop).Value;
            return View("~/PresentationLayer/Views/ManageItems/Index.cshtml",user);
        }

        public IActionResult Add_product_to_shop(UserModel user,string productname,string price,string category,string desciption)
        {
            int priceValue; 
            if(int.TryParse(price,out priceValue))
            {
                Response<int> res = Service.GetService().ShopService.AddProductToShop(user.id, user.shop, productname, priceValue, category, desciption);
                if (!res.HasError)
                {
                    TempData["Message"] = "Add product successfuly!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = res.ErrorMsg;
                    TempData["MessageType"] = "error";
                }
            }
            else
            {
                TempData["Message"] = "please enter a valid price number";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("Index", "ManageItems", user);
        }

        public IActionResult RemoveProduct(ShopProductsModel user,int productid)
        {
            Response<bool> res =Service.GetService().ShopService.RemoveProductFromShop(user.id, user.shop, productid);
            if (res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";

            }
            else
            {
                TempData["Message"] = "remove product successfuly!";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "ManageItems", user);
        }

        public IActionResult Add_to_stock(ShopProductsModel user, int productid,int amount)
        {
            Response<int> res = Service.GetService().ShopService.AddAmountToProductInStock(user.id,user.shop,productid,amount);
            if (res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";

            }
            else
            {
                TempData["Message"] = "Add product to stock successfuly!";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "ManageItems", user);
        }
    }
}
