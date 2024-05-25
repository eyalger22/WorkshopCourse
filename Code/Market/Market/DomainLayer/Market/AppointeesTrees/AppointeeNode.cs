using Market.DataObject;
using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;
using Market.ORM;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using User = Market.DomainLayer.Users.User;

namespace Market.DomainLayer.Market.AppointeesTrees;

[PrimaryKey(nameof(UserName), nameof(ShopName))]
public class AppointeeNode
{
    //private static int appointeeIdCounter = 0;
    //[Key]
    //public int AppointeeId { get; private set; }

    public string UserName { get; private set; }
    public string ShopName { get; private set; }

    //[NotMapped]
    //private Member User { get;  set; }

    [NotMapped]
    public Repository<string, AppointeeNode> Children { get; private set; }

    private AppointeeNode? parent;

    [NotMapped]
    public AppointeeNode? Parent {
        get
        {
            if (Facade.InTestMode)
            {
                return parent;
            }
            if (ParentUserName == "")
            {
                return null;
            }
            return MarketContext.Instance.AppointeeNodes.Find(ParentUserName, ShopName);
        }
        private set => parent = value;
    }

    public string? ParentUserName { get; private set; }

    public AppointeeNode(Member user, AppointeeNode? parent, string shopName)
    {
        //User = user;
        UserName = user.Name;
        ShopName = shopName;
        ParentUserName = parent?.UserName ?? "";

        if (Facade.InTestMode)
        {
            Children = new ListRepository<string, AppointeeNode>();
            Parent = parent;
        }
        else
        {
            Children = new DBRepositoryPartialKeyDouble<string, AppointeeNode>(MarketContext.Instance.AppointeeNodes, (n) => n.ParentUserName == UserName && n.ShopName == ShopName, ShopName, 2);
        }
    }

    //For ORM
    public AppointeeNode(string userName, string shopName, string parentUserName)
    {
        UserName = userName;
        ShopName = shopName;
        ParentUserName = parentUserName;
        if (Facade.InTestMode)
        {
            Children = new ListRepository<string, AppointeeNode>();
        }
        else
        {
            Children = new DBRepositoryPartialKeyDouble<string, AppointeeNode>(MarketContext.Instance.AppointeeNodes, (n) => n.ParentUserName == UserName && n.ShopName == ShopName, ShopName, 2);
            //if (parentUserName != null && parentUserName != "")
            //{
            //    Parent = MarketContext.Instance.AppointeeNodes.Find(ParentUserName, ShopName);
            //}
        }
    }

    public void AddChild(AppointeeNode child)
    {
        Children.AddItem(child, child.UserName);
    }
    public void RemoveChild(AppointeeNode child)
    {
        Validation.ValidationCheck.Validate(!Children.ContainsKey(child.UserName), $"There is no appointee of such name: {child.UserName}");
        Children.DeleteItem(child.UserName);
    }

    public List<Member> GetAppointeesNames(List<Member> appointeesNames)
    {
        Response<User> user = Facade.GetFacade().GetUser(UserName);
        if (user.HasError || !(user.Value is Member))
        {
            return appointeesNames;
        }
        appointeesNames.Add((Member)user.Value);
        foreach (var child in Children.Values())
        {
            appointeesNames.AddRange(child.GetAppointeesNames(appointeesNames));
        }

        return appointeesNames;
    }
    
    public AppointeeNode? FindNodeByName(string appointeeName)
    {
        if (UserName == appointeeName)
        {
            return this;
        }
        foreach (var child in Children.Values())
        {
            AppointeeNode? appointee = child.FindNodeByName(appointeeName);
            if (appointee != null)
            {
                return appointee;
            }
        }

        return null;
    }

    public void Clear()
    {
        foreach (var child in Children.Values())
        {
            child.Clear();
        }
        Children.RemoveAll();
    }
}