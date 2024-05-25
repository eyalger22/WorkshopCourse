using Market.DataObject;
using Facade = Market.DomainLayer.Facade;
using Microsoft.AspNetCore.Identity;
using static Market.DataObject.PermissionsEnum;
using ShoppingCart = Market.DataObject.ShoppingCart;
using Serilog;
using Market.DomainLayer.Market;
using Microsoft.CodeAnalysis;

namespace Market.ServiceLayer
{
    public class UserService
    {
        private Facade facade;
        public UserService()
        {
            facade = Facade.GetFacade();
        }


        public Response<int> EnterGuest()
        {
            string str = "EnterGuest() ... ";
            try
            {
                Response<int> res = facade.EnterGuest();
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<int>(ex.Message, 2);
            }
        }

        public Response<bool> UpdateConnectionId(int userId, string connectionId)
        {
            string str = "UpdateConnectionId(userId:" + userId + ", connectionId:" + connectionId + ") ... ";
            try
            {
                Response<bool> res = facade.UpdateConnectionId(userId, connectionId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }
        public Response<int> Exit(int userId)
        {
            string str = "Exit(userId:" + userId + ") ... ";
            try
            {
                Response<int> res = facade.Exit(userId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;

            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<int>(ex.Message, 2);
            }
        }

        public Response<User> GetUser(int userid)
        {
            string str = "GetUser(userId:" + userid + ") ... ";
            try{
                Response<Market.DomainLayer.Users.User> res = facade.GetUser(userid);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return new Response<User>(res.ErrorMsg, 1);
                }
                Log.Information(str + "Success");
                var resUser = res.Value;
                User user = new User(userid, resUser.Name, resUser.Phone(), resUser.Email(), resUser.Permissions());
                return new Response<User>(user);
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<User>(ex.Message, 2);
            }
        }

        public Response<DataObject.User> GetEmployeeInformation(int userId, int shopId, string username)
        {
            string str = "GetEmployeeInformation(userId" + userId + ", shopId:" + shopId + ", username:" + username + ") ... ";
            try
            {
                Response < DataObject.User > res = facade.GetEmployeeInformation(userId, shopId, username);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<DataObject.User>(ex.Message, 2);
            }
        }

        public Response<bool> InitTheSystem(bool inTestMode = true)
        {
            string str = "InitTheSystem(inTestMode:" + inTestMode.ToString() + ") ... ";
            try
            {
                Response<bool> res= facade.InitTheSystem(inTestMode);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }

        public Response<int> Login(string username, string password, string connectionId = "")
        {
            string str = "Login(userName:" + username + ") ... ";
            try
            {
                Response<int> res = facade.Login(username, password, connectionId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<int>(ex.Message, 2);
            }
        }

        public Response<bool> Logout(int userId)
        {
            string str = "Logout(userId:" + userId + ") ... ";
            try
            {
                Response<bool>  res = facade.Logout(userId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }

        public Response<bool> Register(string username, string password, string email, string address, string phone = "", DateTime birthDate = default)
        {
            string str = "Register(username:" + username + ", email:" + email + ") ... ";
            try
            {
                Response<bool> res = facade.Register(username, password, email, address, phone, birthDate);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }

        public Response<bool> RemovePermission(int userId, int shopId, string username, PermissionsEnum.Permission permission)
        {
            string str = "RemovePermission(userId:" + userId + ", shopId:" + shopId + ", username:" + username + ", permission:" + permission.ToString() + ") ... ";
            try
            {
                Response<bool> res= facade.RemovePermission(userId, shopId, username, permission);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }
        public Response<bool> Can_close_shop(int userId, int shopId)
        {
            string str = "Can_close_shop(userId:" + userId + ", shopId:" + shopId +  ") ... ";
            try
            {
                Response<bool> res = facade.CanCloseShop(userId, shopId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }

        public Response<bool> Can_open_shop(int userId, int shopId)
        {
            string str = "Can_open_shop(userId:" + userId + ", shopId:" + shopId + ") ... ";
            try
            {
                Response<bool> res = facade.CanOpenShop(userId, shopId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }

        public Response<List<string>> ReadAlert(int userId)
        {
            string str = "ReadAlert(userId:" + userId + ") ... ";
            try
            {
                Response < List<string> > res = facade.ReadAlerts(userId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<List<string>>(ex.Message, 2);
            }
        }

        public Response<bool> RemoveAppointOwner(int userId, int shopId, string username)
        {
            string str = "RemoveAppointedOwner(userId" + userId + ", shopId:" + shopId + ", username:" + username + ") ... ";
            try
            {
                Response<bool> res = facade.RemoveAppointOwner(userId, shopId, username);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }
        public Response<bool> RemoveAppointManager(int userId, int shopId, string username)
        {
            string str = "RemoveAppointedManager(userId" + userId + ", shopId:" + shopId + ", username:" + username + ") ... ";
            try
            {
                Response<bool> res = facade.RemoveAppointManager(userId, shopId, username);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }
        public Response<bool> CloseShopForever(int userId, int shopId)
        {
            string str = "CloseShopForever(userId:" + userId + ", shopId:" + shopId + ") ... ";
            try
            {
                Response<bool> res= facade.CloseShopForever(userId, shopId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }

        public Response<bool> RemoveMember(int userId, string username)
        {
            string str = "RemoveMember(userId:" + userId + ", username:" + username + ") ... ";
            try
            {
                Response<bool> res = facade.RemoveMember(userId, username);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>(ex.Message, 2);
            }
        }

        public Response<ShoppingCart> ViewShoppingCart(int userId)
        {
            string str = "ViewShoppingCart(userId:" + userId + ") ... ";
            try
            {
                Response < ShoppingCart > res = facade.ViewShoppingCart(userId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str +"Failure: " + ex.Message);
                return new Response<ShoppingCart>("Error in ViewShoppingCart: " + ex.Message, 2);
            }
        }

        public Response<bool> AddProductToCart(int userId, int shopId, int productId, int amount)
        {
            string str = "AddProductToCart(userId:" + userId + ", shopId:" + shopId + ", productId:" + productId + ", amount:" + amount + ") ... ";
            try
            {
                Response<bool> res = facade.AddProductToCart(userId, shopId, productId, amount);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>("Error in AddProductToCart: " + ex.Message, 2);
            }
        }

        public Response<bool> HasItemInCart(int userId,string shopname,int  item)
        {
            string str = "HasItemInCart(userId:" + userId + ", shopname:" + shopname + ", Item:" + item +  ") ... ";
            try
            {
                Response<bool> res = facade.HasItemInCart(userId, shopname, item);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }
            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<bool>("Error in HasItemInCart: " + ex.Message, 2);
            }
        }
        public Response<List<DataObject.Shop>> GetUserShops(int userId)
        {
            string str = "GetUserShops(userId:" + userId + ") ... ";
            try
            {
                Response<List<DataObject.Shop>> res = facade.getUsersShop(userId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }

            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<List<DataObject.Shop>>("Error in GetUserShops: " + ex.Message, 2);
            }
        }

        public Response<List<DataObject.Shop>> GetUserOpenShops(int userId)
        {
            string str = "GetUserOpenShops(userId:" + userId + ") ... ";
            try
            {
                Response<List<DataObject.Shop>> res = facade.GetUserOpenShops(userId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }

            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<List<DataObject.Shop>>("Error in GetUserOpenShops: " + ex.Message, 2);
            }
        }

        public Response<List<DataObject.Shop>> GetUserShopsToOpen(int userId)
        {
            string str = "GetUserShopsToOpen(userId:" + userId + ") ... ";
            try
            {
                Response<List<DataObject.Shop>> res = facade.GetUserShopsToOpen(userId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }

            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<List<DataObject.Shop>>("Error in GetUserShopsToOpen: " + ex.Message, 2);
            }
        }

        public Response<double> GetCartPrice(int userId)
        {
            string str = "GetCartPrice(userId:" + userId + ") ... ";
            try
            {
                Response<double> res = facade.GetCartPrice(userId);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }

            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<double>("Error in GetCartPrice: " + ex.Message, 2);
            }
        }

        public Response<double> GetBasketPrice(int userId, string shopName)
        {
            string str = "GetBasketPrice(userId:" + userId + ", shopName: " + shopName + ") ... ";
            try
            {
                Response<double> res = facade.GetBasketPrice(userId,shopName);
                if (res.HasError)
                {
                    Log.Error(str + "Failure: " + res.ErrorMsg);
                    return res;
                }
                Log.Information(str + "Success");
                return res;
            }

            catch (Exception ex)
            {
                Log.Error(str + "Failure: " + ex.Message);
                return new Response<double>("Error in GetBasketPrice: " + ex.Message, 2);
            }
        }
    }
}
