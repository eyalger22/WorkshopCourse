using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;
using System.Text;

namespace Market.DomainLayer.Market.SalePolicy.LogicalOperators;

public class ImpliesExp: ISalePolicy
{
    // if condition is true and allowing is false then the policy is false otherwise true
    private ISalePolicy condition;
    private ISalePolicy allowing;
    
    
    public ImpliesExp(ISalePolicy condition, ISalePolicy allowing)
    {
        this.condition = condition;
        this.allowing = allowing;
    }
    public bool CheckPolicy(ShopBasket basket)
    {
        return !condition.CheckPolicy(basket) || allowing.CheckPolicy(basket);
    }
    public void changeItemFuncDecider(Func<Item, bool> func)
    {
        condition.changeItemFuncDecider(func);
        allowing.changeItemFuncDecider(func);
    }

    public override string ToString()
    {
        return condition + " Implies: " + allowing;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(blank + "Implies: \n ");
        sb.AppendLine(blank + "Condition");
        sb.AppendLine(condition.ToString(blankNum + 4));
        sb.AppendLine(blank + "allowing:");
        sb.AppendLine(allowing.ToString(blankNum + 4));

        return sb.ToString();
    }
}