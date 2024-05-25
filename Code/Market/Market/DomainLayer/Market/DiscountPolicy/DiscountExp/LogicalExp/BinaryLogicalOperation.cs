using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;

public abstract class BinaryLogicalOperation : IPredicateDiscountExp
{
    
    
    protected IPredicateDiscountExp operand1;
    protected IPredicateDiscountExp operand2;
    
    public BinaryLogicalOperation(IPredicateDiscountExp operand1, IPredicateDiscountExp operand2)
    {
        this.operand1 = operand1;
        this.operand2 = operand2;
    }

    public abstract bool Evaluate(ShopBasket basket);

    public abstract string ToString(int blankNum);
}