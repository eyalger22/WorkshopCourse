using Market.DataObject;
using Market.DomainLayer.Users;
using Market.ORM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security;
using static Market.DataObject.PermissionsEnum;

namespace Market.DomainLayer.Market.UserPermissions;

[PrimaryKey(nameof(UserName), nameof(ShopName))]
public class Permissions
{
    [NotMapped]
    private List<Permission> _permissions { get; set; }

    //public string Permissions_str
    //{ 
    //    get
    //    {
    //        var str = "";
    //        foreach (var permission in _permissions)
    //        {
    //            int per = (int)permission;
    //            str = str + per + ",";
    //        }
    //        return str;

    //    }
    //    set
    //    {
    //        List<string> permissionsStr = value.Split(',').ToList();
    //        List<int> permissionsInt = permissionsStr.Select(int.Parse).ToList();
    //        _permissions = new List<Permission>();
    //        foreach (var permission in permissionsInt)
    //        {
    //            _permissions.Add((Permission)permission);
    //        }
    //        _permissions_str = value;
    //    }

    //}

    public string Permissions_str { get; set; }


    public string UserName { get; private set; }


    public string ShopName { get; private set; }

    public Permissions(string userName, string shopName, string permissions_str)
    {
        UserName = userName;
        ShopName = shopName;
        Permissions_str = permissions_str;
        UpdateList();

    }

    public Permissions(string userName, string shopName, List<Permission> _permissions)
    {
        UserName = userName;
        ShopName = shopName;
        this._permissions = _permissions;
    }


    public Permissions(Users.User user, Shop shop, List<Permission> _permissions)
    {
        //User = user;
        //Shop = shop;
        this._permissions = _permissions;
        ShopName = shop.Name;
        UserName = user.Name;
    }

    public Permissions(Users.User user, Shop shop)
    {
        //User = user;
        //Shop = shop;
        _permissions = new List<Permission>();
        ShopName = shop.Name;
        UserName = user.Name;
    }

    //for system manager
    public Permissions(Users.User user)
    {
        UserName = user.Name;
        _permissions = new List<Permission>();
        ShopName = "system manager";
    }

    public bool HasPermission(Permission permission)
    {
        return _permissions.Contains(permission);
    }
    
    public void AddPermission(Permission permission)
    {
        if (!_permissions.Contains(permission))
        {
            _permissions.Add(permission);
            int iP = (int)permission;
            Permissions_str += "," + iP;
            MarketContext.Instance.SaveChanges();
        }
    }

    public bool RemovePermission(Permission permission)
    {
        bool ans = _permissions.Remove(permission);
        UpdateStr();
        return ans;

    }

    public List<PermissionsEnum.Permission> GetPermissions() 
    {
        return _permissions;
    }

    private void UpdateList()
    {
        _permissions = new List<Permission>();
        string Permissions_str2 = Permissions_str;
        List<string> permissionsStr = Permissions_str2.Split(',').ToList();
        permissionsStr = permissionsStr.Where(x => x != "").ToList();
        List<int> permissionsInt = permissionsStr.Select(x => int.Parse(x)).ToList();
        foreach (var permission in permissionsInt)
        {
            _permissions.Add((Permission)permission);
        }
    }

    private void UpdateStr()
    {
        Permissions_str = "";
        foreach (var p in _permissions)
        {
            int iP = (int)p;
            Permissions_str += "," + iP;
        }
        MarketContext.Instance.SaveChanges();

    }
}