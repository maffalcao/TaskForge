using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;

namespace Api.Extensions;

public static class AddMigrationsExtensions
{
    public static void ApplyMigration(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            if (db != null)
            {
                if (db.Database.GetPendingMigrations().Any())
                {
                    db.Database.Migrate();
                }
            }
        }
    }

}
