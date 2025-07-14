using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models.Entities;
using DemoMVC.Models.Process;

namespace DemoMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employee/Create
        public IActionResult Create()
        {
            // Tạo mã tự động cho PersonID và EmployeeId
            // Giả sử PersonID và EmployeeId có định dạng là "PS000" và "EP000" tương ứng.
            // Sinh mã tự động cho PersonID
            // Lấy Person cuối cùng trong CSDL để sinh mã mới
            var lastPerson = _context.Persons.OrderByDescending(p => p.PersonID).FirstOrDefault();
            // Nếu không có Person nào, sử dụng mã mặc định
            var lastPersonId = lastPerson?.PersonID ?? "PS000";
            // Sử dụng AutoGenerateCode để sinh mã mới
            var newPersonId = new AutoGenerateCode().GenerateId(lastPersonId);

            // Sinh mã tự động cho EmployeeId
            var lastEmployee = _context.Employees.OrderByDescending(e => e.EmployeeId).FirstOrDefault();
            var lastEmployeeId = lastEmployee?.EmployeeId ?? "EP000";
            var newEmployeeId = new AutoGenerateCode().GenerateId(lastEmployeeId);

            // Khởi tạo Employee với mã mới
            var employee = new Employee
            {
                PersonID = newPersonId,
                EmployeeId = newEmployeeId
            };

            return View(employee);
        }

        // POST: Employee/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PersonID,EmployeeId,Age,FullName,Address,Phone")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                // Nếu dữ liệu hợp lệ, thêm Employee vào CSDL và lưu thay đổi
                // _context là đối tượng DbContext dùng để tương tác với CSDL
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employee/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("EmployeeId,Age,PersonID,FullName,Address,Phone")] Employee employee)
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

        // GET: Employee/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(string id)
        {
            return _context.Employees.Any(e => e.PersonID == id);
        }
    }
}
