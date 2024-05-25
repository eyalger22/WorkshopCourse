namespace Market.DataObject
{
    public class User
    {
        public User(int sessionId, string name, string phone,string email, List<PermissionsEnum.Permission> permissions)
        {
            SessionId = sessionId;
            Name = name;
            Email = email;
            Permissions = permissions;
            Phone = phone;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int SessionId { get; set; }
        public List<PermissionsEnum.Permission> Permissions { get; set; }

    }
}
