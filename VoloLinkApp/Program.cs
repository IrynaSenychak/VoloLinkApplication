using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using VoloLinkApp.Areas.Identity;
using VoloLinkApp.Areas.Identity.Data;
using VoloLinkApp.Models;
using VoloLinkApp.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("VoloLinkDbContextConnection") ?? throw new InvalidOperationException("Connection string 'VoloLinkDbContextConnection' not found.");

builder.Services.AddDbContext<VoloLinkDbContext>(options => options.UseSqlServer(connectionString));


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddAzureWebAppDiagnostics();
});


builder.Services.AddIdentity<VoloLinkUser, IdentityRole>()
    .AddEntityFrameworkStores<VoloLinkDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<UkrainianIdentityErrorDescriber>(); ;

builder.Services.AddTransient<IEmailSender, EmailSender>();


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
   
    app.UseHsts();
}


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRolesAsync(services);
    await SeedUsersAsync(services);
    await SeedEventsAsync(services);
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

async Task SeedEventsAsync(IServiceProvider serviceProvider)
{
    var context = serviceProvider.GetRequiredService<VoloLinkDbContext>();
    var userManager = serviceProvider.GetRequiredService<UserManager<VoloLinkUser>>();

    // Get the VerifiedVolunteer and Admin users for creating events
    var verifiedVolunteer = await userManager.FindByEmailAsync("user5@example.com");
    var admin = await userManager.FindByEmailAsync("admin@example.com");

    if (!context.Events.Any())
    {
        var events = new List<Event>
        {
             new Event
        {
            Title = "���������� �����",
            Description = "����������� �� ���������� �� ����������� ������ �������� �����. ³����� � ����� ��������� �� ����!",
            Date = DateTime.Now.AddDays(7),
            Location = "����������� ����, ���. ��������, 12, �. ���",
            CreatorId = verifiedVolunteer.Id,
            RequiresVerifiedVolunteer = false,
        },
        new Event
        {
            Title = "�������� ��������� � �����",
            Description = "�������� ��������� ���������. ������� �������� ��������� ����� �������� �������� ������.",
            Date = DateTime.Now.AddDays(14),
            Location = "̳���� ������ �3, ���. ������� �������������, 45, �. ����",
            CreatorId = admin.Id,
            RequiresVerifiedVolunteer = true,
        },
        new Event
        {
            Title = "������� �������� ��� ���� �����",
            Description = "��������� ����� �������� ��� ��������� ������������� ����������� �� ����'�������. ������ ����� ������ ������.",
            Date = DateTime.Now.AddDays(3),
            Location = "����� ��� ���� ����� �������� ��, ���. ������, 8, �. �����",
            CreatorId = verifiedVolunteer.Id,
            RequiresVerifiedVolunteer = false,
        },
        new Event
        {
            Title = "�������� � ������� �����",
            Description = "�������� �������� ��� ������ � ����� � �����. ������� �������� ���������.",
            Date = DateTime.Now.AddDays(10),
            Location = "������ ������ �1, ���. ���������, 21, �. �����",
            CreatorId = admin.Id,
            RequiresVerifiedVolunteer = true,
        }
        };

        await context.Events.AddRangeAsync(events);
        await context.SaveChangesAsync();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();





app.MapRazorPages();











app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
