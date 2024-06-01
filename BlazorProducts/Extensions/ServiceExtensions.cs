using BlazorProducts.Services;
using BlazorProducts.Utility;
using dotenv.net;

namespace BlazorProducts.Extensions
{
  public static class ServiceExtensions
  {
    private static string? CosmosDb => Environment.GetEnvironmentVariable("CosmosDb");
    private static string? CosmosAccount => Environment.GetEnvironmentVariable("CosmosAccount");
    private static string? CosmosKey => Environment.GetEnvironmentVariable("CosmosKey");

    public static void LoadEnvironmentVariables(this IServiceCollection services)
    {
      DotEnv.Load();
    }
    public static void ConfigureCosmosDb(this IServiceCollection services,
        IConfiguration config)
    {
      services.AddSingleton<ICategoryService>(InitCategory(
          config).GetAwaiter().GetResult());
      services.AddSingleton<IProductService>(InitProduct(
          config).GetAwaiter().GetResult());
    }
    private static async Task<CategoryService> InitCategory(IConfiguration configuration)
    {
      var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosAccount, CosmosKey);
      var database = await client.CreateDatabaseIfNotExistsAsync(CosmosDb);
      await database.Database.CreateContainerIfNotExistsAsync(Constants.CategoryCt, "/userId");

      var categoryService = new CategoryService(client, CosmosDb ?? "", Constants.CategoryCt ?? "");
      return categoryService;
    }
    private static async Task<ProductService> InitProduct(IConfiguration configuration)
    {
      var client = new Microsoft.Azure.Cosmos.CosmosClient(CosmosAccount, CosmosKey);
      var database = await client.CreateDatabaseIfNotExistsAsync(CosmosDb);
      await database.Database.CreateContainerIfNotExistsAsync(Constants.ProductCt, "/userId");

      var productService = new ProductService(client, CosmosDb ?? "", Constants.ProductCt ?? "");
      return productService;
    }
  }
}
