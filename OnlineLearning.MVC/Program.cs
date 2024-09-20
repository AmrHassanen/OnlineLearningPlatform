using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineLearning.Entity.Entities;
using OnlineLearning.Infrastructure.Data;
using OnlineLearning.Service.Interfaces;
using OnlineLearning.Infrastructure.Helpers;
using Library.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        ConfigureMiddleware(app);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register ApplicationDbContext with SQLite
        var connectionString = configuration.GetConnectionString("SQLiteConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString, b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Add Identity services
        services.AddIdentity<AppUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        // JWT Authentication setup
        var jwtSettings = configuration.GetSection("JWT");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero // Disable delay on token expiration
            };
        });

        // Add CORS support
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        // Add session support
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Register application services
        services.AddScoped<IAuthUser, AuthUser>();

        // Add controllers with views
        services.AddControllersWithViews();

        // Configure application settings (e.g., JWT)
        services.Configure<JWT>(configuration.GetSection("JWT"));
    }

    private static void ConfigureMiddleware(WebApplication app)
    {
        // Environment-based exception handling
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        // Enable session handling
        app.UseSession();

        // Enable CORS
        app.UseCors("AllowAll");

        // Enable authentication and authorization
        app.UseAuthentication();
        app.UseAuthorization();

        // Map default routes
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Account}/{action=Login}/{id?}");
    }
}
