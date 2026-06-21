using Microsoft.EntityFrameworkCore;
using Roomy.Data;

var builder = WebApplication.CreateBuilder(args);


// Add services
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


// Register EF Core and repositories
builder.Services.AddRoomyDataServices(builder.Configuration);
builder.Services.AddRoomySearchServices();


// Add logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Initialize database and populate seed data
using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using var dbContext = dbContextFactory.CreateDbContext();
    
    // Create database and apply migrations
    await dbContext.Database.MigrateAsync();
    
    // Populate seed data
    await DataSeeder.SeedAsync(dbContext);
}

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Roomy API v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
