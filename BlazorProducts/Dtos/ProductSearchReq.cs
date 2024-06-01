using BlazorProducts.Dtos.Paging;

namespace BlazorProducts.Dtos
{
  public class ProductSearchReq : PagedReq
  {
    public string Name { get; set; } = string.Empty;
    public string CategoryFullNamePath { get; set; } = string.Empty;
  }
}
