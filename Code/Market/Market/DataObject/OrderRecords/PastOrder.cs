using Market.DataObject;
using Market.DomainLayer.Market;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Market.DataObject.OrderRecords;

public class PastOrder
{
    [NotMapped]
    public List<(ItemRecord, int, double)> Items {get; private set;}

    [Key]
    public  int OrderId { get; private set; }

    public  string ShopName { get; private set; }

    public  string UserName { get; private set; }

    public  double TotalPrice { get; private set; }

    public  DateTime Date { get; private set; }

    public PastOrder(int orderId, string shopName, string userName,  double totalPrice, DateTime date)
    {
        OrderId = orderId;
        ShopName = shopName;
        UserName = userName;
        TotalPrice = totalPrice;
        Date = date;
    }

    public PastOrder(int orderId, string shopName, string userName, List<(Item, int, double)> items, double totalPrice, DateTime date)
    {
        OrderId = orderId;
        ShopName = shopName;
        UserName = userName;
        Items = items.ConvertAll(item => (new ItemRecord(item.Item1.Name, item.Item1.Price, item.Item1.Category, item.Item1.Description, item.Item1.Rank), item.Item2, item.Item3));
        TotalPrice = totalPrice;
        Date = date;
    }

    public List<(ItemRecord, int, double)> GetItems()
    {
        return Items;
    }

}