using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("==== Ласкаво просимо в магазин електротехніки ====");
            Console.ResetColor();
            Console.WriteLine();

            //Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Товари які є в наявності: ");
            Console.WriteLine("Телевізор - 10000 грн/шт");
            Console.WriteLine("Телефон - 20000 грн/шт");
            Console.WriteLine("Ноутбук - 30000 грн/шт");
            Console.WriteLine("PS 5 - 25000 грн/шт");
            Console.WriteLine("Планшет - 15000 грн/шт");
            Console.WriteLine("Геймпад - 3000 грн/шт");
            //Console.ResetColor();
            Console.ReadLine();
            Console.WriteLine();



            //Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("==== Введіть кількість товарів яких вам потрібно ====");
            Console.WriteLine();

            double TVPrice = 10000;
            double PhonePrice = 20000;
            double LaptopPrice = 30000;
            double PS5Price = 25000;
            double TabletPrice = 15000;
            double GamepadPrice = 3000;

            Console.Write("Введіть кількість телевізорів (шт): ");
            double amountTV = double.Parse(Console.ReadLine());

            Console.Write("Введіть кількість телефонів (шт): ");
            double amountPhone = double.Parse(Console.ReadLine());

            Console.Write("Введіть кількість ноутбуків (шт): ");
            double amountLaptop = double.Parse(Console.ReadLine());

            Console.Write("Введіть кількість PS 5 (шт): ");
            double amountPS5 = double.Parse(Console.ReadLine());

            Console.Write("Введіть кількість планшетів (шт): ");
            double amountTablet = double.Parse(Console.ReadLine());

            Console.Write("Введіть кількість геймпадів (шт): ");
            double amountGamepad = double.Parse(Console.ReadLine());
            //Console.ResetColor();
            Console.WriteLine("\n");


            //Console.ForegroundColor = ConsoleColor.Yellow;
            double totalTV = amountTV * TVPrice;
            double totalPhone = amountPhone * PhonePrice;
            double totalLaptop = amountLaptop * LaptopPrice;
            double totalPS5 = amountPS5 * PS5Price;
            double totalTablet = amountTablet * TabletPrice;
            double totalGamepad = amountGamepad * GamepadPrice;
            double totalPrice = totalTV + totalPhone + totalLaptop + totalPS5 + totalTablet + totalGamepad;

            Console.WriteLine("Загальна сума: " + totalPrice + " грн");

            double randomDiscount = new Random().NextDouble() * 50;
            randomDiscount = Math.Round(randomDiscount, 0);
            Console.WriteLine("Знижка: " + randomDiscount + "%");

            double discountAmount = totalPrice * (randomDiscount / 100);
            discountAmount = Math.Round(discountAmount, 2);
            Console.WriteLine("Сума зі знижкою: " + discountAmount + " грн");

            double SqrtrValue = Math.Sqrt(totalPrice);
            SqrtrValue = Math.Round(SqrtrValue, 2);
            Console.WriteLine("Квадратний корінь від загальної суми: " + SqrtrValue);
            //Console.ResetColor();
            Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("       Дякуємо за покупку!");
            Console.WriteLine();
            Console.WriteLine("Ви придбали: ");
            Console.WriteLine("Телевізор - " + amountTV + " шт");
            Console.WriteLine("Телефон - " + amountPhone + " шт");
            Console.WriteLine("Ноутбук - " + amountLaptop + " шт");
            Console.WriteLine("PS 5 - " + amountPS5 + " шт");
            Console.WriteLine("Планшет - " + amountTablet + " шт");
            Console.WriteLine("Геймпад - " + amountGamepad + " шт");
            Console.WriteLine("Гарного дня, до зустрічі!");
            Console.ResetColor();
            Console.ReadLine();

        }

    }
}
