using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models.Entities;

[Table("Persons")]
public class Person
{
    [Key]
    [Display(Name = "Mã người dùng")]
    public string PersonID { get; set; } = default!;
    [Required(ErrorMessage = "Full Name is required.")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; } = default!;
    [Display(Name = "Địa chỉ")]
    [Required(ErrorMessage = "Address is required.")]
    public string Address { get; set; }
    [Display(Name = "Số điện thoại")]
    [Required(ErrorMessage = "Phone is required.")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Phone number must be 10 or 11 digits.")]
    [StringLength(11, ErrorMessage = "Phone number cannot exceed 11 digits.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string Phone { get; set; }

    }
//50. Validate : đảm bảo tính bảo mật của hệ thống
//await: bất đồng bộ
//redirecttoaction : điều hướng ng dùng về trang index