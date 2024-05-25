using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.SalePolicy.PredicatePolicies;

public class CheckItemInBasket : IPredicateExp
{
    
    private Func<Item, bool> itemFuncDecider;

    public CheckItemInBasket(Func<Item, bool> itemFuncDecider)
    {
        this.itemFuncDecider = itemFuncDecider;
    }
    
    
    public bool CheckPolicy(ShopBasket basket)
    {
        return basket.GetItems().Any(item => itemFuncDecider(item.Item1));
    }

    public void changeItemFuncDecider(Func<Item, bool> func)
    {
        itemFuncDecider = func;
    }

    public override string ToString()
    {
        return "Check Item In Basket:" + itemFuncDecider.ToString();
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Check Item In Basket:" + itemFuncDecider.ToString();
    }
}