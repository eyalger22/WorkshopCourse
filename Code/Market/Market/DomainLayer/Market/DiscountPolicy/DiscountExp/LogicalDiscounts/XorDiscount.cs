using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;

public class XorDiscount : BinaryLogicalDiscount
{
    public XorDiscount(XorExpDiscount xorExpDiscount, IDiscountExp discountExp) : base(xorExpDiscount, discountExp)
    {
    }

    public override string ToString()
    {
        return "Xor Discount: " + expDiscount.ToString() + " " + discountExp.ToString();
    }
}