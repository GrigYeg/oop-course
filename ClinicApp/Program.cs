using System;
using System.Globalization;
using System.Text;
using System.Threading;
using ClinicApp.Enums;
using ClinicApp.Models;
using ClinicApp.Interfaces;

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

        clinic.MedicalRecords.Add(new Diagnosis(1, 1, DateTime.Today.AddDays(-30), "I10", "Гіпертонічна хвороба", true));
        clinic.MedicalRecords.Add(new Diagnosis(1, 1, DateTime.Today.AddDays(-5), "J06.9", "Гострий ринофарингіт", false));
        clinic.MedicalRecords.Add(new LabResult(1, 1, DateTime.Today.AddDays(-7), "Гемоглобін", 145, "г/л", "120-160", true));
        clinic.MedicalRecords.Add(new LabResult(1, 1, DateTime.Today.AddDays(-3), "Холестерин", 6.2, "ммоль/л", "< 5.2", false));
        clinic.MedicalRecords.Add(new Prescription(1, 1, DateTime.Today.AddDays(-5), "Лізиноприл", "10 мг", 30, "1 раз на добу вранці"));

        clinic.MedicalRecords.Add(new Prescription(2, 2, DateTime.Today.AddDays(-40), "Амоксицилін", "500 мг", 10, "3 рази на добу"));
        clinic.MedicalRecords.Add(new LabResult(2, 2, DateTime.Today.AddDays(-10), "Глюкоза", 5.1, "ммоль/л", "3.9-5.8", true));
        clinic.MedicalRecords.Add(new Diagnosis(3, 2, DateTime.Today.AddDays(-2), "J00", "Гострий назофарингіт", false));

        clinic.Appointments.Book(1, 1, DateTime.Today.AddDays(1).AddHours(10), 30);
        clinic.Appointments.Book(1, 1, DateTime.Today.AddDays(1).AddHours(11), 45);
        clinic.Appointments.Book(1, 1, DateTime.Today.AddDays(1).AddHours(12), 20);

        Appointment[] apps = clinic.Appointments.GetAll();
        if (apps.Length > 0)
        {
            apps[0].Cancel();
        }

        while (true)
        {
            Console.WriteLine("\n╔══════════════════════════════════════════════╗");
            Console.WriteLine("║               МЕДИЧНА КЛІНІКА                ║");
            Console.WriteLine("╠══════════════════════════════════════════════╣");
            Console.WriteLine("║ 1. Пацієнти      — реєстрація, пошук         ║");
            Console.WriteLine("║ 2. Лікарі        — персонал, розклад         ║");
            Console.WriteLine("║ 3. Записи        — прийоми, скасування       ║");
            Console.WriteLine("║ 4. Медична картка — діагнози, рецепти        ║");
            Console.WriteLine("║ 5. Рахунки       — оплата, борги             ║");
            Console.WriteLine("║ 6. Звіт          — загальна статистика       ║");
            Console.WriteLine("║ 0. Вийти                                     ║");
            Console.WriteLine("╚══════════════════════════════════════════════╝");
            Console.Write("Оберіть розділ: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            switch (choice)
            {
                case "1": ManagePatients(clinic); break;
                case "2": ManageDoctors(clinic); break;
                case "3": ManageAppointments(clinic); break;
                case "4": MedicalRecordsMenu(clinic); break;
                case "5": BillingMenu(clinic); break;
                case "6": clinic.GenerateReport(); break;
            }
        }
    }

    static void ManagePatients(Clinic clinic)
    {
        clinic.Patients.DisplayAll();
    }

    static void ManageDoctors(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Лікарі -----------------------------");
            Console.WriteLine("1. Усі лікарі");
            Console.WriteLine("5. Вільні години лікаря");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть пункт: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            if (choice == "1")
            {
                clinic.Doctors.DisplayAll();
            }
            else if (choice == "5")
            {
                Console.Write("ID лікаря: ");
                if (!int.TryParse(Console.ReadLine(), out int docId)) continue;

                Doctor? doctor = clinic.Doctors.FindById(docId);
                if (doctor == null)
                {
                    Console.WriteLine("Лікаря не знайдено");
                    continue;
                }

                Console.Write("Дата (dd.MM.yyyy): ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", null, DateTimeStyles.None, out DateTime date))
                {
                    Console.WriteLine("Некоректна дата");
                    continue;
                }

                Console.Write("Скільки слотів: ");
                if (!int.TryParse(Console.ReadLine(), out int slotsCount)) continue;

                ISchedulable schedulable = doctor;

                try
                {
                    DateTime[] slots = schedulable.GetAvailableSlots(date, slotsCount);
                    for (int i = 0; i < slots.Length; i++)
                    {
                        Console.WriteLine(slots[i].ToString("HH:mm"));
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }
            }
        }
    }

    static void ManageAppointments(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Записи -----------------------------");
            Console.WriteLine("1. Майбутні записи");
            Console.WriteLine("8. Скасувати всі записи пацієнта");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть пункт: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            if (choice == "1")
            {
                clinic.Appointments.DisplayList(clinic.Appointments.GetUpcoming());
            }
            else if (choice == "8")
            {
                Console.Write("ID пацієнта: ");
                if (!int.TryParse(Console.ReadLine(), out int patId)) continue;

                Console.Write("Причина (Enter — без причини): ");
                string? reason = Console.ReadLine() ?? "";

                Appointment[] patApps = clinic.Appointments.GetByPatient(patId);
                int canceled = Managers.AppointmentManager.CancelAll(patApps, reason);

                Console.WriteLine($"Скасовано записів: {canceled}");
            }
        }
    }

    static void BillingMenu(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Рахунки ----------------------------");
            Console.WriteLine("1. Борги пацієнта");
            Console.WriteLine("2. Всі неоплачені записи");
            Console.WriteLine("3. Оплатити запис");
            Console.WriteLine("4. Загальний борг клініки");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    Console.Write("ID пацієнта: ");
                    if (!int.TryParse(Console.ReadLine(), out int pId)) continue;

                    IPayable[] patientUnpaid = clinic.Billing.GetUnpaidByPatient(pId);
                    if (patientUnpaid.Length == 0)
                    {
                        Console.WriteLine("Немає неоплачених записів.");
                        Console.WriteLine("Борг: 0.00 грн");
                    }
                    else
                    {
                        Console.WriteLine($"Неоплачені записи пацієнта #{pId}:");
                        clinic.Billing.DisplayUnpaid(patientUnpaid);
                        Console.WriteLine($"Борг: {clinic.Billing.GetPatientDebt(pId):F2} грн");
                    }
                    break;
                case "2":
                    IPayable[] allUnpaid = clinic.Billing.GetAllUnpaid();
                    clinic.Billing.DisplayUnpaid(allUnpaid);
                    break;
                case "3":
                    Console.Write("ID запису для оплати: ");
                    if (!int.TryParse(Console.ReadLine(), out int aId)) continue;

                    if (clinic.Billing.PayAppointment(aId))
                    {
                        Console.WriteLine($"Запис [{aId}] оплачено.");
                    }
                    else
                    {
                        Console.WriteLine("Не вдалося оплатити: запис не знайдено, вже оплачено або скасовано.");
                    }
                    break;
                case "4":
                    Console.WriteLine($"Загальний борг по клініці: {clinic.Billing.GetTotalDebt():F2} грн");
                    break;
            }
        }
    }

    static void MedicalRecordsMenu(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Медична картка -------------");
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