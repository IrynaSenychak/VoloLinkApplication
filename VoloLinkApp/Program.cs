using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using VoloLinkApp.Areas.Identity.Data;
using VoloLinkApp.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("VoloLinkDbContextConnection") ?? throw new InvalidOperationException("Connection string 'VoloLinkDbContextConnection' not found.");

builder.Services.AddDbContext<VoloLinkDbContext>(options => options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<VoloLinkUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<VoloLinkDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
//////////////////////////////////////////////////////////////////////////////
//builder.Services.AddIdentity<VoloLinkUser, IdentityRole>()
//        .AddEntityFrameworkStores<VoloLinkDbContext>()
//        .AddDefaultTokenProviders();

builder.Services.AddIdentity<VoloLinkUser, IdentityRole>()
    .AddEntityFrameworkStores<VoloLinkDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAsync(services);
    await SeedUsersAsync(services);
}

async Task SeedRolesAsync(IServiceProvider serviceProvider)
{
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roleNames = { "Volunteer", "VerifiedVolunteer", "Administrator" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

async Task SeedUsersAsync(IServiceProvider serviceProvider)
{
    var userManager = serviceProvider.GetRequiredService<UserManager<VoloLinkUser>>();

    var users = new List<(VoloLinkUser user, string role)>
    {
        (new VoloLinkUser { UserName = "user4@example.com", Email = "user4@example.com", FirstName = "John", LastName = "Doe", EmailConfirmed = true }, "Volunteer"),
        (new VoloLinkUser { UserName = "user5@example.com", Email = "user5@example.com", FirstName = "Alice", LastName = "Smith", EmailConfirmed = true }, "VerifiedVolunteer"),
        (new VoloLinkUser { UserName = "admin@example.com", Email = "admin@example.com", FirstName = "Admin", LastName = "Superuser", EmailConfirmed = true }, "Administrator")
    };

    foreach (var (user, role) in users)
    {
        var existingUser = await userManager.FindByEmailAsync(user.Email);
        if (existingUser == null)
        {
            await userManager.CreateAsync(user, "Test@123");
            await userManager.AddToRoleAsync(user, role);
        }
    }
}


app.UseAuthentication();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();







app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
