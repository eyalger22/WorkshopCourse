namespace Market.DomainLayer.Market.DiscountStrategies;

public class ConditionalDiscountStrategy : IDiscountStrategy
{
    public ConditionalDiscountStrategy(List<Item> items, Shop shop, Func<(List<(Item, int)>, string), List<(Item, double)>> discountFunc, DateTime deadline) : base(items, shop, discountFunc, deadline)
    {
    }
}