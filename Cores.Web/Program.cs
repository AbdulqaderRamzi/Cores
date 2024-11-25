using Cores.DataService.Data;
using Cores.DataService.DbInitializer;
using Cores.DataService.Repository;
using Cores.DataService.Repository.IRepository;
using Cores.Utilities;
using Cores.Web.BackgroundJobs;
using Cores.Web.Hubs;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Connect TO MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var hangConnectionString = builder.Configuration.GetConnectionString("HangfireConnection");

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    
builder.Services.AddDbContext<HangfireDbContext>(
    options => options.UseMySql(hangConnectionString, ServerVersion.AutoDetect(hangConnectionString)));

builder.Services.AddIdentity<IdentityUser, IdentityRole>().
    AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

// overwrite the default identity path
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    /*options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;*/
});

// Add Session 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();
builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddTransient<IMailer, Mailer>();
builder.Services.AddTransient<IEmailSender, EmailSenderAdapter>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSignalR();

// Add Hangfire services and MySQL storage
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseStorage(
        new MySqlStorage(hangConnectionString,
        new MySqlStorageOptions())
    ));

builder.Services.AddHangfireServer();

QuestPDF.Settings.License = LicenseType.Community;

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

SeedDatabase(); // seed initial data

app.MapRazorPages();

app.UseSession();

app.MapControllerRoute(
    "default",
    "{area=Employee}/{controller=Home}/{action=Welcome}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.UseHangfireDashboard();
app.MapHangfireDashboard("/hangfire");


// Schedule the job to run daily at a specific time
RecurringJob.AddOrUpdate<AttendanceBackgroundJob>(
    "check-missing-attendance",
    job => job.CheckMissingAttendance(),
    Cron.Daily(14, 0)); // Runs every day at 2:00 PM
    
RecurringJob.AddOrUpdate<LeaveBalanceBackgroundJob>(
    "update-leave-balances",
    job => job.UpdateLeaveBalances(),
    Cron.Minutely);
    
RecurringJob.AddOrUpdate<LeaveBalanceBackgroundJob>(
    "deduct-leave-balances",
    job => job.DeductLeaveBalances(),
    Cron.Minutely());


/* For testing  */
//RecurringJob.AddOrUpdate(() => Console.WriteLine("Hello, world!"), "* * * * *");

app.Run();

return;

void SeedDatabase()
{
    using var scope = app.Services.CreateScope();
    var dbInitializer = scope?.ServiceProvider.GetRequiredService<IDbInitializer>();
    dbInitializer?.Initialize().GetAwaiter().GetResult();
    
    // Apply Hangfire migrations
    var hangfireContext = scope?.ServiceProvider.GetRequiredService<HangfireDbContext>();
    hangfireContext?.Database.Migrate();
}