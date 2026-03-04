using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using WarehouseManagement.Domain.Entities;
using WarehouseManagement.Infrastructure.Data;

namespace InterationTest
{
    public abstract class IntegrationTestBase
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory Factory;

        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();

            InitializeDatabase().Wait();
        }

        protected async Task ResetDatabaseAsync()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }

        protected void Authenticate(string role)
        {
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");
            Client.DefaultRequestHeaders.Remove("X-Test-Role");
            Client.DefaultRequestHeaders.Add("X-Test-Role", role);
        }

        protected void AuthenticateAsUser(Guid userId)
        {
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Test");

            Client.DefaultRequestHeaders.Remove("X-Test-UserId");
            Client.DefaultRequestHeaders.Add("X-Test-UserId", userId.ToString());
        }

        protected async Task<Guid> CreateUserAndAssignRoleAsync(string email, string role)
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

            var user = new User(
                email,
                BCrypt.Net.BCrypt.HashPassword("Test@123")
            );

            user.AssignRole(role);

            db.Users?.Add(user);
            await db.SaveChangesAsync();

            return user.Id;
        }

        private async Task InitializeDatabase()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();

        }
    }
}
