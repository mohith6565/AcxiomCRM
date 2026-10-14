using AcxiomCRM.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Lead> Leads => Set<Lead>();
    public DbSet<Opportunity> Opportunities => Set<Opportunity>();
    public DbSet<FollowUp> FollowUps => Set<FollowUp>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Customer>().HasIndex(x => x.CustomerCode).IsUnique();
        builder.Entity<Customer>().HasIndex(x => x.Email).IsUnique();
        builder.Entity<Customer>().HasIndex(x => x.Phone).IsUnique();
        builder.Entity<Lead>().HasIndex(x => x.Email);
        builder.Entity<Opportunity>().HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Entity<FollowUp>().HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.Entity<AuditLog>().HasIndex(x => x.CreatedAtUtc);
    }
}
