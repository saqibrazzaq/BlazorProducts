using BlazorProducts.Dtos.Paging;
using BlazorProducts.Dtos;
using BlazorProducts.Entities;
using Microsoft.Azure.Cosmos;
using System.Linq;
using BlazorProducts.Utility;

namespace BlazorProducts.Services
{
  public interface ICategoryService
  {
    Category? Get(string id);
    Task<Category> AddAsync(Category item);
    Task<Category> UpdateAsync(Category item);
    Task DeleteAsync(Category item);
    Task Reset();
    PagedRes<Category> Search(CategorySearchReq dto);
  }

  public class CategoryService : ICategoryService
  {
    private Container _container;
    public CategoryService(
        CosmosClient cosmosDbClient,
        string databaseName,
        string containerName)
    {
      _container = cosmosDbClient.GetContainer(databaseName, containerName);
    }
    public async Task<Category> AddAsync(Category item)
    {
      return await _container.CreateItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
    public async Task DeleteAsync(Category item)
    {
      var itemFind = Get(item.id);
      if (itemFind is null) return;

      await _container.DeleteItemAsync<Category>(item.id, new PartitionKey(Constants.PartitionKey));
    }
    public async Task Reset()
    {
      await _container.DeleteContainerAsync();
      await _container.Database.CreateContainerIfNotExistsAsync(Constants.CategoryCt, "/userId");
    }
    public Category? Get(string id)
    {
      try
      {
        IQueryable<Category> queryable = _container.GetItemLinqQueryable<Category>(true);
        queryable = queryable.Where(item => item.id == id);
        return queryable.ToArray().FirstOrDefault();
      }
      catch (CosmosException) //For handling item not found and other exceptions
      {
        return null;
      }
    }
    public PagedRes<Category> Search(CategorySearchReq dto)
    {
      try
      {
        IQueryable<Category> queryable = _container.GetItemLinqQueryable<Category>(true);

        if (!string.IsNullOrEmpty(dto.Name))
        {
          queryable = queryable.Where(x => x.name.Contains(dto.Name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(dto.Id))
        {
          queryable = queryable.Where(x => x.id.Equals(dto.Id, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(dto.FullNamePath))
        {
          queryable = queryable.Where(x => x.fullNamePath.Contains(dto.FullNamePath, StringComparison.OrdinalIgnoreCase));
        }

        // Count before paging
        var count = queryable.Count();

        // Sort
        if (string.IsNullOrEmpty(dto.OrderBy))
        {
          queryable = queryable.OrderBy(x => x.fullNamePath);
        }
        else if (dto.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Ascending)
        {
          queryable = queryable.OrderBy(x => x.name);
        }
        else if (dto.OrderBy.Equals("name", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Descending)
        {
          queryable = queryable.OrderByDescending(x => x.name);
        }
        else if (dto.OrderBy.Equals("fullNamePath", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Ascending)
        {
          queryable = queryable.OrderBy(x => x.fullNamePath);
        }
        else if (dto.OrderBy.Equals("fullNamePath", StringComparison.OrdinalIgnoreCase) && dto.SortOrder == Constants.Descending)
        {
          queryable = queryable.OrderByDescending(x => x.fullNamePath);
        }
        else
        {
          queryable = queryable.OrderBy(x => x.fullNamePath);
        }

        // Get list after paging
        queryable = queryable
            .Skip(dto.Skip)
            .Take(dto.Take);

        var items = queryable.ToList();

        return new PagedRes<Category> { Items = items, Count = count };
      }
      catch (CosmosException ex) //For handling item not found and other exceptions
      {
        return new PagedRes<Category>();
      }
    }
    public async Task<Category> UpdateAsync(Category item)
    {
      var itemFind = Get(item.id);
      if (itemFind is null) throw new Exception("Cannot update.");

      return await _container.UpsertItemAsync(item, new PartitionKey(Constants.PartitionKey));
    }
  }
}
