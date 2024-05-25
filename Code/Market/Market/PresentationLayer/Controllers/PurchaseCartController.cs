using Market.DataObject;
using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Owin.BuilderProperties;
using System.Diagnostics.Metrics;
using Market.DataObject.OrderRecords;

namespace Market.PresentationLayer.Controllers
{
    public class PurchaseCartController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            return View("~/PresentationLayer/Views/PurchaseCart/Index.cshtml", user);
        }

        public IActionResult Purchase(UserModel user, string cardNumber,string month,string year,string holder,string cvv, string userid, string name,string address,string city, string country,string zip)
        {
            PaymentDetails pd = new PaymentDetails(cardNumber, month, year, holder, cvv, userid);
            DeliveryDetails dd = new DeliveryDetails(name, address, city, country, zip);
            Response<Order> res = Service.GetService().ShopService.MakePurchase(user.id, pd, dd);
            if (res == null ||res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "cart was purches successfully";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "ShoppingCart", user);
        }
    }
}
