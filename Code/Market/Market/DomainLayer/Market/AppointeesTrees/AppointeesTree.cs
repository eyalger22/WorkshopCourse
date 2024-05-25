using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;
using Market.ORM;
using Microsoft.AspNetCore.Authentication;
using System.Drawing.Printing;
using System.Xml.Linq;

namespace Market.DomainLayer.Market.AppointeesTrees;

public class AppointeesTree
{
    private AppointeeNode? root;
    private string shopName;
    
    public AppointeeNode? Root
    {
        get => root;
        private set => root = value;
    }
    
    
    public AppointeesTree(Member rootUser, string shopName)
    {
        Root = new AppointeeNode(rootUser, null, shopName);
        if (!Facade.InTestMode)
        {
            if (MarketContext.Instance.AppointeeNodes.Find(rootUser.Name, shopName) == null)
            {
                MarketContext.Instance.AppointeeNodes.Add(Root);
            }

            MarketContext.Instance.SaveChanges();
        }

        this.shopName = shopName;
    }
    
    
    
    public void AddAppointee(Member appointeeUser, string parentName)
    {
        if (Root == null)
        {
            Root = new AppointeeNode(appointeeUser, null, shopName);
            return;
        }
        
        AppointeeNode? parent = FindNodeByName(parentName);
        AppointeeNode? Appointee = FindNodeByName(appointeeUser.Name);
        ValidationCheck.Validate(parent == null, $"There is no appointee of such name: {parentName}");
        ValidationCheck.Validate(Appointee != null, $"There is already an appointee of such name: {appointeeUser.Name}");
        AppointeeNode appointee = new AppointeeNode(appointeeUser, parent, shopName);
        parent.AddChild(appointee);
    }

    public AppointeeNode? FindNodeByName(string parentName)
    {
        if (Root == null)
        {
            return null;
        }
        return Root.UserName == parentName ? Root : Root.FindNodeByName(parentName);
    }

    public List<Member> GetAppointeesNames()
    {
        return Root.GetAppointeesNames(new List<Member>());
    }
    
    public bool Contains(string appointeeName)
    {
        return FindNodeByName(appointeeName) != null;
    }

    internal void RemoveAppointee(Member owner, Member ownerToRemove)
    {
        AppointeeNode node = FindNodeByName(ownerToRemove.Name);
        ValidationCheck.Validate(node == null, $"There is no appointee of such name: {ownerToRemove.Name}");
        ValidationCheck.Validate(node.Parent == null, $"can't edit permmision of founder");
        ValidationCheck.Validate(node.Parent.UserName != owner.Name, $"only appointee of owner can remove him");
        RemoveAppointee(ownerToRemove.Name);

    }

    private void RemoveAppointee(string ownerToRemove)
    {
        AppointeeNode node = FindNodeByName(ownerToRemove);
        List<AppointeeNode> appointeesByOwner = node.Children.Values().ToList();
        foreach (var appointee in appointeesByOwner)
        {
            if (ownerToRemove != appointee.UserName)
                RemoveAppointee(appointee.UserName);
        }
        node.Parent.RemoveChild(node);
    }

    //internal List<Member> GetAppointeesByOwner(string owner)
    //{
    //    AppointeeNode node = FindNodeByName(owner);
    //    foreach (var appointee in node.Children.Values())
    //    {
    //        RemoveAppointee(appointee.UserName);
    //    }
    //    if (node == null)
    //    {
    //        return new List<Member>();
    //    }
    //    return node.GetAppointeesNames(new List<Member>());
    //}

    internal void Clear()
    {
        if (Root == null)
        {
            return;
        }
        Root.Clear();
        MarketContext.Instance.AppointeeNodes.Remove(Root);



    }
}