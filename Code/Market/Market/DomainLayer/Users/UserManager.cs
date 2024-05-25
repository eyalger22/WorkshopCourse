
using Market.DataObject;
using Market.ORM;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using User = Market.DomainLayer.Users.User;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Linq;

namespace Market.DomainLayer.Users
{
    
    public class UserManager
    {
        private const string SystemManagerName = "admin";
        private int _counter = 1;
        private Repository<string, User> _usersRep;
        private Repository<string, Member> _members;
        private Repository<int, string> _usersIds;
        private SystemManager _systemManager;
        private MarketContext dbContext;
        
        public UserManager(SystemManager systemManager)
        {
            dbContext = MarketContext.Instance;
            // can be changed in future development
            _systemManager = systemManager;
            _usersRep = new SynchronizedListRepository<string, User>();
            //_members = new DBRepository<string, Member>(dbContext.Members);
            _members = new SynchronizedListRepository<string, Member>();
            _usersIds = new SynchronizedListRepository<int, string>();
            try
            {
                if (!_members.ContainsKey(_systemManager.Name))
                {
                    _members.AddItem(_systemManager, _systemManager.Name);
                }
            }
            catch { }

        }

        public Response<int> addGuest()
        {
            Guest guest = new Guest($"guest{_counter++}");
            Response<User> res = _usersRep.AddItem(guest, guest.Name);
            if(!res.HasError)
            {
                _usersIds[guest.SessionId] = guest.Name;
                return new Response<int>(res.Value.SessionId);
            }
            return new Response<int>($"Error in adding guest: {res.ErrorMsg}", (int)res.ErrorKind);
        }

        public Response<User> Register(string userName, string password, string email, string address, string phone, DateTime birthDate)
        {
            
            if (string.IsNullOrWhiteSpace(userName) || password.Length < 4)
            {
                return new Response<User>("password is invalid", 2);
            }
            if (string.IsNullOrWhiteSpace(userName))
            {
                return new Response<User>("username can't be empty or null", 3);
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return new Response<User>("email can't be empty or null", 4);
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                return new Response<User>("address can't be empty or null", 5);
            }
            lock (this)
            {
                if (_members.ContainsKey(userName))
                {
                    return new Response<User>("user already exists", 1);
                }
                Member member = new Member(userName, password, email, address, phone, birthDate);
                var ans = _members.AddItem(member, member.Name);
                if (ans.HasError)
                {
                    return new Response<User>(ans.ErrorMsg, (int)ans.ErrorKind);
                }
                return new Response<User>(member);
            }
        }

        public Response<int> Login(string username, string password, string connectionId)
        {
            User? user = _members[username];
            if (user is null) { return new Response<int>("user doesn't exists", 1); }
            //if (!_repository.ContainsKey(guestId)) { return new Response<int>("Warning!!!, bot logging in", 1); }
            lock (this)
            {

                if (user.IsMember())
                {
                    Member m = (Member)user;
                    if (m.SessionId != -1)//m.IsLoggedIn())
                    {
                        return new Response<int>("user already logged in", 1);
                    }
                    if (m.ValidatePassword(password))
                    {
                        //Response<User> guest = _repository.deleteItem(guestId);
                        m.ChangeSession(_counter++, connectionId);
                        _usersIds[user.SessionId] = user.Name;
                        return new Response<int>(user.SessionId);
                    }
                    return new Response<int>("incorrect password", 2);
                }
                return new Response<int>("user doesn't exists", 1);
            }
        }

        public Response<bool> Logout(int sessionId)
        {
            string username = _usersIds[sessionId];
            User? user = _members[username];
            if (user is null) { return new Response<bool>("user doesn't exists", 1); }
            if (user.IsMember())
            {
                if (user.SessionId == sessionId)
                {
                    ((Member)user).ChangeSession(-1, "");
                    //Guest guest = new Guest($"guest{_counter++}");
                    //Response<User> res = _repository.addItem(guest, guest.Name);
                    _usersIds.DeleteItem(sessionId);
                    if (true) //!res.HasError
                    {
                        return new Response<bool>(true);
                    }
                    //else return new Response<bool>($"Error in logging out: {res.ErrorMsg}", (int)res.ErrorKind);
                }
                return new Response<bool>("Invalid session", 1);
            }
            return new Response<bool>("user doesn't exists", 1); 
        }

        public Response<bool> Exit(int sessionId)
        {
            if (_usersRep.ContainsKey($"guest{sessionId}")){
                Response<User> res = _usersRep.DeleteItem($"guest{sessionId}");
                if(res.ErrorKind == 0)
                {
                    return new Response<bool>(true);
                }
                return new Response<bool>($"Error in removing guest: {res.ErrorMsg}", (int)res.ErrorKind );
            }
            return new Response<bool>("Invalid session", 1);
        }

        public Response<User> GetUser(int sessionId)
        {
            string username = _usersIds[sessionId];
            return GetUser(username);
        }

        public Response<User> GetUser(string username)
        {
            User? user = _usersRep[username];
            if (user is null)
            {
                user = _members[username];
                if (user is null)
                    return new Response<User>("user doesn't exists", 1);
            }
            return new Response<User>(user);
        }

        private string GetUserNameById(int sessionId)
        {
            foreach (User user in _usersRep.Values())
            {
                if (user.SessionId == sessionId)
                {
                    return user.Name;
                }
            }
            foreach (User user in _members.Values())
            {
                if (user.SessionId == sessionId)
                {
                    return user.Name;
                }
            }
            return null;
        }

        internal void InitSystem()
        {
            try
            {
                bool inTestMode = Facade.InTestMode;
                if (inTestMode)
                {
                    dbContext.RemoveAllRowsOfTable(dbContext.Members);
                    dbContext.RemoveAllRowsOfTable(dbContext.Permissions);
                    dbContext.RemoveAllRowsOfTable(dbContext.ShopBaskets);
                    _usersRep = new SynchronizedListRepository<string, User>();
                    _members = new SynchronizedListRepository<string, Member>();
                    _usersIds = new SynchronizedListRepository<int, string>();
                }
                else
                {
                    _usersRep = new SynchronizedListRepository<string, User>();
                    _members = new DBRepository<string, Member>(dbContext.Members);
                    _usersIds = new SynchronizedListRepository<int, string>();
                }
                
            
                if (!_members.ContainsKey(_systemManager.Name))
                {
                    _members.AddItem(_systemManager, _systemManager.Name);
                }
            }
            catch { }

        }

        internal Response<bool> RemoveMember(Member memberToRemove)
        {
            if (memberToRemove is null) 
            { 
                return new Response<bool>("member doesn't exists", 1); 
            }
            if (memberToRemove.HasPermissions()) 
            { 
                return new Response<bool>("user is owner or manger of shop", 1); 
            }
            _members.DeleteItem(memberToRemove.Name);
            if (_usersIds[memberToRemove.SessionId] != null)
                _usersIds.DeleteItem(memberToRemove.SessionId);
            return new Response<bool>(true);
        }

        public List<DataObject.Shop> GetUserShop(Member member)
        {
            List<string> users_Market_shop = member.GetShops();
            List<DataObject.Shop> users_shops = new List<DataObject.Shop>();
            foreach(string s in users_Market_shop){
                Response< DataObject.Shop> shop = Facade.GetFacade().GetShopByName(s);
                if (shop.HasError)
                {
                    return null;
                }
                if (!shop.Value.IsClosed || member.CanOpenShop(shop.Value.Name))
                    users_shops.Add(shop.Value);
            }
            return users_shops;
        }

        public List<DataObject.Shop> GetUserOpenShops(Member member)
        {
            List<string> users_Market_shop = member.GetShops();
            List<DataObject.Shop> users_shops = new List<DataObject.Shop>();
            foreach (string s in users_Market_shop)
            {
                Response<DataObject.Shop> shop = Facade.GetFacade().GetShopByName(s);
                if (shop.HasError)
                {
                    return null;
                }
                users_shops.Add(shop.Value);
            }
            return users_shops;
        }
        public bool IsSystemManager(User member)
        {
            return member.Name == SystemManagerName;
        }

        public List<DataObject.Shop> GetUserShopsToOpen(Member member)
        {
            List<string> users_Market_shop = member.GetClosedShops();
            List<DataObject.Shop> users_shops = new List<DataObject.Shop>();
            foreach (string s in users_Market_shop)
            {
                Response<DataObject.Shop> shop = Facade.GetFacade().GetShopByName(s);
                if (shop.HasError)
                {
                    return null;
                }
                users_shops.Add(shop.Value);
            }
            return users_shops;
        }
    }
}
