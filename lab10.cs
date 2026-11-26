
using System;
using System.Globalization;
using System.Text;

class Program
{
    const int Capacity = 100;

    // Масиви для зберігання інформації про препарати
    static int[] ids = new int[Capacity];
    static string[] names = new string[Capacity];
    static string[] manufacturers = new string[Capacity];
    static int[] quantities = new int[Capacity];
    static double[] prices = new double[Capacity];
    static DateTime[] expirationDates = new DateTime[Capacity];

    static int count = 0;
    static int nextId = 1;

    static void Main()
        
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        while (true)
        {
            Console.WriteLine("\n========== АПТЕКА ==========");
            Console.WriteLine("1. Показати всі препарати");
            Console.WriteLine("2. Додати препарат");
            Console.WriteLine("3. Додати 6 готових препаратів");
            Console.WriteLine("0. Вихід");
            Console.Write("Ваш вибір: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowAll();
                    break;

                case "2":
                    AddMedicine();
                    break;

                case "3":
                    AddTestData();
                    break;

                case "0":
                    Console.WriteLine("Програму завершено.");
                    return;

                default:
                    Console.WriteLine("Неправильний пункт меню!");
                    break;
            }
        }
    }

    // Введення тексту
    static string ReadText(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string value = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(value))
                return value.Trim();

            Console.WriteLine("Поле не може бути порожнім!");
        }
    }

    // Введення цілого числа
    static int ReadInt(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);

            if (int.TryParse(Console.ReadLine(), out int value)
                && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Введіть число від {min} до {max}!");
        }
    }

    // Введення ціни
    static double ReadDouble(string prompt, double min, double max)
    {
        while (true)
        {
            Console.Write(prompt);

            string input = Console.ReadLine()?.Replace(',', '.');

            if (double.TryParse(input,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out double value)
                && !double.IsNaN(value)
                && !double.IsInfinity(value)
                && value >= min && value <= max)
            {
                return value;
            }

            Console.WriteLine($"Введіть ціну від {min} до {max}!");
        }
    }

    // Введення дати
    static DateTime ReadDate(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine();

            if (DateTime.TryParseExact(
                input,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime date)
                && date >= DateTime.Today)
            {
                return date;
            }

            Console.WriteLine(
                "Введіть правильну майбутню дату у форматі дд.мм.рррр!");
        }
    }

    // Додавання препарату
    static void AddMedicine()
    {
        if (count == Capacity)
        {
            Console.WriteLine("Сховище заповнене!");
            return;
        }

        string name = ReadText("Назва препарату: ");
        string manufacturer = ReadText("Виробник: ");
        int quantity = ReadInt("Кількість: ", 1, 100000);
        double price = ReadDouble("Ціна: ", 0.01, 1000000);
        DateTime expirationDate =
            ReadDate("Термін придатності (дд.мм.рррр): ");

        SaveMedicine(name, manufacturer, quantity,
            price, expirationDate);

        Console.WriteLine(
            $"Препарат успішно додано! Номер: {ids[count - 1]}");
    }

    // Збереження препарату в масивах
    static void SaveMedicine(
        string name,
        string manufacturer,
        int quantity,
        double price,
        DateTime expirationDate)
    {
        if (count == Capacity)
        {
            Console.WriteLine("Сховище заповнене!");
            return;
        }

        ids[count] = nextId;
        names[count] = name;
        manufacturers[count] = manufacturer;
        quantities[count] = quantity;
        prices[count] = price;
        expirationDates[count] = expirationDate;

        count++;
        nextId++;
    }

    // Виведення всіх препаратів
    static void ShowAll()
    {
        if (count == 0)
        {
            Console.WriteLine("Препаратів поки немає.");
            return;
        }

        Console.WriteLine("\n====================== СПИСОК ПРЕПАРАТІВ ======================");

        Console.WriteLine(
            $"{"ID",4} {"Назва",-20} {"Виробник",-20} " +
            $"{"Кількість",10} {"Ціна",12} {"Термін",12}");

        for (int i = 0; i < count; i++)
        {
            Console.WriteLine(
                $"{ids[i],4} {names[i],-20} " +
                $"{manufacturers[i],-20} " +
                $"{quantities[i],10} {prices[i],12:F2} " +
                $"{expirationDates[i].ToString("dd.MM.yyyy"),12}");
        }

        Console.WriteLine("===============================================================");
        Console.WriteLine($"Кількість препаратів: {count}");

        double total =
            CalculateTotalValue(quantities, prices, count);

        Console.WriteLine(
            $"Загальна вартість товарів: {total:F2} грн.");
    }

    // Підрахунок загальної вартості препаратів
    static double CalculateTotalValue(
        int[] quantities,
        double[] prices,
        int count)
    {
        double total = 0;

        for (int i = 0; i < count; i++)
        {
            total += quantities[i] * prices[i];
        }

        return total;
    }

    // Додавання шести готових записів
    static void AddTestData()
    {
        if (count + 6 > Capacity)
        {
            Console.WriteLine(
                "Недостатньо місця для додавання 6 записів!");
            return;
        }

        SaveMedicine("Парацетамол", "Дарниця", 50, 45.50,
            new DateTime(2027, 12, 31));

        SaveMedicine("Ібупрофен", "Фармак", 30, 62.00,
            new DateTime(2027, 10, 15));

        SaveMedicine("Цитрамон", "Київмедпрепарат", 40, 35.00,
            new DateTime(2028, 5, 20));

        SaveMedicine("Активоване вугілля", "Фармак", 60, 28.00,
            new DateTime(2028, 8, 10));

        SaveMedicine("Аскорбінова кислота", "Дарниця", 25, 55.00,
            new DateTime(2027, 11, 25));

        SaveMedicine("Но-шпа", "Санофі", 20, 95.00,
            new DateTime(2028, 3, 18));

        Console.WriteLine("Додано 6 готових препаратів!");
    }
}
