using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;

public class AndDiscount : BinaryLogicalDiscount
{

    public AndDiscount(AndExpDiscount andExpDiscount, IDiscountExp discountExp) : base(andExpDiscount, discountExp)
    { }

    public override string ToString()
    {
        return "And Discount: " + expDiscount.ToString() + " " + discountExp.ToString();
    }
}