using Market.DataObject;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;

namespace Market.Pages
{
    public class ShopsModel : PageModel
    {
        //private IHtmlHelper _htmlHelper;
        public Service service;
        public List<Shop> shops;
        public ShopsModel(IHtmlHelper htmlHelper)
        {
            service = Service.GetService();
            shops = new List<Shop>();
            //_htmlHelper = htmlHelper;
        }
        [BindProperty]
        public string searchInput { get; set; }
        [BindProperty]
        public string bottomMessage { get; set; }
        public void OnGet()
        {
            Response<List<Shop>> response = service.ShopService.GetAllOpenShops();

            if (!response.HasError)
            {
                shops = response.Value;
            }
            else
            {
                bottomMessage = "There was an error, try again";
            }
        }

        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
