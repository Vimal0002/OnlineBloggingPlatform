using Microsoft.EntityFrameworkCore;
using OnlineBloggingPlatform.Data;
using OnlineBloggingPlatform.Models;
using OnlineBloggingPlatform.ViewModels;

namespace OnlineBloggingPlatform.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserDashboardViewModel> GetUserDashboardAsync(string userId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new ArgumentException("User not found");

            var statistics = await GetUserStatisticsAsync(userId);
            var recentPosts = await GetUserRecentPostsAsync(userId, 5);
            var userBlogs = await GetUserBlogsAsync(userId);

            return new UserDashboardViewModel
            {
                User = user,
                Statistics = statistics,
                RecentPosts = recentPosts,
                UserBlogs = userBlogs
            };
        }

        public async Task<UserStatisticsViewModel> GetUserStatisticsAsync(string userId)
        {
            var totalPosts = await _context.BlogPosts
                .Where(bp => bp.UserId == userId)
                .CountAsync();

            var totalViews = await _context.BlogPosts
                .Where(bp => bp.UserId == userId)
                .SumAsync(bp => bp.ViewCount);

            var totalBlogs = await _context.Blogs
                .Where(b => b.UserId == userId && b.IsActive)
                .CountAsync();

            var featuredPosts = await _context.BlogPosts
                .Where(bp => bp.UserId == userId && bp.IsFeatured && bp.IsPublished)
                .CountAsync();

            var publishedPosts = await _context.BlogPosts
                .Where(bp => bp.UserId == userId && bp.IsPublished)
                .CountAsync();

            var draftPosts = await _context.BlogPosts
                .Where(bp => bp.UserId == userId && !bp.IsPublished)
                .CountAsync();

            var totalComments = await _context.Comments
                .Where(c => c.BlogPost.UserId == userId)
                .CountAsync();

            return new UserStatisticsViewModel
            {
                TotalPosts = totalPosts,
                TotalViews = totalViews,
                TotalBlogs = totalBlogs,
                FeaturedPosts = featuredPosts,
                PublishedPosts = publishedPosts,
                DraftPosts = draftPosts,
                TotalComments = totalComments
            };
        }

        public async Task<IEnumerable<BlogPost>> GetUserRecentPostsAsync(string userId, int count = 5)
        {
            return await _context.BlogPosts
                .Where(bp => bp.UserId == userId)
                .Include(bp => bp.Blog)
                .Include(bp => bp.Category)
                .OrderByDescending(bp => bp.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Blog>> GetUserBlogsAsync(string userId)
        {
            return await _context.Blogs
                .Where(b => b.UserId == userId && b.IsActive)
                .Include(b => b.BlogPosts)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();
        }
    }
}