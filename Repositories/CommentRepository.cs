using Microsoft.EntityFrameworkCore;
using OnlineBloggingPlatform.Data;
using OnlineBloggingPlatform.Models;

namespace OnlineBloggingPlatform.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostIdAsync(int postId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.BlogPostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetRecentCommentsAsync(int count = 5)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.BlogPost)
                .OrderByDescending(c => c.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Comment?> GetCommentWithUserAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.BlogPost)
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }
    }
}