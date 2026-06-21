using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Roomy.Data;

/// <summary>
/// Design-time factory for creating AppDbContext instances
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    /// <summary>
    /// Creates a new instance of AppDbContext for design-time operations
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>A new AppDbContext instance</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer("Server=(local);Database=Roomy;Integrated Security=true;TrustServerCertificate=true");
        
        return new AppDbContext(optionsBuilder.Options);
    }
}
