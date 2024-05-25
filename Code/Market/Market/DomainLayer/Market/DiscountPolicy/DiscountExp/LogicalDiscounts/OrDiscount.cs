using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;

public class OrDiscount : BinaryLogicalDiscount
{
    
    public OrDiscount(OrExpDiscount orExpDiscount, IDiscountExp discountExp) : base(orExpDiscount, discountExp)
    { }

    public override string ToString()
    {
        return "Or Discount: " + expDiscount.ToString() + " " + discountExp.ToString();
    }
}