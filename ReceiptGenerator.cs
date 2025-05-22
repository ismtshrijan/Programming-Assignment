using System;
using System.Collections.Generic;
using System.Text;

namespace TurboMartPOS
{
    public static class ReceiptGenerator
    {
        public static string GenerateReceipt(IEnumerable<CartItem> items, decimal taxRate, decimal cashGiven)
        {
            var sb = new StringBuilder();
            sb.AppendLine("TURBO MART");
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Date: {DateTime.Now}");
            sb.AppendLine();

            decimal subtotal = 0;
            foreach (var item in items)
            {
                sb.AppendLine($"{item.Product.Name}");
                sb.AppendLine($"  {item.Quantity} x {item.Product.Price:C2} = {item.GetSubtotal():C2}");
                subtotal += item.GetSubtotal();
            }

            decimal tax = subtotal * taxRate;
            decimal total = subtotal + tax;
            decimal change = cashGiven - total;

            sb.AppendLine();
            sb.AppendLine("----------------------------------------");
            sb.AppendLine($"Subtotal: {subtotal:C2}");
            sb.AppendLine($"Tax (13%): {tax:C2}");
            sb.AppendLine($"Total: {total:C2}");
            sb.AppendLine($"Cash Received: {cashGiven:C2}");
            sb.AppendLine($"Change: {change:C2}");
            sb.AppendLine();
            sb.AppendLine("Thank you for shopping at Turbo Mart!");
            sb.AppendLine("Please come again!");

            return sb.ToString();
        }
    }
} 