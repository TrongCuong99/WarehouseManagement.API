using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Product> Products { get; set; } = [];

        private Category() { }
        public Category(string name, string? description)
        {
            SetName(name);
            Description = description;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Category name cannot be empty.");
            if (name.Length > 100)
                throw new DomainException("Category name cannot exceed 100 characters.");

            Name = name.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? description)
        {
            SetName(name);
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
