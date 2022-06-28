using Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Types;

namespace Persistence;

public class HaSpManContext : DbContext
{
    public HaSpManContext(DbContextOptions<HaSpManContext> options)
       : base(options)
    {
    }

    public DbSet<Member> Members { get; set; } = null!;
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("HaSpMan");
        builder.ApplyConfigurationsFromAssembly(
           typeof(Persistence.EntityConfigurations.MemberConfiguration).Assembly

        );
        base.OnModelCreating(builder);
    }
    
    //public class HaspmanContextFactory : IDesignTimeDbContextFactory<HaSpManContext>
    //{
    //    public HaSpManContext CreateDbContext(string[] args)
    //    {
    //        var context = new HaSpManContext(new DbContextOptionsBuilder<HaSpManContext>().UseSqlServer("Server=localhost,5201;Database=HaSpMan;User Id=sa;Password=Dev-db-Password;").Options);
    //        return context;
    //    }
    //}
}
