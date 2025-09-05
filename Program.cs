using EquipCheck.Models.DB;
using Microsoft.EntityFrameworkCore;
using EquipCheck.Services;

var builder = WebApplication.CreateBuilder(args);

// DB³s½u
var ConnectString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DBContext>(options => options.UseSqlServer(ConnectString));


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<LoginService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CommonService>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<EnumService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<ActionLogService>();
builder.Services.AddScoped<OptionManagementService>();
builder.Services.AddScoped<FormManagementService>();
builder.Services.AddScoped<PersonalFormService>();
builder.Services.AddScoped<AssetManagementService>();

// Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

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
app.UseSession();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
