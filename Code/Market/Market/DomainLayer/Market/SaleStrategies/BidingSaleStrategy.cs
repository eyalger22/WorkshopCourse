using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;

namespace Market.DomainLayer.Market.SaleStrategies;

public class BidingSaleStrategy : ISaleStrategy
{
    
    private User _buyer;
    private double _offer;
    private bool _isSale;
    
    public BidingSaleStrategy(Item item, Shop shop) : base(item, shop)
    {
        _isSale = false;
        _offer = 0;
    }

    public override void makeOffer(User buyer, double offer)
    {
        throw new NotImplementedException();
        ValidationCheck.Validate(_status != SaleStatus.PENDING, "Sale is over");
        _offer = offer;
        _buyer = buyer;
        // send alerts to all the users with permission to approve the sale
        // send them a callback that, when called, will approve the sale for them
    }

    public override void makeSale()
    {
        throw new NotImplementedException();
    }

    public override void rejectSale()
    {
        throw new NotImplementedException();
    }
}