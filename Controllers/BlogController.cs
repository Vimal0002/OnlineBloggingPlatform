using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBloggingPlatform.Models;
using OnlineBloggingPlatform.Repositories;
using OnlineBloggingPlatform.Services;
using OnlineBloggingPlatform.ViewModels;

namespace OnlineBloggingPlatform.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Blog> _blogRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BlogController(
            IBlogService blogService,
            IRepository<Category> categoryRepository,
            IRepository<Blog> blogRepository,
            UserManager<ApplicationUser> userManager)
        {
            _blogService = blogService;
            _categoryRepository = categoryRepository;
            _blogRepository = blogRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Post(string slug)
        {
            try
            {
                var viewModel = await _blogService.GetPostDetailsAsync(slug);
                return View(viewModel);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var viewModel = new CreateBlogPostViewModel
            {
                Categories = await _categoryRepository.GetAllAsync(),
                UserBlogs = await _blogRepository.FindAsync(b => b.UserId == user.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                try
                {
                    var post = await _blogService.CreatePostAsync(model, user.Id);
                    TempData["Success"] = "Blog post created successfully!";
                    return RedirectToAction("Post", new { slug = post.Slug });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error creating post: {ex.Message}");
                }
            }

            // Reload dropdown data
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                model.Categories = await _categoryRepository.GetAllAsync();
                model.UserBlogs = await _blogRepository.FindAsync(b => b.UserId == currentUser.Id);
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            var postRepository = HttpContext.RequestServices.GetRequiredService<IBlogPostRepository>();
            var post = await postRepository.GetByIdAsync(id);
            
            if (post == null)
            {
                return NotFound();
            }

            if (post.UserId != user.Id)
            {
                return Forbid();
            }

            var viewModel = new EditBlogPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Excerpt = post.Excerpt,
                Content = post.Content,
                FeaturedImageUrl = post.FeaturedImageUrl,
                BlogId = post.BlogId,
                CategoryId = post.CategoryId,
                MetaDescription = post.MetaDescription,
                Tags = post.Tags,
                IsPublished = post.IsPublished,
                IsFeatured = post.IsFeatured,
                Slug = post.Slug,
                CreatedAt = post.CreatedAt,
                ViewCount = post.ViewCount,
                Categories = await _categoryRepository.GetAllAsync(),
                UserBlogs = await _blogRepository.FindAsync(b => b.UserId == user.Id)
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditBlogPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                var postRepository = HttpContext.RequestServices.GetRequiredService<IBlogPostRepository>();
                var post = await postRepository.GetByIdAsync(model.Id);
                
                if (post == null)
                {
                    return NotFound();
                }

                if (post.UserId != user.Id)
                {
                    return Forbid();
                }

                try
                {
                    await _blogService.UpdatePostAsync(model);
                    TempData["Success"] = "Blog post updated successfully!";
                    return RedirectToAction("Post", new { slug = post.Slug });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating post: {ex.Message}");
                }
            }

            // Reload dropdown data
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                model.Categories = await _categoryRepository.GetAllAsync();
                model.UserBlogs = await _blogRepository.FindAsync(b => b.UserId == currentUser.Id);
            }

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(CreateCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Challenge();
                }

                try
                {
                    await _blogService.CreateCommentAsync(model, user.Id);
                    TempData["Success"] = "Comment added successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error adding comment: {ex.Message}";
                }
            }

            // Redirect back to the post
            var postRepository = HttpContext.RequestServices.GetRequiredService<IBlogPostRepository>();
            var post = await postRepository.GetByIdAsync(model.BlogPostId);
            if (post != null)
            {
                return RedirectToAction("Post", new { slug = post.Slug });
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var viewModel = await _blogService.SearchAsync(null, id, null, page);
            ViewBag.CategoryName = category.Name;
            
            return View("Search", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Tag(string tag, int page = 1)
        {
            var viewModel = await _blogService.SearchAsync(null, null, tag, page);
            ViewBag.TagName = tag;
            
            return View("Search", viewModel);
        }
    }
}