using Microsoft.AspNetCore.Mvc;
using SmartWeather.Models;
using SmartWeather.Services;

namespace SmartWeather.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;

        public AccountController(UserService userService)
        {
            _userService = userService;
        }

        // =========================
        // REGISTER
        // =========================
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            if (await _userService.GetByUsername(username) != null)
            {
                ViewBag.Error = "Username đã tồn tại";
                return View();
            }

            await _userService.Create(new User
            {
                Username = username,
                Email = email,
                PasswordHash = password
                // Role mặc định = User (đã set trong UserService)
            });

            return RedirectToAction("Login");
        }

        // =========================
        // LOGIN
        // =========================
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _userService.GetByUsername(username);
            if (user == null || !_userService.VerifyPassword(password, user.PasswordHash))
            {
                ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
                return View();
            }

            // Lưu session
            HttpContext.Session.SetString("User", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            // ⭐ PHÂN QUYỀN
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Weather");
        }

        // =========================
        // LOGOUT
        // =========================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
