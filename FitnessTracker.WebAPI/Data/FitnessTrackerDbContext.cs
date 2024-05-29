using FitnessTracker.WebAPI.Models.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.WebAPI.Data;

public class FitnessTrackerDbContext : IdentityDbContext<User>
{
    public FitnessTrackerDbContext(DbContextOptions<FitnessTrackerDbContext> options) : base(options) {}
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Set> Sets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SeedRoles(modelBuilder);

        ConfigureRelationships(modelBuilder);
    }

    private static void ConfigureRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Workout>()
            .HasMany(w => w.Exercises)
            .WithOne(e => e.Workout)
            .HasForeignKey(e => e.WorkoutId);

        modelBuilder
            .Entity<Exercise>()
            .HasMany(e => e.Sets)
            .WithOne(s => s.Exercise)
            .HasForeignKey(s => s.ExerciseId);

        modelBuilder
            .Entity<Workout>()
            .HasOne(w => w.User)
            .WithMany(u => u.Workouts)
            .HasForeignKey(w => w.UserId);
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        const string AdminRoleId = "661420f9-2a30-40cf-afa0-edc498392318";
        const string AdminName = "admin";

        const string UserRoleId = "743f9ebe-0fd5-4382-bf56-2baa0c34e80a";
        const string UserName = "user";

        var roles = new List<IdentityRole>
        {
            new IdentityRole
            {
                Id = AdminRoleId,
                Name = AdminName,
                NormalizedName = AdminName.ToUpper(),
                ConcurrencyStamp = AdminRoleId
            },
            new IdentityRole
            {
                Id = UserRoleId,
                Name = UserName,
                NormalizedName = UserName.ToUpper(),
                ConcurrencyStamp = UserRoleId
            }
        };

        modelBuilder.Entity<IdentityRole>().HasData(roles);
    }
}
