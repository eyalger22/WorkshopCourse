using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;

namespace Market.DomainLayer.Market.SaleStrategies;

public class ImmediateSaleStrategy : ISaleStrategy
{

    private User _buyer;
    
    public ImmediateSaleStrategy(Item item, Shop shop) : base(item, shop)
    {
    }
    
    
    public override void makeOffer(User buyer, double offer)
    {
        throw new NotImplementedException();
        if(offer != _item.Price)
            rejectSale();
        _buyer = buyer;
        makeSale();
    }
    public override void makeSale()
    {
        throw new NotImplementedException();
        // pay the money to the shop
        // send the item to the buyer
        _status = SaleStatus.SOLD;
        
    }
    
    public override void rejectSale()
    {
        throw new NotImplementedException();
        _status = SaleStatus.REJECTED;
        ValidationCheck.Validate(true, "Offer must be equal to item price");
    }
    
}