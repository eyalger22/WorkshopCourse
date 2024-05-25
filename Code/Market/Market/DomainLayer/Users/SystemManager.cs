
namespace Market.DomainLayer.Users;

public class SystemManager : Member
{
    
    
    
    public SystemManager(string username, string password,string email ,string address = "", string phone = "", DateTime birthDate = default) : base(username, password,email ,address, phone, birthDate)
    {
        
        
    }
    
}