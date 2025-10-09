using OnlineBloggingPlatform.ViewModels;
using OnlineBloggingPlatform.Models;

namespace OnlineBloggingPlatform.Services
{
    public interface IDashboardService
    {
        Task<UserDashboardViewModel> GetUserDashboardAsync(string userId);
        Task<UserStatisticsViewModel> GetUserStatisticsAsync(string userId);
        Task<IEnumerable<BlogPost>> GetUserRecentPostsAsync(string userId, int count = 5);
        Task<IEnumerable<Blog>> GetUserBlogsAsync(string userId);
    }
}