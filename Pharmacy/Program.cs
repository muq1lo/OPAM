using System;
using System.Text;

namespace Pharmacy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            bool running = true;

            while (running)
            {
                Console.WriteLine("=== Аптека (Система обліку препаратів) ===");
                Console.WriteLine("1 — довідка про систему та сутність");
                Console.WriteLine("2 — розрахунок вартості замовлення (табулювання)");
                Console.WriteLine("3 — звіт продажів по відділах (таблиця на два входи)");
                Console.WriteLine("0 — вихід");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ShowAbout();
                        Pause();
                        break;
                    case "2":
                        ShowCostTable();
                        Pause();
                        break;
                    case "3":
                        ShowSalesTable();
                        Pause();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Роботу завершено. До побачення!");
                        break;
                    default:
                        Console.WriteLine("Помилка: немає такого пункту меню. Спробуйте ще раз.");
                        Pause();
                        break;
                }
            }
        }

        // Пункт 1: Довідка про тему, сутність та поля
        static void ShowAbout()
        {
            Console.WriteLine("=== Довідка про застосунок ===");
            Console.WriteLine("Тема: Аптека");
            Console.WriteLine("Головна сутність: Лікарський препарат");
            Console.WriteLine("Підсумкова величина: Загальна вартість запасів (грн)");
            Console.WriteLine("\nПерелік полів запису:");
            Console.WriteLine("1. Номер запису        — int");
            Console.WriteLine("2. Назва препарату     — string");
            Console.WriteLine("3. Категорія           — string");
            Console.WriteLine("4. Виробник            — string");
            Console.WriteLine("5. Ціна за упаковку    — double");
            Console.WriteLine("6. Кількість на складі — int");
            Console.WriteLine("7. Потрібен рецепт     — bool");
            Console.WriteLine("8. Термін придатності  — DateTime");
        }

        // Пункт 2: Табулювання функції (не менше 8 рядків, дробовий крок, 2 стовпці значень)
        static void ShowCostTable()
        {
            Console.WriteLine("=== Таблиця вартості закупівлі препарату (зі знижкою 5% від 3 кг) ===");
            Console.WriteLine("Базова ціна за 1 кг сировини: 240,00 грн.\n");

            double basePrice = 240.0;
            double startWeight = 0.5;
            double endWeight = 5.0;
            double step = 0.5;

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"| {"Вага (кг)",-10} | {"Без знижки",11} | {"Зі знижкою",11} |");
            Console.WriteLine("-----------------------------------------");

            for (double w = startWeight; w <= endWeight + 1e-9; w += step)
            {
                double fullPrice = w * basePrice;
                double finalPrice = (w >= 3.0) ? fullPrice * 0.95 : fullPrice;

                Console.WriteLine($"| {w,-10:F1} | {fullPrice,11:F2} | {finalPrice,11:F2} |");
            }

            Console.WriteLine("-----------------------------------------");
        }

        // Пункт 3: Таблиця на два входи вкладеними циклами (Рівень А + Рівень Б)
        static void ShowSalesTable()
        {
            Console.WriteLine("=== Звіт відпущених упаковок (Відділи × Дні тижня) ===");

            Console.Write("Введіть кількість відділів (рядків, наприклад 4): ");
            if (!int.TryParse(Console.ReadLine(), out int rows) || rows <= 0)
            {
                Console.WriteLine("Помилка: кількість рядків має бути додатним цілим числом.");
                return;
            }

            Console.Write("Введіть кількість днів (стовпців, наприклад 5): ");
            if (!int.TryParse(Console.ReadLine(), out int cols) || cols <= 0)
            {
                Console.WriteLine("Помилка: кількість стовпців має бути додатним цілим числом.");
                return;
            }

            Console.WriteLine();

            int[] colTotals = new int[cols];
            int grandTotal = 0;

            Console.Write($"{"Відділ \\ День",-15}");
            for (int c = 1; c <= cols; c++)
            {
                Console.Write($"{"День " + c,9}");
            }
            Console.WriteLine($"{"Разом",11}");

            int lineWidth = 15 + cols * 9 + 11;
            Console.WriteLine(new string('-', lineWidth));

            for (int r = 1; r <= rows; r++)
            {
                Console.Write($"{"Відділ №" + r,-15}");
                int rowSum = 0;

                for (int c = 1; c <= cols; c++)
                {
                    int soldUnits = (r * 12) + (c * 5);

                    rowSum += soldUnits;
                    colTotals[c - 1] += soldUnits;

                    Console.Write($"{soldUnits,9}");
                }

                grandTotal += rowSum;
                Console.WriteLine($"{rowSum,11}");
            }

            Console.WriteLine(new string('-', lineWidth));
            Console.Write($"{"Разом за день",-15}");
            for (int c = 0; c < cols; c++)
            {
                Console.Write($"{colTotals[c],9}");
            }
            Console.WriteLine($"{grandTotal,11}");
        }

        // Пауза та очищення екрана після виконання дії
        static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу для повернення в меню...");
            if (!Console.IsInputRedirected)
            {
                Console.ReadKey(true);
                Console.Clear();
            }
            else
            {
                Console.ReadLine();
            }
        }
    }
}
