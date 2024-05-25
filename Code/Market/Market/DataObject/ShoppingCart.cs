namespace Market.DataObject
{
    public class ShoppingCart
    {
        //private DomainLayer.Market.ShoppingCart cart;
        public List<(string, ShoppingBasket)> Baskets { get; set; }

        public ShoppingCart(DomainLayer.Market.ShoppingCart cart)
        {
            Baskets = new List<(string, ShoppingBasket)> ();
            foreach (var domainBasket in cart.GetBaskets())
            {
                ShoppingBasket basket = new ShoppingBasket(domainBasket.ShopName, domainBasket.GetItems());
                Baskets.Add((basket.ShopName, basket));
            }
        }
        public ShoppingCart()
        {
            Baskets = new List<(string, ShoppingBasket)>();
        }
        
    }
}
