using Market.ORM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DomainLayer.Market
{
    public class ShopBasketItem
    {
        private static int Counter = -1;

        [Key]
        public int Id { get; private set; }
        public string UserName { get; set; }
        public string ShopName { get; set; }

        public int ItemId { get; private set; }
        [NotMapped]
        public Item Item { get; private set; }
        public int Amount { get; set; }
        public ShopBasketItem(string userName, string shopName, Item item, int amount)
        {
            UpdateCounter();
            UserName = userName;
            ShopName = shopName;
            Item = item;
            Amount = amount;
            ItemId = item.ItemId;
            Id = Counter++;
        }

        public ShopBasketItem(int id, string userName, string shopName, int itemId, int amount)
        {
            UserName = userName;
            ShopName = shopName;
            Item = Facade.GetFacade().market.GetProduct(itemId);
            ItemId = itemId;
            Amount = amount;
            Id = id;
        }

        private static void UpdateCounter()
        {
            if (Counter == -1)
            {
                Counter = MarketContext.Instance.ShopBasketsItems.Count();
                if (Counter > 0)
                {
                    Counter = MarketContext.Instance.Shops.Max(x => x.ShopId) + 1;
                }
                else
                {
                    Counter++;
                }
            }
        }
    }
}
