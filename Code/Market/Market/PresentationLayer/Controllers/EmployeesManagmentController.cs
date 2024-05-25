using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
using static Market.DataObject.PermissionsEnum;

namespace Market.PresentationLayer.Controllers
{
    public class EmployeesManagmentController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            return View("~/PresentationLayer/Views/EmployeesManagment/Index.cshtml", user);
        }

        public IActionResult Managers(UserModel user, string EnameManeger, string action)
        {
            if (action.Equals("makeManager"))
            {
                return AddManager(user, EnameManeger);
            }
            if (action.Equals("removeManager"))
            {
                return RemoveManager(user,EnameManeger);
            }
            TempData["Message"] = "Error apear in pressing button";
            TempData["MessageType"] = "error";
            return RedirectToAction("Index", "EmployeesManagment", user);
        }

        public IActionResult AddManager(UserModel user, string username)
        {
            Response <bool> res = Service.GetService().ShopService.AddManager(user.id,user.shop,username);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Add Manager successfuly!";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "EmployeesManagment", user);
        }

        public IActionResult RemoveManager(UserModel user, string username)
        {
            Response<bool> res = Service.GetService().UserService.RemoveAppointManager(user.id, user.shop, username);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Remove Manager successfuly!";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "EmployeesManagment", user);
        }

        public IActionResult Owners(UserModel user, string EnameOwner, string action)
        {
            if (action.Equals("makeOwner"))
            {
                return AddOwner(user, EnameOwner);
            }
            if (action.Equals("removeOwner"))
            {
                return RemoveOwner(user,EnameOwner);
            }
            TempData["Message"] = "Error apear in pressing button";
            TempData["MessageType"] = "error";
            return RedirectToAction("Index", "EmployeesManagment", user);
        }


        public IActionResult AddOwner(UserModel user, string username)
        {
            Response<bool> res = Service.GetService().ShopService.AddOwner(user.id, user.shop, username);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Add Owner successfuly!";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "EmployeesManagment", user);
        }

        public IActionResult RemoveOwner(UserModel user, string username)
        {
            Response<bool> res = Service.GetService().UserService.RemoveAppointOwner(user.id, user.shop, username);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Remove Owner successfuly!";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "EmployeesManagment", user);
        }

        public IActionResult Premissions(UserModel user,string namepre,string managePremissions,string action) 
        {
            if (action.Equals("addPre"))
            {
                return AddPremission(user, namepre, managePremissions);
            }
            if (action.Equals("removePre"))
            {
                return RemovePremission(user, namepre, managePremissions);
            }
            TempData["Message"] = "Error apear in pressing button";
            TempData["MessageType"] = "error";
            return RedirectToAction("Index", "EmployeesManagment", user);
        }
        public IActionResult AddPremission(UserModel user,string name,string managePremissions)
        {
            Permission pr;
            Enum.TryParse(managePremissions,out pr);
            Response<string> res = Service.GetService().ShopService.AddPermission(user.id,user.shop,name,pr);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "Add Premission successfuly!";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "EmployeesManagment", user);
        }

        public IActionResult RemovePremission(UserModel user, string name, string managePremissions)
        {
            Permission pr;
            Enum.TryParse(managePremissions, out pr);
            Response<bool> res = Service.GetService().UserService.RemovePermission(user.id, user.shop, name, pr);
            if (res == null || res.HasError)
            {
                TempData["Message"] = res.ErrorMsg;
                TempData["MessageType"] = "error";
            }
            else
            {
                TempData["Message"] = "remove Premission successfuly!";
                TempData["MessageType"] = "success";
            }
            return RedirectToAction("Index", "EmployeesManagment", user);
        }
    }
}
