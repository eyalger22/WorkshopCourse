using Market.DomainLayer.Market.Validation;
using Market.ORM;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Runtime.Intrinsics.X86;

namespace Market.DomainLayer.Market;

public class Item
{
    [NotMapped]
    private static int itemIdCounter = - 1;

    private string category;
    public virtual string Category
    {
        get  => category;
        private set
        {
            ValidationCheck.Validate(String.IsNullOrEmpty(value), "Category cannot be empty or null");
            category = value;
        }
    }
    
    private string name;
    
    public virtual string Name
    {
        get => name;
        private set
        {
            ValidationCheck.Validate(String.IsNullOrEmpty(value), "Name cannot be empty or null");
            name = value;
        }
    }
    
    private double price;
    
    public virtual double Price
    {
        get => price;
        private set
        {
            ValidationCheck.Validate(value <= 0, "Price cannot be negative");
            price = value;
        }
    }

    private int rank;

    public virtual int Rank
    {
        get => rank;
        set
        {
            ValidationCheck.Validate(value < 0, "Rank cannot be negative");
            rank = value;
        }
    }
    
    public virtual string ShopName { get; set; }


    private string description;

    public virtual string Description
    {
        get => description;
        private set
        {
            //ValidationCheck.Validate(value is null or "", "Description cannot be empty or null");
            description = value;
        }
    }

    [Key]
    public virtual int ItemId { get; private set; }

    public virtual int Quantity { get; set; }

    private static void UpdateCounter()
    {
        if (itemIdCounter == -1)
        {
            itemIdCounter = MarketContext.Instance.Items.Count();
            if (itemIdCounter > 0)
            {
                itemIdCounter = MarketContext.Instance.Items.Max(x => x.ItemId) + 1;
            }
            else
            {
                itemIdCounter++;
            }
        }
    }
    public Item(){}
    public Item(string category, string name, double price, string description, string shopName, int quantity)
    {
        UpdateCounter();
        Category = category;
        Name = name;
        Price = price;
        Description = description;
        Rank = 0;
        ShopName = shopName;
        ItemId = itemIdCounter++;
        Quantity = quantity;
    }

    public Item(string category, string name, double price, string description, string shopName)
    {
        UpdateCounter();
        Category = category;
        Name = name;
        Price = price;
        Description = description;
        Rank = 0;
        ShopName = shopName;    
        ItemId = itemIdCounter++;
    }

    public void ChangeDetails(string name, string category, double price, string description)
    {
        Category = category;
        Price = price;
        Description = description;
        Name = name;
    }
    public static void initItemId()
    {
        itemIdCounter = 1;
    }


}