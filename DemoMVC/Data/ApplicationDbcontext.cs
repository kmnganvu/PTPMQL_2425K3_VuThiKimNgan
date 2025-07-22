using Microsoft.EntityFrameworkCore;
using DemoMVC.Models.Entities;
using DemoMVC.Models.Entities.DaiLy;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace DemoMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>

    {
        internal object DaiLy;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        // khai báo việc ánh xạ class Person vào trong database)
        public DbSet<Person> Persons { get; set; }

        // khai báo việc ánh xạ class Employee vào trong database)
        public DbSet<Employee> Employees { get; set; }
        // khai báo việc ánh xạ class MemberUnit vào trong database)
        public DbSet<MemberUnit> MemberUnits { get; set; }

        // khai báo việc ánh xạ class HeThongPhanPhoi vào trong database)
        public DbSet<HeThongPhanPhoi> HeThongPhanPhois { get; set; }
        // khai báo việc ánh xạ class DaiLy vào trong database)
        public DbSet<DaiLy> DaiLies { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Đặt tên bảng cho các thực thể
            builder.Entity<ApplicationUser>().ToTable("Users");
            // builder.Entity<IdentityRole>().ToTable("Roles");
            // builder.Entity<IdentityUserRole>().ToTable("UserRoles");
        }

    }
}