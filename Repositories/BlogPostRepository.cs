using Microsoft.EntityFrameworkCore;
using OnlineBloggingPlatform.Data;
using OnlineBloggingPlatform.Models;

namespace OnlineBloggingPlatform.Repositories
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<BlogPost>> GetFeaturedPostsAsync(int count);
        Task<IEnumerable<BlogPost>> GetLatestPostsAsync(int count);
        Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count);
        Task<IEnumerable<BlogPost>> GetPostsByCategoryAsync(int categoryId, int pageNumber, int pageSize);
        Task<IEnumerable<BlogPost>> SearchPostsAsync(string query, int pageNumber, int pageSize);
        Task<BlogPost?> GetPostBySlugAsync(string slug);
        Task<IEnumerable<BlogPost>> GetRelatedPostsAsync(int postId, int categoryId, int count);
        Task IncrementViewCountAsync(int postId);
    }

    public class BlogPostRepository : Repository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int pageNumber, int pageSize)
        {
            return await _dbSet
                .Include(bp => bp.User)
                .Include(bp => bp.Category)
                .Include(bp => bp.Blog)
                .Where(bp => bp.IsPublished)
                .OrderByDescending(bp => bp.PublishedAt ?? bp.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetFeaturedPostsAsync(int count)
        {
            return await _dbSet
                .Include(bp => bp.User)
                .Include(bp => bp.Category)
                .Include(bp => bp.Blog)
                .Where(bp => bp.IsPublished && bp.IsFeatured)
                .OrderByDescending(bp => bp.PublishedAt ?? bp.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetLatestPostsAsync(int count)
        {
            return await _dbSet
                .Include(bp => bp.User)
                .Include(bp => bp.Category)
                .Include(bp => bp.Blog)
                .Where(bp => bp.IsPublished)
                .OrderByDescending(bp => bp.PublishedAt ?? bp.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetPopularPostsAsync(int count)
        {
            return await _dbSet
                .Include(bp => bp.User)
                .Include(bp => bp.Category)
                .Include(bp => bp.Blog)
                .Where(bp => bp.IsPublished)
                .OrderByDescending(bp => bp.ViewCount)
                .ThenByDescending(bp => bp.PublishedAt ?? bp.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetPostsByCategoryAsync(int categoryId, int pageNumber, int pageSize)
        {
            return await _dbSet
                .Include(bp => bp.User)
                .Include(bp => bp.Category)
                .Include(bp => bp.Blog)
                .Where(bp => bp.IsPublished && bp.CategoryId == categoryId)
                .OrderByDescending(bp => bp.PublishedAt ?? bp.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> SearchPostsAsync(string query, int pageNumber, int pageSize)
        {
            return await _dbSet
                .Include(bp => bp.User)
                .Include(bp => bp.Category)
                .Include(bp => bp.Blog)
                .Where(bp => bp.IsPublished && 
                       (bp.Title.Contains(query) || 
                        bp.Content.Contains(query) ||
                        bp.Excerpt!.Contains(query) ||
                        bp.Tags!.Contains(query)))
                .OrderByDescending(bp => bp.PublishedAt ?? bp.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetPostBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(bp => bp.User)
                .Include(bp => bp.Category)
                .Include(bp => bp.Blog)
                .Include(bp => bp.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(bp => bp.Slug == slug);
        }

        public async Task<IEnumerable<BlogPost>> GetRelatedPostsAsync(int postId, int categoryId, int count)
        {
            return await _dbSet
                .Include(bp => bp.User)
                .Include(bp => bp.Category)
                .Where(bp => bp.IsPublished && bp.Id != postId && bp.CategoryId == categoryId)
                .OrderByDescending(bp => bp.ViewCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task IncrementViewCountAsync(int postId)
        {
            var post = await _dbSet.FindAsync(postId);
            if (post != null)
            {
                post.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }
    }
}