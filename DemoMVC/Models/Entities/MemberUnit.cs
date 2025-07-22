using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models.Entities
{
    public class MemberUnit
    {
    // Lớp MemberUnit đại diện cho một đơn vị thành viên trong hệ thống
    // Các thuộc tính của lớp MemberUnit:
    // - MemberUnitId: Mã định danh của đơn vị thành viên (int)
    // - Name: Tên của đơn vị thành viên (string)
    // - Address: Địa chỉ của đơn vị thành viên (string)
    // - PhoneNumber: Số điện thoại của đơn vị thành viên (string)
    // - WebsiteUrl: Địa chỉ website của đơn vị thành viên (string)
    [Key]
    public int MemberUnitId { get; set; }
    [Required]
    public string Name { get; set; } = default!;
    [Required]
    public string Address { get; set; } = default!;
    [Required]
    public string PhoneNumber { get; set; } = default!;
    [Required]
    [Url]
    public string WebsiteUrl { get; set; } = default!;
    }
}