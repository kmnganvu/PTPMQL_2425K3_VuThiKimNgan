using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models;
// Lớp Employee kế thừa từ lớp Person
// Lớp này đại diện cho một nhân viên trong hệ thống
// Các thuộc tính của lớp Employee:
// - EmployeeId: Mã định danh của nhân viên (string)
// - Age: Tuổi của nhân viên (int)
[Table("Employee")]
// Chú ý: Lớp Employee sẽ được ánh xạ với bảng Employee trong cơ sở dữ liệu thông qua Entity Framework Core.

public class Employee : Person
{

    public string EmployeeId { get; set; }
    public int Age { get; set; }
}
