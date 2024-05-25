using Market.DataObject;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.NumericDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using System.Text;

namespace Market.DomainLayer.Market.DiscountPolicy;

public class DiscountPolicyManager
{
    public int idCounter = 1;
    public int predicateIdCounter = 1;
    private Repository<int, DiscountPolicy> policies;
    private Repository<int, DiscountPolicy> added_policies;
    private Repository<int, IPredicateDiscountExp> predicates;

    public DiscountPolicyManager()
    {
        this.policies = new SynchronizedListRepository<int, DiscountPolicy>();
        this.added_policies = new SynchronizedListRepository<int, DiscountPolicy>();
        this.predicates = new SynchronizedListRepository<int, IPredicateDiscountExp>();
    }

    public override string ToString()
    {
        int blankNum = 4;
        string blank = new string('-', blankNum);
        blank += ">";
        int i = 1;
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Applied Discount Policies:");
        sb.AppendLine();
        foreach (DiscountPolicy policy in policies.Values())
        {
            sb.AppendLine(i + ". " + policy.ToString(blankNum) + Environment.NewLine);
            sb.AppendLine();
            i++;
        }
        sb.AppendLine("Not Applied Discount Policies:");
        sb.AppendLine();
        i = 1;
        foreach (DiscountPolicy policy in added_policies.Values())
        {
            sb.AppendLine(i + ". " + policy.ToString(blankNum) + Environment.NewLine);
            sb.AppendLine();
            i++;
        }
        sb.AppendLine("Predicates:" + Environment.NewLine);
        sb.AppendLine();
        i = 1;
        foreach (IPredicateDiscountExp predicate in predicates.Values())
        {
            sb.AppendLine(i + ". " + predicate.ToString(blankNum) + "\n");
            sb.AppendLine();
            i++;
        }
        return sb.ToString();
        
    }


    public List<(Item, double)> GetDiscounts(ShopBasket basket)
    {
        
        IDiscountExp policy = new MaxDiscount(policies.Values().ToList().ConvertAll(item => item.Policy).ToList());
        return policy.EvaluateDiscount(basket);
    }

    public Response<int> AddDiscountPolicy(string description, IDiscountExp policy)
    {
        int id = idCounter++;
        DiscountPolicy discountPolicy = new DiscountPolicy(id, description, policy);
        Response<DiscountPolicy> response = added_policies.AddItem(discountPolicy, id);
        return response.HasError ? new Response<int>(response.ErrorMsg, 1) : new Response<int>(id);
    }

    public Response<IDiscountExp> GetDiscountPolicy(int id)
    {
        DiscountPolicy? response = policies.GetItem(id);
        if (response == null)
        {
            response = added_policies.GetItem(id);
        }
        return response is null ? new Response<IDiscountExp>("There is no policy", 1) : new Response<IDiscountExp>(response.Policy);
    }

    public Response<bool> ApplyDiscountPolicy(int id)
    {
        if (added_policies.ContainsKey(id))
        {
            DiscountPolicy d = added_policies.GetItem(id);
            policies.AddItem(d,id);
            added_policies.DeleteItem(id);
            return new Response<bool>(true);
        }
        return new Response<bool>("policy id is not valid or already applied",2);
    }
    
    
    public Response<int> AddPredicate(IPredicateDiscountExp predicate)
    {
        int id = predicateIdCounter++;
        Response<IPredicateDiscountExp> response = predicates.AddItem(predicate, id);
        return response.HasError ? new Response<int>(response.ErrorMsg, 1) : new Response<int>(id);
    }
    public Response<IPredicateDiscountExp> GetPredicate(int id)
    {
        IPredicateDiscountExp? response = predicates.GetItem(id);
        return response is null ? new Response<IPredicateDiscountExp>("There is no predicate", 1) : new Response<IPredicateDiscountExp>(response);
    }
}