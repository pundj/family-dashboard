using FamilyDashboard.Api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FamilyDashboard.Api.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<SmartThingsCredential> SmartThingsCredentials => Set<SmartThingsCredential>();
    public DbSet<CalendarPreferencesEntity> CalendarPreferences => Set<CalendarPreferencesEntity>();

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

        builder.Entity<CalendarPreferencesEntity>(entity =>
        {
            entity.ToTable("CalendarPreferences");
            entity.HasKey(x => x.PreferenceKey);
            entity.Property(x => x.PreferenceKey).HasMaxLength(64);
            entity.Property(x => x.PreferencesJson).IsRequired();
            entity.Property(x => x.UpdatedUtc).IsRequired();
        });
    }
}
