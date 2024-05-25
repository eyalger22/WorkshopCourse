using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.PredicatePolicies;

public class CheckItemAmountMoreEqualsPredicate : CheckItemAmountLessPredicate
{
    public CheckItemAmountMoreEqualsPredicate(Func<Item, bool> itemFuncDecider, int amount) : base(itemFuncDecider, amount)
    {
        
    }
    
    
    public override bool CheckPolicy(ShopBasket basket)
    {
        return basket.GetItems().All(item => !itemFuncDecider(item.Item1) || item.Item2 >= amount);
    }

    public override string ToString()
    {
        return "Check Item Amount More Equals Predicate: " + amount;
    }
}