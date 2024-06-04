using BlazorProducts.Dtos.Paging;

namespace BlazorProducts.Dtos
{
  public class CategorySearchReq : PagedReq
  {
    public string Name { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public string ParentId { get; set; } = string.Empty;
    public string FullNamePath { get; set; } = string.Empty;
  }
}
