using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.WebSockets;
using Market.DataObject;
using Market.DataObject.OrderRecords;
using ShoppingCart = Market.DomainLayer.Market.ShoppingCart;

namespace Market.DomainLayer.Users
{

    public abstract class User

    {
        
        private string name;

        [Key]
        public virtual string Name
        {
            get => name;
            private set
            {
                if (value is null or "")
                {
                    throw new Exception("Name cannot be null or empty");
                }

                name = value;
            }
        }
        [NotMapped]
        private int sessionId;

        [NotMapped]
        public int SessionId
        {
            get => sessionId;
            protected set { sessionId = value; }
        }

        [NotMapped]
        public virtual ShoppingCart ShoppingCart { get; set; }

        public User(string name)
        {
            Name = name;
            ShoppingCart = new ShoppingCart(this);
        }


        public abstract bool IsMember();

        public abstract int Age();
        
        public string Phone()
        {
            if (IsMember())
            {
                return ((Member)this).Phone;
            }

            return "";
        }

        public string Email()
        {
            if (IsMember())
            {
                return  ((Member)this).Email;
            }

            return "";
        }

        public List<PermissionsEnum.Permission> Permissions()
        {
            if (IsMember())
            {
                return  ((Member)this).Permissions();
            }

            return new List<PermissionsEnum.Permission>();
        }


        public virtual Response<bool> AddPastOrder(PastUserOrder pastUserOrder)
        {
            return new Response<bool>(true);
        }

        public abstract bool ValidatePassword(string password);
    }
}
