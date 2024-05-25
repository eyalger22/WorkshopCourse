using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using System.Text;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.NumericDiscounts;

public class MaxDiscount : IDiscountExp
{
    private List<IDiscountExp> _discounts;

    public MaxDiscount() : this(new List<IDiscountExp>())
    {
        
    }
    
    public MaxDiscount(List<IDiscountExp> discounts)
    {
        _discounts = discounts;
    }

    public List<(Item, double)> EvaluateDiscount(ShopBasket basket)
    {
        double maxDiscountValue = -1;
        List<(Item, double)> maxDiscount = new List<(Item, double)>();
        foreach (var item in _discounts)
        {
            var discount = item.EvaluateDiscount(basket);
            var discountValue = SumBasket(discount, basket);
            if (maxDiscountValue == -1 || discountValue < maxDiscountValue)
            {
                maxDiscountValue = discountValue;
                maxDiscount = discount;
            }
        }
        return maxDiscount;
    }
    
    private double SumBasket(List<(Item, double)> basket, ShopBasket shopBasket)
    {
        var lst = shopBasket.GetItems().ToList();
        double ans = 0;
        for(int i = 0; i < basket.Count; i++)
        {
            ans += basket[i].Item1.Price * (1 - basket[i].Item2 == 0 ? 1 : 1 - basket[i].Item2 ) * lst[i].Item2;
        }
        return ans;
    }

    public override string ToString()
    {
        return "Max Discount: " + _discounts;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(blank + "Max Discount: ");
        foreach (var item in _discounts)
        {
            sb.AppendLine(item.ToString(blankNum + 4));
        }
        return sb.ToString();
    }
}