using BlazorProducts.Utility;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BlazorProducts.Entities
{
  public class Product
  {
    public string id { get; set; } = Guid.NewGuid().ToString();
    [Display(Name = "Name")]
    public string name { get; set; } = string.Empty;
    [Display(Name = "Price")]
    public double price { get; set; }
    [Display(Name = "Description")]
    public string description { get; set; } = string.Empty;
    public string categoryId { get; set; } = string.Empty;
    [Display(Name = "Category Full Name Path")]
    public string categoryFullNamePath { get; set; } = string.Empty;
    public string userId { get; set; } = Constants.PartitionKey;
  }
}
