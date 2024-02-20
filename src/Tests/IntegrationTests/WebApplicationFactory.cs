using Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace Tests.IntegrationTests;

internal class WebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {       
        

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationContext>));

            var connString = GetConnmectionString();
            services.AddSqlServer<ApplicationContext>(connString);

            var dbContext = CreateDbContext(services);
            dbContext.Database.Migrate();
            
        });
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            // Clean up the in-memory database
            using (var scope = Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
                dbContext.Database.EnsureDeleted();
            }
        }
    }

    private static string? GetConnmectionString()
    {
        //var configuration = new ConfigurationBuilder()
        //    .AddUserSecrets<WebApplicationFactory>()
        //    .Build();

        //var connString = configuration.GetConnectionString("TaskForgeDbTestConnectionString");
        //return connString;

        return "Server=localhost;Database=TaskForgeDb_Test;User Id=sa;Password=SwN12345678;TrustServerCertificate=True";
    }

    private static ApplicationContext CreateDbContext(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        return dbContext;
    }


}
