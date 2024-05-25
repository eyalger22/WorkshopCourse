using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;
using System.Text;

namespace Market.DomainLayer.Market.SalePolicy;

public class SalePolicy
{
    private int id;
    private string description;
    private ISalePolicy policy;
    
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
    
    public ISalePolicy Policy
    {
        get => policy;
        private set => policy = value;
    }
    public SalePolicy(int id, string description, ISalePolicy policy)
    {
        this.id = id;
        this.description = description;
        this.policy = policy;
    }

    public bool AbleToPurchase(ShopBasket basket)
    {
        return policy.CheckPolicy(basket);
    }

    public override string ToString()
    {
        return "\tId: " + id + "\n" +
               "\tDescription: " + description + "\n" +
               "\tPolicy: " + policy + "\n";
    }

    public string ToString(int blankNum)
    {
        string blank = new string('-', blankNum);
        blank += ">";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(blank + "\tId: " + id);
        sb.AppendLine(blank + "\tDescription: " + description);
        sb.AppendLine(blank + "\tPolicy: ");
        sb.AppendLine(policy.ToString(blankNum + 4));
        return sb.ToString();
    }
}