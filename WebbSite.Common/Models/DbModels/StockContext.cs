using Microsoft.EntityFrameworkCore;

namespace WebbSite.Common.Models.DbModels;

public class StockContext : DbContext
{
    public DbSet<Stock> Stock { get; set; }
    public DbSet<StockCCASS> StockCCASS { get; set; }
    public StockContext(DbContextOptions<StockContext> options) : base(options)
    {

    }

    public StockContext() : base()
    {

    }

    override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=Stock.db", o =>
        {
            o.MigrationsAssembly("WebbSite.Common");                    
        });
        
        
    }
}