using System;
using System.Security.Cryptography;
using System.Text;
using System.Globalization;

namespace ElectronicsStore
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public string ToCsv() => $"{Id};{Email};{PasswordHash};{Role}";

        public static User FromCsv(string line)
        {
            var p = line.Split(';');
            return new User { Id = int.Parse(p[0]), Email = p[1], PasswordHash = p[2], Role = p[3] };
        }
    }

    public static class SecurityHelper
    {
        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes) builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }

        public string ToCsv() => $"{Id};{Name};{Category};{Price.ToString(CultureInfo.InvariantCulture)}";

        public static Product FromCsv(string line)
        {
            var p = line.Split(';');
            return new Product { Id = int.Parse(p[0]), Name = p[1], Category = p[2], Price = double.Parse(p[3], CultureInfo.InvariantCulture) };
        }
    }

    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
        public DateTime WarrantyEnd { get; set; }

        public string ToCsv() => $"{Id};{ProductId};{Quantity};{UnitPrice.ToString(CultureInfo.InvariantCulture)};{WarrantyEnd:yyyy-MM-dd}";

        public static Sale FromCsv(string line)
        {
            var p = line.Split(';');
            return new Sale
            {
                Id = int.Parse(p[0]),
                ProductId = int.Parse(p[1]),
                Quantity = int.Parse(p[2]),
                UnitPrice = double.Parse(p[3], CultureInfo.InvariantCulture),
                WarrantyEnd = DateTime.ParseExact(p[4], "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };
        }
    }

    public class Supply
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SupplyDate { get; set; }

        public string ToCsv() => $"{Id};{ProductId};{Quantity};{SupplyDate:yyyy-MM-dd}";

        public static Supply FromCsv(string line)
        {
            var p = line.Split(';');
            return new Supply
            {
                Id = int.Parse(p[0]),
                ProductId = int.Parse(p[1]),
                Quantity = int.Parse(p[2]),
                SupplyDate = DateTime.ParseExact(p[3], "yyyy-MM-dd", CultureInfo.InvariantCulture)
            };
        }
    }
}