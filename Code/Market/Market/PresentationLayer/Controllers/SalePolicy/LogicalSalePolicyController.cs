using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
namespace Market.PresentationLayer.Controllers.SalePolicy
{
    public class LogicalSalePolicyController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<string> res = Service.GetService().ShopService.GetSalePolicies(user.id, user.shop);
            if (res == null || res.HasError)
            {
                TempData["Message"] = "Error getting all policies";
                TempData["MessageType"] = "error";
            }
            else
            {
                char[] del = { '\n' };
                List<string> strings = res.Value.Split(del).ToList();
                TempData["policy"] = strings;
            }
            return View("~/PresentationLayer/Views/SalePolicy/LogicalSalePolicy.cshtml", user);
        }

        public IActionResult Create(UserModel user, string description,string type,int param1, int param2)
        {
            Response<int> res = Service.GetService().ShopService.AddSalePolicyLogical(user.id,user.shop,description,type,param1,param2);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Add policy successfuly!";
                TempData["MessageType"] = "success";
                return RedirectToAction("Index", "SalePolicy", user);
            }
            return RedirectToAction("Index", "LogicalSalePolicy", user);
        }
        

        }
    }

