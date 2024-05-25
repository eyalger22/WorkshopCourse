namespace Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

public interface IDiscountExp
{
    public List<(Item, double)> EvaluateDiscount(ShopBasket basket);

    public String ToString(int blank);
}