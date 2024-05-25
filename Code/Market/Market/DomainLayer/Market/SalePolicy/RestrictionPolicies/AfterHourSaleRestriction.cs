using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;

public class AfterHourSaleRestriction : IRestrictionExp
{
    public Func<Item, bool> itemFuncDecider;
    protected int hour;

    public AfterHourSaleRestriction(int hour)
    {
        this.hour = hour;
        itemFuncDecider = item => true;
    }
    // returns true if the current hour is after the hour of the restriction
    public virtual bool CheckPolicy(ShopBasket basket)
    {
        return DateTime.Now.Hour >= hour || basket.GetItems().All(item => !itemFuncDecider(item.Item1));
    }

    public void changeItemFuncDecider(Func<Item, bool> func)
    {
        itemFuncDecider = func;
    }

    public override string ToString()
    {
        return "After Hour Sale Restriction: " + hour;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "After Hour Sale Restriction: " + hour;
    }
}