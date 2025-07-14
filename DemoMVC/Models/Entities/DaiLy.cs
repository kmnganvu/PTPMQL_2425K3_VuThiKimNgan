using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models.Entities.DaiLy
{
    [Table("DaiLies")]
    public class DaiLy
    {
        [Key]
        public string MaDaiLy { get; set; } = default!;
        [Required(ErrorMessage = "Tên đại lý là bắt buộc.")]
        public string TenDaiLy { get; set; }= default!;
        [Required(ErrorMessage = "Địa chỉ là bắt buộc.")]
        public string DiaChi { get; set; }  = default!;
        public string NguoiDaiDien { get; set; } = default!;
        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        public string DienThoai { get; set; } = default!;

        // Liên kết với hệ thống phân phối
        [ForeignKey("HeThongPhanPhoi")]
        public string MaHTPP { get; set; } = default!;
        public HeThongPhanPhoi? HeThongPhanPhoi { get; set; } 
    }
}
