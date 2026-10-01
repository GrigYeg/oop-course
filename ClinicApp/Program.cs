using System;

namespace ClinicApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Clinic clinic = new Clinic("Моя Клініка");

            Console.WriteLine("=== 1. Перевірка початкових даних (Enum) ===");
            
            clinic.Patients.Add(new Patient("Іван", "Петренко", new DateTime(1985, 5, 12), BloodType.APositive, "0501234567"));
            clinic.Patients.Add(new Patient("Олена", "Коваль", new DateTime(1993, 8, 24), BloodType.BNegative, "0672345678"));
            
            clinic.Doctors.Add(new Doctor("Олег", "Сидоренко", Speciality.Cardiology, "LIC-001", "0441234567"));
            clinic.Doctors.Add(new Doctor("Наталія", "Мороз", Speciality.Neurology, "LIC-002", "0442345678"));
            clinic.Doctors.Add(new Doctor("Андрій", "Шевченко", Speciality.Cardiology, "LIC-003", "0443456789"));

            Console.WriteLine($"Успішно додано пацієнтів: {clinic.Patients.Count}");
            Console.WriteLine($"Успішно додано лікарів: {clinic.Doctors.Count}\n");
            
            Console.WriteLine("=== 2. Пошук лікаря за спеціальністю ===");
            Doctor[] cardiologists = clinic.Doctors.FindBySpeciality(Speciality.Cardiology);
            
            Console.WriteLine($"Знайдено кардіологів: {cardiologists.Length}");
            foreach (var doc in cardiologists)
            {
                Console.WriteLine($"- {doc.FirstName} {doc.LastName}");
            }
            Console.WriteLine();
            
            Console.WriteLine("=== 3. Тест GrowablePatientManager ===");
            Console.WriteLine("Додаємо пацієнтів одного за одним...");

            GrowablePatientManager manager = new GrowablePatientManager();

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
            
            if (p10 != null) 
            {
                Console.WriteLine($"FindById(10) -> {p10.FullName}");
            }
            else
            {
                Console.WriteLine("FindById(10) -> Пацієнта не знайдено");
            }
        }
    }
}