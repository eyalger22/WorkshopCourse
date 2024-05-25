using Market.DataObject;
using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;
using System.Text;

namespace Market.DomainLayer.Market.SalePolicy;

public class SalePolicyManager
{
    public int idCounter = 1;
    private Repository<int, SalePolicy> policies;
    private Repository<int, SalePolicy> added_policies;

    public SalePolicyManager()
    {
        this.policies = new SynchronizedListRepository<int, SalePolicy>();
        this.added_policies = new SynchronizedListRepository<int, SalePolicy>();
    }
    
    
    public bool AbleToPurchase(ShopBasket basket)
    {
        return policies.Values().All(item => item.AbleToPurchase(basket));
    }

    public Response<int> AddSalePolicy(string description, ISalePolicy policy)
    {
        int id = idCounter++;
        SalePolicy salePolicy = new SalePolicy(id, description, policy);
        Response<SalePolicy> response = added_policies.AddItem(salePolicy, id);
        return response.HasError ? new Response<int>(response.ErrorMsg, 1) : new Response<int>(id);
    }
    
    public Response<ISalePolicy> GetSalePolicy(int id)
    {
        SalePolicy? response = policies.GetItem(id);
        if(response == null)
        {
            response = added_policies.GetItem(id);
        }
        return response is null ? new Response<ISalePolicy>("There is no policy", 1) : new Response<ISalePolicy>(response.Policy);
    }

    public override string ToString()
    {
        int blankNum = 4;
        string blank = new string('-', blankNum);
        blank += ">";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Applied Sale Policies:\n");
        foreach (SalePolicy policy in policies.Values())
        {
            sb.AppendLine(policy.ToString(blankNum));
        }
        sb.AppendLine("Not Applied Sale Policies:");
        sb.AppendLine();
        foreach (SalePolicy policy in added_policies.Values())
        {
            sb.AppendLine(policy.ToString(blankNum));
        }
        return sb.ToString();
    }
    public Response<bool> ApplySalePolicy(int policyId)
    {
        if (added_policies.ContainsKey(policyId))
        {
            SalePolicy policy = added_policies.GetItem(policyId);
            policies.AddItem(policy ,policyId);
            added_policies.DeleteItem(policyId);
            return new Response<bool>(true);
        }
        return new Response<bool>("policy id is not valid or already applied", 2);
    }

}