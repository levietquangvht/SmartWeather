using Microsoft.AspNetCore.Mvc;
using SmartWeather.Services;

namespace SmartWeather.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserService _userService;

        public AdminController(UserService userService)
        {
            _userService = userService;
        }

        // =========================
        // CHECK ADMIN
        // =========================
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        // =========================
        // GET: /Admin
        // =========================
        public IActionResult Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            return View();
        }

        // =========================
        // GET: /Admin/Users
        // =========================
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            var users = await _userService.GetAllUsers();
            return View(users);
        }

        // =========================
        // POST: /Admin/SetRole
        // =========================
        [HttpPost]
        public async Task<IActionResult> SetRole(string username, string role)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account");

            await _userService.SetRole(username, role);
            return RedirectToAction("Users");
        }
    }
}
