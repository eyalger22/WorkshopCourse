using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;

public class CategoryBasedRestriction : IRestrictionExp
{
    
    private string CategoryName;
    public Func<Item, bool> itemFuncDecider;
    private ISalePolicy policy;
    
    public CategoryBasedRestriction(string categoryName, ISalePolicy policy)
    {
        this.CategoryName = categoryName;
        itemFuncDecider = item => item.Category == CategoryName;
        this.policy = policy;
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
        return "CategoryBasedRestriction: " + CategoryName + "\n\t" + policy;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "CategoryBasedRestriction: " + CategoryName + "\n\t" + policy.ToString(blankNum + 4);
    }
}