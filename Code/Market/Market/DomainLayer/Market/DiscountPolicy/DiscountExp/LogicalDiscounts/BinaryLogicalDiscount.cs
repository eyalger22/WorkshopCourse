using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;

public abstract class BinaryLogicalDiscount : IDiscountExp
{
    
    protected IPredicateDiscountExp expDiscount;
    protected IDiscountExp discountExp;
    
    public BinaryLogicalDiscount(IPredicateDiscountExp expDiscount, IDiscountExp discountExp){
        this.expDiscount = expDiscount;
        this.discountExp = discountExp;
    }
    public List<(Item, double)> EvaluateDiscount(ShopBasket basket)
    {
        if (expDiscount.Evaluate(basket))
        {
            return discountExp.EvaluateDiscount(basket);
        }
        return basket.GetItems().ToList().ConvertAll(t => (t.Item1, 1.0)).ToList();
    }

    public override string ToString()
    {
        return "Binary Logical Discount: " + expDiscount + " " + discountExp;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Binary Logical Discount: " + expDiscount + " " + discountExp;
    }
}