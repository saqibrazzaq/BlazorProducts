namespace BlazorProducts.Dtos.Paging
{
  public class PagedRes<T>
  {
    public List<T> Items { get; set; } = new List<T>();
    public PagedResMetadata Metadata { get; set; }
    public PagedRes()
    {
      Metadata = new PagedResMetadata();
    }
    public PagedRes(List<T> items, int count, int pageNumber, int pageSize)
    {
      Metadata = new PagedResMetadata
      {
        TotalCount = count,
        PageSize = pageSize,
        CurrentPage = pageNumber,
        TotalPages = (int)Math.Ceiling(count / (double)pageSize)
      };
      Items = items;
    }
  }

  public class PagedResMetadata
  {
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious => CurrentPage > 0;
    public bool HasNext => CurrentPage < TotalPages - 1;
  }
}
