using System;
using System.Globalization;
using System.Text;
using System.Threading;
using ClinicApp.Enums;
using ClinicApp.Models;

namespace ClinicApp;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

        Clinic clinic = new Clinic("Моя Клініка");

        clinic.Patients.Add(new Patient("Іван", "Петренко", new DateTime(1985, 5, 12), BloodType.APositive, "0501234567"));
        clinic.Patients.Add(new Patient("Олена", "Коваль", new DateTime(1993, 8, 24), BloodType.BNegative, "0672345678"));
        clinic.Patients.Add(new Patient("Марія", "Бондар", new DateTime(2000, 11, 5), BloodType.OPositive, "0933456789"));

        clinic.Doctors.Add(new Doctor("Олег", "Сидоренко", Speciality.Cardiology, "LIC-001", "0441234567"));
        clinic.Doctors.Add(new Doctor("Наталія", "Мороз", Speciality.Neurology, "LIC-002", "0442345678"));
        
        clinic.MedicalRecords.Add(new Diagnosis(1, 1, DateTime.Today.AddDays(-30), "I10", "Гіпертонічна хвороба", isChronic: true));
        clinic.MedicalRecords.Add(new Diagnosis(1, 1, DateTime.Today.AddDays(-5), "J06.9", "Гострий ринофарингіт", isChronic: false));
        clinic.MedicalRecords.Add(new LabResult(1, 1, DateTime.Today.AddDays(-7), "Гемоглобін", 145, "г/л", "120-160", isNormal: true));
        clinic.MedicalRecords.Add(new LabResult(1, 1, DateTime.Today.AddDays(-3), "Холестерин", 6.2, "ммоль/л", "< 5.2", isNormal: false));
        clinic.MedicalRecords.Add(new Prescription(1, 1, DateTime.Today.AddDays(-5), "Лізиноприл", "10 мг", 30, "1 раз на добу вранці"));

        clinic.MedicalRecords.Add(new Prescription(2, 2, DateTime.Today.AddDays(-40), "Амоксицилін", "500 мг", 10, "3 рази на добу"));
        clinic.MedicalRecords.Add(new LabResult(2, 2, DateTime.Today.AddDays(-10), "Глюкоза", 5.1, "ммоль/л", "3.9-5.8", isNormal: true));
        clinic.MedicalRecords.Add(new Diagnosis(3, 2, DateTime.Today.AddDays(-2), "J00", "Гострий назофарингіт", isChronic: false));
        
        Console.WriteLine("\n=== Поліморфний вивід записів пацієнта #1 ===");
        MedicalRecord[] patientRecords = clinic.MedicalRecords.GetByPatient(1);
        for (int i = 0; i < patientRecords.Length; i++)
        {
            Console.WriteLine($"{patientRecords[i].GetRecordType()}: {patientRecords[i].GetSummary()}");
        }

        while (true)
        {
            Console.WriteLine("\n=== ГОЛОВНЕ МЕНЮ ===");
            Console.WriteLine("1. Пацієнти");
            Console.WriteLine("2. Лікарі");
            Console.WriteLine("3. Записи на прийом");
            Console.WriteLine("4. Медична картка");
            Console.WriteLine("5. Звіт");
            Console.WriteLine("0. Вихід");
            Console.Write("Оберіть: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            switch (choice)
            {
                case "1": clinic.Patients.DisplayAll(); break;
                case "2": clinic.Doctors.DisplayAll(); break;
                case "3": clinic.Appointments.DisplayList(clinic.Appointments.GetUpcoming()); break;
                case "4": MedicalRecordsMenu(clinic); break;
                case "5": clinic.GenerateReport(); break;
                default: Console.WriteLine("Невідомий пункт меню."); break;
            }
        }
    }

    static void MedicalRecordsMenu(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Медична картка ------------");
            Console.WriteLine("1. Картка пацієнта (зведення)");
            Console.WriteLine("2. Усі записи пацієнта");
            Console.WriteLine("3. Додати діагноз");
            Console.WriteLine("4. Додати аналіз");
            Console.WriteLine("5. Додати рецепт");
            Console.WriteLine("6. Записи лікаря");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                case "2":
                {
                    Console.Write("ID пацієнта: ");
                    if (!int.TryParse(Console.ReadLine(), out int pid)) { Console.WriteLine("Помилка: некоректне число."); break; }
                    if (clinic.Patients.FindById(pid) == null) { Console.WriteLine("Пацієнта не знайдено."); break; }

                    if (choice == "1") clinic.MedicalRecords.DisplayPatientSummary(pid);
                    else clinic.MedicalRecords.DisplayList(clinic.MedicalRecords.GetByPatient(pid));
                    break;
                }
                case "3":
                {
                    if (!TryReadIds(clinic, out int pid, out int did)) break;
                    Console.Write("Код діагнозу (напр. J06.9): ");
                    string code = Console.ReadLine() ?? "";
                    Console.Write("Опис: ");
                    string desc = Console.ReadLine() ?? "";
                    Console.Write("Хронічне? (1=так, 0=ні): ");
                    bool chronic = Console.ReadLine()?.Trim() == "1";

                    try { clinic.MedicalRecords.Add(new Diagnosis(pid, did, DateTime.Today, code, desc, chronic)); }
                    catch (ArgumentOutOfRangeException ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
                    catch (ArgumentException ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
                    break;
                }
                case "4":
                {
                    if (!TryReadIds(clinic, out int pid, out int did)) break;
                    Console.Write("Назва аналізу: ");
                    string name = Console.ReadLine() ?? "";
                    Console.Write("Значення (число): ");
                    if (!double.TryParse(Console.ReadLine(), NumberStyles.Float, CultureInfo.InvariantCulture, out double val))
                    {
                        Console.WriteLine("Помилка: некоректне число.");
                        break;
                    }
                    Console.Write("Одиниці виміру: ");
                    string unit = Console.ReadLine() ?? "";
                    Console.Write("Норма (напр. 4.0-9.0): ");
                    string norm = Console.ReadLine() ?? "";
                    Console.Write("В нормі? (1=так, 0=ні): ");
                    bool isNorm = Console.ReadLine()?.Trim() == "1";

                    try { clinic.MedicalRecords.Add(new LabResult(pid, did, DateTime.Today, name, val, unit, norm, isNorm)); }
                    catch (ArgumentOutOfRangeException ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
                    catch (ArgumentException ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
                    break;
                }
                case "5":
                {
                    if (!TryReadIds(clinic, out int pid, out int did)) break;
                    Console.Write("Препарат: ");
                    string med = Console.ReadLine() ?? "";
                    Console.Write("Дозування (напр. 10 мг): ");
                    string dose = Console.ReadLine() ?? "";
                    Console.Write("Кількість днів: ");
                    if (!int.TryParse(Console.ReadLine(), out int days)) { Console.WriteLine("Помилка: некоректне число."); break; }
                    Console.Write("Інструкція (Enter — пропустити): ");
                    string instr = Console.ReadLine() ?? "";

                    try { clinic.MedicalRecords.Add(new Prescription(pid, did, DateTime.Today, med, dose, days, instr)); }
                    catch (ArgumentOutOfRangeException ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
                    catch (ArgumentException ex) { Console.WriteLine($"Помилка: {ex.Message}"); }
                    break;
                }
                case "6":
                {
                    Console.Write("ID лікаря: ");
                    if (!int.TryParse(Console.ReadLine(), out int did)) { Console.WriteLine("Помилка: некоректне число."); break; }
                    if (clinic.Doctors.FindById(did) == null) { Console.WriteLine("Лікаря не знайдено."); break; }
                    clinic.MedicalRecords.DisplayList(clinic.MedicalRecords.GetByDoctor(did));
                    break;
                }
                default:
                    Console.WriteLine("Невідомий пункт меню.");
                    break;
            }
        }
    }

    static bool TryReadIds(Clinic clinic, out int patientId, out int doctorId)
    {
        patientId = 0;
        doctorId = 0;
        Console.Write("ID пацієнта: ");
        if (!int.TryParse(Console.ReadLine(), out patientId)) { Console.WriteLine("Помилка: некоректне число."); return false; }
        if (clinic.Patients.FindById(patientId) == null) { Console.WriteLine("Пацієнта не знайдено."); return false; }

        Console.Write("ID лікаря: ");
        if (!int.TryParse(Console.ReadLine(), out doctorId)) { Console.WriteLine("Помилка: некоректне число."); return false; }
        if (clinic.Doctors.FindById(doctorId) == null) { Console.WriteLine("Лікаря не знайдено."); return false; }
        return true;
    }
}