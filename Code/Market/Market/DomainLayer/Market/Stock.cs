using Market.DataObject;
using Market.DomainLayer.Market.Validation;
using Market.ORM;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Xml.Linq;

namespace Market.DomainLayer.Market;

public class Stock
{
    
    // in the tuple, the first item is the item, and the second is the quantity
    //private readonly Repository<string, (Item, int)> itemsQuantity;
    //private readonly Repository<int, string> itemIds;
    private readonly Repository<int, Item> _items;
    private readonly string ShopName;
    public Stock(string shopName)
    {
        this.ShopName = shopName;
        //itemsQuantity = new ListRepository<string, (Item, int)>();
        //itemIds = new ListRepository<int, string>();
        if (!Facade.InTestMode)
            _items = new DBRepositoryPartial<int, Item>(MarketContext.Instance.Items, x => x.ShopName == ShopName);
        else
            _items = new ListRepository<int, Item>();
        //_items = new ListRepository<int, Item>();
        //_items = new DBRepositoryPartial<int, Item>(MarketContext.Instance.Items, x => x.ShopId==ShopId);


    }

    private Item? GetItemByName(string name)
    {
        foreach (Item i in _items.Values())
        {
            if (i.Name.Equals(name))
            {
                return i;
            }
        }
        return null;
    }

    private bool IsItemExist(string name)
    {
        return GetItemByName(name) != null;
    }
    
    public void AddItem(string category, string name, double price, string description)
    {
        lock (this)
        {
            ValidationCheck.Validate(IsItemExist(name), "Item already exists");
            Item t = new Item(category, name, price, description, ShopName);
            //itemsQuantity[name] = (t, 0);
            //itemIds[t.ItemId] = name;
            using var transaction = MarketContext.Instance.Database.BeginTransaction();
            try
            {
                FormattableString sql = $"SET IDENTITY_INSERT Items ON";
                MarketContext.Instance.Database.ExecuteSqlInterpolated(sql);
                _items[t.ItemId] = t;
                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }
    }
    
    public void RemoveItem(string name)
    {
        lock (this)
        {
            ValidationCheck.Validate(!IsItemExist(name), "Item does not exist");
            Item i = GetItemByName(name)!;
            ValidationCheck.Validate(i.Quantity > 0, "Item exist in stock");
            _items.DeleteItem(i.ItemId);
        }
    }
    
    public void EditStock(Item item, int quantity)
    {
        //ValidationCheck.Validate(quantity < 0, $"Quantity cannot be negative ");
        lock (this)
        {            

            if (_items.ContainsKey(item.ItemId))
            {
                quantity += item.Quantity;
            }
            else
            {
                _items[item.ItemId] = item;
            }
            ValidationCheck.Validate(quantity < 0, $"Quantity cannot be negative Item: {item.Name}");
            item.Quantity = quantity;
            MarketContext.Instance.SaveChanges();

        }
    }
    
    public List<Item> GetItems()
    {
        //List<Item> items = new List<Item>();
        //foreach (var item in itemsQuantity)
        //    items.Add(item.Value.Item1);
        //return items;
        return _items.Values().ToList();
    }

    public Item GetItem(int itemId)
    {
        ValidationCheck.Validate(!_items.ContainsKey(itemId), "Item does not exist");
        return _items[itemId];
    }

    public bool HasItem(int itemId)
    {
        try
        {
            return _items.ContainsKey(itemId);
        }
        catch (Exception e)
        {
            return false;
        }
    }

    public Item GetItem(string itemName)
    {
        return GetItemByName(itemName)!;
    }

    public void ChangeItemDetails(int itemId, string name, string category, double price, string description)
    {
        lock (this)
        {
            ValidationCheck.Validate(!_items.ContainsKey(itemId), "Item does not exist");
            if (IsItemExist(name))
            {
                ValidationCheck.Validate(GetItemByName(name).ItemId != itemId, "Item already exists with this name");
            }
            Item item = GetItem(itemId);
            item.ChangeDetails(name, category, price, description);
            //string old = item.Name;
            //itemsQuantity[old].Item1.ChangeDetails(name, category, price, description);
            //itemIds[itemId] = name;
            //if (!itemsQuantity.ContainsKey(name))
            //{
            //    itemsQuantity[name] = itemsQuantity[old];
            //    itemsQuantity.deleteItem(old);
            //}
            MarketContext.Instance.SaveChanges();
        }

    }

    internal void AddItemsToStock(int itemId, int amount, Users.Member member)
    {
        lock (this) { 
            ValidationCheck.Validate(!_items.ContainsKey(itemId), "Item does not exist");
            ValidationCheck.Validate(amount <= 0, "Amount must be positive number");
            //string name = itemIds[itemId];
            //itemsQuantity[name] = (itemsQuantity[name].Item1, itemsQuantity[name].Item2 + amount);
            _items[itemId].Quantity += amount;
            MarketContext.Instance.SaveChanges();
        }

    }

    public int GetItemQuantity(int itemId)
    {
        ValidationCheck.Validate(!_items.ContainsKey(itemId), "Item does not exist");
        return _items[itemId].Quantity;
    }

    public List<(Item, int)> GetStockItems()
    {
        List<(Item, int)> values = new List<(Item, int)>();
        List<Item> items = _items.Values().ToList();
        foreach (var item in items)
            values.Add((item, item.Quantity));
        return values;
    }

    public void Clear()
    {
        _items.RemoveAll();

    }
}