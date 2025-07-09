using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers
{
    public class HeThongPhanPhoiController : Controller
    {
        // Khai báo ApplicationDbContext để làm việc với CSDL:
        private readonly ApplicationDbContext _context; // ApplicationDbContext là lớp kết nối với CSDL
        public HeThongPhanPhoiController(ApplicationDbContext context)
        {
            _context = context; //_context dùng để truy vấn hoặc cập nhật dữ liệu
        }

        // Action Index (trả về View 1 list dữ liệu HeThongPhanPhoi trong CSDL):
        public async Task<IActionResult> Index()
        {
            var model = await _context.HeThongPhanPhois.ToListAsync();// Truy vấn toàn bộ dữ liệu từ bảng HeThongPhanPhoi theo kiểu bất đồng bộ (async/await).
            return View(model);
        }
        // Action Create (trả về View thực hiện thêm mới 1 HeThongPhanPhoi vào CSDL)
        public IActionResult Create()
        {
            return View(); // Trả về view hiển thị danh sách.
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // đảm bảo tính bảo mật của hệ thống
        public async Task<IActionResult> Create([Bind("MaHTPP, TenHTPP")] HeThongPhanPhoi heThongPhanPhoi)
        {
            if (ModelState.IsValid) // Kiểm tra tính hợp lệ của dữ liệu nhập vào
            {
                _context.Add(heThongPhanPhoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(heThongPhanPhoi);
        }
        // Action Edit (trả về View thực hiện sửa thông tin 1 HeThongPhanPhoi trong CSDL)
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.HeThongPhanPhois == null)
            {
                return NotFound();
            }

            var heThongPhanPhoi = await _context.HeThongPhanPhois.FindAsync(id);
            if (heThongPhanPhoi == null)
            {
                return NotFound();
            }
            return View(heThongPhanPhoi);
        }
        // Action Edit (xử lý cập nhật thông tin HeThongPhanPhoi)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHTPP, TenHTPP")] HeThongPhanPhoi heThongPhanPhoi)
        {
            if (id != heThongPhanPhoi.MaHTPP)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(heThongPhanPhoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HeThongPhanPhoiExists(heThongPhanPhoi.MaHTPP))
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
            return View(heThongPhanPhoi);
        }
        // Action Details (trả về View hiển thị thông tin chi tiết 1 HeThongPhanPhoi trong CSDL)
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.HeThongPhanPhois == null)
            {
                return NotFound();
            }

            var heThongPhanPhoi = await _context.HeThongPhanPhois.FirstOrDefaultAsync(m => m.MaHTPP == id);
            if (heThongPhanPhoi == null)
            {
                return NotFound();
            }

            return View(heThongPhanPhoi);
        }
        // Action Delete (trả về View thực hiện xoá thông tin 1 HeThongPhanPhoi trong CSDL)
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.HeThongPhanPhois == null)
            {
                return NotFound();
            }

            var heThongPhanPhoi = await _context.HeThongPhanPhois.FirstOrDefaultAsync(m => m.MaHTPP == id);
            if (heThongPhanPhoi == null)
            {
                return NotFound();
            }
            return View(heThongPhanPhoi);
        }
        // Action Delete (xử lý xoá thông tin HeThongPhanPhoi)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.HeThongPhanPhois == null)
            {
                return Problem("Entity set 'ApplicationDbContext.HeThongPhanPhois'  is null.");
            }
            var heThongPhanPhoi = await _context.HeThongPhanPhois.FindAsync(id);
            if (heThongPhanPhoi != null)
            {
                _context.HeThongPhanPhois.Remove(heThongPhanPhoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            // return RedirectToAction("Index", "HeThongPhanPhoi");
        }
        private bool HeThongPhanPhoiExists(string id)
        {
            return _context.HeThongPhanPhois.Any(e => e.MaHTPP == id); // Kiểm tra xem HeThongPhanPhoi có tồn tại trong CSDL hay không
        }
    }
}