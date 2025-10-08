using OnlineBloggingPlatform.Models;

namespace OnlineBloggingPlatform.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<Category?> GetByNameAsync(string name);
        Task<IEnumerable<Category>> GetCategoriesWithPostCountAsync();
    }
}