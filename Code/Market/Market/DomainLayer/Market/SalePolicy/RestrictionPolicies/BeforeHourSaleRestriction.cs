namespace Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;

public class BeforeHourSaleRestriction : AfterHourSaleRestriction
{
    
    
    public BeforeHourSaleRestriction(int hour) : base(hour)
    {
    }

    public override bool CheckPolicy(ShopBasket basket)
    {
        return DateTime.Now.Hour < hour || basket.GetItems().All(item => !itemFuncDecider(item.Item1));
    }

    public override string ToString()
    {
        return "Before Hour Sale Restriction: " + hour;
    }
    
    
    
}