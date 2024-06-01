using BlazorProducts.Utility;

namespace BlazorProducts.Entities
{
  public class Category
  {
    public string id { get; set; } = Guid.NewGuid().ToString();
    public string name { get; set; } = string.Empty;
    public string parentId { get; set; } = string.Empty;
    public string fullIdPath { get; set; } = string.Empty;
    public string fullNamePath { get; set; } = string.Empty;
    public string userId { get; set; } = Constants.PartitionKey;
  }
}
