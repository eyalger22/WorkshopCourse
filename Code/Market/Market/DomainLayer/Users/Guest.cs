
using Market.DataObject;
using Market.DataObject.OrderRecords;
using ShoppingCart = Market.DomainLayer.Market.ShoppingCart;

namespace Market.DomainLayer.Users
{
    public class Guest : User
    {
        public Guest(string userName) : base(userName)
        {
            SessionId = int.Parse(userName.Substring(5));
            
        }

        public override int Age()
        {
            return 0;
        }

        public override bool IsMember()
        {
            return false;
        }
        

        public override bool ValidatePassword(string password)
        {
            return true;
        }
    }
}
