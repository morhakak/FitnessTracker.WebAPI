using FitnessTracker.WebAPI.Models.Domain;
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

        modelBuilder.Entity<Workout>()
            .HasMany(w => w.Exercises)
            .WithOne(e => e.Workout)
            .HasForeignKey(e => e.WorkoutId);

        modelBuilder.Entity<Exercise>()
            .HasMany(e => e.Sets)
            .WithOne(s => s.Exercise)
            .HasForeignKey(s => s.ExerciseId);

        modelBuilder.Entity<Workout>()
            .HasOne(w => w.User)
            .WithMany(u => u.Workouts)
            .HasForeignKey(w => w.UserId);
    }

}
