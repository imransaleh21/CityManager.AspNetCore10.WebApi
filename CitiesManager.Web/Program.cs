using Asp.Versioning;
using CitiesManager.Infrastructure.DataBaseContext;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddApiVersioning(config =>
{
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
})
// Enable API versioning in the project. Also, add the API explorer to help with versioning in Swagger
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Format code to represent the versioning format as "v{Major}.{Minor}.{Patch}"
    options.SubstituteApiVersionInUrl = true; // Substitute the version number in the URL
});

// Db Context is added as a service
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    );

builder.Services.AddEndpointsApiExplorer(); // Generate OpenAPI/Swagger documents for API endpoints

builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "ApiDoc.xml"));

    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Cities Manager API",
        Version = "1.0",
    });

    options.SwaggerDoc("v2", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Cities Manager API",
        Version = "2.0",
    });
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
        .WithOrigins(builder.Configuration.GetSection("AllowedOrigin")
            .Get<string[]>()!)
            .WithHeaders("Authorization", "Origin", "Content-Type", "Accept")
            //.AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger(); // Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cities Manager API v1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Cities Manager API v2");
}); // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseRouting();
app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
