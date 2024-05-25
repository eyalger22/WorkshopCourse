using Market.DomainLayer.Users;
using static Market.DataObject.PermissionsEnum;

namespace Market.DomainLayer.Market.UserPermissions;

public class ManagerPermissions : Permissions
{

    public ManagerPermissions(User user, Shop shop) : base(user, shop)
    {
        AddPermission(Permission.MANAGE_ITEMS);
        AddPermission(Permission.MANAGE_DISCOUNTS);
        AddPermission(Permission.GET_HISTORY_ORDERS);
        AddPermission(Permission.RESPONSE_TO_USERS);
        AddPermission(Permission.SHOP_MANAGER);

    }
}