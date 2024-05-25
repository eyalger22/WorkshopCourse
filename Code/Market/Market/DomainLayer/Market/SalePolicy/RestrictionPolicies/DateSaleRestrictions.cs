using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;

public class DateSaleRestrictions : IRestrictionExp
{
    private int year;
    private int month;
    private int day;
    public Func<Item, bool> itemFuncDecider;
    
    public DateSaleRestrictions(int year, int month, int day)
    {
        this.year = year;
        this.month = month;
        this.day = day;
        itemFuncDecider = item => true;
    }
    // returns true if the current date is matching the date of the restriction, you cant buy in the same date
    public bool CheckPolicy(ShopBasket basket)
    {
        DateTime now = DateTime.Now;
        bool cond1 = (year <= 0) || (year == now.Year);
        bool cond2 = (month <= 0) || (month == now.Month);
        bool cond3 = (day <= 0) || (day == now.Day);
        return !(cond1 && cond2 && cond3) || basket.GetItems().All(item => !itemFuncDecider(item.Item1));
    }
    
    public void changeItemFuncDecider(Func<Item, bool> func)
    {
        itemFuncDecider = func;
    }

    public override string ToString()
    {
        return "Date Sale Restrictions: " + day + "/" + month + "/" + year;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Date Sale Restrictions: " + day + "/" + month + "/" + year;
    }
}