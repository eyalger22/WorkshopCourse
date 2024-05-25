using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market.Pages
{
    public class ShopPromotionModel : PageModel
    {
        public Service service;
        [BindProperty]
        public string UserNameInput { get; set; }
        public ShopPromotionModel()
        {
            service = Service.GetService();
        }
        public void OnGet()
        {
        }
    }
}
