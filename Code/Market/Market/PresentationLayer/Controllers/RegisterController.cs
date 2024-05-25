using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;

namespace Market.Pages.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            return View("~/PresentationLayer/Views/Register/Index.cshtml",user);
        }
        public IActionResult Register(UserModel user,string username, string email, string password, string confirmPassword,string address,string phone, DateTime birthday)
        {
            // Call the registration service function
            bool success = false;
            if (password == confirmPassword)
            {
                Response<bool> registrationSuccess = Service.GetService().UserService.Register(username,password,email, address,phone,birthday);
                success = registrationSuccess.Value;
                if (success)
                {
                    TempData["Message"] = "Registration successful!";
                    TempData["MessageType"] = "success";
                }
                else
                {
                    TempData["Message"] = registrationSuccess.ErrorMsg;
                    TempData["MessageType"] = "error";
                }
                return RedirectToAction("index", "Register", user);
            }
            else
            {
                TempData["Message"] = "passwords not match";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("index", "Register", user);
        }
    }
}
