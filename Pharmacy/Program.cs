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
                ShowMenu();
                string choice = Console.ReadLine() ?? "";
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowAbout(); Pause(); break;
                    case "2": HandleCostTabulation(); Pause(); break;
                    case "3": HandleSalesReport(); Pause(); break;
                    case "0": running = false; Console.WriteLine("Роботу завершено. До побачення!"); break;
                    default: Console.WriteLine("Помилка: немає такого пункту меню. Спробуйте ще раз."); Pause(); break;
                }
            }
        }

        // Головне меню застосунку
        static void ShowMenu()
        {
            PrintHeader("Аптека (Система обліку препаратів)");
            Console.WriteLine("1 — довідка про систему та сутність");
            Console.WriteLine("2 — розрахунок вартості замовлення (табулювання)");
            Console.WriteLine("3 — звіт продажів по відділах (таблиця на два входи)");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
        }

        // Універсальний вивід заголовка розділу
        static void PrintHeader(string title)
        {
            Console.WriteLine($"=== {title} ===");
            Console.WriteLine();
        }

        // Пункт 1: Довідка про тему, сутність та поля
        static void ShowAbout()
        {
            PrintHeader("Довідка про застосунок");
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

        // Пункт 2: Збір параметрів через ReadDouble та запуск табулювання
        static void HandleCostTabulation()
        {
            PrintHeader("Налаштування табулювання вартості закупівлі");

            double startWeight = ReadDouble("Введіть початкову вагу в кг (0,1 – 50): ", 0.1, 50.0);
            double endWeight = ReadDouble($"Введіть кінцеву вагу в кг ({startWeight:F1} – 100): ", startWeight, 100.0);
            double step = ReadDouble("Введіть крок ваги в кг (0,1 – 20): ", 0.1, 20.0);

            Console.WriteLine();
            PrintCostTable(startWeight, endWeight, step);
        }

        // Табулювання функції з трьома параметрами (початок, кінець, крок)
        static void PrintCostTable(double startWeight, double endWeight, double step)
        {
            const double basePrice = 240.0;

            PrintHeader("Таблиця вартості закупівлі (знижка 5% від 3 кг)");
            Console.WriteLine($"Базова ціна за 1 кг сировини: {basePrice:F2} грн.\n");

            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"| {"Вага (кг)",-10} | {"Без знижки",11} | {"Зі знижкою",11} |");
            Console.WriteLine("-----------------------------------------");

            for (double w = startWeight; w <= endWeight + 1e-9; w += step)
            {
                double fullPrice = CalculateOrderCost(w, basePrice, false);
                double discountedPrice = CalculateOrderCost(w, basePrice, true);

                Console.WriteLine($"| {w,-10:F1} | {fullPrice,11:F2} | {discountedPrice,11:F2} |");
            }

            Console.WriteLine("-----------------------------------------");
        }

        // Чистий розрахунковий метод (Рівень А, п. 3): обчислює вартість замовлення і повертає результат
        static double CalculateOrderCost(double weightKg, double pricePerKg, bool applyDiscount)
        {
            double total = weightKg * pricePerKg;
            if (applyDiscount && weightKg >= 3.0)
            {
                return total * 0.95;
            }
            return total;
        }

        // Пункт 3: Збір розмірів через ReadInt та вивід двовимірної таблиці
        static void HandleSalesReport()
        {
            PrintHeader("Звіт відпущених упаковок (Відділи × Дні тижня)");

            int rows = ReadInt("Введіть кількість відділів (1 – 20): ", 1, 20);
            int cols = ReadInt("Введіть кількість днів (1 – 14): ", 1, 14);

            Console.WriteLine();
            PrintSalesTable(rows, cols);
        }

        // Вивід таблиці з двома входами та підсумками по рядках і стовпцях
        static void PrintSalesTable(int rows, int cols)
        {
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
                    int soldUnits = CalculateDailyUnits(r, c);
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

        // Чистий розрахунковий метод для комірки звіту
        static int CalculateDailyUnits(int departmentIndex, int dayIndex)
        {
            return (departmentIndex * 12) + (dayIndex * 5);
        }

        // Рівень Б: Безпечне читання дробового числа в межах [min; max]
        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? "";

                if (!double.TryParse(input, out double value))
                {
                    Console.WriteLine("Помилка: введено не число. Спробуйте ще раз.");
                    continue;
                }

                if (value < min || value > max)
                {
                    Console.WriteLine($"Помилка: число має бути в межах від {min} до {max}.");
                    continue;
                }

                return value;
            }
        }

        // Рівень Б: Безпечне читання цілого числа в межах [min; max]
       

        // Пауза та очищення екрана
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
