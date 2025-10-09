using Microsoft.AspNetCore.Mvc;
using OnlineBloggingPlatform.Services;
using OnlineBloggingPlatform.ViewModels;
using System.Diagnostics;

namespace OnlineBloggingPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogService _blogService;

        public HomeController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = await _blogService.GetHomePageDataAsync();
            return View(viewModel);
        }

        public async Task<IActionResult> Search(string? query, int? categoryId, string? tag, int page = 1)
        {
            var viewModel = await _blogService.SearchAsync(query, categoryId, tag, page);
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
}