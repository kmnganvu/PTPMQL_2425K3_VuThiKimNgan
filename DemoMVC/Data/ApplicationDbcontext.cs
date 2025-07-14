using Microsoft.EntityFrameworkCore;
using DemoMVC.Models.Entities;
using DemoMVC.Models.Entities.DaiLy;

namespace DemoMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        internal object DaiLy;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        // khai báo việc ánh xạ class Person vào trong database)
        public DbSet<Person> Persons { get; set; }

        // khai báo việc ánh xạ class Employee vào trong database)
        public DbSet<Employee> Employees { get; set; }

        // khai báo việc ánh xạ class HeThongPhanPhoi vào trong database)
        public DbSet<HeThongPhanPhoi> HeThongPhanPhois { get; set; }
        // khai báo việc ánh xạ class DaiLy vào trong database)
        public DbSet<DaiLy> DaiLies { get; set; }
        
    }
}