using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;

namespace Market.DomainLayer.Market.DiscountPolicy;

public class DiscountPolicy
{
    private int id;
    private string description;
    private IDiscountExp policy;
    
    public int Id
    {
        get => id;
        private set => id = value;
    }
    public string Description
    {
        get => description;
        private set => description = value;
    }

    public IDiscountExp Policy
    {
        get => policy;
        private set => policy = value;
    }
    public DiscountPolicy(int id, string description, IDiscountExp policy)
    {
        this.id = id;
        this.description = description;
        this.policy = policy;
    }

    public List<(Item, double)> GetDiscounts(ShopBasket basket)
    {
        return policy.EvaluateDiscount(basket);
    }

    public override string ToString()
    {
        return ">Id: " + id + "\r\n" +
               "\tDescription: " + Description + "\r\n" +
               "\tPolicy: " + Policy.ToString() + "\r\n";
    }
    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank+= ">";
        return blank + "Id: " + id + "\r\n" +
               blank + "\tDescription: " + Description + "\r\n" +
               blank + "\tPolicy: \r\n" + 
               Policy.ToString(blankNum + 4) + "\r\n";
    }
}