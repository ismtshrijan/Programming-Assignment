using System.Collections.Generic;
using System.Linq;

namespace TurboMartPOS
{
    public class CartManager
    {
        private readonly List<CartItem> cart = new List<CartItem>();

        public IReadOnlyList<CartItem> Items => cart.AsReadOnly();

        public void AddItem(Product product, int quantity)
        {
            cart.Add(new CartItem(product, quantity));
        }

        public void Clear()
        {
            cart.Clear();
        }

        public decimal GetSubtotal() => cart.Sum(item => item.GetSubtotal());
        public decimal GetTax(decimal taxRate) => GetSubtotal() * taxRate;
        public decimal GetTotal(decimal taxRate) => GetSubtotal() + GetTax(taxRate);
    }
} 