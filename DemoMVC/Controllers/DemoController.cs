using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc;
using DemoMVC.Models;

namespace DemoMVC.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        // GET: /HelloWorld/Welcome/ 

        public IActionResult Welcome()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(Demo dm)
        {
            string strOutput = "Xin Chào " + dm.FullName + "-" + dm.Address;
            ViewBag.Message = strOutput;
            return View();

        }
    }
}
