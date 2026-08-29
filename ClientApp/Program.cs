using System.Text;
using Application.Models.Options;
using ClientApp.Extensions;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        string policyName = "ClientApp";

        builder.Host.UseSerilog((configure, context) =>
        {
            context.WriteTo.File(
                path: "Logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
            );
            context.WriteTo.Console(Serilog.Events.LogEventLevel.Information);
        });

        builder.Services.AddControllersWithViews();
        builder.Services.AddHealthChecks();
        builder.AddNpgsqlDbContext<FlexiRoomsContext>("hotelis");

        builder.Logging.AddConsole();
#if WINDOWS
        builder.Logging.AddEventLog();
#endif
        builder.Logging.AddJsonConsole();

        builder.Services.AddCors(cors =>
        {
            cors.AddPolicy(policyName, policy =>
            {
                policy.WithOrigins("https://localhost:44432", "http://localhost:44432")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        JwtOptions jwtOptions = builder.Configuration.GetSection(JwtOptions.JWTOPTIONS).Get<JwtOptions>()
            ?? throw new InvalidOperationException("JwtOptions configuration is required.");

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<FlexiRoomsContext>()
        .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Authority,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key!)),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("SuperAdmin"));
            options.AddPolicy("OwnerAccess", policy => policy.RequireRole("SuperAdmin", "Owner"));
            options.AddPolicy("HotelManagement", policy => policy.RequireRole("SuperAdmin", "Owner", "Admin"));
            options.AddPolicy("ReservationUser", policy => policy.RequireRole("SuperAdmin", "Owner", "Admin", "User"));
        });

        builder.Services.AddSwaggerGen();
        builder.Services.AddMemoryCache();
        builder.AddInfraStructure();
        builder.AddApplication();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FlexiRoomsContext>();
            context.Database.Migrate();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "SuperAdmin", "Owner", "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors(policyName);
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        app.MapHealthChecks("/health");
        app.MapControllers();
        app.MapFallbackToFile("index.html");
        app.Run();
    }
}
