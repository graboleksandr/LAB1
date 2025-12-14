using ElectronicsStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

public class Program
{
    const string P_FILE = "products.csv", P_HEADER = "Id;Name;Category;Price";
    const string U_FILE = "users.csv", U_HEADER = "Id;Email;PasswordHash;Role";
    const string S_FILE = "sales.csv", S_HEADER = "Id;ProductId;Quantity;UnitPrice;WarrantyEnd";
    const string SUP_FILE = "supplies.csv", SUP_HEADER = "Id;ProductId;Quantity;SupplyDate";

    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        CsvHandler.EnsureFileExists(P_FILE, P_HEADER);
        CsvHandler.EnsureFileExists(U_FILE, U_HEADER);
        CsvHandler.EnsureFileExists(S_FILE, S_HEADER);
        CsvHandler.EnsureFileExists(SUP_FILE, SUP_HEADER);

        if (LoadAllItems(U_FILE, User.FromCsv).Count == 0)
            CsvHandler.AppendLine(U_FILE, $"1;admin;{SecurityHelper.HashPassword("admin")};Admin");

        if (LoginSystem()) MainMenu();
    }

    public static List<T> LoadAllItems<T>(string filePath, Func<string, T> parser)
    {
        var list = new List<T>();
        foreach (var line in CsvHandler.ReadAllLines(filePath))
        {
            try { list.Add(parser(line)); } catch { }
        }
        return list;
    }

    public static bool LoginSystem()
    {
        for (int i = 0; i < 4; i++)
        {
            Console.Clear();
            Console.WriteLine("LOGIN");
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Pass: ");
            string password = Console.ReadLine();

            string hash = SecurityHelper.HashPassword(password);

            var users = LoadAllItems(U_FILE, User.FromCsv);
            User currentUser = null;

            foreach (var user in users)
            {
                if (user.Email == email && user.PasswordHash == hash)
                {
                    currentUser = user;
                    break;
                }
            }

            if (currentUser != null) return true;
            Console.WriteLine($"Невірно! Спроб: {3 - i}");
            Pause();
        }
        return false;
    }

    public static int GetInventory(int productId)
    {
        int supplied = 0;
        foreach (var supply in LoadAllItems(SUP_FILE, Supply.FromCsv))
        {
            if (supply.ProductId == productId) supplied += supply.Quantity;
        }

        int sold = 0;
        foreach (var sale in LoadAllItems(S_FILE, Sale.FromCsv))
        {
            if (sale.ProductId == productId) sold += sale.Quantity;
        }

        return supplied - sold;
    }

    public static void MainMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("------ ЕЛЕКТРО МАГАЗИН-------");
            Console.WriteLine("1. Каталог");
            Console.WriteLine("2. Продаж");
            Console.WriteLine("3. Постачання");
            Console.WriteLine("4. Гарантія");
            Console.WriteLine("5. Аналітика");
            Console.WriteLine("0. Вихід");
            Console.Write(">> ");

            switch (Console.ReadLine())
            {
                case "1": ProductMenu(); break;
                case "2": SaleItem(); break;
                case "3": SupplyItem(); break;
                case "4": CheckWarranty(); break;
                case "5": ShowAnalytics(); break;
                case "0": return;
            }
        }
    }

    public static void SaleItem()
    {
        int cont;
        do
        {
            Console.Clear();
            ShowCatalogWithInventory();

            int id = (int)ReadNumber("ID товару: ");
            var products = LoadAllItems(P_FILE, Product.FromCsv);
            Product product = null;

            foreach (var p in products) { if (p.Id == id) { product = p; break; } }

            if (product == null) { Console.WriteLine("Не знайдено."); cont = ReadContinuation(); continue; }

            int available = GetInventory(id);
            if (available <= 0) { Console.WriteLine("Немає в наявності."); cont = ReadContinuation(); continue; }

            int quantity = (int)ReadNumber($"Кількість (Max {available}): ");

            if (quantity <= 0 || quantity > available) { Console.WriteLine("Помилка кількості."); cont = ReadContinuation(); continue; }

            var sale = new Sale
            {
                Id = CsvHandler.GenerateId(S_FILE),
                ProductId = id,
                Quantity = quantity,
                UnitPrice = product.Price,
                WarrantyEnd = DateTime.Today.AddYears(1)
            };

            CsvHandler.AppendLine(S_FILE, sale.ToCsv());
            Console.WriteLine($"Продано. Гарантія до: {sale.WarrantyEnd:yyyy-MM-dd}");

            cont = ReadContinuation();
        } while (cont == 1);
    }

    public static void SupplyItem()
    {
        int cont;
        do
        {
            Console.Clear();
            ViewProducts(true);

            int id = (int)ReadNumber("ID товару, що надійшов: ");
            var products = LoadAllItems(P_FILE, Product.FromCsv);
            Product product = null;
            foreach (var p in products) { if (p.Id == id) { product = p; break; } }

            if (product == null) { Console.WriteLine("Товар не в каталозі."); cont = ReadContinuation(); continue; }

            int quantity = (int)ReadNumber("Кількість: ");

            if (quantity <= 0) { Console.WriteLine("Кількість > 0."); cont = ReadContinuation(); continue; }

            var supply = new Supply
            {
                Id = CsvHandler.GenerateId(SUP_FILE),
                ProductId = id,
                Quantity = quantity,
                SupplyDate = DateTime.Now
            };

            CsvHandler.AppendLine(SUP_FILE, supply.ToCsv());
            Console.WriteLine("Постачання враховано.");

            cont = ReadContinuation();
        } while (cont == 1);
    }

    public static void CheckWarranty()
    {
        int cont;
        do
        {
            Console.Clear();
            var sales = LoadAllItems(S_FILE, Sale.FromCsv);
            if (sales.Count == 0) { Console.WriteLine("Немає продажів."); cont = ReadContinuation(); continue; }

            int saleId = (int)ReadNumber("ID продажу для перевірки: ");

            Sale sale = null;
            foreach (var s in sales) { if (s.Id == saleId) { sale = s; break; } }

            if (sale == null) { Console.WriteLine("Продаж не знайдено."); cont = ReadContinuation(); continue; }

            string status = (sale.WarrantyEnd > DateTime.Today) ? "АКТИВНА" : "ЗАКІНЧИЛАСЯ";
            Console.WriteLine($"Гарантія до: {sale.WarrantyEnd:yyyy-MM-dd}. Статус: {status}");

            cont = ReadContinuation();
        } while (cont == 1);
    }

    public static void ShowCatalogWithInventory()
    {
        var products = LoadAllItems(P_FILE, Product.FromCsv);
        Console.WriteLine($"{"ID",-5} | {"Назва",-20} | {"Ціна",-10} | {"Залишок",-10}");
        Console.WriteLine(new string('-', 50));

        foreach (var p in products)
        {
            Console.WriteLine($"{p.Id,-5} | {p.Name,-20} | {p.Price,-10:N2} | {GetInventory(p.Id),-10}");
        }
    }

    public static void ProductMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("------ КАТАЛОГ ТОВАРІВ -------");
            Console.WriteLine("1. Додати");
            Console.WriteLine("2. Склад");
            Console.WriteLine("3. Виправлення ціни");
            Console.WriteLine("4. Видалення");
            Console.WriteLine("5. Сортуванння");
            Console.WriteLine("0. Вихід");
            Console.Write(">> ");

            switch (Console.ReadLine())
            {
                case "1": AddProduct(); break;
                case "2": ShowCatalogWithInventory(); Pause(); break;
                case "3": EditProduct(); break;
                case "4": DeleteProduct(); break;
                case "5": SortProducts(); break;
                case "0": return;
            }
        }
    }

    public static void AddProduct()
    {
        int cont;
        do
        {
            Console.Clear();
            Console.Write("Назва: ");
            string name = Console.ReadLine();
            Console.Write("Категорія: ");
            string cat = Console.ReadLine();
            double price = ReadNumber("Ціна: ");

            var p = new Product { Id = CsvHandler.GenerateId(P_FILE), Name = name, Category = cat, Price = price };

            CsvHandler.AppendLine(P_FILE, p.ToCsv());
            Console.WriteLine("Додано.");

            cont = ReadContinuation("Бажаєте додати ще (1-Так, 0-Ні)?");
        } while (cont == 1);
    }

    public static void ViewProducts(bool internalCall = false)
    {
        if (!internalCall) Console.Clear();
        var products = LoadAllItems(P_FILE, Product.FromCsv);
        Console.WriteLine($"{"ID",-5} | {"Назва",-20} | {"Ціна",-10}");
        Console.WriteLine(new string('-', 40));

        foreach (var p in products)
            Console.WriteLine($"{p.Id,-5} | {p.Name,-20} | {p.Price,-10:N2}");

        if (!internalCall) Pause();
    }

    public static void EditProduct()
    {
        int cont;
        do
        {
            ShowCatalogWithInventory();
            int id = (int)ReadNumber("ID для зміни ціни: ");
            double price = ReadNumber("Нова ціна: ");

            var products = LoadAllItems(P_FILE, Product.FromCsv);
            Product target = null;
            foreach (var p in products) { if (p.Id == id) { target = p; break; } }

            if (target != null)
            {
                target.Price = price;
                SaveAllProducts(products);
                Console.WriteLine("Оновлено!");
            }
            else Console.WriteLine("ID не знайдено.");

            cont = ReadContinuation("Бажаєте редагувати ще (1-Так, 0-Ні)?");
        } while (cont == 1);
    }

    public static void DeleteProduct()
    {
        int cont;
        do
        {
            ShowCatalogWithInventory();
            int id = (int)ReadNumber("ID для видалення: ");

            var products = LoadAllItems(P_FILE, Product.FromCsv);
            var filtered = new List<Product>();
            bool deleted = false;

            foreach (var p in products)
            {
                if (p.Id != id)
                {
                    filtered.Add(p);
                }
                else
                {
                    deleted = true;
                }
            }

            if (deleted)
            {
                SaveAllProducts(filtered);
                Console.WriteLine("Видалено!");
            }
            else Console.WriteLine("ID не знайдено.");

            cont = ReadContinuation("Бажаєте видалити ще (1-Так, 0-Ні)?");
        } while (cont == 1);
    }

    private static void SaveAllProducts(List<Product> products)
    {
        CsvHandler.RewriteFile(P_FILE, P_HEADER, products.Select(p => p.ToCsv()).ToList());
    }

    public static void SortProducts()
    {
        int cont;
        do
        {
            Console.Clear();
            var products = LoadAllItems(P_FILE, Product.FromCsv);
            Console.WriteLine("1. Ціна (Зростання)\n2. Ціна (Спадання)");

            string choice = Console.ReadLine();

            if (choice == "1") products = products.OrderBy(p => p.Price).ToList();
            else if (choice == "2") products = products.OrderByDescending(p => p.Price).ToList();

            foreach (var p in products) Console.WriteLine($"{p.Id} - {p.Name} - {p.Price:N2}");

            cont = ReadContinuation("Бажаєте сортувати ще (1-Так, 0-Ні)?");
        } while (cont == 1);
    }

    public static void ShowAnalytics()
    {
        int cont;
        do
        {
            Console.Clear();

            var products = LoadAllItems(P_FILE, Product.FromCsv);

            Console.WriteLine("--- АНАЛІТИКА ЦІН ТОВАРІВ ---");
            Console.WriteLine(new string('-', 40));

            if (products.Count > 0)
            {
                var mostExpensive = products.OrderByDescending(p => p.Price).First();

                var leastExpensive = products.OrderBy(p => p.Price).First();

                double averagePrice = products.Average(p => p.Price);

                Console.WriteLine($"  Кількість товарів у каталозі: {products.Count}");
                Console.WriteLine($"  Найдорожчий товар: {mostExpensive.Name} ({mostExpensive.Price:N2} грн)");
                Console.WriteLine($"  Найдешевший товар: {leastExpensive.Name} ({leastExpensive.Price:N2} грн)");
                Console.WriteLine($"  Середня ціна всіх товарів: {averagePrice:N2} грн");
            }
            else
            {
                Console.WriteLine("  Каталог порожній. Немає даних для аналізу цін.");
            }

            Console.WriteLine(new string('-', 40));

            cont = ReadContinuation("Бажаєте переглянути аналітику ще раз (1-Так, 0-Ні)? ");
        } while (cont == 1);
    }

    public static double ReadNumber(string prompt)
    {
        Console.Write(prompt);
        double res;
        while (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out res) || res < 0)
        {
            Console.Write("Помилка! Введіть число >= 0: ");
        }
        return res;
    }

    public static int ReadContinuation(string prompt = "\nБажаєте продовжити (1-Так, 0-Ні)? ")
    {
        int res;
        Console.Write(prompt);
        string input = Console.ReadLine();

        while (!int.TryParse(input, out res) || (res != 0 && res != 1))
        {
            Console.Write("Помилка! Введіть 1 або 0: ");
            input = Console.ReadLine();
        }
        return res;
    }

    public static void Pause()
    {
        Console.Write("\nНатисніть клавішу...");
        Console.ReadKey(true);
    }
}