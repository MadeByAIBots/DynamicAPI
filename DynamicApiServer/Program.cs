
Console.WriteLine("[INFO] Starting application...");

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddApiServices();

Console.WriteLine("[INFO] EndpointExecutor would be initialized here.");

var app = builder.BuildConfiguredApplication();

Console.WriteLine("[INFO] Web application created and configured.");

// Use the request logging middleware
app.UseRequestLogging();

app.UseCustomStaticFiles();

app.UseTokenValidation();


// Use the dynamic endpoint middleware
app.UseDynamicEndpoints();

// Map the fallback route
app.MapFallbackRoute();

Console.WriteLine("[INFO] Endpoints mapped.");

Console.WriteLine("[INFO] Application is now listening for requests...");

app.Run();
