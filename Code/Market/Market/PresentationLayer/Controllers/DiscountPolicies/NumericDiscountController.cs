using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
namespace Market.PresentationLayer.Controllers.DiscountPolicies
{
    public class NumericDiscountController : Controller
    {
        public IActionResult Index(UserModel user)
        {
            Response<string> res = Service.GetService().ShopService.GetDiscountPolicies(user.id, user.shop);
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
            return View("~/PresentationLayer/Views/DiscountPolicy/NumericDiscount.cshtml", user);
        }

        public IActionResult Create(UserModel user,string description,string typeList, string AllPolicies)
        {
            List<int> Policies = ParseStringToIntList(AllPolicies);
            if (Policies.Count > 0)
            {
                Response<int> res = Service.GetService().ShopService.AddDiscountPolicyNumeric(user.id, user.shop, description, typeList, Policies);
                if (res == null || res.HasError)
                {
                    TempData["Message"] = res.ErrorMsg;
                    TempData["MessageType"] = "error";
                }
                else
                {
                    TempData["Message"] = "Add policy successfuly!";
                    TempData["MessageType"] = "success";
                    return RedirectToAction("Index", "DiscountPolicy", user);
                }
            }
            else
            {
                TempData["Message"] = "not entered policies right";
                TempData["MessageType"] = "error";
            }
            return RedirectToAction("Index", "NumericDiscount", user);
        }





        public List<int> ParseStringToIntList(string input)
        {
            List<int> intList = new List<int>();

            if (!string.IsNullOrEmpty(input))
            {
                string[] splitStrings = input.Split(',');

                foreach (string str in splitStrings)
                {
                    if (int.TryParse(str.Trim(), out int intValue))
                    {
                        intList.Add(intValue);
                    }
                }
            }

            return intList;
        }
    }
}
