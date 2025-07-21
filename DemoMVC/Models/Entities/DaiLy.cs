using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models.Entities.DaiLy
{
    [Table("DaiLies")]
    public class DaiLy
    {
        [Key]
        [Display(Name = "Mã đại lý")]
        public string MaDaiLy { get; set; } = default!;
        [Display(Name = "Tên đại lý")]
        [Required(ErrorMessage = "Tên đại lý là bắt buộc.")]
        public string TenDaiLy { get; set; }= default!;
        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        [Display(Name = "Địa chỉ")]
        public string DiaChi { get; set; } = default!;
        [Display(Name = "Người đại diện")]
        public string NguoiDaiDien { get; set; } = default!;
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [Display(Name = "Điện thoại")]
        // Sử dụng DataType và RegularExpression để xác thực định dạng số điện thoại
        // Chỉ cho phép số điện thoại có 10 hoặc 11 chữ số
        // Sử dụng StringLength để giới hạn độ dài của số điện thoại
        // Sử dụng Phone để xác thực định dạng số điện thoại
        // Lưu ý: Số điện thoại có thể bắt đầu bằng số 0, vì vậy không cần phải loại trừ số 0 ở đầu
        // Sử dụng DataType để xác định kiểu dữ liệu của thuộc tính
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Số điện thoại phải có 10 hoặc 11 chữ số.")]
        [StringLength(11, ErrorMessage = "Số điện thoại không được vượt quá 11 chữ số.")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string DienThoai { get; set; } = default!;

        // Liên kết với hệ thống phân phối
        [ForeignKey("HeThongPhanPhoi")]
        [Display(Name = "Mã hệ thống phân phối")]
        public string MaHTPP { get; set; } = default!;

        public HeThongPhanPhoi? HeThongPhanPhoi { get; set; } 
    }
}
