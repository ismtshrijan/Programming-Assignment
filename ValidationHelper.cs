using System;

namespace TurboMartPOS
{
    public static class ValidationHelper
    {
        public static bool ValidateProductName(string name, out string error)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                error = "Please enter a product name.";
                return false;
            }
            error = null;
            return true;
        }

        public static bool ValidatePrice(string priceText, out decimal price, out string error)
        {
            if (!decimal.TryParse(priceText, out price) || price <= 0)
            {
                error = "Please enter a valid price greater than zero.";
                return false;
            }
            error = null;
            return true;
        }

        public static bool ValidateQuantity(string quantityText, out int quantity, out string error)
        {
            if (!int.TryParse(quantityText, out quantity) || quantity <= 0)
            {
                error = "Please enter a valid quantity greater than zero.";
                return false;
            }
            error = null;
            return true;
        }

        public static bool ValidateCash(string cashText, out decimal cash, out string error)
        {
            if (!decimal.TryParse(cashText, out cash) || cash <= 0)
            {
                error = "Please enter a valid cash amount.";
                return false;
            }
            error = null;
            return true;
        }
    }
} 