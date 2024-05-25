using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;

public class ShopBasedRestriction : IRestrictionExp
{
    
    private ISalePolicy policy;
    public Func<Item, bool> itemFuncDecider;
    public ShopBasedRestriction(ISalePolicy policy)
    {
        this.policy = policy;
        itemFuncDecider = item => true;
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
        return "Shop Based Restriction: " + policy;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Shop Based Restriction: \n" + policy.ToString(blankNum+4);
    }
}