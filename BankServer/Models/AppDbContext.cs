using BankServer.Models.DbEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankServer.Models;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<Account>? Accounts { get; set; }
    public DbSet<Currency>? Currencies { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Currency>().HasIndex(x => new { x.Name });
        base.OnModelCreating(modelBuilder);
    }
}