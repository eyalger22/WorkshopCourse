using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;
using System.Text;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;

public class AndExpDiscount : BinaryLogicalOperation
{
    public AndExpDiscount(IPredicateDiscountExp operand1, IPredicateDiscountExp operand2) : base(operand1, operand2)
    {
    }

    public override bool Evaluate(ShopBasket basket)
    {
        return operand1.Evaluate(basket) && operand2.Evaluate(basket);
    }

    public override string ToString()
    {
        return "And Exp: " + operand1.ToString() + " AND " + operand2.ToString();
    }

    public override string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(blank + "And Discount Exp:");
        sb.AppendLine(operand1.ToString(blankNum + 4));
        sb.AppendLine(operand2.ToString(blankNum + 4));
        return sb.ToString();

    }
}