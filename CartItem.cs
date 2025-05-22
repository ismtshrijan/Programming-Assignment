using System;

namespace TurboMartPOS
{
    /// <summary>
    /// Represents an item in the shopping cart
    /// </summary>
    public class CartItem
    {
        public Product Product { get; private set; }
        public int Quantity { get; set; }

        public CartItem(Product product, int quantity)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

            Product = product;
            Quantity = quantity;
        }

        public decimal GetSubtotal()
        {
            return Product.Price * Quantity;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));
            
            Quantity = newQuantity;
        }
    }
} 