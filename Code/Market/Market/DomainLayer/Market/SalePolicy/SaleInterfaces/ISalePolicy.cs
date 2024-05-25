namespace Market.DomainLayer.Market.SalePolicy.SaleInterfaces;

public interface ISalePolicy
{
    public bool CheckPolicy(ShopBasket basket);


    public void changeItemFuncDecider(Func<Item, bool> func);

    public string ToString(int blankNum);
}