using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using WarehouseManagement.Domain.Common;
using WarehouseManagement.Domain.Enums;

namespace WarehouseManagement.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Password { get; private set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Staff";
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Warehouse> Warehouses { get; set; } = [];
        public ICollection<WarehouseTransaction> CreatedTransactions { get; set; } = [];
        public ICollection<WarehouseTransaction> ApprovedTransactions { get; set; } = [];
        private User() { }

        public User(string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email is required.");

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password is required.");

            Email = email;
            Password = passwordHash;
            Role = Roles.UserRoles.Staff.ToString();
        }

        public void AssignRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new DomainException("Role cannot be empty");

            Role = role;
        }
        public void UpdatePassword(string newPasswordHash)
        {
            Password = newPasswordHash;
        }
    }
}
