namespace Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

public interface IPredicateDiscountExp
{
    public bool Evaluate(ShopBasket basket);

    public string ToString(int blankNum);
}