using System;

namespace ClinicApp;

class Program
{
    static void Main(string[] args)
    {
        GrowablePatientManager manager = new GrowablePatientManager();

        Console.WriteLine("=== Тест GrowablePatientManager ===");
        Console.WriteLine("Додаємо пацієнтів одного за одним...");

        for (int i = 1; i <= 20; i++)
        {
            Patient p = new Patient($"Тест", $"Пацієнт{i}");
            manager.Add(p);
            
            if (i <= 9 || i == 20) 
            {
                Console.WriteLine($"Додано [{i}]. Розмір: {manager.Count} / {manager.Capacity}");
            }
            else if (i == 10)
            {
                Console.WriteLine("...");
            }
        }

        Console.WriteLine("\nТест пошуку:");
        
        Patient? p10 = manager.FindById(10);
        if (p10 != null) Console.WriteLine($"FindById(10) -> {p10.FullName}");
        else Console.WriteLine("FindById(10) -> не знайдено");

        Patient? p99 = manager.FindById(99);
        if (p99 != null) Console.WriteLine($"FindById(99) -> {p99.FullName}");
        else Console.WriteLine("FindById(99) -> не знайдено");

        Console.WriteLine("\nПорівняння:");
        Console.WriteLine($"PatientManager:         100 місць (фіксовано)");
        Console.WriteLine($"GrowablePatientManager: {manager.Capacity} місця (зросте при потребі)");
    }
}