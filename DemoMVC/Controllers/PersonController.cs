using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models.Entities;
using DemoMVC.Models.Process;
using OfficeOpenXml;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DemoMVC.Controllers
{
    public class PersonController : Controller
    {
        // Khai báo ApplicationDbContext để làm việc với CSDL:
        private readonly ApplicationDbContext _context; // ApplicationDbContext là lớp kết nối với CSDL
        private ExcelProcess _excelProcess = new ExcelProcess(); // Khởi tạo đối tượng ExcelProcess để xử lý file Excel
        public PersonController(ApplicationDbContext context)
        {
            _context = context; //_context dùng để truy vấn hoặc cập nhật dữ liệu
           
        }
        // Action Index (trả về View 1 list dữ liệu Person trong CSDL): Truy vấn toàn bộ dữ liệu từ bảng Person theo kiểu bất đồng bộ (async/await).
        // Sử dụng phân trang với thư viện X.PagedList để hiển thị dữ liệu theo trang
        public async Task<IActionResult> Index(int? page, int? pageSize)
        {
            ViewBag.PageSize = new List<SelectListItem>()
            {
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "5", Value = "5" },
                new SelectListItem { Text = "10", Value = "10" },
                new SelectListItem { Text = "15", Value = "15" },
                new SelectListItem { Text = "25", Value = "25" },
                new SelectListItem { Text = "50", Value = "50" }
            };
            int pagesize = (pageSize ?? 3); // Lấy kích thước trang từ tham số pageSize, nếu không có thì mặc định là 3
            ViewBag.psize = pagesize; // Lưu kích thước trang vào ViewBag để sử dụng trong View
            
            var model = _context.Persons.ToList().ToPagedList(page ?? 1, pagesize); // Lấy danh sách Person từ CSDL mà không theo dõi thay đổi

            return View(model);
        }
        public async Task<IActionResult> Upload()
        {
            // Trả về View Upload để người dùng có thể tải lên file Excel
            return View("Create");
        }
        // Action UploadFile (xử lý việc tải lên file Excel và chuyển đổi thành DataTable)
        [HttpPost]
        [ValidateAntiForgeryToken] //đảm bảo tính bảo mật của hệ thống
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null)
            {
                
                string fileExtension = Path.GetExtension(file.FileName);
                // Kiểm tra định dạng file tải lên
                // Nếu file không phải là định dạng Excel (.xls hoặc .xlsx), thêm lỗi vào
                if (fileExtension != ".xls" && fileExtension != ".xlsx")
                {
                    ModelState.AddModelError("", "Please choose excel file to upload");

                }
                else
                {
                    // Tạo đường dẫn lưu file Excel

                    var fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileExtension;
                    var filePath = Path.Combine(Directory.GetCurrentDirectory() + "/Uploads/Excel", fileName);
                    var fileLocation = new FileInfo(filePath).ToString();
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // Lưu file vào thư mục Uploads/Excel
                        // Sử dụng IFormFile để lưu trữ file tải lên
                        await file.CopyToAsync(stream);
                        // Chuyển đổi file Excel thành DataTable
                        // Sử dụng phương thức ExcelToDataTable từ lớp ExcelProcess để chuyển đổi
                        var dt = _excelProcess.ExcelToDataTable(fileLocation);
                        // Kiểm tra xem DataTable có dữ liệu hay không
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            // Tạo đối tượng Person từ DataTable
                            var ps = new Person();
                            // Gán giá trị từ DataTable vào các thuộc tính của Person
                            ps.PersonID = dt.Rows[i][0].ToString();
                            ps.FullName = dt.Rows[i][1].ToString();
                            ps.Address = dt.Rows[i][2].ToString();
                            ps.Phone = dt.Rows[i][3].ToString();
                            // Thêm đối tượng Person vào CSDL
                            _context.Add(ps);
                        }
                        // Lưu thay đổi vào CSDL
                        await _context.SaveChangesAsync();
                        // Chuyển hướng về Action Index để hiển thị danh sách Person
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            return View("Create");
        }
                // Action Download (trả về file Excel cho người dùng tải xuống)
        public IActionResult Download()
        {
            // Tạo file Excel từ dữ liệu trong CSDL và trả về file cho người dùng tải xuống
            // Sử dụng thư viện EPPlus để tạo file Excel
            var fileName = "YourFileName" + ".xlsx";
            // Tạo một đối tượng ExcelPackage để làm việc với file Excel
            // ExcelPackage là lớp chính của thư viện EPPlus để tạo và thao tác với file
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                // Tạo một worksheet mới trong file Excel
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");
                // Đặt tiêu đề cho các cột trong worksheet
                // Các cột này sẽ tương ứng với các thuộc tính của Person
                worksheet.Cells["A1"].Value = "PersonID";
                worksheet.Cells["B1"].Value = "FullName";
                worksheet.Cells["C1"].Value = "Address";
                worksheet.Cells["D1"].Value = "Phone";
                // Lấy danh sách Person từ CSDL và nạp vào worksheet
                // Sử dụng _context để truy vấn dữ liệu từ bảng Persons
                var personList = _context.Persons.ToList();
                // Sử dụng LoadFromCollection để nạp dữ liệu từ danh sách Person vào worksheet
                // Dữ liệu sẽ được nạp từ hàng thứ 2 (A2) trở đi
                worksheet.Cells["A2"].LoadFromCollection(personList);
                // Định dạng các cột trong worksheet
                // Đặt độ rộng cho các cột để hiển thị đầy đủ dữ liệu
                var stream = new MemoryStream(excelPackage.GetAsByteArray());
                // Đặt tên file và trả về file Excel cho người dùng tải xuống
                // Sử dụng File để trả về file Excel với kiểu MIME là application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                // MemoryStream được sử dụng để lưu trữ dữ liệu của file Excel trong bộ nhớ
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
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