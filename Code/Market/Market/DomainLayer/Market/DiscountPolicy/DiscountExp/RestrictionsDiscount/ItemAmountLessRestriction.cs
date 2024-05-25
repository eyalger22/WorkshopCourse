using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.RestrictionsDiscount;

public class ItemAmountLessRestriction : IPredicateDiscountExp
{
    
    private int itemId;
    
    private int amount;
    
    public ItemAmountLessRestriction(int itemId, int amount)
    {
        this.itemId = itemId;
        this.amount = amount;
    }
    public bool Evaluate(ShopBasket basket)
    {
        return basket.GetItems().Any(item => item.Item1.ItemId == itemId && item.Item2 < amount);
    }

    public override string ToString()
    {
        return "Item Amount Less Restriction: " + itemId + " Amount: " + amount;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Item Amount Less Restriction: " + itemId + " Amount: " + amount;
    }

}