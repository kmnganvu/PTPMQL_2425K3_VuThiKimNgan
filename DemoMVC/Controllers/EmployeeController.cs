using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers
{
    // Controller EmployeeController để xử lý các yêu cầu liên quan đến nhân viên
    // Tên của Controller phải kết thúc bằng "Controller"
    // Ví dụ: PersonController, EmployeeController, ProductController, OrderController...
    // Tên của Action phải là một phương thức công khai (public) trong Controller
    // Ví dụ: Index, Create, Edit, Delete...
    public class EmployeeController : Controller
    {
        // Khai báo ApplicationDbContext để làm việc với CSDL:
        private readonly ApplicationDbContext _context;
        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Action Index (trả về View 1 list dữ liệu Employee trong CSDL): await bất đồng bộ
        public async Task<IActionResult> Index()
        {
            var model = await _context.Employee.ToListAsync();
            return View(model);
        }
        // Action Create (trả về View thực hiện thêm mới 1 Employee vào CSDL)
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // đảm bảo tính bảo mật của hệ thống
        public async Task<IActionResult> Create([Bind("PersonID,EmployeeId, FullName, Address, Phone, Age")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        // Action Edit (trả về View thực hiện sửa thông tin 1 Employee trong CSDL)
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);// id là PersonID
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PersonID,EmployeeId,FullName,Address,Phone,Age")] Employee employee)
        {
            if (id != employee.PersonID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.PersonID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        // Action Delete (trả về View thực hiện xóa thông tin 1 Employee trong CSDL)
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Employee == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Employee == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Employee'  is null.");
            }
            var employee = await _context.Employee.FindAsync(id); //id là PersonID
            // Kiểm tra xem Employee có tồn tại trong CSDL hay không
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // Kiểm tra xem Employee có tồn tại trong CSDL hay không
        private bool EmployeeExists(string id)
        {
            return _context.Employee.Any(e => e.PersonID == id);
        }
    }
} 
