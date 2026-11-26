using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public Medicine(int id, string name, decimal price, int quantity, bool requiresPrescription)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            RequiresPrescription = requiresPrescription;
        }

        public override string ToString()
        {
            string prescriptionText = RequiresPrescription ? "Так" : "Ні";
            return $"[{Id}] {Name,-18} | Ціна: {Price,7:F2} грн | Залишок: {Quantity,3} шт. | Рецепт: {prescriptionText}";
        }
    }

    // Клас для управління логікою аптеки
    class Pharmacy
    {
        private List<Medicine> _inventory = new List<Medicine>();
        private int _nextId = 1;

        public Pharmacy()
        {
            // Початкове заповнення складу
            AddMedicine("Парацетамол", 45.50m, 30, false);
            AddMedicine("Ібупрофен", 82.00m, 15, false);
            AddMedicine("Амоксицилін", 210.00m, 10, true);
            AddMedicine("Но-шпа", 135.75m, 20, false);
        }

        public void AddMedicine(string name, decimal price, int quantity, bool requiresPrescription)
        {
            _inventory.Add(new Medicine(_nextId++, name, price, quantity, requiresPrescription));
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
        }
    }

    class Program
    {
        static void Main()
        {
            // Налаштування кодування для коректного відображення кирилиці
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Pharmacy pharmacy = new Pharmacy();
            bool isRunning = true;

            while (isRunning)
            {
                Console.WriteLine("\n=== МЕНЮ АПТЕКИ ===");
                Console.WriteLine("1. Показати всі ліки");
                Console.WriteLine("2. Знайти препарат за назвою");
                Console.WriteLine("3. Продати препарат");
                Console.WriteLine("4. Додати новий препарат на склад");
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
                        Console.Write("\nВведіть ID препарату для продажу: ");
                        if (int.TryParse(Console.ReadLine(), out int sellId))
                        {
                            Console.Write("Введіть кількість упаковок: ");
                            if (int.TryParse(Console.ReadLine(), out int amount))
                            {
                                pharmacy.SellMedicine(sellId, amount);
                            }
                            else
                            {
                                Console.WriteLine("Некоректна кількість.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Некоректний ID.");
                        }
                        break;

                    case "4":
                        Console.Write("Назва препарату: ");
                        string name = Console.ReadLine() ?? "Невідомо";

                        Console.Write("Ціна (грн): ");
                        decimal.TryParse(Console.ReadLine(), out decimal price);

                        Console.Write("Кількість на складі: ");
                        int.TryParse(Console.ReadLine(), out int qty);

                        Console.Write("Потрібен рецепт? (так/ні): ");
                        bool rx = Console.ReadLine()?.Trim().ToLower() == "так";

                        pharmacy.AddMedicine(name, price, qty, rx);
                        Console.WriteLine("Препарат успішно додано!");
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