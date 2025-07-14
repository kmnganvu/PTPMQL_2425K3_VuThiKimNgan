using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models.Entities;

[Table("Persons")]
public class Person
{
    [Key]
    public string PersonID { get; set; } = default!;
    [Required(ErrorMessage = "Full Name is required.")]
    public string FullName { get; set; } = default!;
    public string Address { get; set; }
    public string Phone { get; set; }

    }
//50. Validate : đảm bảo tính bảo mật của hệ thống
//await: bất đồng bộ
//redirecttoaction : điều hướng ng dùng về trang index