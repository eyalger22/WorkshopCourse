using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;

public class ItemBasedRestriction : ISalePolicy
{
    
    private int itemId;
    public Func<Item, bool> itemFuncDecider;
    private ISalePolicy policy;
    
    public ItemBasedRestriction(int itemId, ISalePolicy policy)
    {
        this.itemId = itemId;
        this.policy = policy;
        itemFuncDecider = item => item.ItemId == itemId;
    }

    public bool CheckPolicy(ShopBasket basket)
    {
        policy.changeItemFuncDecider(itemFuncDecider);
        return policy.CheckPolicy(basket);
    }
    
    public void changeItemFuncDecider(Func<Item, bool> func)
    {
        itemFuncDecider = func;
    }

    public override string ToString()
    {
        return "Item Based Restriction: " + itemId + "\n" + policy;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Item Based Restriction: " + itemId + "\n" + policy.ToString(blankNum + 4);
    }
}