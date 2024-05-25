using Market.Pages.Modles;
using Shop = Market.DataObject.Shop;

namespace Market.PresentationLayer.Modles
{
    public class MyShopsModel:UserModel
    {
        public List<Shop> shop_list { get; set; }
    }
}
