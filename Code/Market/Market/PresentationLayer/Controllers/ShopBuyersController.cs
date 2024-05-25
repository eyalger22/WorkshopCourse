using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
using Newtonsoft.Json;

namespace Market.PresentationLayer.Controllers
{
    public class ShopBuyersController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<Shop> res1 = Service.GetService().ShopService.GetShopInfo(user.shop);
            if(res1 == null || res1.HasError)
            {
                TempData["Message"] = res1.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                string ShopDate = JsonConvert.SerializeObject(res1.Value);
                if (!string.IsNullOrEmpty(ShopDate))
                {
                    TempData["shop"] = ShopDate;
                }
                Response<List<(Product, int)>> res = Service.GetService().ShopService.GetShopProducts(user.shop);
                if (res == null || res.HasError)
                {
                    TempData["Message"] = res.ErrorMsg;
                    TempData["MessageType"] = "error";
                }
                else
                {
                    List<(Product, int)> products = new List<(Product, int)>(res.Value);
                    string serializedProducts = JsonConvert.SerializeObject(products);
                    if (!string.IsNullOrEmpty(serializedProducts))
                    {
                        TempData["SerializedProducts"] = serializedProducts;
                    }
                }
            }
            return View("~/PresentationLayer/Views/ShopBuyers/Index.cshtml", user);
        }
    }
}
