using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models.Entities;
using DemoMVC.Models.Process;
using OfficeOpenXml;

namespace DemoMVC.Controllers
{
    public class HeThongPhanPhoiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ExcelProcess _excelProcess = new ExcelProcess(); // Khởi tạo đối tượng ExcelProcess để xử lý file Excel

        public HeThongPhanPhoiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HeThongPhanPhoi
        public async Task<IActionResult> Index()
        {
            return View(await _context.HeThongPhanPhois.ToListAsync());
        }
        // GET: HeThongPhanPhoi/ImportExcel
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
                            // Tạo đối tượng HTPP từ DataTable
                            var ht = new HeThongPhanPhoi();
                            // Gán giá trị từ DataTable vào các thuộc tính của Person
                            ht.MaHTPP = dt.Rows[i][0].ToString();
                            ht.TenHTPP = dt.Rows[i][1].ToString();
                            
                        
                            _context.Add(ht); // Thêm đối tượng HTPP vào DbContext
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
                worksheet.Cells["A1"].Value = "MaHTPP";
                worksheet.Cells["B1"].Value = "TenHTPP";

                // Lấy danh sách Person từ CSDL và nạp vào worksheet
                // Sử dụng _context để truy vấn dữ liệu từ bảng Persons
                var htppList = _context.HeThongPhanPhois.ToList();
                // Sử dụng LoadFromCollection để nạp dữ liệu từ danh sách Person vào worksheet
                // Dữ liệu sẽ được nạp từ hàng thứ 2 (A2) trở đi
                worksheet.Cells["A2"].LoadFromCollection(htppList);
                // Định dạng các cột trong worksheet
                // Đặt độ rộng cho các cột để hiển thị đầy đủ dữ liệu
                var stream = new MemoryStream(excelPackage.GetAsByteArray());
                // Đặt tên file và trả về file Excel cho người dùng tải xuống
                // Sử dụng File để trả về file Excel với kiểu MIME là application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                // MemoryStream được sử dụng để lưu trữ dữ liệu của file Excel trong bộ nhớ
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // GET: HeThongPhanPhoi/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heThongPhanPhoi = await _context.HeThongPhanPhois
                .FirstOrDefaultAsync(m => m.MaHTPP == id);
            if (heThongPhanPhoi == null)
            {
                return NotFound();
            }

            return View(heThongPhanPhoi);
        }

        // GET: HeThongPhanPhoi/Create
        public IActionResult Create()
        {
            var last = _context.HeThongPhanPhois.OrderByDescending(x => x.MaHTPP).FirstOrDefault();
            var lastId = last == null ? "HT000" : last.MaHTPP;
            var newId = new AutoGenerateCode().GenerateId(lastId);

            var heThongPhanPhoi = new HeThongPhanPhoi
            {
                MaHTPP = newId
            };

            return View(heThongPhanPhoi);
        }

        // POST: HeThongPhanPhoi/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHTPP,TenHTPP")] HeThongPhanPhoi heThongPhanPhoi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(heThongPhanPhoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(heThongPhanPhoi);
        }

        // GET: HeThongPhanPhoi/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
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

        // POST: HeThongPhanPhoi/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaHTPP,TenHTPP")] HeThongPhanPhoi heThongPhanPhoi)
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

        // GET: HeThongPhanPhoi/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var heThongPhanPhoi = await _context.HeThongPhanPhois
                .FirstOrDefaultAsync(m => m.MaHTPP == id);
            if (heThongPhanPhoi == null)
            {
                return NotFound();
            }

            return View(heThongPhanPhoi);
        }

        // POST: HeThongPhanPhoi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var heThongPhanPhoi = await _context.HeThongPhanPhois.FindAsync(id);
            if (heThongPhanPhoi != null)
            {
                _context.HeThongPhanPhois.Remove(heThongPhanPhoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HeThongPhanPhoiExists(string id)
        {
            return _context.HeThongPhanPhois.Any(e => e.MaHTPP == id);
        }
    }
}
