using Roomy.Data;
using Roomy.Search.API.Handlers;

var builder = WebApplication.CreateBuilder(args);


// Add services
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


// Register EF Core and repositories
builder.Services.AddRoomyDataServices(builder.Configuration);
builder.Services.AddRoomySearchServices();

builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails();

// Add logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    // Initialize database and populate seed data
    await app.Services.SeedAsync();

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Roomy API v1");
        options.RoutePrefix = string.Empty;
    });    
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();   // Converts exceptions to Problem Details
app.UseStatusCodePages();    // Converts bare 4xx/5xx to Problem Details

app.MapControllers();

app.Run();
