using Market.DataObject;
using Market.DataObject.OrderRecords;
using Market.DomainLayer.Hubs;
using Market.DomainLayer.Market;
using Market.DomainLayer.Market.UserPermissions;
using Market.DomainLayer.Market.Validation;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;

using ShoppingCart = Market.DomainLayer.Market.ShoppingCart;
using Market.ORM;
using System.Collections.Generic;

namespace Market.DomainLayer.Users
{
    public class Member : User , IComparable
    {
        private byte[] password;
        private int alertCounter = 0;
        private string email;
        private string address;
        private string phone;
        private Repository<int, PastUserOrder> userOrderHistory;
        [NotMapped]
        public List<PastUserOrder> PastOrders { get => userOrderHistory.Values().ToList(); }
        [NotMapped]
        private Repository<string, Permissions> permissions;
        public string Email
        {
            get { return email; }
            protected set { email = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string Phone
        {
            get { return phone; }
            protected set { phone = value; }
        }

        public byte[] Password
        {
            get { return password; }
            set { password = value; }
        }
        [NotMapped]
        private DateTime birthDate;

        public DateTime BirthDate
        {
            get { return birthDate; }
            protected set
            {
                ValidationCheck.Validate(DateTime.Now < value, "Birth date cannot be in the future");
                birthDate = value;
            }
        }

        [NotMapped]
        public string ConnectionId { get; private set; }
        [NotMapped]
        public Repository<int, Alert> _alerts { get; private set; }


        public static byte[] Encryption(string inputString)
        {
            //encrypt
            byte[] data = System.Text.Encoding.ASCII.GetBytes(inputString);
            data = new System.Security.Cryptography.SHA256Managed().ComputeHash(data);
            String hash = System.Text.Encoding.ASCII.GetString(data);
            return data;
        }

        private void Init()
        {
            if (Facade.InTestMode)
            {
                userOrderHistory = new ListRepository<int, PastUserOrder>();
                permissions = new ListRepository<string, Permissions>();
                _alerts = new ListRepository<int, Alert>();
            }
            else
            {
                userOrderHistory = new ListRepository<int, PastUserOrder>();
                permissions = new DBRepositoryPartialKeyDouble<string, Permissions>(MarketContext.Instance.Permissions, x => x.UserName == Name, Name, 1);
                //permissions = new ListRepository<string, Permissions>();
                _alerts = new DBRepositoryPartial<int, Alert>(MarketContext.Instance.Alerts, x => x.User == Name);
            }
            

        }

        public Member(string name, byte[] password, string email, string address, string phone, DateTime birthDate) : base(name)
        {
            Password = password;
            SessionId = -1;
            Email = email;
            Address = address;
            Phone = phone;
            BirthDate = birthDate;
            //permissions = new ListRepository<string, Permissions>();
            Init();
        }

        public Member(string name, string password, string email, string address, string phone, DateTime birthDate) : base(name)
        {
            Password = Encryption(password);
            SessionId = -1;
            Email = email;
            Address = address;
            Phone = phone;
            BirthDate = birthDate;
            //permissions = new ListRepository<string, Permissions>();
            Init();

        }
        
        public override int Age()
        {
            return DateTime.Now.Year - BirthDate.Year;
        }



        
        public override Response<bool> AddPastOrder(PastUserOrder pastUserOrder)
        {
            userOrderHistory[pastUserOrder._orderId] = pastUserOrder;
            return base.AddPastOrder(pastUserOrder);
        }

        public List<string> ReadAlerts()
        {
            List<Alert> alerts = _alerts.Values().ToList();
            List<string> alertsStr = new List<string>();
            foreach (Alert alert in alerts)
            {
                alertsStr.Add(alert.Message);
            }
            alertsStr = alertsStr.ToHashSet().ToList();
            _alerts.RemoveAll();
            return alertsStr;

        }


        public override bool IsMember()
        {
            return true;
        }
        
        public override bool ValidatePassword(string password)
        {
            byte[] encPass = Encryption(password);
            return encPass.SequenceEqual(Password);
        }

        public void ChangeSession(int sessionId, string connectionId)
        {
            SessionId = sessionId;
            ConnectionId = connectionId;
        }

        public void AddShopPermissions(Market.Shop shop, PermissionsEnum.Permission p)
        {
            if (!permissions.ContainsKey(shop.Name))
            {
                permissions[shop.Name] = new Permissions(this, shop);
                shop.AddPermissions(permissions[shop.Name], this);
            }
            permissions[shop.Name].AddPermission(p);
        }
        public void AddShopPermissions(Market.Shop shop, Permissions p)
        {
            permissions[shop.Name] = p;
        }

        public bool RemovePermissions(Market.Shop shop, PermissionsEnum.Permission p)
        {
            if (!permissions.ContainsKey(shop.Name))
            {
                return false;
            }
            return permissions[shop.Name].RemovePermission(p);
        }

        public bool HasPermission(Market.Shop shop, PermissionsEnum.Permission permission)
        {
            if (!permissions.ContainsKey(shop.Name))
            {
                return false;
            }
            return permissions[shop.Name].HasPermission(permission);
        }

        public bool CanOpenShop(string shopname)
        {
            if (!permissions.ContainsKey(shopname))
            {
                return false;
            }
            return permissions[shopname].HasPermission(PermissionsEnum.Permission.OPEN_SHOP);
        }

        public List<PermissionsEnum.Permission> GetPermissions(Market.Shop shop)
        {
            if (!permissions.ContainsKey(shop.Name))
            {
                return new List<PermissionsEnum.Permission>();
            }
            return permissions[shop.Name].GetPermissions();
        }

        internal void RemoveShopPermissions(Market.Shop shop)
        {
            permissions.DeleteItem(shop.Name);
        }

        internal void RemoveShopPermissions(Market.Shop shop, PermissionsEnum.Permission per)
        {
            permissions[shop.Name].RemovePermission(per);
        }

        internal bool HasPermissions()
        {
            return permissions.Count() > 0;
        }

        public List<string> GetShops()
        {
            List <string> shops = new List<string>();

            foreach (Permissions p in permissions.Values())
            {
                //p.Shop;
                shops.Add(p.ShopName);
            }
            return shops;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }
            Member m = (Member)obj;
            return m.Name.Equals(Name);
        }

        public int CompareTo(object? obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return -1;
            }
            Member m = (Member)obj;
            return m.Name.CompareTo(Name);
        }

        internal List<string> GetClosedShops()
        {
            List<string> shops = new List<string>();

            foreach (Permissions p in permissions.Values())
            {
                if (p.HasPermission(PermissionsEnum.Permission.OPEN_SHOP))
                    shops.Add(p.ShopName);
            }
            return shops;
        }

        internal void ChangeSessionId(string connectionId)
        {
            ConnectionId = connectionId;
        }
    }
}

