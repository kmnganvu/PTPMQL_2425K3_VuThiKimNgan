using Microsoft.EntityFrameworkCore;
using DemoMVC.Models;

namespace DemoMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }
        // khai báo việc ánh xạ class Person vào trong database)
        public DbSet<Person> Person { get; set; }
        
        // khai báo việc ánh xạ class Employee vào trong database)
        public DbSet<Employee> Employee { get; set; }
        
    }
}