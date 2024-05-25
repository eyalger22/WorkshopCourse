using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

using Moq;
using System.Text;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.NumericDiscounts;

public class AddingDiscount : IDiscountExp
{
    
    
    private readonly List<IDiscountExp> _discounts;
    
    public AddingDiscount() : this(new List<IDiscountExp>())
    {
        
    }
    
    public AddingDiscount(List<IDiscountExp> discounts)
    {
        _discounts = discounts;
    }
    public List<(Item, double)> EvaluateDiscount(ShopBasket basket)
    {
        Dictionary<int, double> addingDiscount = new Dictionary<int, double>();
        Dictionary<int, Item> items = new Dictionary<int, Item>();
        foreach (var item in _discounts)
        {
            var discount = item.EvaluateDiscount(basket);
            foreach (var dis in discount)
            {
                items[dis.Item1.ItemId] = dis.Item1;
                if (!addingDiscount.ContainsKey(dis.Item1.ItemId))
                {
                    addingDiscount[dis.Item1.ItemId] = 0;
                }
                addingDiscount[dis.Item1.ItemId] = Math.Min(addingDiscount[dis.Item1.ItemId] + (dis.Item2 == 1 ? 0 : dis.Item2), 1);
            }
        }
        return addingDiscount.ToList().ConvertAll(item => (items[item.Key], item.Value == 0 ? 1 : item.Value));
    }

    public override string ToString()
    {
        return "Adding Discount: " + _discounts;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(blank + "Adding Discount: ");
        foreach (var item in _discounts)
        {
            sb.AppendLine(item.ToString(blankNum + 4));
        }
        return sb.ToString();
    }
}