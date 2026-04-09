using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WarehouseManagement.API.MiddleWares;
using WarehouseManagement.API.Service;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using WarehouseManagement.Application.Validators;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Domain.Enums;
using WarehouseManagement.Domain.Interfaces;
using WarehouseManagement.Infrastructure.Data;
using WarehouseManagement.Infrastructure.Data.Logging;
using WarehouseManagement.Infrastructure.Repositories;
using WarehouseManagement.Infrastructure.Security;
using Serilog;
using WarehouseManagement.Infrastructure.Logging;
using AutoMapper;
using WarehouseManagement.Application.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace WarehouseManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureSerilog();
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("ConnectionString is not configured in appsettings.json!");
            }

            if (builder.Environment.IsEnvironment("Testing"))
            {
                builder.Services.AddDbContext<WarehouseDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                builder.Services.AddDbContext<WarehouseDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }

            builder.Services.AddEndpointsApiExplorer();

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddAuthentication(options =>
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
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                });

            // Add DI
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IJwtService, JwtService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();

            // Add AutoMapper
            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddMaps(typeof(ProductProfile).Assembly);
            });

            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IStockService, StockService>();
            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped<IWarehouseTransactionService, WarehouseTransactionService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            builder.Services.AddFluentValidationAutoValidation();

            builder.Services.AddScoped<IAuditLogger, EfAuditLogger>();

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Warehouse API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token with format: bearer {token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            var app = builder.Build();


            if (!app.Environment.IsEnvironment("Testing"))
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

                if (!db.Users!.Any(u => u.Email == "admin@system.com"))
                {
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");

                    var admin = new User(
                        email: "admin@system.com",
                        passwordHash: passwordHash
                    );
                    admin.AssignRole(Roles.UserRoles.Admin.ToString());

                    db.Users!.Add(admin);
                    db.SaveChanges();
                }
            }    

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warehouse API v1");
                    c.RoutePrefix = string.Empty;
                });
            }
            Console.WriteLine("JWT KEY = " + builder.Configuration["Jwt:Key"]);


            // Configure the HTTP request pipeline.
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
