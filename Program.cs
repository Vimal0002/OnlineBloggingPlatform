using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineBloggingPlatform.Data;
using OnlineBloggingPlatform.Models;
using OnlineBloggingPlatform.Services;
using OnlineBloggingPlatform.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<ApplicationUser>(options => {
    options.SignIn.RequireConfirmedAccount = false;
    // Enhanced password policy
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 1;
    
    // Account lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    
    // User settings
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Register repositories and services
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

// Register generic repositories
builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
builder.Services.AddScoped<IRepository<Comment>, Repository<Comment>>();
builder.Services.AddScoped<IRepository<Blog>, Repository<Blog>>();

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IHtmlSanitizationService, HtmlSanitizationService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

builder.Services.AddControllersWithViews(options =>
{
    // Add global anti-forgery filter
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.AutoValidateAntiforgeryTokenAttribute());
});

// Add security headers
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.HttpOnly = true;
    // More flexible SSL policy for cloud deployments
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Add rate limiting (simple implementation)
builder.Services.AddMemoryCache();

// Configure cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = builder.Environment.IsDevelopment() ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Content-Security-Policy"] = 
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
        "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
        "img-src 'self' data: https:; " +
        "font-src 'self' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com;";
    
    await next();
});

app.UseHttpsRedirection();
app.UseCookiePolicy();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Seed data
        await SeedData(context, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();

static async Task SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{
    // Check if admin user already exists
    if (await userManager.FindByEmailAsync("admin@blogplatform.com") == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = "admin@blogplatform.com",
            Email = "admin@blogplatform.com",
            FirstName = "Admin",
            LastName = "User",
            Bio = "Platform administrator and content creator.",
            DateJoined = DateTime.UtcNow,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");
    }

    // Seed categories if they don't exist
    if (!context.Categories.Any())
    {
        var categories = new List<Category>
        {
            new Category { Name = "Technology", Description = "Latest in tech and innovation" },
            new Category { Name = "Lifestyle", Description = "Tips for better living" },
            new Category { Name = "Business", Description = "Business insights and trends" },
            new Category { Name = "Health", Description = "Health and wellness advice" },
            new Category { Name = "Travel", Description = "Travel guides and experiences" }
        };

        context.Categories.AddRange(categories);
        await context.SaveChangesAsync();
    }

    // Seed sample users if they don't exist
    await SeedSampleUsers(context, userManager);

    // Seed sample blog posts if they don't exist
    await SeedSampleBlogPosts(context, userManager);
}

static async Task SeedSampleUsers(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{
    var sampleUsers = new[]
    {
        new { Email = "john.doe@example.com", FirstName = "John", LastName = "Doe", Bio = "Tech enthusiast and software developer." },
        new { Email = "sarah.wilson@example.com", FirstName = "Sarah", LastName = "Wilson", Bio = "Health and wellness coach." },
        new { Email = "mike.thompson@example.com", FirstName = "Mike", LastName = "Thompson", Bio = "Travel blogger and photographer." }
    };

    foreach (var userData in sampleUsers)
    {
        if (await userManager.FindByEmailAsync(userData.Email) == null)
        {
            var user = new ApplicationUser
            {
                UserName = userData.Email,
                Email = userData.Email,
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                Bio = userData.Bio,
                DateJoined = DateTime.UtcNow.AddMonths(-6),
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "User123!");
        }
    }
}

static async Task SeedSampleBlogPosts(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{
    // Check if sample data already exists
    if (context.BlogPosts.Any())
        return;

    // Get users
    var adminUser = await userManager.FindByEmailAsync("admin@blogplatform.com");
    var johnUser = await userManager.FindByEmailAsync("john.doe@example.com");
    var sarahUser = await userManager.FindByEmailAsync("sarah.wilson@example.com");
    var mikeUser = await userManager.FindByEmailAsync("mike.thompson@example.com");

    if (adminUser == null) return;

    var samplePosts = new[]
    {
        new BlogPost
        {
            Title = "Getting Started with ASP.NET Core 8",
            Excerpt = "Discover the latest features and best practices for building scalable web applications with ASP.NET Core 8.",
            Content = @"<div class='post-content'>
                <h2>Introduction to ASP.NET Core 8</h2>
                <p class='lead'>ASP.NET Core 8 represents a significant leap forward in web development technology, offering unprecedented performance and developer productivity enhancements.</p>
                
                <h3>Key Performance Improvements</h3>
                <ul>
                    <li><strong>Native AOT Support:</strong> Significantly reduced startup time and memory footprint</li>
                    <li><strong>Enhanced Minimal APIs:</strong> Better performance with streamlined endpoint definitions</li>
                    <li><strong>Improved JSON Serialization:</strong> Up to 30% faster JSON processing</li>
                    <li><strong>HTTP/3 Support:</strong> Better connection handling and reduced latency</li>
                </ul>
                
                <h3>Developer Experience Enhancements</h3>
                <p>The new version introduces several features that make development more enjoyable:</p>
                <blockquote class='blockquote'>
                    <p>ASP.NET Core 8 feels like a completely different framework - in the best way possible.</p>
                    <footer class='blockquote-footer'>Senior Developer at Microsoft</footer>
                </blockquote>
                
                <h3>Getting Started</h3>
                <p>Ready to dive in? Here's how to create your first ASP.NET Core 8 application:</p>
                <pre><code>dotnet new webapi -n MyAwesomeApi
cd MyAwesomeApi
dotnet run</code></pre>
                
                <div class='alert alert-info mt-4'>
                    <h5>Pro Tip</h5>
                    <p>Start with the minimal API template for rapid prototyping, then migrate to controllers as your application grows.</p>
                </div>
            </div>",
            Slug = "getting-started-aspnet-core-8",
            CategoryId = 1, // Technology
            UserId = johnUser?.Id ?? adminUser.Id,
            IsPublished = true,
            IsFeatured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-2),
            ViewCount = 1247,
            Tags = "ASP.NET Core 8, Web Development, C#, Performance, Minimal APIs",
            MetaDescription = "Complete guide to ASP.NET Core 8 features, performance improvements, and best practices for modern web development"
        },
        new BlogPost
        {
            Title = "The Ultimate Guide to Mindful Living",
            Excerpt = "Discover how mindfulness can revolutionize your daily life with practical tips and easy-to-follow exercises.",
            Content = @"<div class='post-content'>
                <h2>What is Mindful Living?</h2>
                <p class='lead'>Mindful living is the practice of being fully present and engaged in each moment, aware of your thoughts, feelings, and surroundings.</p>
                
                <h3>The Science Behind Mindfulness</h3>
                <p>Research shows that regular mindfulness practice can:</p>
                <div class='row'>
                    <div class='col-md-6'>
                        <ul>
                            <li>Reduce stress and anxiety by up to 58%</li>
                            <li>Improve focus and concentration</li>
                            <li>Enhance emotional regulation</li>
                            <li>Boost immune system function</li>
                        </ul>
                    </div>
                    <div class='col-md-6'>
                        <ul>
                            <li>Lower blood pressure</li>
                            <li>Improve sleep quality</li>
                            <li>Increase life satisfaction</li>
                            <li>Reduce symptoms of depression</li>
                        </ul>
                    </div>
                </div>
                
                <h3>Morning Mindfulness Routine</h3>
                <p>Start your day with intention using this 15-minute routine:</p>
                <ol>
                    <li><strong>Mindful Awakening (3 minutes):</strong> Take three deep breaths and set an intention</li>
                    <li><strong>Gratitude Practice (5 minutes):</strong> Write down three things you're grateful for</li>
                    <li><strong>Meditation (5 minutes):</strong> Simple breathing meditation</li>
                    <li><strong>Mindful Movement (2 minutes):</strong> Gentle stretching with awareness</li>
                </ol>
                
                <div class='alert alert-success'>
                    <h5>Your Next Steps</h5>
                    <p class='mb-0'>Choose one technique and practice it for seven days. Notice how it affects your mood and well-being.</p>
                </div>
            </div>",
            Slug = "ultimate-guide-mindful-living",
            CategoryId = 4, // Health
            UserId = sarahUser?.Id ?? adminUser.Id,
            IsPublished = true,
            IsFeatured = true,
            PublishedAt = DateTime.UtcNow.AddDays(-1),
            ViewCount = 892,
            Tags = "Mindfulness, Mental Health, Wellness, Meditation, Self-Care",
            MetaDescription = "Complete guide to mindful living with practical exercises and daily routines for better mental health"
        },
        new BlogPost
        {
            Title = "Hidden Gems: Amazing Travel Destinations",
            Excerpt = "Escape the crowds and discover stunning locations that most travelers never see.",
            Content = @"<div class='post-content'>
                <h2>Why Choose Off-the-Beaten-Path Destinations?</h2>
                <p class='lead'>In a world where popular destinations are crowded and expensive, hidden gems offer authentic experiences and better value.</p>
                
                <h3>1. Faroe Islands, Denmark</h3>
                <p><strong>What makes it special:</strong> Dramatic cliffs, grass-roof houses, and spectacular hiking trails.</p>
                <div class='card mb-3 border-info'>
                    <div class='card-body'>
                        <h6 class='text-info'>Insider Tip:</h6>
                        <p class='mb-0'>Visit during summer for hiking, or winter for Northern Lights.</p>
                    </div>
                </div>
                
                <h3>2. Svaneti, Georgia</h3>
                <p>Ancient stone towers, pristine glaciers, and traditional villages frozen in time.</p>
                <ul>
                    <li><strong>Best activity:</strong> Trekking to Shkhara Glacier</li>
                    <li><strong>Must-try:</strong> Traditional Georgian feast</li>
                    <li><strong>When to visit:</strong> June-September for hiking</li>
                </ul>
                
                <div class='alert alert-success'>
                    <h5>Start Your Journey</h5>
                    <p class='mb-0'>Which hidden gem calls to your adventurous spirit? The best time to visit is now!</p>
                </div>
            </div>",
            Slug = "hidden-gems-amazing-travel-destinations",
            CategoryId = 5, // Travel
            UserId = mikeUser?.Id ?? adminUser.Id,
            IsPublished = true,
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddHours(-6),
            ViewCount = 567,
            Tags = "Travel, Hidden Gems, Adventure, Exploration",
            MetaDescription = "Discover amazing hidden travel destinations away from crowds with practical travel tips"
        },
        new BlogPost
        {
            Title = "Sustainable Business Practices That Work",
            Excerpt = "How companies are discovering that sustainable practices benefit both planet and profits.",
            Content = @"<div class='post-content'>
                <h2>The Business Case for Sustainability</h2>
                <p class='lead'>Environmental responsibility and profitability go hand in hand in today's business world.</p>
                
                <h3>Key Benefits</h3>
                <ul>
                    <li>73% of consumers willing to pay more for sustainable products</li>
                    <li>25% average cost reduction from sustainable practices</li>
                    <li>16% higher employee retention in sustainable companies</li>
                </ul>
                
                <h3>Getting Started</h3>
                <ol>
                    <li><strong>Assess Current Impact:</strong> Conduct a sustainability audit</li>
                    <li><strong>Set Clear Goals:</strong> Establish measurable targets</li>
                    <li><strong>Engage Stakeholders:</strong> Include employees and customers</li>
                    <li><strong>Implement Gradually:</strong> Start with high-impact initiatives</li>
                </ol>
                
                <div class='alert alert-info'>
                    <h5>The Future is Sustainable</h5>
                    <p class='mb-0'>Companies embracing sustainable practices today will be tomorrow's leaders.</p>
                </div>
            </div>",
            Slug = "sustainable-business-practices-that-work",
            CategoryId = 3, // Business
            UserId = adminUser.Id,
            IsPublished = true,
            IsFeatured = false,
            PublishedAt = DateTime.UtcNow.AddDays(-4),
            ViewCount = 423,
            Tags = "Sustainability, Business, ESG, Corporate Responsibility",
            MetaDescription = "Learn how sustainable business practices drive profitability and environmental impact"
        }
    };

    // Add posts to context
    await context.BlogPosts.AddRangeAsync(samplePosts);
    await context.SaveChangesAsync();
    
    // Add some sample comments
    var comments = new[]
    {
        new Comment
        {
            Content = "Great article! ASP.NET Core 8 really has impressive performance improvements.",
            UserId = sarahUser?.Id ?? adminUser.Id,
            BlogPostId = samplePosts[0].Id,
            CreatedAt = DateTime.UtcNow.AddHours(-2)
        },
        new Comment
        {
            Content = "Thank you for this comprehensive guide. The mindfulness techniques are really helpful.",
            UserId = mikeUser?.Id ?? adminUser.Id,
            BlogPostId = samplePosts[1].Id,
            CreatedAt = DateTime.UtcNow.AddHours(-1)
        },
        new Comment
        {
            Content = "I've been looking for off-the-beaten-path destinations. The Faroe Islands look incredible!",
            UserId = johnUser?.Id ?? adminUser.Id,
            BlogPostId = samplePosts[2].Id,
            CreatedAt = DateTime.UtcNow.AddMinutes(-30)
        }
    };
    
    await context.Comments.AddRangeAsync(comments);
    await context.SaveChangesAsync();
}