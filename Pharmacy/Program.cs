using System;
using System.Text;

namespace Pharmacy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            const int Capacity = 100;

            // Дані живуть у Main і передаються в методи параметрами
            double[] prices = new double[Capacity];
            int count = 0;

            bool running = true;
            while (running)
            {
                ShowMenu();
                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        PrintPricesTable(prices, count);
                        Pause();
                        break;
                    case "2":
                        double newPrice = ReadDouble("Введіть ціну препарату (грн): ", 0.01, 100000);
                        AddValue(prices, ref count, newPrice, Capacity);
                        Pause();
                        break;
                    case "3":
                        int indexToRemove = ReadInt("Введіть порядковий номер для видалення: ", 1, int.MaxValue) - 1;
                        RemoveAt(prices, ref count, indexToRemove);
                        Pause();
                        break;
                    case "4":
                        ShowStatistics(prices, count);
                        Pause();
                        break;
                    case "5":
                        RunReferenceDemo();
                        Pause();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("До побачення.");
                        break;
                    default:
                        Console.WriteLine("Немає такого пункту.");
                        Pause();
                        break;
                }
            }
        }

        static void ShowMenu()
        {
            Console.WriteLine("=== Аптека (ЛР №9: Масив цін) ===");
            Console.WriteLine("1 — показати таблицю цін");
            Console.WriteLine("2 — додати ціну препарату");
            Console.WriteLine("3 — видалити ціну за номером");
            Console.WriteLine("4 — показати статистику (сума, середнє, максимум)");
            Console.WriteLine("5 — дослід: значимий тип проти посилального");
            Console.WriteLine("0 — вихід");
            Console.Write("Ваш вибір: ");
        }

        // Рівень А (п. 1): Додавання з використанням ref-лічильника
        static void AddValue(double[] values, ref int count, double newValue, int capacity)
        {
            if (count == capacity)
            {
                Console.WriteLine("Сховище заповнене. Місця більше немає.");
                return;
            }

            values[count] = newValue;
            count++;
            Console.WriteLine($"Ціну {newValue:F2} грн додано. Усього записів: {count}.");
        }

        // Рівень А (п. 2): Виведення вмісту масиву таблицею до count
        static void PrintPricesTable(double[] values, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Список цін порожній.");
                return;
            }

            Console.WriteLine($"{"№",5}{"Ціна (грн)",15}");
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"{i + 1,5}{values[i],15:F2}");
            }
        }

        // Рівень А (п. 3): Метод із двома out-параметрами (максимум та його індекс)
        static void FindMax(double[] values, int count, out double maxVal, out int maxIndex)
        {
            maxVal = values[0];
            maxIndex = 0;

            for (int i = 1; i < count; i++)
            {
                if (values[i] > maxVal)
                {
                    maxVal = values[i];
                    maxIndex = i;
                }
            }
        }

        static void Statistics(double[] values, int count, out double sum, out double average)
        {
            sum = 0;
            for (int i = 0; i < count; i++)
            {
                sum += values[i];
            }
            average = count > 0 ? sum / count : 0;
        }

        static void ShowStatistics(double[] prices, int count)
        {
            if (count == 0)
            {
                Console.WriteLine("Немає даних для розрахунку статистики.");
                return;
            }

            Statistics(prices, count, out double sum, out double avg);
            FindMax(prices, count, out double maxPrice, out int maxIdx);

            Console.WriteLine($"Кількість позицій: {count}");
            Console.WriteLine($"Загальна сума:     {sum:F2} грн");
            Console.WriteLine($"Середня ціна:      {avg:F2} грн");
            Console.WriteLine($"Найдорожчий товар: {maxPrice:F2} грн (позиція №{maxIdx + 1})");
        }

        // Рівень Б: Видалення зі зсувом вліво
        static void RemoveAt(double[] values, ref int count, int index)
        {
            if (count == 0 || index < 0 || index >= count)
            {
                Console.WriteLine("Помилка: некоректний номер запису.");
                return;
            }

            for (int i = index; i < count - 1; i++)
            {
                values[i] = values[i + 1];
            }
            count--;
            Console.WriteLine($"Запис №{index + 1} видалено. Залишилось: {count}.");
        }

        // Рівень А (п. 4): Демонстрація різниці між int (копія) та int[] (посилання)
        static void RunReferenceDemo()
        {
            int singlePrice = 100;
            int[] arrayPrices = { 100, 200 };

            Console.WriteLine($"До виклику: singlePrice = {singlePrice}, arrayPrices[0] = {arrayPrices[0]}");
            TryChangeValue(singlePrice);
            ChangeArrayElement(arrayPrices);
            Console.WriteLine($"Після виклику: singlePrice = {singlePrice} (не змінився), arrayPrices[0] = {arrayPrices[0]} (змінився)");
        }

        static void TryChangeValue(int number) => number = 999;
        static void ChangeArrayElement(int[] arr) => arr[0] = 999;

        // Допоміжні методи введення з ЛР №8
        static double ReadDouble(string prompt, double min, double max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (double.TryParse(Console.ReadLine(), out double val) && val >= min && val <= max)
                    return val;
                Console.WriteLine($"Потрібне число від {min} до {max}.");
            }
        }

        static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int val) && val >= min && val <= max)
                    return val;
                Console.WriteLine($"Потрібне ціле число від {min} до {max}.");
            }
        }

        static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey(true);
            Console.Clear();
        }
    }
}
