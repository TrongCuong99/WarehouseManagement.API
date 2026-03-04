using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities
{
    public class Warehouse : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Stock> Stocks { get; set; } = [];
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        private Warehouse() { }

        public Warehouse(string name, string location, int capacity, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name is required.");
            if (userId == Guid.Empty)
                throw new DomainException("Warehouse must belong to a valid user.");
            SetLocation(location);
            SetCapacity(capacity);
            Name = name;
            UserId = userId;
        }

        public void SetLocation(string? location)
        {
            if (string.IsNullOrWhiteSpace(location))
                throw new DomainException("Location is required.");
            Location = location;
        }

        public void SetCapacity(int capacity)
        {
            if (capacity <= 0)
                throw new DomainException("Capacity must be greater than zero.");
            Capacity = capacity;
        }

        public void AddStock(Stock stock)
        {
            if (stock == null)
                throw new DomainException("Stock cannot be null.");

            var totalQuantity = Stocks.Sum(s => s.QuantityOnHand) + stock.QuantityOnHand;

            if (totalQuantity > Capacity)
                throw new DomainException("Cannot exceed warehouse capacity.");

            Stocks.Add(stock);
        }
    }
}
