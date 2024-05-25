using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;

namespace Market.DomainLayer.Market.SaleStrategies;

public class AuctionSaleStrategy : ISaleStrategy
{
    private double _highestOffer;
    private double _startingPrice;
    private User _highestOfferUser;
    private DateTime _deadline;

    public AuctionSaleStrategy(Item item, Shop shop, double startingPrice, DateTime deadline) : base(item, shop)
    {
        _highestOffer = startingPrice;
        _startingPrice = startingPrice;
        _deadline = deadline;
    }

    public override void makeOffer(User buyer, double offer)
    {
        throw new NotImplementedException();
        ValidationCheck.Validate(_status != SaleStatus.PENDING, "Sale is over");
        if (_deadline < DateTime.Now)
        {
            if (_highestOfferUser == null)
                rejectSale();
            else
                makeSale();
        }
        if (offer > _highestOffer)
        {
            _highestOffer = offer;
            _highestOfferUser = buyer;
        }
        else
        {
            ValidationCheck.Validate(true, "Offer must be higher than the highest offer");
        }
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
        
    }
}