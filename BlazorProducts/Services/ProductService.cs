using BlazorProducts.Dtos.Paging;
using BlazorProducts.Dtos;
using BlazorProducts.Entities;
using Microsoft.Azure.Cosmos;
using System.Linq;
using BlazorProducts.Utility;

namespace BlazorProducts.Services
{
  public interface IProductService
  {
    Product? Get(string id);
    Task<Product> AddAsync(Product item);
    Task<Product> UpdateAsync(Product item);
    Task DeleteAsync(Product item);
    Task Reset();
    PagedRes<Product> Search(ProductSearchReq dto);
  }

  public class ProductService : IProductService
  {
    private Container _container;
    public ProductService(
        CosmosClient cosmosDbClient,
        string databaseName,
        string containerName)
    {
      _container = cosmosDbClient.GetContainer(databaseName, containerName);
    }
    public async Task<Product> AddAsync(Product item)
    {
      return await _container.CreateItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
    public async Task DeleteAsync(Product item)
    {
      var itemFind = Get(item.id);
      if (itemFind is null) return;

      await _container.DeleteItemAsync<Product>(item.id, new PartitionKey(Constants.PartitionKey));
    }
    public async Task Reset()
    {
      await _container.DeleteContainerAsync();
      await _container.Database.CreateContainerIfNotExistsAsync(Constants.ProductCt, "/userId");
    }
    public Product? Get(string id)
    {
      try
      {
        IQueryable<Product> queryable = _container.GetItemLinqQueryable<Product>(true);
        queryable = queryable.Where(item => item.id == id);
        return queryable.ToArray().FirstOrDefault();
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return null;
      }
    }
    public PagedRes<Product> Search(ProductSearchReq dto)
    {
      try
      {
        IQueryable<Product> queryable = _container.GetItemLinqQueryable<Product>(true);

        if (!string.IsNullOrEmpty(dto.Name))
        {
          queryable = queryable.Where(x => x.name.Contains(dto.Name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(dto.CategoryFullNamePath))
        {
          queryable = queryable.Where(x => x.categoryFullNamePath.Contains(dto.CategoryFullNamePath, StringComparison.OrdinalIgnoreCase));
        }

        // Count before paging
        var count = queryable.Count();

        // Sort
        if (string.IsNullOrEmpty(dto.OrderBy))
        {
          queryable = queryable.OrderBy(x => x.categoryFullNamePath);
        }
        else if (dto.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Ascending)
        {
          queryable = queryable.OrderBy(x => x.name);
        }
        else if (dto.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Descending)
        {
          queryable = queryable.OrderByDescending(x => x.name);
        }
        else if (dto.OrderBy.Equals("categoryFullNamePath", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Ascending)
        {
          queryable = queryable.OrderBy(x => x.categoryFullNamePath);
        }
        else if (dto.OrderBy.Equals("categoryFullNamePath", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Descending)
        {
          queryable = queryable.OrderByDescending(x => x.categoryFullNamePath);
        }
        else
        {
          queryable = queryable.OrderBy(x => x.categoryFullNamePath);
        }

        // Get list after paging
        queryable = queryable
            .Skip(dto.Skip)
            .Take(dto.Take);
        var items = queryable.ToList();

        return new PagedRes<Product> { Items = items, Count = count };
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return new PagedRes<Product>();
      }
    }
    public async Task<Product> UpdateAsync(Product item)
    {
      var itemFind = Get(item.id);
      if (itemFind is null) throw new Exception("Cannot update.");

      return await _container.UpsertItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
  }
}
