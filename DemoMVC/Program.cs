using DemoMVC.Data;
using DemoMVC.Models.Process;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;


var builder = WebApplication.CreateBuilder(args);
// Đăng ký DbContext với dịch vụ DI (Dependency Injection) để sử dụng Entity Framework Core.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

builder.Services.AddDefaultIdentity<DemoMVC.Models.Entities.ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// Đăng ký AutoGenerateCode để sử dụng trong các controller.
builder.Services.AddTransient<AutoGenerateCode>();
// Add services to the container.
builder.Services.AddControllersWithViews();
// Đăng ký Identity để sử dụng xác thực người dùng.
builder.Services.Configure<IdentityOptions>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    //Config Login
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // User settings
    options.User.RequireUniqueEmail = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Chỉ sử dụng cookie qua HTTPS
    options.Cookie.SameSite = SameSiteMode.Lax; // Cài đặt SameSite giảm thiểu rủi ro CSRF
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Indentity/Account/Login";
    options.AccessDeniedPath = "/Indentity/Account/AccessDenied";
    options.SlidingExpiration = true;
});
// Cấu hình bảo vệ dữ liệu bằng cách sử dụng Data Protection API.
builder.Services.AddDataProtection()
// Chỉ định nơi lưu trữ khóa bảo vệ dữ liệu.
    .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))
    // Xác định tên ứng dụng để bảo vệ dữ liệu.
    .SetApplicationName("DemoMVC")
    // Chỉ định thời gian sống của khóa bảo vệ dữ liệu.
    .SetDefaultKeyLifetime(TimeSpan.FromDays(14));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication(); 

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();
app.MapRazorPages(); // Để Razor Pages (bao gồm Identity UI) hoạt động


app.Run();
