using Market.DataObject;
using Market.DomainLayer;
using Market.DomainLayer.Users;
using Serilog;
using System.Collections;

namespace Market.ServiceLayer
{
    public class Service
    {
        private static Service? instance;
        public ShopService ShopService { get; }
        public UserService UserService { get; }

        
        public Service()
        {
            ShopService = new ShopService();
            UserService = new UserService();
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Hour)
                    .CreateLogger();
            UserService.InitTheSystem(false);
            ReadInitFile();
        }
        
        public static Service GetService()
        {
            if (instance == null)
                instance = new Service();
            return instance;
        }

        private Dictionary<string,int> user_init = new Dictionary<string, int>();
        private Dictionary<string,int> shops_init = new Dictionary<string, int>();

        private void ReadInitFile()
        {
            try
            {
                // Read the file
                string[] lines = File.ReadAllLines("Init files\\Init_File.txt");

                // Process the data
                string section = "";
                foreach (string line in lines)
                {
                    if (line.StartsWith("_Register:"))
                        section = "users";
                    else if (line.StartsWith("_Shop:"))
                        section = "shops";
                    else if (line.StartsWith("_Owner:"))
                        section = "owners";
                    else if (line.StartsWith("_Manager:"))
                        section = "managers";
                    else if (line.StartsWith("_Product:"))
                        section = "products";
                    else if (line.StartsWith("_Login:"))
                        section = "login";
                    else if (line.StartsWith("_Logout:"))
                        section = "logout";
                    else
                    {
                        string[] paramsArr = line.Split(',');

                        //Console.WriteLine(string.Join(",", paramsArr));

                        switch (section)
                        {
                            case "users":
                                ProcessUsers(paramsArr);
                                break;
                            case "shops":
                                ProcessShops(paramsArr);
                                break;
                            case "owners":
                                ProcessOwners(paramsArr);
                                break;
                            case "managers":
                                ProcessManagers(paramsArr);
                                break;
                            case "products":
                                ProcessProducts(paramsArr);
                                break;
                            case "login":
                                ProcessLogin(paramsArr);
                                break;
                            case "logout":
                                ProcessLogout(paramsArr);
                                break;
                        }
                    }
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ProcessUsers(string[] parameters)
        {
            string username = parameters[0];
            string password = parameters[1];
            string email = parameters[2];
            string address = parameters[3];
            string phone = null;
            DateTime birthDate = DateTime.Now;
            bool hasDate = false;

            for(int i=4; i<parameters.Length; i++)
            {
                if (parameters[i].StartsWith("_")) {
                    phone = parameters[i].Substring(1);
                } else if (parameters[i].StartsWith("#"))
                {
                    birthDate = DateTime.Parse(parameters[i].Substring(1));
                    hasDate = true;
                }
            }

            if(phone != null)
            {
                if(hasDate)
                {
                    UserService.Register(username, password, email, address, phone, birthDate);
                }
                else
                {
                    UserService.Register(username,password, email, address, phone);
                }
            }
            else
                UserService.Register(username, password, email, address);
        }

        private void ProcessShops(string[] parameters)
        {
            string username = parameters[0];
            string shopName = parameters[1];
            string shopAddress = parameters[2];
            string bank = parameters[3];
            if (user_init.ContainsKey(username))
            {
                Response<Shop> res1 = ShopService.OpenShop(user_init[username],shopName, shopAddress, bank);
                if(!res1.HasError)
                {
                    shops_init.Add(shopName, res1.Value.ShopId);
                }
            }
        }

        private void ProcessOwners(string[] parameters)
        {
            string appointerUsername = parameters[0];
            string shopName = parameters[1];
            string appointedUsername = parameters[2];

            if(user_init.ContainsKey(appointerUsername) && shops_init.ContainsKey(shopName))
            {
                ShopService.AddOwner(user_init[appointedUsername], shops_init[shopName], appointedUsername);
            }
        }

        private void ProcessManagers(string[] parameters)
        {
            string appointerUsername = parameters[0];
            string shopName = parameters[1];
            string appointedUsername = parameters[2];

            if (user_init.ContainsKey(appointerUsername) && shops_init.ContainsKey(shopName))
            {
                ShopService.AddManager(user_init[appointedUsername], shops_init[shopName], appointedUsername);
            }
        }

        private void ProcessProducts(string[] parameters)
        {
            string username = parameters[0];
            string shopName = parameters[1];
            string productName = parameters[2];
            string price = parameters[3];
            string category = parameters[4];
            string description = parameters[5];
            string amount = parameters[6];
            if (user_init.ContainsKey(username) && shops_init.ContainsKey(shopName))
            {
                Response<int> res1 = ShopService.AddProductToShop(user_init[username], shops_init[shopName], productName, int.Parse(price), category, description);
                if (!res1.HasError)
                {
                    ShopService.AddAmountToProductInStock(user_init[username], shops_init[shopName], res1.Value, int.Parse(amount));
                }
            }
        }

        private void ProcessLogin(string[] parameters)
        {
            string username = parameters[0];
            string password = parameters[1];

            Response<int> loginId = UserService.Login(username, password);
            if(!loginId.HasError)
            {
                user_init.Add(username,loginId.Value);
            }
        }

        private void ProcessLogout(string[] parameters)
        {
            string username = parameters[0];

            if(user_init.ContainsKey(username))
            {
                Response<bool> logout = UserService.Logout(user_init[username]);
                if(!logout.HasError)
                {
                    user_init.Remove(username);
                }
            }
        }
    }
}
