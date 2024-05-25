using Microsoft.EntityFrameworkCore;

namespace Market.DataObject.OrderRecords;
[PrimaryKey("ItemName")]
public record ItemRecord(string ItemName, double Price, string Category, string Description, int Rank);