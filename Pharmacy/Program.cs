using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PharmacyApp
{
    // Модель лікарського засобу
    class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool RequiresPrescription { get; set; }
        public DateTime ExpiryDate { get; set; }

        public Medicine() { }

        public Medicine(int id, string name, decimal price, int quantity, bool requiresPrescription, DateTime expiryDate)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            RequiresPrescription = requiresPrescription;
            ExpiryDate = expiryDate;
        }

        public bool IsExpired()
        {
            return ExpiryDate.Date < DateTime.Today;
        }

        public override string ToString()
        {
            string prescriptionText = RequiresPrescription ? "Так" : "Ні";
            string status = IsExpired() ? "ПРОСТРОЧЕНО" : "Придатний";
            return $"[{Id}] {Name,-18} | Ціна: {Price,7:F2} грн | Залишок: {Quantity,3} шт. | Рецепт: {prescriptionText,-3} | Придатний до: {ExpiryDate:yyyy-MM-dd} ({status})";
        }
    }

    // Клас управління логікою аптеки та збереженням даних
    class Pharmacy
    {
        private readonly string _filePath;
        private List<Medicine> _inventory = new List<Medicine>();
        private int _nextId = 1;

        public Pharmacy(string filePath = "pharmacy_data.json")
        {
            _filePath = filePath;
            LoadFromFile();

            if (_inventory.Count == 0)
            {
                AddMedicine("Парацетамол", 45.50m, 30, false, DateTime.Today.AddMonths(8));
                AddMedicine("Ібупрофен", 82.00m, 15, false, DateTime.Today.AddMonths(4));
                AddMedicine("Амоксицилін", 210.00m, 10, true, DateTime.Today.AddDays(-5));
                AddMedicine("Но-шпа", 135.75m, 20, false, DateTime.Today.AddYears(1));
                SaveToFile();
            }
        }

        public void AddMedicine(string name, decimal price, int quantity, bool requiresPrescription, DateTime expiryDate)
        {
            _inventory.Add(new Medicine(_nextId++, name, price, quantity, requiresPrescription, expiryDate));
        }

        public void SaveToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_inventory, options);
            File.WriteAllText(_filePath, json, Encoding.UTF8);
            Console.WriteLine($"Дані збережено у файл '{_filePath}'.");
        }

        public void LoadFromFile()
        {
            if (!File.Exists(_filePath))
            {
                return;
            }

            try
            {
                string json = File.ReadAllText(_filePath, Encoding.UTF8);
                var loaded = JsonSerializer.Deserialize<List<Medicine>>(json);
                if (loaded != null)
                {
                    _inventory = loaded;
                    _nextId = _inventory.Count > 0 ? _inventory.Max(m => m.Id) + 1 : 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка читання файлу '{_filePath}': {ex.Message}");
            }
        }

        public void ShowAll()
        {
            Console.WriteLine("\n--- Асортимент аптеки ---");
            if (_inventory.Count == 0)
            {
                Console.WriteLine("Склад порожній.");
                return;
            }

            foreach (var med in _inventory)
            {
                Console.WriteLine(med);
            }
        }

        public void ShowExpired()
        {
            var expired = _inventory.Where(m => m.IsExpired()).ToList();
            Console.WriteLine("\n--- Прострочені препарати ---");
            if (expired.Count == 0)
            {
                Console.WriteLine("Прострочених препаратів немає.");
                return;
            }

            foreach (var med in expired)
            {
                Console.WriteLine(med);
            }
        }

        public void RemoveExpired()
        {
            int removedCount = _inventory.RemoveAll(m => m.IsExpired());
            if (removedCount == 0)
            {
                Console.WriteLine("Прострочених позицій для списання не знайдено.");
                return;
            }

            Console.WriteLine($"Списано прострочених позицій: {removedCount}.");
            SaveToFile();
        }

        public void ShowSummary()
        {
            int totalUnits = _inventory.Sum(m => m.Quantity);
            decimal totalValue = _inventory.Sum(m => m.Price * m.Quantity);
            int expiredCount = _inventory.Count(m => m.IsExpired());

            Console.WriteLine("\n--- Звіт по складу ---");
            Console.WriteLine($"Кількість найменувань: {_inventory.Count}");
            Console.WriteLine($"Загальна кількість упаковок: {totalUnits} шт.");
            Console.WriteLine($"Загальна вартість запасів: {totalValue:F2} грн.");
            Console.WriteLine($"Прострочених позицій: {expiredCount}");
        }

        public void SearchByName(string query)
        {
            var results = _inventory
                .Where(m => m.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Console.WriteLine($"\n--- Результати пошуку для \"{query}\" ---");
            if (results.Count == 0)
            {
                Console.WriteLine("Препаратів не знайдено.");
                return;
            }

            foreach (var med in results)
            {
                Console.WriteLine(med);
            }
        }

        public void SellMedicine(int id, int amount)
        {
            var med = _inventory.FirstOrDefault(m => m.Id == id);
            if (med == null)
            {
                Console.WriteLine("Помилка: препарат з таким ID не знайдено.");
                return;
            }

            if (med.IsExpired())
            {
                Console.WriteLine("Продаж заборонено: термін придатності препарату минув.");
                return;
            }

            if (amount <= 0)
            {
                Console.WriteLine("Помилка: кількість має бути більшою за нуль.");
                return;
            }

            if (med.Quantity < amount)
            {
                Console.WriteLine($"Недостатньо товару на складі. Доступно: {med.Quantity} шт.");
                return;
            }

            if (med.RequiresPrescription)
            {
                Console.Write("Увага! Препарат рецептурний. Рецепт перевірено? (так/ні): ");
                string answer = Console.ReadLine()?.Trim().ToLower();
                if (answer != "так")
                {
                    Console.WriteLine("Продаж скасовано: відсутній рецепт.");
                    return;
                }
            }

            med.Quantity -= amount;
            decimal totalCost = med.Price * amount;
            Console.WriteLine($"Продано: {med.Name} ({amount} шт.). До сплати: {totalCost:F2} грн.");
            SaveToFile();
        }
    }

    class Program
    {
        static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Помилка вводу. Введіть коректне ціле додатне число.");
            }
        }

        static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (decimal.TryParse(Console.ReadLine(), out decimal value) && value > 0)
                {
                    return value;
                }
                Console.WriteLine("Помилка вводу. Введіть коректну ціну.");
            }
        }

        static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                if (DateTime.TryParseExact(input, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    return date;
                }
                Console.WriteLine("Некоректна дата. Введіть у форматі РРРР-ММ-ДД (наприклад, 2027-05-15).");
            }
        }

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Pharmacy pharmacy = new Pharmacy();
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n=== МЕНЮ АПТЕКИ (ЛАБ 10) ===");
                Console.WriteLine("1. Показати всі ліки");
                Console.WriteLine("2. Знайти препарат за назвою");
                Console.WriteLine("3. Продати препарат");
                Console.WriteLine("4. Додати новий препарат на склад");
                Console.WriteLine("5. Переглянути прострочені ліки");
                Console.WriteLine("6. Списати прострочені ліки");
                Console.WriteLine("7. Звіт по складу");
                Console.WriteLine("0. Вихід");
                Console.Write("Оберіть дію: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        pharmacy.ShowAll();
                        break;

                    case "2":
                        Console.Write("Введіть назву або частину назви: ");
                        string query = Console.ReadLine() ?? "";
                        pharmacy.SearchByName(query);
                        break;

                    case "3":
                        pharmacy.ShowAll();
                        int sellId = ReadInt("\nВведіть ID препарату для продажу: ");
                        int amount = ReadInt("Введіть кількість упаковок: ");
                        pharmacy.SellMedicine(sellId, amount);
                        break;

                    case "4":
                        Console.Write("Назва препарату: ");
                        string name = Console.ReadLine() ?? "Невідомо";
                        decimal price = ReadDecimal("Ціна (грн): ");
                        int qty = ReadInt("Кількість на складі: ");
                        DateTime expiry = ReadDate("Придатний до (РРРР-ММ-ДД): ");

                        Console.Write("Потрібен рецепт? (так/ні): ");
                        bool rx = Console.ReadLine()?.Trim().ToLower() == "так";

                        pharmacy.AddMedicine(name, price, qty, rx, expiry);
                        pharmacy.SaveToFile();
                        Console.WriteLine("Препарат успішно додано!");
                        break;

                    case "5":
                        pharmacy.ShowExpired();
                        break;

                    case "6":
                        pharmacy.RemoveExpired();
                        break;

                    case "7":
                        pharmacy.ShowSummary();
                        break;

                    case "0":
                        isRunning = false;
                        break;

                    default:
                        Console.WriteLine("Невідома команда. Спробуйте ще раз.");
                        break;
                }
            }
        }
    }
}