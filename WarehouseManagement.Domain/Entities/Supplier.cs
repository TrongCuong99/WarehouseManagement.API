using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WarehouseManagement.Domain.Common;

namespace WarehouseManagement.Domain.Entities
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? ContactEmail { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ProductSupplier> ProductSuppliers { get; set; } = [];
        private Supplier() { }

        public Supplier(string name, string? contactEmail = null, string? phone = null, string? address = null)
        {
            SetName(name);
            SetContactEmail(contactEmail);
            SetPhoneNumber(phone);
            Address = address;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Supplier name is required.");
            Name = name.Trim();
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetContactEmail(string? email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(email, emailPattern))
                    throw new DomainException("Invalid email format.");
            }
            ContactEmail = email;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetPhoneNumber(string? phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                var phonePattern = @"^[0-9+\-\s]{6,20}$";
                if (!Regex.IsMatch(phone, phonePattern))
                    throw new DomainException("Invalid phone number format.");
            }
            PhoneNumber = phone;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
