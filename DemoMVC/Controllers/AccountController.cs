using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Models;
using DemoMVC.Models.Entities;

namespace DemoMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;


        public AccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;

        }
        public async Task<ActionResult> Index()
        {
            // Lấy danh sách người dùng từ UserManager
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
    }
}