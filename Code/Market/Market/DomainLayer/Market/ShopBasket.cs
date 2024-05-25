using Market.DataObject;
using Market.DomainLayer.Users;
using Market.DomainLayer.Market.Validation;
using User = Market.DomainLayer.Users.User;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Market.ORM;
using Microsoft.Owin.Security.Provider;
using Market.DomainLayer.Market.DiscountPolicy;

namespace Market.DomainLayer.Market;

[PrimaryKey(nameof(UserName), nameof(ShopName))]
public class ShopBasket
{
    
    [NotMapped]
    //private Repository<string, (Item, int)> items;
    public Repository<int, ShopBasketItem> items;
    [NotMapped]
    public virtual User User { get; private set; }
    
    public virtual string UserName { get; private set; }

    private string shopName;

    public virtual string ShopName
    {
        get => shopName;
        private set => shopName = value;
    }

    
    public ShopBasket(string shopName, string userName)
    {
        ShopName = shopName;
        UserName = userName;
        if (Facade.InTestMode)
        {
            items = new ListRepository<int, ShopBasketItem>();
        }
        else
        {
            items = new DBRepositoryPartial<int, ShopBasketItem>(MarketContext.Instance.ShopBasketsItems, (item) => item.UserName == UserName && item.ShopName == ShopName);
        }
    }

    public ShopBasket(string shop, User user)
    {
        ShopName = shop;
        User = user;
        UserName = user.Name;
        if (Facade.InTestMode || !user.IsMember())
        {
            items = new ListRepository<int, ShopBasketItem>();
        }
        else
        {
            items = new DBRepositoryPartial<int, ShopBasketItem>(MarketContext.Instance.ShopBasketsItems, (item) => item.UserName == UserName && item.ShopName == ShopName);
        }
    }
    
    public ShopBasket(){}

    public virtual void Clear()
    {
        items.RemoveAll(); 
    }
    private ShopBasketItem GetBasketItem(Item item)
    {
        foreach (var i in items.Values())
        {
            if (i.Item.Name == item.Name)
            {
                return i;
            }
        }
        return null;
    }

    private int GetBasketItemId(Item item)
    {
        foreach (var i in items.Values())
        {
            if (i.Item.Name == item.Name)
            {
                return i.Id;
            }
        }
        return -1;
    }
    public Response<Item> AddNewItem(Item item, int amount)
    {
        if (HasItem(item))
        {
            amount += GetBasketItem(item).Amount;
            if (amount < 0)
            {
                return new Response<Item>("Item amount in basket cannot be negative", 1);
            }
            if (amount == 0)
            {
                items.DeleteItem(GetBasketItemId(item));
            }
            else
            {
                items[GetBasketItemId(item)].Amount = amount;
            }
        }
        else //not in basket
        {
            if (amount < 0)
            {
                return new Response<Item>("Item amount in basket cannot be negative", 1);
            }
            if (amount == 0)
            {
                return new Response<Item>("Item amount in basket cannot be zero", 1);
            }
            using (var transaction = MarketContext.Instance.Database.BeginTransaction())
            {
                try
                {
                    FormattableString sql = $"SET IDENTITY_INSERT ShopBasketsItems ON";
                    MarketContext.Instance.Database.ExecuteSqlInterpolated(sql);
                    MarketContext.Instance.SaveChanges();
                    ShopBasketItem basketItem = new ShopBasketItem(UserName, ShopName, item, amount);
                    items[basketItem.Id] = basketItem;
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }
        }
        return new Response<Item>(item);
    }
    public Response<Item> DeleteItem(string itemName)
    {
        if (HasItem(itemName))
        {
            Item item = GetItem(itemName);
            Response<ShopBasketItem> res = items.DeleteItem(GetBasketItemId(item));
            if (res.ErrorKind == 0)
            {
                return new Response<Item>(res.Value.Item);
            }
            return new Response<Item>(res.ErrorMsg, (int)res.ErrorKind);
        }
        return new Response<Item>($"busket doesn't contain item {itemName}", 1);
    }

    private Item GetItem(string itemName)
    {
        foreach (var i in items.Values())
        {
            if (i.Item.Name == itemName)
            {
                return i.Item;
            }
        }
        return null;
    }

    public Response<bool> EditItemAmount(Item item, int amount)
    {
        if (HasItem(item))
        {
            //int newAmount = items[item.Name].amount;
            int newAmount = amount;
            if (newAmount < 0)
            {
                return new Response<bool>("Item amount in basket canot be negative", 1);
            }
            items[GetBasketItemId(item)].Amount = newAmount;
            if (newAmount == 0)
            {
                items.DeleteItem(GetBasketItemId(item));
            }
            return new Response<bool>(true);
        }
        return new Response<bool>("No such item in the basket", 1);
    }
    public virtual IEnumerable<(Item, int)> GetItems()
    {
        List<(Item, int)> items = new List<(Item, int)>();
        foreach (var item in this.items.Values())
        {
            items.Add((item.Item, item.Amount));
        }
        return items;
    }

    public int GetItemCount(Item item)
    {
        if (HasItem(item))
        {
            return GetBasketItem(item).Amount;
        }
        return 0;
    }



    public virtual bool HasItem(Item item)
    {
        foreach (var i in items.Values())
        {
            if (i.Item.Name == item.Name)
            {
                return true;
            }
        }
        return false;
    }


    public bool HasItem(string itemName)
    {
        foreach (var i in items.Values())
        {
            if (i.Item.Name == itemName)
            {
                return true;
            }
        }
        return false;
    }
    public virtual bool IsEmpty()
    {
        return items.Count() == 0;
    }

    public double GetPrice(DiscountPolicyManager policy)
    {
        List<(Item,double)> discountList = policy.GetDiscounts(this);
        double price = 0;
        foreach ((Item,double) discount in discountList)
        {
            ShopBasketItem basketItem = GetBasketItem(discount.Item1);
            if(basketItem != null)
            {
                price += discount.Item1.Price * (1-discount.Item2) * basketItem.Amount;
            }
        }
        return price;
    }
}