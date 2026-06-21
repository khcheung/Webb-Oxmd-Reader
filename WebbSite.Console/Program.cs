using System;
using Microsoft.EntityFrameworkCore;
using WebbSite.Common.Models.DbModels;
using WebbSite.Common.Logic;    
using Con = System.Console;
using Microsoft.Extensions.DependencyInjection;

namespace WebbSite.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddDbContext<StockContext>(options =>
        {
            options.UseSqlite("Data Source=Stock.db", o =>
            {
                o.MigrationsAssembly("WebbSite.Common");
            });
        });

        services.AddScoped<DbService>();
        services.AddScoped<ConsoleService>();
        services.AddScoped<WebbSiteClient>();

        var serviceProvider = services.BuildServiceProvider();
        

        // var db = new StockContext();
        // await db.Database.MigrateAsync();

        // Con.WriteLine("Checking if StockCode 5 exists...");

        // var existCount = await (from s in db.Stock
        //                         where s.StockCode == 5
        //                         select s).CountAsync();

        // if (existCount > 0)
        // {
        //     Con.WriteLine("StockCode 5 already exists.");
        // }
        // else
        // {
        //     db.Stock.Add(new Stock() { StockCode = 5 });
        //     await db.SaveChangesAsync();
        // }

        // WebbSiteClient client = new WebbSiteClient();
        // var records = await client.GetCCASSAsync(5);

        // DbService dbService = new DbService(db);
        // await dbService.SaveCCASSRecordsAsync(5, records);

        // Con.WriteLine("Done.");

        using (var scope = serviceProvider.CreateScope())
        {
            var sp = scope.ServiceProvider;
            var consoleService = sp.GetRequiredService<ConsoleService>();
            await consoleService.StartAsync();
        }
    }
}