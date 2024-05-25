using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.BasicDiscounts;

public class ShopDiscount : IDiscountExp
{
    private double _discount;
    
    public double Discount {
        get => _discount;
        private set => _discount = value;
    }
    public ShopDiscount(double discount)
    {
        Discount = discount;
    }

    public List<(Item, double)> EvaluateDiscount(ShopBasket basket)
    {
        return basket.GetItems().ToList().ConvertAll(item => (item.Item1,Discount));
        //return basket.GetItems().ToList().ConvertAll(item => (item.Item1, item.Item1.Price * Discount));
    }

    public override string ToString()
    {
        return "Shop Discount: " + Discount;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Shop Discount: " + Discount;
    }
}