using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using WarehouseManagement.API;
using WarehouseManagement.Application.Interfaces;
using WarehouseManagement.Application.Services;
using Microsoft.EntityFrameworkCore.Storage;
using WarehouseManagement.Application.Comom;
using WarehouseManagement.API.Service;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace InterationTest
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {

        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<WarehouseDbContext>>();
                services.AddDbContext<WarehouseDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                }).AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("Test", options => { });

                services.AddScoped<IUserService, UserService>();

                services.AddHttpContextAccessor();
                services.AddScoped<ICurrentUserService, CurrentUserService>();
            });
        }
    }
}
