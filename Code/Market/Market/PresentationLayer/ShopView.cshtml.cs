using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market.Pages
{
    public class ShopViewModel : PageModel
    {
        public Service service;
        public string ShopName { get; set; }
        public ShopViewModel()
        {
            service = Service.GetService();
        }
        public void OnGet()
        {
            ShopName = "shop name";
        }
    }
}
