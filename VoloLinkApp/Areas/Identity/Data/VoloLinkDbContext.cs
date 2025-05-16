using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VoloLinkApp.Areas.Identity.Data;

namespace VoloLinkApp.Areas.Identity.Data;

public class VoloLinkDbContext : IdentityDbContext<VoloLinkUser>
{
    public VoloLinkDbContext(DbContextOptions<VoloLinkDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

     
    }
}
