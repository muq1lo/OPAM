using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace PharmacyApp
{
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

        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Pharmacy pharmacy = new Pharmacy();
            pharmacy.ShowAll();
        }
    }
}