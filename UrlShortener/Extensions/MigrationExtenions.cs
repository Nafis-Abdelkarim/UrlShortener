using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Extensions;

public static class MigrationExtenions 
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}
