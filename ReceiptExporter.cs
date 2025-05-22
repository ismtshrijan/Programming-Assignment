using System.IO;
using System.Windows.Forms;

namespace TurboMartPOS
{
    public static class ReceiptExporter
    {
        public static void SaveReceiptAsTxt(string receiptText)
        {
            using (var sfd = new SaveFileDialog { Filter = "Text Files (*.txt)|*.txt", DefaultExt = "txt" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, receiptText);
                }
            }
        }

        public static void SaveReceiptAsCsv(System.Collections.Generic.IEnumerable<CartItem> items)
        {
            using (var sfd = new SaveFileDialog { Filter = "CSV Files (*.csv)|*.csv", DefaultExt = "csv" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var writer = new StreamWriter(sfd.FileName))
                    {
                        writer.WriteLine("Product Name,Unit Price,Quantity,Total");
                        foreach (var item in items)
                        {
                            writer.WriteLine($"{Escape(item.Product.Name)},{item.Product.Price},{item.Quantity},{item.GetSubtotal()}");
                        }
                    }
                }
            }
        }

        private static string Escape(string value)
        {
            if (value.Contains(",") || value.Contains("\""))
                return $"\"{value.Replace("\"", "\"\"")}";
            return value;
        }
    }
} 