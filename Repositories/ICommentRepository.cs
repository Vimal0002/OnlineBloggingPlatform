using OnlineBloggingPlatform.Models;

namespace OnlineBloggingPlatform.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId);
        Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count = 5);
        Task<Comment?> GetCommentWithUserAsync(int commentId);
    }
}