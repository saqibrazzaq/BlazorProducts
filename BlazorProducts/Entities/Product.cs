using BlazorProducts.Utility;

namespace BlazorProducts.Entities
{
  public class Product
  {
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string name { get; set; } = string.Empty;
    public double price { get; set; }
    public string description { get; set; } = string.Empty;
    public string categoryId { get; set; } = string.Empty;
    public string categoryFullNamePath { get; set; } = string.Empty;
    public string userId { get; set; } = Constants.PartitionKey;
  }
}
