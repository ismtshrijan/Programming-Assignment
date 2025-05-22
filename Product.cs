using System;

namespace TurboMartPOS
{
    /// <summary>
    /// Represents a product in the POS system
    /// </summary>
    public class Product
    {
        public string Name { get; private set; }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }

        public Product(string name, decimal price, int quantity)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Product name cannot be empty.", nameof(name));
            if (price <= 0)
                throw new ArgumentException("Price must be greater than zero.", nameof(price));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public decimal GetTotal()
        {
            return Price * Quantity;
        }
    }
} 