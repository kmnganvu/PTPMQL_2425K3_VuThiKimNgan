using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models.Entities;
using DemoMVC.Models.Process;
using OfficeOpenXml;

namespace DemoMVC.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Constructor nhận vào ApplicationDbContext để tương tác với CSDL
        // ApplicationDbContext là lớp DbContext được định nghĩa trong DemoMVC.Data
        private ExcelProcess _excelProcess = new ExcelProcess(); // Khởi tạo đối tượng ExcelProcess để xử lý file Excel

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employee
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.ToListAsync());
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
                        int successCount = 0; // Biến đếm số lượng bản ghi thành công
                        // Biến đếm số lượng bản ghi trùng lặp
                        int duplicateCount = 0;
                        // Kiểm tra xem DataTable có dữ liệu hay không
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            // Tạo đối tượng Employee từ DataTable
                            var ep = new Employee();
                            // Gán giá trị từ DataTable vào các thuộc tính của Person
                            ep.PersonID = dt.Rows[i][0].ToString();
                            ep.EmployeeId = dt.Rows[i][1].ToString();
                            ep.Age = int.Parse(dt.Rows[i][2].ToString());
                            ep.FullName = dt.Rows[i][3].ToString();
                            ep.Address = dt.Rows[i][4].ToString();
                            ep.Phone = dt.Rows[i][5].ToString();

                            if (_context.Persons.Any(p => p.PersonID == ep.PersonID))
                            {
                                // Nếu PersonID đã tồn tại trong CSDL, tăng biến đếm trùng lặp
                                // và bỏ qua bản ghi này
                                ModelState.AddModelError("", "PersonID " + ep.PersonID + " already exists.");
                                duplicateCount++;
                                continue; // bỏ qua dòng này
                            }

                            // Thêm đối tượng Person vào CSDL
                            _context.Add(ep);
                            // Tăng biến đếm thành công
                            successCount++;
                        }
                        // Lưu thay đổi vào CSDL
                        await _context.SaveChangesAsync();
                        TempData["UploadResult"] = $"✅ {successCount} bản ghi được thêm. ⚠️ {duplicateCount} bản ghi bị trùng PersonID và đã bị bỏ qua.";
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
                worksheet.Cells["B1"].Value = "EmployeeId";
                worksheet.Cells["C1"].Value = "Age";
                worksheet.Cells["D1"].Value = "FullName";
                worksheet.Cells["E1"].Value = "Address";
                worksheet.Cells["F1"].Value = "Phone";
                // Lấy danh sách Employee từ CSDL và nạp vào worksheet
                // Sử dụng _context để truy vấn dữ liệu từ bảng Persons
                var employeeList = _context.Employees.ToList();
                // Sử dụng LoadFromCollection để nạp dữ liệu từ danh sách Person vào worksheet
                // Dữ liệu sẽ được nạp từ hàng thứ 2 (A2) trở đi
                worksheet.Cells["A2"].LoadFromCollection(employeeList);
                // Định dạng các cột trong worksheet
                // Đặt độ rộng cho các cột để hiển thị đầy đủ dữ liệu
                var stream = new MemoryStream(excelPackage.GetAsByteArray());
                // Đặt tên file và trả về file Excel cho người dùng tải xuống
                // Sử dụng File để trả về file Excel với kiểu MIME là application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                // MemoryStream được sử dụng để lưu trữ dữ liệu của file Excel trong bộ nhớ
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
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
