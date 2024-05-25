namespace Market.DataObject
{
    public class Shop
    {
        public string ShopAddress { get; set; }

        public string Name { get; set; }
        public int ShopId { get; set; }

        public bool IsClosed { get; set; }

        public Shop(int shopId, string name, string shopAddress, bool isClosed)
        {
            ShopId = shopId;
            Name = name;
            this.ShopAddress = shopAddress;
            IsClosed = isClosed;
        }
    }
}
