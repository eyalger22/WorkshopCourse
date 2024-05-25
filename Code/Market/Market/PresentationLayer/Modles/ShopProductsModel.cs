using Market.DataObject;
using Market.Pages.Modles;

namespace Market.PresentationLayer.Modles
{
    public class ShopProductsModel:UserModel
    {
        public List<(Product,int)> products {  get; set; }
    }
}
