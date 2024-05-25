namespace Market.DataObject
{
    public class DeliveryDetails
    {
        public string Name;
        public string Address;
        public string City;
        public string Country;
        public string Zip;
        public DeliveryDetails(string name, string address, string city, string country, string zip)
        {
            Name = name;
            Address = address;
            City = city;
            Country = country;
            Zip = zip;
        }
        public override string ToString()
        {
            return "Name: " + Name + "\n" +
                   "Address: " + Address + "\n" +
                   "City: " + City + "\n" +
                   "Country: " + Country + "\n" +
                   "Zip: " + Zip + "\n";
        }
    }
}
