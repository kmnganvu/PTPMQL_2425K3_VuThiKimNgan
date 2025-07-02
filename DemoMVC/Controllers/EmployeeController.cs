using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using DemoMVC.Models;

namespace DemoMVC.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public string Employee()
        {
            return "This is the Employee action method...";
        }
    }
} 
