using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models;

[Table("Persons")]
public class Person
{
    [Key]
    public string PersonID { get; set; }
    public string FullName { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }

    }
//50. Validate : đảm bảo tính bảo mật của hệ thống
//await: bất đồng bộ
//redirecttoaction : điều hướng ng dùng về trang index