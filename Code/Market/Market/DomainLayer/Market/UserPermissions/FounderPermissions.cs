using Market.DomainLayer.Users;
using static Market.DataObject.PermissionsEnum;

namespace Market.DomainLayer.Market.UserPermissions;

public class FounderPermissions : OwnerPermissions
{
    
    public FounderPermissions(User user, Shop shop) : base(user, shop)
    {
        AddPermission(Permission.CLOSE_SHOP);
        AddPermission(Permission.MANAGE_ITEMS);
        AddPermission(Permission.MANAGE_DISCOUNTS);
        AddPermission(Permission.GET_HISTORY_ORDERS);
        AddPermission(Permission.ADD_MANAGER);
        AddPermission(Permission.ADD_OWNER);
        AddPermission(Permission.REMOVE_MANAGER);
        AddPermission(Permission.GET_EMOLOEE_INFO);
        AddPermission(Permission.RESPONSE_TO_USERS);
        AddPermission(Permission.MANAGE_PERMISSION);
    }
    
}