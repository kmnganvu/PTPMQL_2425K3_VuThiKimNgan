using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models.Entities;
// Lớp Employee kế thừa từ lớp Person
// Lớp này đại diện cho một nhân viên trong hệ thống
// Các thuộc tính của lớp Employee:
// - EmployeeId: Mã định danh của nhân viên (string)
// - Age: Tuổi của nhân viên (int)
[Table("Employees")]
// // Chú ý: Lớp Employee sẽ được ánh xạ với bảng Employee trong cơ sở dữ liệu thông qua Entity Framework Core.

// public class Employee : Person
// {
//     [Display (Name = "Mã nhân viên")]
//     [Required(ErrorMessage = "EmployeeId is required.")]
//     public string EmployeeId { get; set; } = default!;
//     [Display(Name = "Tuổi")]
//     [Range(18, 50, ErrorMessage = "Age must be between 18 and 65.")]
//     public int Age { get; set; }
// }

public class Employee
{
    [Key]
    public int EmployeeId { get; set; }
    [Required]

    public string FirstName { get; set; } = default!;
    [Required]

    public string LastName { get; set; } = default!;
    [Required]

    public string Address { get; set; } = default!;
    [Required]
    [DataType(DataType.Date)]

    public DateTime DateOfBirth { get; set; }
    [Required]

    public string Position { get; set; } = default!;
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
    [DataType(DataType.Date)]
    public DateTime HireDate { get; set; }

}