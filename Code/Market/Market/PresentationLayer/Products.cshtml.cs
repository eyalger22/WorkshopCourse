using Market.DataObject;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market.Pages
{
    public class ProductsModel : PageModel
    {
        public Service service;

        public List<Product> products;
        public ProductsModel()
        {
            service = Service.GetService();
            products = new List<Product>();
        }
        [BindProperty]
        public string searchInput { get; set; }
        [BindProperty]
        public string bottomMessage { get; set; }
        public void OnGet()
        {
            Response<List<Product>> response = service.ShopService.SearchProducts();

            if (!response.HasError)
            {
                products = response.Value;
            }
            else
            {
                bottomMessage = "There was an error, try again";
            }
        }
        public IActionResult OnPost()
        {
            Response<List<Product>> response = service.ShopService.SearchProducts(searchInput);
            if (!response.HasError)
            {
                //products = response.Value;
                //products.Add(new Product(1,"Product1",20.0,"Category1","something1", "shop1"));
                //products.Add(new Product(2, "Product2", 10.0, "Category1", "something2", "shop1"));
                //products.Add(new Product(3, "Product3", 5.2, "Category2", "something3", "shop1"));
                //products.Add(new Product(4, "Product4", 30.0, "Category3", "something4", "shop1"));
                //products.Add(new Product(5, "Product5", 4.0, "Category1", "something5", "shop1"));
                //products.Add(new Product(6, "Product6", 7.5, "Category1", "something6", "shop1"));
                //products.Add(new Product(7, "Product7", 3.0, "Category3", "something7", "shop1"));
                //products.Add(new Product(8, "Product8", 19.0, "Category1", "something8", "shop1"));
                //products.Add(new Product(9, "Product9", 103.0, "Category2", "something9", "shop1"));
                //products.Add(new Product(10, "Product10", 9.99, "Category1", "something10", "shop1"));
                //products.Add(new Product(11, "Product11", 32.0, "Category1", "something11", "shop1"));
                //products.Add(new Product(12, "Product12", 20.0, "Category1", "something12", "shop1"));
                //bottomMessage = "";
            }
            else
            {
                bottomMessage = "There was an error, try again";
            }
            return Page();
        }
    }
}
