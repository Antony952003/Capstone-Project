using EmployeeGrievanceRedressal.Models;
using Microsoft.EntityFrameworkCore;

public class EmployeeGrievanceContext : DbContext
{
    public EmployeeGrievanceContext(DbContextOptions<EmployeeGrievanceContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Grievance> Grievances { get; set; }
    public DbSet<Solution> Solutions { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
    public DbSet<GrievanceHistory> GrievanceHistories { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Rating>()
            .HasKey(r => r.RatingId);

        modelBuilder.Entity<Rating>()
            .HasOne(r => r.Solver)
            .WithMany(u => u.Ratings)
            .HasForeignKey(r => r.SolverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship between User and Grievance as Employee
        modelBuilder.Entity<User>()
            .HasMany(u => u.RaisedGrievances)
            .WithOne(g => g.Employee)
            .HasForeignKey(g => g.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship between User and Feedback
        modelBuilder.Entity<User>()
            .HasMany(u => u.Feedbacks)
            .WithOne(f => f.Employee)
            .HasForeignKey(f => f.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship between User and Solution
        modelBuilder.Entity<User>()
            .HasMany(u => u.Solutions)
            .WithOne(s => s.Solver)
            .HasForeignKey(s => s.SolverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship between Grievance and Solution
        modelBuilder.Entity<Grievance>()
            .HasMany(g => g.Solutions)
            .WithOne(s => s.Grievance)
            .HasForeignKey(s => s.GrievanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship between Grievance and GrievanceHistory
        modelBuilder.Entity<Grievance>()
            .HasMany(g => g.GrievanceHistories)
            .WithOne(h => h.Grievance)
            .HasForeignKey(h => h.GrievanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship between Solution and Feedback (One-to-One)
        modelBuilder.Entity<Solution>()
            .HasOne(s => s.Feedback)
            .WithOne(f => f.Solution)
            .HasForeignKey<Feedback>(f => f.SolutionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship between ApprovalRequest and Employee
        modelBuilder.Entity<ApprovalRequest>()
            .HasOne(ar => ar.Employee)
            .WithMany()
            .HasForeignKey(ar => ar.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship between GrievanceHistory and Grievance
        modelBuilder.Entity<GrievanceHistory>()
            .HasOne(h => h.Grievance)
            .WithMany(g => g.GrievanceHistories)
            .HasForeignKey(h => h.GrievanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship between Grievance and DocumentUrl
        modelBuilder.Entity<Grievance>()
            .HasMany(g => g.DocumentUrls)
            .WithOne(d => d.Grievance)
            .HasForeignKey(d => d.GrievanceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the DocumentUrl entity
        modelBuilder.Entity<DocumentUrl>()
            .HasKey(d => d.DocumentUrlId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId);

    }
}
