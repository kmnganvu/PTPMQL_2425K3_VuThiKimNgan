using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models;

namespace DemoMVC.Controllers
{
    public class PersonController : Controller
    {
        // Khai báo ApplicationDbContext để làm việc với CSDL:
        private readonly ApplicationDbContext _context; // ApplicationDbContext là lớp kết nối với CSDL
        private readonly AutoGenerateCode _autoGenerateCode;// AutoGenerateCode là lớp dùng để tự động sinh mã cho các lớp khác trong dự án.
        public PersonController(ApplicationDbContext context, AutoGenerateCode autoGenerateCode)
        {
            _context = context; //_context dùng để truy vấn hoặc cập nhật dữ liệu
            _autoGenerateCode = autoGenerateCode; // _autoGenerateCode dùng để tự động sinh mã cho các lớp khác trong dự án.
        }
        // Action Index (trả về View 1 list dữ liệu Person trong CSDL): Truy vấn toàn bộ dữ liệu từ bảng Person theo kiểu bất đồng bộ (async/await).
        public async Task<IActionResult> Index()
        {
            var model = await _context.Person.ToListAsync();
            return View(model);
        }
        // Action Create (trả về View thực hiện thêm mới 1 Person vào CSDL)
        public IActionResult Create()
        {
            // Tạo mã PersonID mới tự động:
            // Lấy PersonID cuối cùng trong CSDL để sinh mã mới.
            // Nếu không có Person nào trong CSDL, PersonID sẽ là "PS000".
            // Nếu có Person, lấy PersonID cuối cùng và sinh mã mới dựa trên nó
            var lastPerson = _context.Person.OrderByDescending(p => p.PersonID).FirstOrDefault(); 
            var lastId = lastPerson?.PersonID ?? "PS000"; // Nếu không có Person nào, sử dụng mã LastId mặc định "PS000" là mã khởi đầu
            // Sử dụng AutoGenerateCode để sinh mã mới dựa trên mã cuối cùng.
            var newId = _autoGenerateCode.GenerateCode(lastId);

            ViewBag.NewPersonID = newId;// Lưu mã mới vào ViewBag để sử dụng trong View Create.

            return View();//Trả về view hiển thị danh sách.
        }
        [HttpPost]// Action này sẽ được gọi khi người dùng gửi dữ liệu từ form Create
        [ValidateAntiForgeryToken] //đảm bảo tính bảo mật của hệ thống
        public async Task<IActionResult> Create([Bind("PersonID, FullName, Address,Phone")] Person person)
        {
            if (ModelState.IsValid)// Kiểm tra tính hợp lệ của dữ liệu nhập vào
            {
                
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }
        // Action Edit (trả về View thực hiện sửa thông tin 1 Person trong CSDL)
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Person == null)
            {
                return NotFound();
            }

            var person = await _context.Person.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PersonID,FullName,Address,Phone")] Person person)
        {
            if (id != person.PersonID)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonID))
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
            return View(person);
        }
        // Action Details (trả về View hiển thị thông tin chi tiết 1 Person trong CSDL)
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Person == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }
        // Action Delete (trả về View thực hiện xoá thông tin 1 Person trong CSDL)

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.Person == null)
            {
                return NotFound();
            }

            var person = await _context.Person
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Person == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Person'  is null.");
            }
            var person = await _context.Person.FindAsync(id);
            if (person != null)
            {
                _context.Person.Remove(person);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            // return RedirectToAction("Index", "Person");
        }
        // Phương thức PersonExists (Kiểm tra xem 1 Person có tồn tài trong CSDL không):
        private bool PersonExists(string id)
        {
            return (_context.Person?.Any(e => e.PersonID == id)).GetValueOrDefault();
        }

    }
} 