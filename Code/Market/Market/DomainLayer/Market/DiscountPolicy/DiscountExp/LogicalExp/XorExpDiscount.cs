using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;
using System.Text;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;

public class XorExpDiscount: BinaryLogicalOperation
{
    public XorExpDiscount(IPredicateDiscountExp operand1, IPredicateDiscountExp operand2) : base(operand1, operand2)
    {
    }

    public override bool Evaluate(ShopBasket basket)
    {
        bool cond1 = operand1.Evaluate(basket);
        bool cond2 = operand2.Evaluate(basket);
        return cond1 ^ cond2;
    }

    public override string ToString()
    {
        return "Xor Exp: " + operand1.ToString() + " XOR" + operand2.ToString();
    }

    public override string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(blank + "Xor Discount Exp:");
        sb.AppendLine(operand1.ToString(blankNum + 4));
        sb.AppendLine(operand2.ToString(blankNum + 4));
        return sb.ToString();

    }
}