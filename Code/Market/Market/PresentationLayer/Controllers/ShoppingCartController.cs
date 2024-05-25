using Market.DataObject;
using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Market.PresentationLayer.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<ShoppingCart> cart = Service.GetService().UserService.ViewShoppingCart(user.id);
            if(!cart.HasError)
            {
                List<((string, ShoppingBasket), double)> Baskets_and_price = new List<((string, ShoppingBasket), double)>();
                foreach ((string, ShoppingBasket) Basket in cart.Value.Baskets)
                {
                    Response<double> price = Service.GetService().UserService.GetBasketPrice(user.id, Basket.Item1);
                    if(price == null || price.HasError)
                    {
                        TempData["Message"] = price.ErrorMsg;
                        TempData["MessageType"] = "error";
                        return View("~/PresentationLayer/Views/ShoppingCart/Index.cshtml", user);
                    }
                    else
                    {
                        Baskets_and_price.Add((Basket, price.Value));
                    }
                }
                Response<double> Cartprice = Service.GetService().UserService.GetCartPrice(user.id);
                if (Cartprice == null || Cartprice.HasError)
                {
                    TempData["Message"] = Cartprice.ErrorMsg;
                    TempData["MessageType"] = "error";
                    return View("~/PresentationLayer/Views/ShoppingCart/Index.cshtml", user);
                }
                else
                {
                    TempData["cart"] = Cartprice.Value;
                    string serializedCart = JsonConvert.SerializeObject(Baskets_and_price);
                    if (!string.IsNullOrEmpty(serializedCart))
                    {
                        TempData["serializedCart"] = serializedCart;
                    }
                }
            }
            return View("~/PresentationLayer/Views/ShoppingCart/Index.cshtml", user);
        }

        public IActionResult EditAmount(UserModel user, int amount)
        {
            Response<bool> res = Service.GetService().ShopService.EditProductInCart(user.id, user.shop, user.product, amount);
            if(!res.HasError)
            {
                TempData["Message"] = "Edit amount auccessfully!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("Index", "ShoppingCart", user);
        }
    }
}
