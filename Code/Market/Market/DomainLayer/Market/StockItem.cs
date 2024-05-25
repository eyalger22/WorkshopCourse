using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DomainLayer.Market
{
    public class StockItem
    {
        [NotMapped]
        private static int Counter = 0;
        [Key]
        public int Id { get; private set; }
        public int ShopId { get; private set; }
        public int ItemId { get; private set; }
        public int Amount { get; private set; }
        [NotMapped]
        public Item Item { get; private set; }
        public StockItem(int shopId, Item item, int amount)
        {
            ShopId = shopId;
            ItemId = item.ItemId;
            Amount = amount;
            Id = Counter++;
            Item = item;
        }

        public StockItem(int id, int shopId, int itemId, int amount)
        {
            ShopId = shopId;
            ItemId = itemId;
            Amount = amount;
            Id = id;
            Item = Facade.GetFacade().market.GetProduct(itemId);
        }
    }
}
