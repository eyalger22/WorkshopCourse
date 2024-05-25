using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy.DiscountExp.BasicDiscounts;

public class SingleItemDiscount : IDiscountExp
{
    private double _discount;
    
    public double Discount {
        get => _discount;
        private set => _discount = value;
    }
    
    private int itemId;
    
    public SingleItemDiscount(double discount, int itemId)
    {
        Discount = discount;
        this.itemId = itemId;
    }

    public List<(Item, double)> EvaluateDiscount(ShopBasket basket)
    {
        return basket.GetItems().ToList().ConvertAll(item => (item.Item1,(itemId == item.Item1.ItemId ? Discount : 1)));
    }

    public override string ToString()
    {
        return "Single Item Discount: " + itemId + " Discount: " + Discount;
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        return blank + "Single Item Discount: " + itemId + " Discount: " + Discount;
    }
}