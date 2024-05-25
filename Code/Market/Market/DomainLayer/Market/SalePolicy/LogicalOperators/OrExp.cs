using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;
using System.Text;

namespace Market.DomainLayer.Market.SalePolicy.LogicalOperators;

public class OrExp: ISalePolicy
{
    private ISalePolicy operand1;
    private ISalePolicy operand2;
    
    public OrExp(ISalePolicy operand1, ISalePolicy operand2)
    {
        this.operand1 = operand1;
        this.operand2 = operand2;
    }
    public bool CheckPolicy(ShopBasket basket)
    {
        return operand1.CheckPolicy(basket) || operand2.CheckPolicy(basket);
    }
    
    public void changeItemFuncDecider(Func<Item, bool> func)
    {
        operand1.changeItemFuncDecider(func);
        operand2.changeItemFuncDecider(func);
    }

    public override string ToString()
    {
        return operand1 + " OR " + operand2;
    }
    
    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(blank + "Or Sale Exp:");
        sb.AppendLine(operand1.ToString(blankNum + 4));
        sb.AppendLine(operand2.ToString(blankNum + 4));
        return sb.ToString();
    }
}