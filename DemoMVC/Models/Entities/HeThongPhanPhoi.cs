using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DemoMVC.Models.Entities;

[Table("HeThongPhanPhois")]
public class HeThongPhanPhoi
{
    [Key]
    public string MaHTPP { get; set; } = default!;
    [Required(ErrorMessage = "Tên hệ thống phân phối là bắt buộc.")]
    public string TenHTPP { get; set; } = default!;
    
    //Liên kết với các đại lý
    public ICollection<DaiLy.DaiLy>? DaiLies { get; set; }
}
