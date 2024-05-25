using Market.DomainLayer;

namespace Market.DataObject
{
    public class Product
    {

        public string Name { get; set; }
        public double Price { get; set; }
        public string Category { get; set; }
        public string? Description { get; set; }
        public int ProductId { get; set; }
        public int ShopId { get; set; }
        public string ShopName { get; set; }


        public Product(int itemId, string name, double price, string category, string description, int shopId=0)
        {
            this.ProductId = itemId;
            Name = name;
            this.Price = price;
            Category = category;
            Description = description;
            ShopId = shopId;
            //ShopName = shopName;
        }
        public Product(int itemId, string name, double price, string category, string description, string shopName)
        {
            this.ProductId = itemId;
            Name = name;
            this.Price = price;
            Category = category;
            Description = description;
            ShopName = shopName;
            Response<Shop> s = Facade.GetFacade().GetShopByName(ShopName);
            if (s.HasError)
            {
                ShopId = 0;
            }
            else
            {
                ShopId = s.Value.ShopId;
            }
        }
    }
}
