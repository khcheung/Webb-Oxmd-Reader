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
        
        using (var dbScope = serviceProvider.CreateScope())
        {
            var sp = dbScope.ServiceProvider;
            var db = sp.GetRequiredService<StockContext>();
            await db.Database.MigrateAsync();
        }

        using (var scope = serviceProvider.CreateScope())
        {
            var sp = scope.ServiceProvider;
            var consoleService = sp.GetRequiredService<ConsoleService>();
            await consoleService.StartAsync();
        }
    }
}