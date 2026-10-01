using System;

namespace ClinicApp;

class Program
{
    static void Main(string[] args)
    {
        Clinic clinic = new Clinic("Моя Клініка");

        clinic.Patients.Add(new Patient("Іван", "Петренко", new DateTime(1985, 5, 12), BloodType.APositive, "0501234567"));
        clinic.Patients.Add(new Patient("Олена", "Коваль", new DateTime(1993, 8, 24), BloodType.BNegative, "0672345678"));

        clinic.Doctors.Add(new Doctor("Олег", "Сидоренко", Speciality.Cardiology, "LIC-001", "0441234567"));
        clinic.Doctors.Add(new Doctor("Наталія", "Мороз", Speciality.Neurology, "LIC-002", "0442345678"));
        clinic.Doctors.Add(new Doctor("Андрій", "Шевченко", Speciality.Cardiology, "LIC-003", "0443456789"));

        Console.WriteLine("=== Перевантаження FindBySpeciality ===");
        Doctor[] enumVersion = clinic.Doctors.FindBySpeciality(Speciality.Cardiology);
        Console.WriteLine($"За enum (Cardiology): {enumVersion.Length} лікарів");
        
        Doctor[] stringVersion = clinic.Doctors.FindBySpeciality("кардіо");
        Console.WriteLine($"За рядком ('кардіо'): {stringVersion.Length} лікарів\n");

        Console.WriteLine("=== Перевантаження GetByDate ===");
        Appointment[] today = clinic.Appointments.GetByDate(2026, 5, 10);
        Console.WriteLine($"Знайдено записів на 2026-05-10: {today.Length}\n");

        Console.WriteLine("=== TryFindById (out parameter) ===");
        if (clinic.Patients.TryFindById(1, out Patient? p))
        {
            Console.WriteLine($"Знайдено: {p!.FullName}");
        }
        else
        {
            Console.WriteLine("Пацієнта не знайдено.");
        }

        Console.WriteLine("\n=== Оператори ?. та ?? ===");
        string name = clinic.Patients.FindById(99)?.FullName ?? "не знайдено";
        Console.WriteLine($"Пошук пацієнта 99: {name}");
    }
}