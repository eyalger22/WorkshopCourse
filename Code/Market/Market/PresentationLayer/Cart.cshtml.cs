using Market.DataObject;
using Market.ServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market.Pages
{
    public class CartModel : PageModel
    {
        public DataObject.ShoppingCart cart;
        public Service service;
        public CartModel()
        {
            service = Service.GetService();
            cart = new DataObject.ShoppingCart();
        }
        public void OnGet()
        {
            // for view purposes only
            List<(Product, int)> pr1 = new List<(Product, int)>
            {
                (new Product(1,"product1",20.0,"category1", "description1", "shop1"), 3),
                (new Product(2,"product2",14.99,"category2", "description2", "shop1"), 1)
            };
            List<(Product, int)> pr2 = new List<(Product, int)>
            {
                (new Product(3,"product3",2.5,"category2", "description3", "shop1"), 1)
            };
            cart.Baskets.Add(("shop1", new ShoppingBasket("shop1", pr1)));
            cart.Baskets.Add(("shop2", new ShoppingBasket("shop2", pr2)));
        }
    }
}
