//using Market.DataObject;
using Market.DataObject;
using Market.DomainLayer.Market.AppointeesTrees;
using Market.DomainLayer.Market.DiscountStrategies;
using Market.DomainLayer.Market.SaleStrategies;
using Market.DomainLayer.Market.UserPermissions;
using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;
using System.Runtime.CompilerServices;
using System.Xml.XPath;
using Market.DomainLayer.Market.DiscountPolicy;
using Market.DomainLayer.Market.SalePolicy;
using OrdersHistory = Market.DataObject.OrderRecords.OrdersHistory;
using System.Xml.Linq;
using System.Xml.Schema;
using Market.DataObject.OrderRecords;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.BasicDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.LogicalExp;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.NumericDiscounts;
using Market.DomainLayer.Market.DiscountPolicy.DiscountExp.RestrictionsDiscount;
using Market.DomainLayer.Market.DiscountPolicy.DiscountInterfaces;
using Market.DomainLayer.Market.SalePolicy.LogicalOperators;
using Market.DomainLayer.Market.SalePolicy.PredicatePolicies;
using Market.DomainLayer.Market.SalePolicy.RestrictionPolicies;
using Market.DomainLayer.Market.SalePolicy.SaleInterfaces;
using Market.ORM;
using System.Diagnostics.Metrics;
using Microsoft.Owin.Security;
using Market.DomainLayer.Hubs;

namespace Market.DomainLayer.Market;

public class Shop
{
    [NotMapped]
    private static int shopIdCounter = -1;
    [NotMapped]
    private SalePolicyManager salePolicy;
    [NotMapped]
    public DiscountPolicyManager discountPolicy;
    [NotMapped]
    public DataObject.Repository<string, PastOrder> orders { get; private set; }
    [NotMapped]
    private OrdersHistory ordersHistory;
    [NotMapped]
    public OrdersHistory OrderHistory => ordersHistory;
    [NotMapped]
    public Stock Stock { get; private set; }
    //private AppointeesTree owners;
    [NotMapped]
    private AppointeesTree appointeesTree;
    [NotMapped]
    public Member Founder { get; private set; }

    public bool IsClosed { get; set; }
    [NotMapped]
    private Repository<string, Permissions> permissions;
    [NotMapped]
    private DataObject.Repository<string, ShopBasket> shopBaskets;
    [NotMapped]
    private string name;

    public string Name
    {
        get => name;
        private set => name = value;
    }
    [NotMapped]
    private int rank;

    public int Rank
    {
        get => rank;
        private set => rank = value;
    }

    [Key]
    public int ShopId { get; set; }
    
    public string ShopAddress { get; private set; }

    public string FounderName { get; private set; }

    public string? ShopBank { get; private set; }

    private void InitRepositories()
    {
        if (!Facade.InTestMode)
        {
            orders = new DataObject.ListRepository<string, PastOrder>();
            permissions = new DBRepositoryPartialKeyDouble<string, Permissions>(MarketContext.Instance.Permissions, x => x.ShopName == Name, Name, 2);
            //permissions = new DataObject.ListRepository<string, Permissions>();
            shopBaskets = new DBRepositoryPartialKeyDouble<string, ShopBasket>(MarketContext.Instance.ShopBaskets, x => x.ShopName == Name, Name, 2);
            //shopBaskets = new DataObject.ListRepository<string, ShopBasket>();
        }
        else
        {
            orders = new DataObject.ListRepository<string, PastOrder>();
            permissions = new DataObject.ListRepository<string, Permissions>();
            shopBaskets = new DataObject.ListRepository<string, ShopBasket>();
        }
        ordersHistory = new OrdersHistory();
        Stock = new Stock(Name);
        salePolicy = new SalePolicyManager();
        discountPolicy = new DiscountPolicyManager();
    }

    private static void UpdateCounter()
    {
        if (shopIdCounter == -1)
        {
            shopIdCounter = MarketContext.Instance.Shops.Count();
            if (shopIdCounter > 0)
            {
                shopIdCounter = MarketContext.Instance.Shops.Max(x => x.ShopId) + 1;
            }
            else
            {
                shopIdCounter++;
            }
        }
    }
    //For ORM
    public Shop(int shopId, string name, bool IsClosed, string shopAddress, int rank, string founderName, string shopBank)
    {
        ShopId = shopId;
        ShopAddress = shopAddress;
        Rank = rank;
        Founder = Facade.GetFacade().GetDomainUser(founderName);
        FounderName = founderName;
        Name = name;
        this.IsClosed = IsClosed;
        ShopBank = shopBank;
        InitRepositories();
        appointeesTree = new AppointeesTree(Founder, Name);
        IDiscountExp exp = new ShopDiscount(0.0);
        Response<int> id = discountPolicy.AddDiscountPolicy("First policy", exp);
        discountPolicy.ApplyDiscountPolicy(id.Value);
    }

    public Shop(string name, Member founder, string address, string shopBank)
    {
        UpdateCounter();
        Name = name;
        ShopId = shopIdCounter++;
        InitRepositories();
        ValidationCheck.Validate(!founder.IsMember(), "Founder must be a member");
        Founder = (Member)founder;
        permissions[Founder.Name] = new FounderPermissions(founder, this);
        appointeesTree = new AppointeesTree(founder, Name);
        Founder.AddShopPermissions(this, permissions[Founder.Name]);
        IsClosed = false;
        ShopBank = shopBank;
        Rank = 0;
        ShopAddress = address;
        FounderName = Founder.Name;
        IDiscountExp exp = new ShopDiscount(0.0);
        Response<int> id = discountPolicy.AddDiscountPolicy("First policy", exp);
        discountPolicy.ApplyDiscountPolicy(id.Value);

    }

    public string GetDiscountPolicyManager()
    {
        return discountPolicy.ToString();
    }
    public string GetSalePolicyManager()
    {
        return salePolicy.ToString();
    }

    private Response<int> AddDiscountPolicy(Member member, string description, IDiscountExp policy)
    {
        if (member.HasPermission(this, PermissionsEnum.Permission.MANAGE_POLICIES))
        {
            return discountPolicy.AddDiscountPolicy(description, policy);
        }
        return new Response<int>("Member doesn't have permission to add discount policy", 1);
    }
    public Response<bool> ApplyDiscountPolicy(Member member, int policy)
    {
        if (member.HasPermission(this, PermissionsEnum.Permission.MANAGE_POLICIES))
        {
            return discountPolicy.ApplyDiscountPolicy(policy);
        }
        return new Response<bool>("Member doesn't have permission to add discount policy", 1);
    }

    public Response<int> BuildDiscountPredicate(Member member, string type, int firstParam, int? secondParam = 0)
    {
        if (member.HasPermission(this, PermissionsEnum.Permission.MANAGE_POLICIES))
        {
            if (type == "And" || type == "Or" || type == "Xor")
            {
                if (!secondParam.HasValue)
                    return new Response<int>("Second param is null", 1);
                Response<IPredicateDiscountExp> first = discountPolicy.GetPredicate(firstParam);
                Response<IPredicateDiscountExp> second = discountPolicy.GetPredicate(secondParam.Value);
                if (first.HasError || second.HasError)
                    return new Response<int>("One of the predicates doesn't exist", 1);
                if (type == "And")
                    return discountPolicy.AddPredicate(new AndExpDiscount(first.Value, second.Value));
                if (type == "Or")
                    return discountPolicy.AddPredicate(new OrExpDiscount(first.Value, second.Value));
                if (type == "Xor")
                    return discountPolicy.AddPredicate(new XorExpDiscount(first.Value, second.Value));
            }
            else if (type == "Amount Less")
            {
                if (!secondParam.HasValue)
                    return new Response<int>("Second param is null", 1);
                return discountPolicy.AddPredicate(new ItemAmountLessRestriction(firstParam, secondParam.Value));
            }
            else if (type == "Amount More")
            {
                if (!secondParam.HasValue)
                    return new Response<int>("Second param is null", 1);
                return discountPolicy.AddPredicate(new ItemAmountMoreEqualsRestriction(firstParam, secondParam.Value));
            }
            else if (type == "Item In Basket")
            {
                return discountPolicy.AddPredicate(new ItemInBasketRestriction(firstParam));
            }
            else
            {
                return new Response<int>("Type is not valid", 1);
            }
            
        }     
        return new Response<int>("Member doesn't have permission to add discount predicate", 1);
    }
    private Response<int> AddSalePolicy(Member member, string description, ISalePolicy policy)
    {
        if (member.HasPermission(this, PermissionsEnum.Permission.MANAGE_POLICIES))
        {
            return salePolicy.AddSalePolicy(description, policy);
        }
        return new Response<int>("Member doesn't have permission to add sale policy", 1);
    }

    public Response<bool> ApplySalePolicy(Member member, int policy)
    {
        if (member.HasPermission(this, PermissionsEnum.Permission.MANAGE_POLICIES))
        {
            return salePolicy.ApplySalePolicy( policy);
        }
        return new Response<bool>("Member doesn't have permission to add sale policy", 1);
    }

    public Response<int> AddDiscountPolicyNumeric(Member member, string description, string type, List<int> policies)
    {
        List<IDiscountExp> policiesList = new List<IDiscountExp>();
        foreach (var i in policies)
        {
            Response<IDiscountExp> res = discountPolicy.GetDiscountPolicy(i);
            if (res.HasError)
            {
                return new Response<int>(res.ErrorMsg, 1);
            }
            policiesList.Add(res.Value);
        }

        IDiscountExp? ipolicy = null;
        if (type == "Adding")
        {
            ipolicy = new AddingDiscount(policiesList);
        } 
        else if (type == "Max")
        {
            ipolicy = new MaxDiscount(policiesList);
        }
        else
        {
            return new Response<int>("Type is not valid", 1);
        }

        return AddDiscountPolicy(member, description, ipolicy);
    }

    public Response<int> AddDiscountPolicyConditional(Member member, string description, int policyId, int predicateId)
    {
        Response<IDiscountExp> res = discountPolicy.GetDiscountPolicy(policyId);
        if (res.HasError)
        {
            return new Response<int>(res.ErrorMsg, 1);
        }
        Response<IPredicateDiscountExp> predicateDiscountExp = GetDiscountPredicate(predicateId);
        if (predicateDiscountExp.HasError)
        {
            return new Response<int>(predicateDiscountExp.ErrorMsg, 1);
        }
        IDiscountExp? ipolicy = new ConditionalDiscount(predicateDiscountExp.Value, res.Value);
        return AddDiscountPolicy(member, description, ipolicy);
    }

    public Response<int> AddDiscountPolicyLogical(Member member, string description, string type, int policyId, int predicateId)
    {
        Response<IDiscountExp> res = discountPolicy.GetDiscountPolicy(policyId);
        if (res.HasError)
        {
            return new Response<int>(res.ErrorMsg, 1);
        }
        Response<IPredicateDiscountExp> predicateDiscountExp = GetDiscountPredicate(predicateId);
        if (predicateDiscountExp.HasError)
        {
            return new Response<int>(predicateDiscountExp.ErrorMsg, 1);
        }
        IDiscountExp? ipolicy = null;
        if (type == "And")
        {
            ipolicy = new AndDiscount((AndExpDiscount)predicateDiscountExp.Value, res.Value);
        }
        else if (type == "Or")
        {
            ipolicy = new OrDiscount((OrExpDiscount)predicateDiscountExp.Value, res.Value);
        }
        else if (type == "Xor")
        {
            ipolicy = new XorDiscount((XorExpDiscount)predicateDiscountExp.Value, res.Value);
        }
        else
        {
            return new Response<int>("Type is not valid", 1);
        }
        return AddDiscountPolicy(member, description, ipolicy);
    }

    public Response<int> AddDiscountPolicyBasic(Member member, string description, double discount, string type, int? firstParam=0, string? secondParam="")
    {
        IDiscountExp? ipolicy = null;
        if (type == "Shop")
        {
            ipolicy = new ShopDiscount(discount);
        }
        else if (type == "Item")
        {
            if (!firstParam.HasValue)
            {
                return new Response<int>("First param is null", 1);
            }

            ipolicy = new SingleItemDiscount(discount, firstParam.Value);
        }
        else if (type == "Category")
        {
            if (secondParam is "" or null)
            {
                return new Response<int>("Second param is null", 1);
            }
            ipolicy = new CategoryDiscount(discount, secondParam);
        }
        else
        {
            return new Response<int>("Type is not valid", 1);
        }

        return AddDiscountPolicy(member, description, ipolicy);
    }


    private Response<IPredicateDiscountExp> GetDiscountPredicate(int predicateId)
    {
        return discountPolicy.GetPredicate(predicateId);
    }
    public Response<int> AddSalePolicyRestriction(Member member, string description, string? type = "", string? appliesOn = "Shop", int? firstParam = 0, int? secondParam = 0, int? thirdParam = 0, string? forthParam = "", int? fifthParam = 0 )
    {
        ISalePolicy? ipolicy = null;
        if (!firstParam.HasValue)
        {
            return new Response<int>("First param is null", 1);
        }
        if (type == "Lower Age")
        {
            ipolicy = new LowerAgeRestriction(firstParam.Value);
        }
        else if(type == "Date Sale")
        {
            if (!secondParam.HasValue || !thirdParam.HasValue)
            {
                return new Response<int>("Second or third param is null", 1);
            }

            ipolicy = new DateSaleRestrictions(firstParam.Value, secondParam.Value, thirdParam.Value);
        }
        else if (type == "Before Hour")
        {
            ipolicy = new BeforeHourSaleRestriction(firstParam.Value);
        }
        else if (type == "After Hour")
        {
            ipolicy = new AfterHourSaleRestriction(firstParam.Value);
        }
        else if (type == "Item Amount Less")
        {
            ipolicy = new CheckItemAmountLessPredicate(item => true, firstParam.Value);
            return AddSalePolicy(member, description, ipolicy);
        }
        else if (type == "Item Amount More")
        {
            ipolicy = new CheckItemAmountMoreEqualsPredicate(item => true, firstParam.Value);
            return AddSalePolicy(member, description, ipolicy);
        }
        else if (type == "Item In Basket")
        {
            ipolicy = new CheckItemInBasket(item => item.ItemId == firstParam.Value);
            return AddSalePolicy(member, description, ipolicy);
        }
        else
        {
            return new Response<int>("Type of policy is not valid", 1);
        }

        if (appliesOn == "Shop")
        {
            ipolicy = new ShopBasedRestriction(ipolicy);
        }
        else if (appliesOn == "Category")
        {
            if(forthParam is "" or null)
            {
                return new Response<int>("Category is null", 1);
            }

            ipolicy = new CategoryBasedRestriction(forthParam, ipolicy);
        }
        else if(appliesOn == "Item")
        {
            if (!fifthParam.HasValue)
            {
                return new Response<int>("Item is null", 1);
            }

            ipolicy = new ItemBasedRestriction(fifthParam.Value, ipolicy);
        }
        else
        {
            
            return new Response<int>("Applies on is not valid", 1);
        }

        return AddSalePolicy(member, description, ipolicy);
    }

    public Response<int> AddSalePolicyLogical(Member member, string description, string? type = "", int? firstParam = 0, int? secondParam = 0)
    {
        if (!secondParam.HasValue || !firstParam.HasValue)
        {
            return new Response<int>("First or second param is null", 1);
        }
        Response<ISalePolicy> ipolicy1 = salePolicy.GetSalePolicy(firstParam.Value);
        Response<ISalePolicy> ipolicy2 = salePolicy.GetSalePolicy(secondParam.Value);
        ISalePolicy? ipolicy = null;
        if (ipolicy1.HasError)
        {
            return new Response<int>(ipolicy1.ErrorMsg, 1);
        }
        if (ipolicy2.HasError)
        {
            return new Response<int>(ipolicy2.ErrorMsg, 1);
        }
        if (type == "And")
        {
            ipolicy = new AndExp(ipolicy1.Value, ipolicy2.Value);
        }
        else if (type == "Or")
        {
            ipolicy = new OrExp(ipolicy1.Value, ipolicy2.Value);
        }
        else if (type == "Implies")
        {
            ipolicy = new ImpliesExp(ipolicy1.Value, ipolicy2.Value);
        }
        else
        {
            return new Response<int>("Type of policy is not valid", 1);
        }

        return AddSalePolicy(member, description, ipolicy);
    }
    public void AppointManager(Member manager, Member appointer)
    {
        ValidationCheck.Validate(IsClosed, "Shop is closed");
        //check Permission
        ValidationCheck.Validate(!appointer.HasPermission(this, PermissionsEnum.Permission.ADD_MANAGER), "member doesn't have this permission");
        ValidationCheck.Validate(!manager.IsMember(), "Manager must be a member");
        ValidationCheck.Validate(appointeesTree.Contains(manager.Name), "Manager already exists");
        ValidationCheck.Validate(manager.HasPermission(this, PermissionsEnum.Permission.SHOP_MANAGER), "Manager already exists");
        ValidationCheck.Validate(manager.HasPermission(this, PermissionsEnum.Permission.SHOP_OWNER), "Owner cannot be a manager");
        ValidationCheck.Validate(!appointeesTree.Contains(appointer.Name), "Only appointeesTree can appoint a manager");
        appointeesTree.AddAppointee(manager, appointer.Name);
        permissions[manager.Name] = new ManagerPermissions(manager, this);
        manager.AddShopPermissions(this, permissions[manager.Name]);
        try
        {
            string message = $"You added as Manager in shop {Name}";
            AlertsManager.Instance.SendAlert(manager, message);
        }
        catch (Exception e)
        {

        }
    }

    public void AppointOwner(Member owner, Member appointer)
    {
        ValidationCheck.Validate(IsClosed, "Shop is closed");
        //check Permission
        ValidationCheck.Validate(!owner.IsMember(), "Owner must be a member");
        ValidationCheck.Validate(appointeesTree.Contains(owner.Name), "Owner already exists");
        ValidationCheck.Validate(owner.HasPermission(this, PermissionsEnum.Permission.SHOP_OWNER), "Owner already exists");
        ValidationCheck.Validate(owner.HasPermission(this, PermissionsEnum.Permission.SHOP_MANAGER), "manager cannot be a Owner"); ValidationCheck.Validate(!appointeesTree.Contains(appointer.Name), "Only owners can appoint an owner");
        //check Permission
        ValidationCheck.Validate(!appointer.HasPermission(this, PermissionsEnum.Permission.ADD_OWNER), "member doesn't have this permission");
        appointeesTree.AddAppointee(owner, appointer.Name);
        permissions[owner.Name] = new OwnerPermissions(owner, this);
        owner.AddShopPermissions(this, permissions[owner.Name]);
        try
        {
            string message = $"You added as Owner in shop {Name}";
            AlertsManager.Instance.SendAlert(owner, message);
        }
        catch (Exception e)
        {
            
        }

    }

    public Response<List<(Item, double)>> AttemptPurchaseBasket(string userName)
    {
        if (IsClosed)
        {
            return new Response<List<(Item, double)>>("Purchase can't be made due to shop being closed", 1);
        }
        if (shopBaskets.ContainsKey(userName))
        {
            ShopBasket basket = shopBaskets[userName];
            bool isSucceed = salePolicy.AbleToPurchase(basket);
            if (!isSucceed)
            {
                return new Response<List<(Item, double)>>("Purchase can't be made due to sale policy", 1);
            }

            List<(Item, double)> itemsDiscounts = discountPolicy.GetDiscounts(basket);
            foreach (var item in basket.GetItems())
            {
                if(Stock.GetItemQuantity(item.Item1.ItemId) < item.Item2)
                {
                    return new Response<List<(Item, double)>>($"Purchase can't be made due to lack of stock in item {item.Item1.Name}", 1);
                }
            }
            return new Response<List<(Item, double)>>(itemsDiscounts);
        }
        return new Response<List<(Item, double)>>(new List<(Item, double)>());
    }

    public void AddBasket(ShopBasket basket)
    {
        shopBaskets[basket.UserName] = basket;
    }
    
    public Response<bool> PurchaseBasket(string userName, Order order)
    {
        //double price = 0;
        //int itemsProccesed = 0;
        if (shopBaskets.ContainsKey(userName))
        {
            ShopBasket basket = shopBaskets[userName];
            if (basket.IsEmpty())
            {
                return new Response<bool>("Basket is empty", 1);
            }
            bool isSucceed = salePolicy.AbleToPurchase(basket);
            if (!isSucceed)
            {
                return new Response<bool>("Purchase can't be made due to sale policy", 1);
            }

            List<(Item, double)> itemsDiscounts = discountPolicy.GetDiscounts(basket);
            Dictionary<string, double> itemsDiscountsDict = new Dictionary<string, double>();
            foreach ((Item, double) pair in itemsDiscounts)
            {
                itemsDiscountsDict[pair.Item1.Name] = pair.Item2;
            }
            foreach (var item in basket.GetItems())
            {
                if(!itemsDiscountsDict.ContainsKey(item.Item1.Name))
                {
                    itemsDiscountsDict[item.Item1.Name] = 1;
                }
            }
            foreach (var item in basket.GetItems())
            {
                if(Stock.GetItemQuantity(item.Item1.ItemId) < item.Item2)
                {
                    return new Response<bool>($"Purchase can't be made due to lack of stock in item {item.Item1.Name}", 2);
                }
            }
            
            

            //Order order = new Order();
            IEnumerable<(Item, int)> items = shopBaskets[userName].GetItems();
            foreach ((Item, int) pair in items)
            {
                    // Stock.EditStock(pair.Item1, -pair.Item2);
                    //itemsProccesed++;
                    // TODO: change the price to match policy in the future
                    order.AddItem(pair.Item1, pair.Item2, itemsDiscountsDict[pair.Item1.Name]);
                    // TODO: Add price update here in the future
                
            }

            double price = 0;
            List<(Item, int, double)> itemsList = new List<(Item, int, double)>();
            foreach ((Item, int) pair in items)
            {
                Stock.EditStock(pair.Item1, -pair.Item2);
                //itemsProccesed++;
                // TODO: change the price to match policy in the future
                itemsList.Add((pair.Item1, pair.Item2, itemsDiscountsDict[pair.Item1.Name]));
                price += pair.Item1.Price * pair.Item2 * itemsDiscountsDict[pair.Item1.Name];
                // order.AddItem(pair.Item1, pair.Item2, itemsDiscountsDict[pair.Item1]);
                // TODO: Add price update here in the future
                
            }
            // TODO: Add call to paying service here in the future
            // shopBaskets.deleteItem(userName);
            PastOrder pastOrder = new PastOrder(order.Id, Name, userName, itemsList, price, order.Date);
            orders[userName] = pastOrder;
            try
            {
                SendAlertToShopOwners($"user {userName} Purchase his Basket in shop {Name}");
            }
            catch (Exception e)
            {
                return new Response<bool>("Error in send alerts to Shop Owners", 1);
            }


            return new Response<bool>(true);
        }
        return new Response<bool>("User doesn't have a cart in this shop", 1);
    }

    public List<Item> GetItems()
    {
        return Stock.GetItems();
    }

    public Response<bool> RevertUserBasket(string userName)
    {
        foreach (var item in shopBaskets[userName].GetItems())
        {
            Stock.EditStock(item.Item1, item.Item2);
        }

        return new Response<bool>(true);
    }

    internal bool IsEmployeeInShop(Member m) 
    {
        if (m == Founder)
        {
            return true;
        }
        if (appointeesTree.Contains(m.Name) || appointeesTree.Contains(m.Name))
        {
            return true;
        }
        
        return false;
        
    }


    internal Response<bool> RemoveBasket(string userName)
    {
        if (shopBaskets.ContainsKey(userName))
        {
            ShopBasket basket = shopBaskets[userName];
            basket.Clear();
            var res2 = shopBaskets.DeleteItem(userName);
            return new Response<bool>(true);
        }
        return new Response<bool>("User doesn't have a cart in this shop", 1);
    }
    internal Response<bool> AfterPurchase(bool success, string userName)
    {
        if (success)
        {
            ordersHistory.AddOrder(orders[userName]);
            orders.DeleteItem(userName);
            shopBaskets.DeleteItem(userName);
            return new Response<bool>(true);
        }
        return new Response<bool>("Purchase can't be made", 1);
    }
        
    internal bool RemoveAppointOwner(Member owner, Member ownerToRemove)
    {
        ValidationCheck.Validate(IsClosed, "Shop is closed");
        appointeesTree.RemoveAppointee(owner, ownerToRemove);
        permissions.DeleteItem(ownerToRemove.Name);
        if (Facade.InTestMode)
            ownerToRemove.RemoveShopPermissions(this);
        try
        {
            string message = $"You have been removed from shop {Name} by {owner.Name}";
            AlertsManager.Instance.SendAlert(owner, message);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;

    }

    internal bool RemoveAppointManager(Member owner, Member managerToRemove)
    {
        ValidationCheck.Validate(IsClosed, "Shop is closed");
        appointeesTree.RemoveAppointee(owner, managerToRemove);
        permissions.DeleteItem(managerToRemove.Name);
        if (Facade.InTestMode)
            managerToRemove.RemoveShopPermissions(this);
        try
        {
            string message = $"You have been removed from shop {Name} by {owner.Name}";
            AlertsManager.Instance.SendAlert(owner, message);
        }
        catch (Exception e)
        {
            return false;
        }
        return true;

    }

    internal bool CheckAppointeeManager(Member appointee, string username)
    {
        try
        {
            AppointeeNode node = appointeesTree.FindNodeByName(username);
            ValidationCheck.Validate(node == null, $"There is no appointee of such name: {username}");
            ValidationCheck.Validate(node.Parent == null, $"can't edit permmision of founder");
            ValidationCheck.Validate(node.Parent.UserName != appointee.Name, $"only appointee of owner can remove him");
            return true;
        }
        catch (Exception e)
        {
            return false;
        }
    }

    internal void AddPermissions(Permissions? p, Member member)
    {
        ValidationCheck.Validate(permissions.ContainsKey(member.Name), $"already has permission to this user");
        this.permissions[member.Name] = p;
    }

    public void SendAlertToShopOwners(string message)
    {
        foreach (var member in appointeesTree.GetAppointeesNames())
        {
            if (member.HasPermission(this, PermissionsEnum.Permission.SHOP_OWNER))
            {
                AlertsManager.Instance.SendAlert(member, message);

            }
                
        }
    }
    public List<(Item,int)> GetProducts()
    {
        return Stock.GetStockItems();
    }

    public void Clear()
    {
        Stock.Clear();
        appointeesTree.Clear();
        permissions.RemoveAll();
        shopBaskets.RemoveAll();
        


    }
}