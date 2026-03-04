using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = [];
        public ICollection<WarehouseTransactionDetail> TransactionDetails { get; set; } = [];
        public ICollection<Stock> Stocks { get; set; } = [];
        public ICollection<Category> Categories { get; set; } = [];

        private Product() { }

        public Product(string name, string description, string sku, decimal price)
        {
            SetName(name);
            SetDescription(description);
            SetSKU(sku);
            SetPrice(price);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Product name cannot be empty.");
            Name = name.Trim();
        }
        public void SetDescription(string description)
        {
            if (description.Length > 1000)
                throw new DomainException("Description is too long.");
            Description = description.Trim();
        }
        public void SetSKU(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new DomainException("SKU cannot be empty.");
            SKU = sku.Trim().ToUpper();
        }
        public void SetPrice(decimal price)
        {
            if (price < 0)
                throw new DomainException("Price cannot be negative.");
            Price = price;
        }
    }
}
