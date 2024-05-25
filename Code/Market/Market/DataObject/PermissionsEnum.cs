namespace Market.DataObject
{
    public class PermissionsEnum
    {
        public enum Permission
        {
            //OWNER
            CLOSE_SHOP,
            MANAGE_ITEMS,
            MANAGE_DISCOUNTS,
            MANAGE_POLICIES,
            OPEN_SHOP,
            ADD_MANAGER,
            ADD_OWNER,
            REMOVE_MANAGER,
            GET_EMOLOEE_INFO,
            GET_HISTORY_ORDERS,
            RESPONSE_TO_USERS,
            MANAGE_PERMISSION,

            //MANAGER
            SHOP_MANAGER,
            //OWNER 
            SHOP_OWNER,

            //SYSTEM MANAGER
            SYSTEM_MANAGER

        }


    }
}
