using Asp.Versioning;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Infrastructure.AuthenticationService;
using CitiesManager.Infrastructure.DataBaseContext;
using CitiesManager.Infrastructure.Identity.IdentityEntities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers( options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));
    options.Filters.Add(new ConsumesAttribute("application/json"));

    var policy = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Add the JWT service to the dependency injection container, allowing it to be injected into controllers or other services that require JWT functionality.
builder.Services.AddTransient<IJwtService, JwtService>();

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

// ASP.NET Core Identity is configured with custom options for password requirements
// and is set up to use Entity Framework Core for storing user and role information in the database.
// The UserStore and RoleStore are also specified to use the custom ApplicationUser and ApplicationRole classes, along with the ApplicationDbContext for database operations.
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 3;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>()
    ;

//
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer( options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:ISSUER"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:AUDIENCE"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SECRET_KEY"]!))
        };

    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwagger(); // Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Cities Manager API v1");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "Cities Manager API v2");
}); // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
