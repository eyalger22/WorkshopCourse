using Market.DomainLayer.Users;

namespace Market.DomainLayer.Market.SaleStrategies;

public abstract class ISaleStrategy
{
    protected SaleStatus _status;
    
    protected Item _item;
    
    protected Shop _shop;


    public ISaleStrategy(Item item, Shop shop)
    {
        _item = item;
        _shop = shop;
        _status = SaleStatus.PENDING;
    }
    public SaleStatus saleMade()
    {
        return _status;
    }

    public abstract void makeOffer(User buyer, double offer);
    
    public abstract void makeSale();
    
    public abstract void rejectSale();
    
    

}