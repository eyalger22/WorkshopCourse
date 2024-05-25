using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.PredicatePolicies;

public class CheckItemAmountLessPredicate : IPredicateExp
{
    
    public Func<Item, bool> itemFuncDecider;
    protected int amount;
    
    public CheckItemAmountLessPredicate(Func<Item, bool> itemFuncDecider, int amount)
    {
        this.itemFuncDecider = itemFuncDecider;
        this.amount = amount;
    }
    public virtual bool CheckPolicy(ShopBasket basket)
    {
        return basket.GetItems().All(item => !itemFuncDecider(item.Item1) || item.Item2 < amount);
    }

    public void changeItemFuncDecider(Func<Item, bool> func)
    {
        itemFuncDecider = func;
    }

    public override string ToString()
    {
        return "Check Item Amount Less Predicate: " + itemFuncDecider + " " + amount;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Check Item Amount Less Predicate: " + itemFuncDecider + " amount: " + amount;
    }
}