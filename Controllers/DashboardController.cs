using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OnlineBloggingPlatform.Models;
using OnlineBloggingPlatform.Services;

namespace OnlineBloggingPlatform.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IDashboardService dashboardService, UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            try
            {
                var dashboardData = await _dashboardService.GetUserDashboardAsync(user.Id);
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading dashboard: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Statistics()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge();
            }

            try
            {
                var statistics = await _dashboardService.GetUserStatisticsAsync(user.Id);
                return PartialView("_UserStatistics", statistics);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error loading statistics: {ex.Message}");
            }
        }
    }
}