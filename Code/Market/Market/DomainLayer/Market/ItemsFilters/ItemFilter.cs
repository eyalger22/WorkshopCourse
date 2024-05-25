namespace Market.DomainLayer.Market.ItemsFilters;

public class ItemFilter
{
    public List<Item> ItemFilterByCategory(List<Item> items, string category)
    {
        return items.Where(item => item.Category.Equals(category)).ToList();
    }
    
    public List<Item> ItemFilterByName(List<Item> items, string name)
    {
        return items.Where(item => item.Name.Equals(name)).ToList();
    }
    
    
    public List<Item> ItemFilterByPriceRange(List<Item> items, double minPrice, double maxPrice)
    {
        return items.Where(item => item.Price >= minPrice && item.Price <= maxPrice).ToList();
    }
    
    public List<Item> ItemFilterByItemRank(List<Item> items, int rank)
    {
        return items.Where(item => item.Rank >= rank).ToList();
    }
    
    public List<Shop> ItemFilterByShopRank(List<Shop> shops, int rank)
    {
        return shops.Where(shop => shop.Rank >= rank).ToList();
    }
    
    public List<Item> ItemFilterByKeyWord(List<Item> items, string word)
    {
        return items.Where(item => item.Name.Contains(word) || item.Category.Contains(word) || item.Description.Contains(word)).ToList();
    }
}