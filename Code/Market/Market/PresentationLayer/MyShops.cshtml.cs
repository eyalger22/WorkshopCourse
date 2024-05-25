using Market.DataObject;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market.Pages
{
    public class MyShopsModel : PageModel
    {
        public Service service;
        public List<(string,string)> Jobs { get; set; }
        public MyShopsModel()
        {
            service = Service.GetService();
            Jobs = new List<(string,string)>();
            //shops = service.ShopService.
        }
        public void OnGet()
        {
            Jobs.Add(("shop1", "manager"));
            Jobs.Add(("shop2", "owner"));
        }
    }
}
