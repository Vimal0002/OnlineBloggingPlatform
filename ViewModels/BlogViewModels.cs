using OnlineBloggingPlatform.Models;
using System.ComponentModel.DataAnnotations;

namespace OnlineBloggingPlatform.ViewModels
{
    public class CreateBlogPostViewModel
    {
        [Required]
        [StringLength(300)]
        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Excerpt")]
        public string? Excerpt { get; set; }

        [Required]
        [Display(Name = "Content")]
        public string Content { get; set; } = string.Empty;

        [Display(Name = "Featured Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(2000, ErrorMessage = "URL cannot exceed 2000 characters")]
        public string? FeaturedImageUrl { get; set; }

        [Display(Name = "Blog")]
        public int? BlogId { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [StringLength(500)]
        [Display(Name = "Meta Description")]
        public string? MetaDescription { get; set; }

        [StringLength(200)]
        [Display(Name = "Tags (comma separated)")]
        public string? Tags { get; set; }

        [Display(Name = "Published")]
        public bool IsPublished { get; set; }

        [Display(Name = "Featured")]
        public bool IsFeatured { get; set; }

        // For dropdowns
        public IEnumerable<Blog>? UserBlogs { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
    }

    public class EditBlogPostViewModel : CreateBlogPostViewModel
    {
        public int Id { get; set; }
        public string? Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
    }

    public class BlogPostDetailsViewModel
    {
        public BlogPost BlogPost { get; set; } = null!;
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
        public CreateCommentViewModel NewComment { get; set; } = new CreateCommentViewModel();
        public IEnumerable<BlogPost> RelatedPosts { get; set; } = new List<BlogPost>();
    }

    public class CreateCommentViewModel
    {
        [Required(ErrorMessage = "Comment content is required")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Comment must be between 3 and 1000 characters")]
        [Display(Name = "Comment")]
        public string Content { get; set; } = string.Empty;

        public int BlogPostId { get; set; }
        public int? ParentCommentId { get; set; }
    }

    public class HomeViewModel
    {
        public IEnumerable<BlogPost> FeaturedPosts { get; set; } = new List<BlogPost>();
        public IEnumerable<BlogPost> LatestPosts { get; set; } = new List<BlogPost>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public IEnumerable<BlogPost> PopularPosts { get; set; } = new List<BlogPost>();
    }

    public class SearchViewModel
    {
        public string? Query { get; set; }
        public int? CategoryId { get; set; }
        public string? Tag { get; set; }
        public IEnumerable<BlogPost> Results { get; set; } = new List<BlogPost>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }

    public class UserDashboardViewModel
    {
        public ApplicationUser User { get; set; } = null!;
        public UserStatisticsViewModel Statistics { get; set; } = null!;
        public IEnumerable<BlogPost> RecentPosts { get; set; } = new List<BlogPost>();
        public IEnumerable<Blog> UserBlogs { get; set; } = new List<Blog>();
    }

    public class UserStatisticsViewModel
    {
        public int TotalPosts { get; set; }
        public int TotalViews { get; set; }
        public int TotalBlogs { get; set; }
        public int FeaturedPosts { get; set; }
        public int PublishedPosts { get; set; }
        public int DraftPosts { get; set; }
        public int TotalComments { get; set; }
    }

}
