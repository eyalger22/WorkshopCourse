using Market.DataObject;
using Market.DomainLayer.Users;
using Market.DomainLayer.Market.Validation;
using System.Xml.XPath;
using User = Market.DomainLayer.Users.User;
using Market.ORM;
using Market.DomainLayer.Market.DiscountPolicy;

namespace Market.DomainLayer.Market;

public class ShoppingCart
{
    private Repository<string, ShopBasket> _shopBaskets;

    private User _user;

    public User User
    {
        get => _user;
        private set => _user = value;
    }

    public ShoppingCart(User user)
    {
        User = user;
        if (Facade.InTestMode || !user.IsMember())
        {
            _shopBaskets = new SynchronizedListRepository<string, ShopBasket>();
        }
        else
        {
            _shopBaskets = new DBRepositoryPartialKeyDouble<string, ShopBasket>(MarketContext.Instance.ShopBaskets, x => x.UserName == User.Name, User.Name, 1);
        }
    }

    public ShoppingCart()
    {
        
    }

    public Response<bool> RemoveBaskets()
    {
           _shopBaskets.RemoveAll();
            return new Response<bool>(true);
    }

    public Response<Item> AddNewItemToCart(string shop, Item item, int amount)
    {
        if (amount <= 0)
        {
            return new Response<Item>("Amount of item to the cart must be positive", 2);
        }
        if (!_shopBaskets.ContainsKey(shop))
        {
            _shopBaskets[shop] = new ShopBasket(shop, User);
        }
        return _shopBaskets[shop].AddNewItem(item, amount);
    }

    public virtual ShopBasket GetBasket(string shopName)
    {
        foreach (var basket in _shopBaskets.Values())
        {
            if (basket.ShopName == shopName)
            {
                return basket;
            }
        }

        return null;
    }

    public Response<int> RemoveItemFromCart(string shopName, Item item, int amountToRemove)
    {
        if (!_shopBaskets.ContainsKey(shopName))
        {
            return new Response<int>("No such shop basket in the shopping cart", 1);
        }
        int amountInBasket = _shopBaskets[shopName].GetItemCount(item);
        if (amountInBasket > amountToRemove)
        {
            Response<bool> res = _shopBaskets[shopName].EditItemAmount(item, amountInBasket - amountToRemove);
            if (res.HasError)
            {
                return new Response<int>(res.ErrorMsg, 4);
            }
            return new Response<int>(amountInBasket - amountToRemove);
        }
        Response<Item> res2 = _shopBaskets[shopName].DeleteItem(item.Name);
        if (res2.HasError)
        {
            return new Response<int>(res2.ErrorMsg, 4);
        }
        return new Response<int>(0);
    }

    public void RemoveAllItems()
    {
        foreach (ShopBasket sb in _shopBaskets.Values())
        {
            _shopBaskets.DeleteItem(sb.ShopName);
        }
    }
    public Response<bool> EditItemAmount(string shopName, Item item, int amount)
    {
        if (!_shopBaskets.ContainsKey(shopName))
        {
            return new Response<bool>("No such shop basket in the shopping cart", 1);
        }
        Response<bool> res = _shopBaskets[shopName].EditItemAmount(item, amount);
        if(!res.HasError)
        {
            if(_shopBaskets[shopName].GetItems().Count() == 0)
            {
                _shopBaskets.DeleteItem(shopName);
            }
        }
        return res;
    }

    private bool itemExists(ShopBasket basket, Item item)
    {
        foreach ((Item, int) pair in basket.GetItems())
        {
            if (pair.Item1 == item)
            {
                return true;
            }
        }
        return false;
    }
    public bool isExist(string shopName, Item item)
    {
        if (!_shopBaskets.ContainsKey(shopName))
        {
            return false;
        }
        ShopBasket sb = _shopBaskets[shopName];
        return sb.HasItem(item);

    }

    public int getItemAmount(string shopName, Item item)
    {
        if (!_shopBaskets.ContainsKey(shopName))
        {
            return 0;
        }
        ShopBasket sb = _shopBaskets[shopName];
        int res = sb.GetItemCount(item);
        return res;
    }
    public virtual IEnumerable<ShopBasket> GetBaskets()
    {
        return _shopBaskets.Values();
    }

    public double GetPrice(Dictionary<string,DiscountPolicyManager> policies)
    {
        double price = 0;
        foreach (ShopBasket basket in GetBaskets())
        {
            price += basket.GetPrice(policies[basket.ShopName]);
        }
        return price;
    }
}