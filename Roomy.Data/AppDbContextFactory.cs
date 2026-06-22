using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Roomy.Data;

/// <summary>
/// Design-time factory for creating AppDbContext instances
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private readonly IConfiguration configuration;

    public AppDbContextFactory()
    {
    }

    public AppDbContextFactory(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    /// <summary>
    /// Creates a new instance of AppDbContext for design-time operations
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>A new AppDbContext instance</returns>
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);
        
        return new AppDbContext(optionsBuilder.Options);
    }
}
