using OnlineBloggingPlatform.Models;
using OnlineBloggingPlatform.Repositories;
using OnlineBloggingPlatform.ViewModels;
using System.Text.RegularExpressions;

namespace OnlineBloggingPlatform.Services
{
    public interface IBlogService
    {
        Task<HomeViewModel> GetHomePageDataAsync();
        Task<SearchViewModel> SearchAsync(string? query, int? categoryId, string? tag, int page = 1);
        Task<BlogPostDetailsViewModel> GetPostDetailsAsync(string slug);
        Task<BlogPost> CreatePostAsync(CreateBlogPostViewModel model, string userId);
        Task<BlogPost> UpdatePostAsync(EditBlogPostViewModel model);
        Task<Comment> CreateCommentAsync(CreateCommentViewModel model, string userId);
        string GenerateSlug(string title);
    }

    public class BlogService : IBlogService
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly IHtmlSanitizationService _htmlSanitizer;
        private const int PageSize = 10;

        public BlogService(
            IBlogPostRepository blogPostRepository,
            IRepository<Category> categoryRepository,
            IRepository<Comment> commentRepository,
            IRepository<Blog> blogRepository,
            IHtmlSanitizationService htmlSanitizer)
        {
            _blogPostRepository = blogPostRepository;
            _categoryRepository = categoryRepository;
            _commentRepository = commentRepository;
            _blogRepository = blogRepository;
            _htmlSanitizer = htmlSanitizer;
        }

        public async Task<HomeViewModel> GetHomePageDataAsync()
        {
            var featuredPosts = await _blogPostRepository.GetFeaturedPostsAsync(5);
            var latestPosts = await _blogPostRepository.GetLatestPostsAsync(10);
            var popularPosts = await _blogPostRepository.GetPopularPostsAsync(5);
            var categories = await _categoryRepository.GetAllAsync();

            return new HomeViewModel
            {
                FeaturedPosts = featuredPosts,
                LatestPosts = latestPosts,
                PopularPosts = popularPosts,
                Categories = categories
            };
        }

        public async Task<SearchViewModel> SearchAsync(string? query, int? categoryId, string? tag, int page = 1)
        {
            IEnumerable<BlogPost> results = new List<BlogPost>();
            int totalResults = 0;

            if (!string.IsNullOrWhiteSpace(query))
            {
                results = await _blogPostRepository.SearchPostsAsync(query, page, PageSize);
                totalResults = await _blogPostRepository.CountAsync(bp => 
                    bp.IsPublished && 
                    (bp.Title.Contains(query) || 
                     bp.Content.Contains(query) ||
                     bp.Excerpt!.Contains(query) ||
                     bp.Tags!.Contains(query)));
            }
            else if (categoryId.HasValue)
            {
                results = await _blogPostRepository.GetPostsByCategoryAsync(categoryId.Value, page, PageSize);
                totalResults = await _blogPostRepository.CountAsync(bp => 
                    bp.IsPublished && bp.CategoryId == categoryId);
            }
            else if (!string.IsNullOrWhiteSpace(tag))
            {
                results = await _blogPostRepository.FindAsync(bp => 
                    bp.IsPublished && bp.Tags != null && bp.Tags.Contains(tag));
                totalResults = results.Count();
                results = results.Skip((page - 1) * PageSize).Take(PageSize);
            }

            var categories = await _categoryRepository.GetAllAsync();
            var totalPages = (int)Math.Ceiling((double)totalResults / PageSize);

            return new SearchViewModel
            {
                Query = query,
                CategoryId = categoryId,
                Tag = tag,
                Results = results,
                Categories = categories,
                TotalResults = totalResults,
                CurrentPage = page,
                TotalPages = totalPages
            };
        }

        public async Task<BlogPostDetailsViewModel> GetPostDetailsAsync(string slug)
        {
            var post = await _blogPostRepository.GetPostBySlugAsync(slug);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            await _blogPostRepository.IncrementViewCountAsync(post.Id);

            var comments = post.Comments.Where(c => c.IsApproved && c.ParentCommentId == null)
                .OrderBy(c => c.CreatedAt).ToList();

            var relatedPosts = post.CategoryId.HasValue 
                ? await _blogPostRepository.GetRelatedPostsAsync(post.Id, post.CategoryId.Value, 3)
                : new List<BlogPost>();

            return new BlogPostDetailsViewModel
            {
                BlogPost = post,
                Comments = comments,
                RelatedPosts = relatedPosts,
                NewComment = new CreateCommentViewModel { BlogPostId = post.Id }
            };
        }

        public async Task<BlogPost> CreatePostAsync(CreateBlogPostViewModel model, string userId)
        {
            var slug = GenerateSlug(model.Title);
            
            // Ensure slug is unique
            var existingPost = await _blogPostRepository.FirstOrDefaultAsync(p => p.Slug == slug);
            if (existingPost != null)
            {
                slug = $"{slug}-{DateTime.Now.Ticks}";
            }

            var post = new BlogPost
            {
                Title = model.Title?.Trim() ?? string.Empty,
                Excerpt = model.Excerpt?.Trim(),
                Content = _htmlSanitizer.Sanitize(model.Content),
                FeaturedImageUrl = model.FeaturedImageUrl?.Trim(),
                UserId = userId,
                BlogId = model.BlogId,
                CategoryId = model.CategoryId,
                MetaDescription = model.MetaDescription?.Trim(),
                Tags = model.Tags?.Trim(),
                IsPublished = model.IsPublished,
                IsFeatured = model.IsFeatured,
                Slug = slug,
                PublishedAt = model.IsPublished ? DateTime.UtcNow : null,
                UpdatedAt = DateTime.UtcNow
            };

            return await _blogPostRepository.AddAsync(post);
        }

        public async Task<BlogPost> UpdatePostAsync(EditBlogPostViewModel model)
        {
            var post = await _blogPostRepository.GetByIdAsync(model.Id);
            if (post == null)
                throw new InvalidOperationException("Post not found");

            post.Title = model.Title?.Trim() ?? string.Empty;
            post.Excerpt = model.Excerpt?.Trim();
            post.Content = _htmlSanitizer.Sanitize(model.Content);
            post.FeaturedImageUrl = model.FeaturedImageUrl?.Trim();
            post.BlogId = model.BlogId;
            post.CategoryId = model.CategoryId;
            post.MetaDescription = model.MetaDescription?.Trim();
            post.Tags = model.Tags?.Trim();
            post.IsFeatured = model.IsFeatured;
            post.UpdatedAt = DateTime.UtcNow;

            // Handle publish status change
            if (!post.IsPublished && model.IsPublished)
            {
                post.IsPublished = true;
                post.PublishedAt = DateTime.UtcNow;
            }
            else if (post.IsPublished && !model.IsPublished)
            {
                post.IsPublished = false;
                post.PublishedAt = null;
            }

            await _blogPostRepository.UpdateAsync(post);
            return post;
        }

        public async Task<Comment> CreateCommentAsync(CreateCommentViewModel model, string userId)
        {
            var comment = new Comment
            {
                Content = _htmlSanitizer.SanitizeForDisplay(model.Content),
                UserId = userId,
                BlogPostId = model.BlogPostId,
                ParentCommentId = model.ParentCommentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _commentRepository.AddAsync(comment);
        }

        public string GenerateSlug(string title)
        {
            if (string.IsNullOrEmpty(title))
                return string.Empty;

            // Convert to lowercase
            string slug = title.ToLowerInvariant();

            // Remove invalid characters
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");

            // Replace spaces with hyphens
            slug = Regex.Replace(slug, @"\s+", "-");

            // Remove duplicate hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Trim hyphens from ends
            slug = slug.Trim('-');

            // Limit length
            if (slug.Length > 50)
                slug = slug.Substring(0, 50).TrimEnd('-');

            return slug;
        }
    }
}