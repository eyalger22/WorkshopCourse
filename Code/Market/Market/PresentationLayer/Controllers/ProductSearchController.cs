using Market.DataObject;
using Market.Pages.Modles;
using Market.PresentationLayer.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace Market.PresentationLayer.Controllers
{
    public class ProductSearchController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            return View("~/PresentationLayer/Views/ProductSearch/Index.cshtml", user);
        }

        public IActionResult Search(UserModel user, string productname, int productid, int minprice, int maxprice, string category)
        {
            int? newId = productid;
            int? newMax = maxprice;
            if (productid == 0)
            {
                newId = null;
            }
            if (maxprice == 0)
            {
                newMax = null;
            }
            Response<List<Product>> res = Service.GetService().ShopService.SearchProducts(productname, newId, minprice, newMax, category);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                return RedirectToAction("Index", "ProductSearch",User);
            }
            else
            {
                List<Product> products = new List<Product>(res.Value.ToList());
                string serializedProducts = JsonConvert.SerializeObject(products);
                if (!string.IsNullOrEmpty(serializedProducts))
                {
                    TempData["SerializedProducts"] = serializedProducts;
                }
                return RedirectToAction("found", "ProductSearch", user);
            }
            
        }

        public IActionResult found(UserModel model) {
            return View("~/PresentationLayer/Views/ProductSearch/Index.cshtml", model);
        }
    }
}
