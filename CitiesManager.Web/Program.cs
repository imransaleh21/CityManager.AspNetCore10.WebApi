using Asp.Versioning;
using CitiesManager.Web.DataBaseContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddApiVersioning(config =>
{
    config.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Db Context is added as a service
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
    );

builder.Services.AddEndpointsApiExplorer(); // Generate OpenAPI/Swagger documents for API endpoints
builder.Services.AddSwaggerGen(options =>
options.IncludeXmlComments(Path.Combine
(AppContext.BaseDirectory, "ApiDoc.xml"))); // Add Swagger generator services

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseHttpsRedirection();

app.UseSwagger(); // Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwaggerUI(); // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.

app.UseAuthorization();

app.MapControllers();

app.Run();
