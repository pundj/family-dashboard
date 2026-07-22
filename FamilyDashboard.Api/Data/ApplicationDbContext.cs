using FamilyDashboard.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FamilyDashboard.Api.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SmartThingsCredential> SmartThingsCredentials => Set<SmartThingsCredential>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SmartThingsCredential>(entity =>
        {
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.ProtectedToken).IsRequired();
            entity.Property(x => x.UpdatedUtc).IsRequired();

            entity.HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<SmartThingsCredential>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
