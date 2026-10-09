using System;
using ClinicApp;
using ClinicApp.Models;
using ClinicApp.Enums;
using ClinicApp.Managers;
using ClinicApp.Utils;

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

        Console.WriteLine("\n=== Демонстрація перехоплення винятків (Task 04) ===");

        try
        {
            Patient invalidPatient = new Patient("", "Петренко", new DateTime(2000, 1, 1), BloodType.Unknown, "0501234567");
            clinic.Patients.Add(invalidPatient);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Помилка: " + e.Message);
        }

        try
        {
            WorkSchedule invalidSchedule = new WorkSchedule(20, 6);
            Doctor invalidDoctor = new Doctor("Іван", "Франко", Speciality.General, "LIC-999", "0501112233");
            invalidDoctor.Schedule = invalidSchedule;
            clinic.Doctors.Add(invalidDoctor);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Помилка: " + e.Message);
        }

        try
        {
            Appointment invalidAppointment = new Appointment(1, 1, DateTime.Now, 0);
        }
        catch (ArgumentOutOfRangeException e)
        {
            Console.WriteLine("Помилка: " + e.Message);
        }

        Console.WriteLine("\nПрограма успішно продовжила роботу після всіх винятків.");
    
    }
}