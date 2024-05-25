using Humanizer.Localisation.TimeToClockNotation;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.BasicDiscounts;

public class CategoryDiscount : IDiscountExp
{
    private double _discount;

    public double Discount
    {
        get => _discount;
        private set => _discount = value;
    }

    private string category;

    public CategoryDiscount(double discount, string category)
    {
        Discount = discount;
        this.category = category;
    }

    public List<(Item, double)> EvaluateDiscount(ShopBasket basket)
    {
        return basket.GetItems().ToList().ConvertAll(item => (item.Item1, (category == item.Item1.Category ? Discount : 1)));
    }

    public override string ToString()
    {
        return "Category Discount: " + category + " Discount: " + Discount;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Category Discount: " + category + " Discount: " + Discount;
    }
}