using Market.DomainLayer.Users;
using static Market.DataObject.PermissionsEnum;

namespace Market.DomainLayer.Market.UserPermissions;

public class OwnerPermissions : Permissions
{
    
    public OwnerPermissions(User user, Shop shop) : base(user, shop)
    {
        //_permissions.Add(Permission.CLOSE_SHOP);
        AddPermission(Permission.MANAGE_ITEMS);
        AddPermission(Permission.MANAGE_DISCOUNTS);
        AddPermission(Permission.GET_HISTORY_ORDERS);
        AddPermission(Permission.ADD_MANAGER);
        AddPermission(Permission.ADD_OWNER);
        AddPermission(Permission.REMOVE_MANAGER);
        AddPermission(Permission.GET_EMOLOEE_INFO);
        AddPermission(Permission.RESPONSE_TO_USERS);
        AddPermission(Permission.MANAGE_PERMISSION);
        AddPermission(Permission.SHOP_OWNER);
        AddPermission(Permission.MANAGE_POLICIES);
    }

}