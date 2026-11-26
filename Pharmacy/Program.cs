using System;
using System.Collections.Generic;
using System.Text;

namespace PharmacyApp
{
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

    class Pharmacy
    {
        private List<Medicine> _inventory = new List<Medicine>();
        private int _nextId = 1;

        public Pharmacy()
        {
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
    }

    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Pharmacy pharmacy = new Pharmacy();
            pharmacy.ShowAll();
        }
    }
}