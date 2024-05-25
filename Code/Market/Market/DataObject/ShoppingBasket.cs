using Market.DomainLayer.Market;

namespace Market.DataObject
{
    public class ShoppingBasket
    {
        public string ShopName { get; set; }
        public List<(Product, int)> Products { get; set; }
        public ShoppingBasket(string shopName, IEnumerable<(Item,int)> items) {
            ShopName = shopName;
            Products = new List<(Product, int)> ();
            foreach(var item in items)
            {
                Products.Add((new Product(item.Item1.ItemId, item.Item1.Name, item.Item1.Price, item.Item1.Category, item.Item1.Description, item.Item1.ShopName), item.Item2));
            }
        }
        public ShoppingBasket(string shopName, IEnumerable<(Product, int)> items)
        {
            ShopName = shopName;
            Products = new List<(Product, int)>();
            foreach (var item in items)
            {
                Products.Add(item);
            }
        }
    }
}
