using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
//using Market.DomainLayer.Market;

namespace Market.PresentationLayer.Controllers
{
    public class ProductPageController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<Product> product = Service.GetService().ShopService.GetProductInfo(user.product);
            if(product.HasError) {
                TempData["message"] = product.ErrorMsg;
            }
            else
            {
                Response<Shop> shop = Service.GetService().ShopService.GetShopInfo(user.shop);
                if (shop.HasError)
                {
                    TempData["message"] = shop.ErrorMsg;
                }
                else
                {
                    Product p = product.Value;
                    TempData["name"] = p.Name;
                    TempData["price"] = p.Price;
                    TempData["id"] = p.ProductId;
                    TempData["category"] = p.Category;
                    TempData["shop"] = shop.Value.Name;
                    TempData["description"] = p.Description;
                    Response<bool> incart = Service.GetService().UserService.HasItemInCart(user.id, shop.Value.Name, product.Value.ProductId);
                    if (!incart.HasError && incart.Value)
                    {
                        TempData["incart"] = "In";
                    }

                }
                
            }
            return View("~/PresentationLayer/Views/ProductPage/Index.cshtml", user);
        }

        public IActionResult Add_to_cart(UserModel user, int amount) 
        {
            Response<bool> res = Service.GetService().UserService.AddProductToCart(user.id, user.shop, user.product, amount);
            if(res.HasError)
            {
                TempData["message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["message"] = "Product added successfully";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "ProductPage", user);
        }

        public IActionResult Remove_from_cart(UserModel user) {
            Response<bool> res = Service.GetService().ShopService.EditProductInCart(user.id, user.shop, user.product, 0);
            if (!res.HasError)
            {
                TempData["Message"] = "Product removed from cart!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("Index", "ProductPage", user);
        }
    }
}
