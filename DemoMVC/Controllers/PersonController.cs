using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models.Entities;
using DemoMVC.Models.Process;

namespace DemoMVC.Controllers
{
    public class PersonController : Controller
    {
        // Khai báo ApplicationDbContext để làm việc với CSDL:
        private readonly ApplicationDbContext _context; // ApplicationDbContext là lớp kết nối với CSDL
        
        public PersonController(ApplicationDbContext context)
        {
            _context = context; //_context dùng để truy vấn hoặc cập nhật dữ liệu
           
        }
        // Action Index (trả về View 1 list dữ liệu Person trong CSDL): Truy vấn toàn bộ dữ liệu từ bảng Person theo kiểu bất đồng bộ (async/await).
        public async Task<IActionResult> Index()
        {
            // Truy vấn toàn bộ dữ liệu từ bảng Persons theo kiểu bất đồng bộ (async/await).
            // Sử dụng ToListAsync() để lấy danh sách Person từ CSDL.
            return View(await _context.Persons.ToListAsync());
        }
        // Action Create (trả về View thực hiện thêm mới 1 Person vào CSDL)
        public IActionResult Create()
        {
            // Tạo mã PersonID mới tự động:
            // Lấy PersonID cuối cùng trong CSDL để sinh mã mới.
            // Nếu không có Person nào trong CSDL, PersonID sẽ là "PS000".
            // Nếu có Person, lấy PersonID cuối cùng và sinh mã mới dựa trên nó
            //1. Lay ra ban ghi moi nhat cua Person
            var person = _context.Persons.OrderByDescending(p => p.PersonID).FirstOrDefault();
            //2. Neu person == null thi gan PersonID = PS000
            var personID = person == null ? "PS000" : person.PersonID;
            //3. Goi toi phuong thuc sinh id tu dong
            var autoGenerateId = new AutoGenerateCode();
            var newPersonID = autoGenerateId.GenerateId(personID);
            var newPerson = new Person
            {
                PersonID = newPersonID
            };
            return View(newPerson);
        }
        [HttpPost]// Action này sẽ được gọi khi người dùng gửi dữ liệu từ form Create
        [ValidateAntiForgeryToken] //đảm bảo tính bảo mật của hệ thống
        public async Task<IActionResult> Create([Bind("PersonID, FullName, Address,Phone")] Person person)
        {
            if (ModelState.IsValid)// Kiểm tra tính hợp lệ của dữ liệu nhập vào
            {
                // Nếu dữ liệu hợp lệ, thêm Person vào CSDL và lưu thay đổi
                // _context là đối tượng DbContext dùng để tương tác với CSDL
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(person);
        }
        // Action Edit (trả về View thực hiện sửa thông tin 1 Person trong CSDL)
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons.FindAsync(id);
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
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
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
            if (id == null || _context.Persons == null)
            {
                return NotFound();
            }

            var person = await _context.Persons
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
            if (_context.Persons == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Person'  is null.");
            }
            var person = await _context.Persons.FindAsync(id);
            if (person != null)
            {
                _context.Persons.Remove(person);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            // return RedirectToAction("Index", "Person");
        }
        // Phương thức PersonExists (Kiểm tra xem 1 Person có tồn tài trong CSDL không):
        private bool PersonExists(string id)
        {
            return (_context.Persons?.Any(e => e.PersonID == id)).GetValueOrDefault();
        }

    }
} 