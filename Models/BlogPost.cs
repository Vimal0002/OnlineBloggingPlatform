using System.ComponentModel.DataAnnotations;

namespace OnlineBloggingPlatform.Models
{
    public class BlogPost
    {
        public int Id { get; set; }

        [Required]
        [StringLength(300)]
        public string Title { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Excerpt { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? FeaturedImageUrl { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public int? BlogId { get; set; }

        public int? CategoryId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAt { get; set; }

        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;

        public int ViewCount { get; set; } = 0;

        [StringLength(500)]
        public string? MetaDescription { get; set; }

        [StringLength(200)]
        public string? Tags { get; set; }

        // SEO-friendly URL slug
        [StringLength(300)]
        public string? Slug { get; set; }

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Blog? Blog { get; set; }
        public virtual Category? Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}