using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp;

public class ConditionalDiscount : IDiscountExp
{
    
    private IPredicateDiscountExp condition;
    
    private IDiscountExp discount;
    
    public ConditionalDiscount(IPredicateDiscountExp condition, IDiscountExp discount)
    {
        this.condition = condition;
        this.discount = discount;
    }
    public List<(Item, double)> EvaluateDiscount(ShopBasket basket)
    {
        if(condition.Evaluate(basket))
            return discount.EvaluateDiscount(basket);
        return basket.GetItems().ToList().ConvertAll(item => (item.Item1, 1.0));
    }

    public override string ToString()
    {
        return "Conditional Discount: " + condition + " discount: " + discount;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Conditional Discount: " + condition + " discount: " + discount;
    }
}