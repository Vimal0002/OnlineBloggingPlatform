using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnlineBloggingPlatform.Models;

namespace OnlineBloggingPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure relationships
            builder.Entity<Blog>()
                .HasOne(b => b.User)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BlogPost>()
                .HasOne(bp => bp.User)
                .WithMany(u => u.BlogPosts)
                .HasForeignKey(bp => bp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<BlogPost>()
                .HasOne(bp => bp.Blog)
                .WithMany(b => b.BlogPosts)
                .HasForeignKey(bp => bp.BlogId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<BlogPost>()
                .HasOne(bp => bp.Category)
                .WithMany(c => c.BlogPosts)
                .HasForeignKey(bp => bp.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.BlogPost)
                .WithMany(bp => bp.Comments)
                .HasForeignKey(c => c.BlogPostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure indexes
            builder.Entity<BlogPost>()
                .HasIndex(bp => bp.Slug)
                .IsUnique();

            builder.Entity<BlogPost>()
                .HasIndex(bp => bp.IsPublished);

            builder.Entity<BlogPost>()
                .HasIndex(bp => bp.CreatedAt);

            builder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();

            // Seed data
            SeedData(builder);
        }

        private static void SeedData(ModelBuilder builder)
        {
            // Seed categories
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Technology", Description = "All about technology and innovation", IconClass = "fas fa-laptop-code" },
                new Category { Id = 2, Name = "Lifestyle", Description = "Lifestyle tips and trends", IconClass = "fas fa-heart" },
                new Category { Id = 3, Name = "Business", Description = "Business insights and strategies", IconClass = "fas fa-briefcase" },
                new Category { Id = 4, Name = "Health", Description = "Health and wellness articles", IconClass = "fas fa-heartbeat" },
                new Category { Id = 5, Name = "Travel", Description = "Travel guides and experiences", IconClass = "fas fa-plane" },
                new Category { Id = 6, Name = "Food", Description = "Recipes and culinary adventures", IconClass = "fas fa-utensils" }
            );
        }
    }
}