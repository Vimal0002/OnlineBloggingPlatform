using Microsoft.EntityFrameworkCore;
using OnlineBloggingPlatform.Data;
using OnlineBloggingPlatform.Models;

namespace OnlineBloggingPlatform.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Category>> GetCategoriesWithPostCountAsync()
        {
            return await _context.Categories
                .Include(c => c.BlogPosts.Where(p => p.IsPublished))
                .ToListAsync();
        }
    }
}