using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;

public class LowerAgeRestriction : IRestrictionExp
{
    
    private int age;
    public Func<Item, bool> itemFuncDecider;
    public LowerAgeRestriction(int age)
    {
        this.age = age;
        itemFuncDecider = item => true;
    }
    public bool CheckPolicy(ShopBasket basket)
    {
        return basket.GetItems().All(item => !itemFuncDecider(item.Item1)) || basket.User.Age() >= age;
    }
    public void changeItemFuncDecider(Func<Item, bool> func)
    {
        itemFuncDecider = func;
    }

    public override string ToString()
    {
        return "Lower Age Restriction: " + age;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Lower Age Restriction: " + age;
    }
}