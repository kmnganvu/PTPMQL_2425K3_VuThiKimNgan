using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DemoMVC.Models;
 
[Table("HeThongPhanPhois")]
public class HeThongPhanPhoi
{
    [Key]
    public string MaHTPP { get; set; }
    public string TenHTPP { get; set; }
}
