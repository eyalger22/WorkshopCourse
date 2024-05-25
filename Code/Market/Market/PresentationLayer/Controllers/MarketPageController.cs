using Market.Pages.Modles;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Market.DataObject;
using Newtonsoft.Json;

namespace Market.Pages.Controllers
{
	public class MarketPageController : Controller
	{
		public IActionResult Index(UserModel userModel)
		{
			Response<List<Shop>> res = Service.GetService().ShopService.GetAllOpenShops();
			if (res == null || res.HasError)
			{
				TempData["Message"] = res.ErrorMsg;
				TempData["MessageType"] = "error";
			}
			else
			{
				string serializedShops = JsonConvert.SerializeObject(res.Value);
				if (!string.IsNullOrEmpty(serializedShops))
				{
					TempData["SerializedShop"] = serializedShops;
				}
			}
			return View("~/PresentationLayer/Views/MarketPage/Index.cshtml", userModel);
		}

		public IActionResult Home(UserModel userModel)
		{
			return RedirectToAction("Index", "MarketPage", userModel);
		}


		public IActionResult Search(UserModel userModel, string shopname)
		{
			Response<Shop> res = Service.GetService().ShopService.GetShopByName(shopname);
			if(res == null || res.HasError)
			{
				TempData["Message"] = "no such shop with this name";
				TempData["MessageType"] = "error";
			}
			else
			{
				string serializedShops = JsonConvert.SerializeObject(res.Value);
				if (!string.IsNullOrEmpty(serializedShops))
				{
					TempData["shop"] = serializedShops;
				}
				TempData["message"] = "Found shop";
				TempData["MessageType"] = "success";
			}
			return View("~/PresentationLayer/Views/MarketPage/Index.cshtml", userModel);
		}
	}
}
