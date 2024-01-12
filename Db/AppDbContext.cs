using Microsoft.EntityFrameworkCore;
using TicTacToe.Models;

namespace TicTacToe.Db;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }

    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>()
            .HasKey(x => x.Guid);
        
        modelBuilder.Entity<Account>()
            .HasKey(x => x.Id);
        
        modelBuilder.Entity<Account>()
            .HasOne(x => x.Player)
            .WithOne(x => x.Account)
            .HasForeignKey<Player>(x => x.AccountId)
            .IsRequired();
    }
}