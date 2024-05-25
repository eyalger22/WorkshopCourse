using Market.DomainLayer.Market.Validation;
using Market.DomainLayer.Users;

namespace Market.DomainLayer.Market.SaleStrategies;

public class CasinoSaleStrategy : ISaleStrategy
{
    
    private List<(User, double)> _bidders;
    
    private double _currentSumBid;
    
    private DateTime _endTime;
    
    public CasinoSaleStrategy(Item item, Shop shop, DateTime endTime) : base(item, shop)
    {
        _bidders = new List<(User, double)>();
        _currentSumBid = 0;
        _endTime = endTime;
    }

    public override void makeOffer(User buyer, double offer)
    {
        throw new NotImplementedException();
        ValidationCheck.Validate(_status != SaleStatus.PENDING, "Sale is over");
        if (_endTime < DateTime.Now)
            rejectSale();
        ValidationCheck.Validate(_currentSumBid + offer <= _item.Price, "Offer must be equal to item price");
        _bidders.Add((buyer, offer));
        _currentSumBid += offer;
        if (_currentSumBid == _item.Price)
            makeSale();
        
    }

    public override void makeSale()
    {
        throw new NotImplementedException();
        Random random = new Random();
        // random between 0 to currentSumBid
        double randomValue = random.NextDouble() * _currentSumBid;
        double sum = 0;
        foreach (var (user, offer) in _bidders)
        {
            sum += offer;
            if (sum >= randomValue)
            {
                // pay the money to the shop
                // send the item to the buyer
                _status = SaleStatus.SOLD;
                return;
            }
        }
        

    }

    public override void rejectSale()
    {
        throw new NotImplementedException();
        // return money to all bidders
        _status = SaleStatus.REJECTED;
    }
}