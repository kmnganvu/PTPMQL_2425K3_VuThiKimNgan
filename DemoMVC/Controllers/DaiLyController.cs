using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DemoMVC.Data;
using DemoMVC.Models.Entities.DaiLy;
using DemoMVC.Models.Process;
using OfficeOpenXml;

namespace DemoMVC.Controllers
{
    public class DaiLyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ExcelProcess _excelProcess = new ExcelProcess(); // Khởi tạo đối tượng ExcelProcess để xử lý file Excel

        public DaiLyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DaiLy
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DaiLies.Include(d => d.HeThongPhanPhoi);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: DaiLy/ImportExcel
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
            // Kiểm tra xem file có được chọn hay không
            // Nếu không có file được chọn, thêm lỗi vào ModelState để hiển thị thông
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
                            var dl = new DaiLy();
                            // Gán giá trị từ DataTable vào các thuộc tính của Person
                            dl.MaDaiLy = dt.Rows[i][0].ToString();
                            dl.TenDaiLy = dt.Rows[i][1].ToString();
                            dl.DiaChi = dt.Rows[i][2].ToString();
                            dl.NguoiDaiDien = dt.Rows[i][3].ToString();
                            dl.DienThoai = dt.Rows[i][4].ToString();
                            dl.MaHTPP = dt.Rows[i][5].ToString();
                            // Thêm đối tượng Person vào CSDL
                            _context.Add(dl);
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
                worksheet.Cells["A1"].Value = "MaDaiLy";
                worksheet.Cells["B1"].Value = "TenDaiLy";
                worksheet.Cells["C1"].Value = "DiaChi";
                worksheet.Cells["D1"].Value = "NguoiDaiDien";
                worksheet.Cells["E1"].Value = "DienThoai";
                worksheet.Cells["F1"].Value = "MaHTPP";
                // Lấy danh sách Daily từ CSDL và nạp vào worksheet
                // Sử dụng _context để truy vấn dữ liệu từ bảng Persons
                var dailyList = _context.DaiLies.ToList();
                // Sử dụng LoadFromCollection để nạp dữ liệu từ danh sách Person vào worksheet
                // Dữ liệu sẽ được nạp từ hàng thứ 2 (A2) trở đi
                worksheet.Cells["A2"].LoadFromCollection(dailyList);
                // Định dạng các cột trong worksheet
                // Đặt độ rộng cho các cột để hiển thị đầy đủ dữ liệu
                var stream = new MemoryStream(excelPackage.GetAsByteArray());
                // Đặt tên file và trả về file Excel cho người dùng tải xuống
                // Sử dụng File để trả về file Excel với kiểu MIME là application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
                // MemoryStream được sử dụng để lưu trữ dữ liệu của file Excel trong bộ nhớ
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        // GET: DaiLy/Details/
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daiLy = await _context.DaiLies
                .Include(d => d.HeThongPhanPhoi)
                .FirstOrDefaultAsync(m => m.MaDaiLy == id);
            if (daiLy == null)
            {
                return NotFound();
            }

            return View(daiLy);
        }

        // GET: DaiLy/Create
        public IActionResult Create()
        {
            var last = _context.DaiLies.OrderByDescending(x => x.MaDaiLy).FirstOrDefault();
            var lastId = last == null ? "DL000" : last.MaDaiLy;
            var newId = new AutoGenerateCode().GenerateId(lastId);

             ViewData["MaHTPP"] = new SelectList(_context.HeThongPhanPhois, "MaHTPP", "TenHTPP", null);

            var daily = new DaiLy
            {
                MaDaiLy = newId
            };

            return View(daily);
        }

        // POST: DaiLy/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDaiLy,TenDaiLy,DiaChi,NguoiDaiDien,DienThoai,MaHTPP")] DaiLy daiLy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(daiLy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHTPP"] = new SelectList(_context.HeThongPhanPhois, "MaHTPP", "TenHTPP", daiLy.MaHTPP);

            return View(daiLy);
        }

        // GET: DaiLy/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daiLy = await _context.DaiLies.FindAsync(id);
            if (daiLy == null)
            {
                return NotFound();
            }
            ViewData["MaHTPP"] = new SelectList(_context.HeThongPhanPhois, "MaHTPP", "MaHTPP", daiLy.MaHTPP);
            return View(daiLy);
        }

        // POST: DaiLy/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaDaiLy,TenDaiLy,DiaChi,NguoiDaiDien,DienThoai,MaHTPP")] DaiLy daiLy)
        {
            if (id != daiLy.MaDaiLy)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(daiLy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DaiLyExists(daiLy.MaDaiLy))
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
            ViewData["MaHTPP"] = new SelectList(_context.HeThongPhanPhois, "MaHTPP", "TenHTPP", daiLy.MaHTPP);

            return View(daiLy);
        }

        // GET: DaiLy/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var daiLy = await _context.DaiLies
                .Include(d => d.HeThongPhanPhoi)
                .FirstOrDefaultAsync(m => m.MaDaiLy == id);
            if (daiLy == null)
            {
                return NotFound();
            }

            return View(daiLy);
        }

        // POST: DaiLy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var daiLy = await _context.DaiLies.FindAsync(id);
            if (daiLy != null)
            {
                _context.DaiLies.Remove(daiLy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DaiLyExists(string id)
        {
            return _context.DaiLies.Any(e => e.MaDaiLy == id);
        }
    }
}
