using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.RestrictionsDiscount;

public class ItemInBasketRestriction : IPredicateDiscountExp
{
    
    private int itemId;
    
    public ItemInBasketRestriction(int itemId)
    {
        this.itemId = itemId;
    }
    public bool Evaluate(ShopBasket basket)
    {
        return basket.GetItems().Any(item => item.Item1.ItemId == itemId);
    }

    public override string ToString()
    {
        return "Item In Basket Restriction: " + itemId;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Item In Basket Restriction: " + itemId;
    }
}