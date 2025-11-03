using GetEmpStatus.BusinessLayer;
using GetEmpStatus.DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



// Register services with dependency injection
builder.Services.AddScoped<ProcessStatus>();
builder.Services.AddScoped<DataAccess>();

// CORS configuration for public API access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

var app = builder.Build();

// Test Supabase connection on startup (only in development)
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dataAccess = scope.ServiceProvider.GetRequiredService<DataAccess>();
        try
        {
            bool canConnect = dataAccess.CheckNationalNumberExists(123456789);
            Console.WriteLine(" Supabase PostgreSQL connection verified at startup");
            Console.WriteLine($"   Test employee (NAT: 123456789) found: {canConnect}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($" Supabase connection failed: {ex.Message}");
            Console.WriteLine("   Please check your connection string");
        }
    }
}


// Configure middleware pipeline
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Updated root endpoint with Railway deployment info
app.MapGet("/", () => new
{
    message = "GetEmpStatus Web API - Deployed on Railway with Supabase!",
    version = "1.0.0",
    framework = ".NET 9.0",
    database = "Supabase PostgreSQL",
    environment = Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") ?? "Local",
    endpoints = new[]
    {
        "GET /api/employee/status/{nationalNumber} - Get employee status",
        "GET /api/employee/health - Health check",
        "GET /api/employee/info - Service information",
    },
    testEmployees = new[]
    {
        new { nat = "123456789", name = "John Doe", expectedStatus = "GREEN" },
        new { nat = "987654321", name = "Jane Smith", expectedStatus = "RED" },
        new { nat = "555666777", name = "Ahmed Hassan", expectedStatus = "ORANGE" },
    },
    deployedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC"),
});

// Use the PORT environment variable from Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Clear();
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();