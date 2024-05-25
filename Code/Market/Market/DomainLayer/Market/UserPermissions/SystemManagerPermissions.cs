using Market.DataObject;
using Market.DomainLayer.Users;

namespace Market.DomainLayer.Market.UserPermissions;

public class SystemManagerPermissions : Permissions
{
    
    public SystemManagerPermissions(Users.User user) : base(user)
    {
        AddPermission(PermissionsEnum.Permission.SYSTEM_MANAGER);
    }
    
}