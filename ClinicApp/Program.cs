using System;
using System.Globalization;
using System.Text;
using System.Threading;
using ClinicApp.Enums;
using ClinicApp.Utils;
using ClinicApp.Models;
using ClinicApp.Interfaces;
using ClinicApp.Comparators;

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
        clinic.Appointments.BookUrgent(1, 1, DateTime.Today.AddDays(1).AddHours(11), "біль у грудях", 45);
        clinic.Appointments.BookSpecialist(1, 1, DateTime.Today.AddDays(1).AddHours(12), "кардіологія", 20);

        Console.WriteLine("\n== new vs override ==");
        UrgentAppointment urgentApp = new UrgentAppointment(1, 1, DateTime.Today, "тест");
        Appointment baseApp = urgentApp;

        Console.WriteLine($"Через Appointment:      {baseApp.GetDescription()} | пріоритет {baseApp.GetPriority()}");
        Console.WriteLine($"Через UrgentAppointment: {urgentApp.GetDescription()} | пріоритет {urgentApp.GetPriority()}");

        /* Відповіді на запитання Задачі 3:
           1. Чому GetDescription() через змінну типу Appointment дає опис термінового прийому?
              Тому що метод GetDescription() є віртуальним (virtual/override). Викликається реалізація, яка відповідає фактичному типу об'єкта в пам'яті (UrgentAppointment), незалежно від типу змінної-посилання.

           2. Чому GetPriority() через ту саму змінну повертає 3, хоча об'єкт — UrgentAppointment?
              Тому що GetPriority() прихований (модифікатор new), а не перевизначений. Для невіртуальних методів компілятор жорстко прив'язує виклик до типу змінної (Appointment), а не до типу самого об'єкта.

           3. Що треба змінити в Appointment і UrgentAppointment, щоб GetPriority() теж працював поліморфно?
              У класі Appointment додати ключове слово virtual до методу GetPriority(). У класі UrgentAppointment змінити модифікатор new на override для методу GetPriority().
        */

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
			Console.WriteLine("║ 6. Черга         — очікування, прийом        ║");
            Console.WriteLine("║ 7. Звіт          — загальна статистика       ║");
            Console.WriteLine("║ 8. Аналітика     — рейтинги                  ║");
            Console.WriteLine("║ 9. Плани лікування — рефлексія, атрибути     ║");
            Console.WriteLine("║ 10. Файли        — журнал, звіти             ║");
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
                case "6": WaitingRoomMenu(clinic); break;
				case "7": clinic.GenerateReport(); break;
                case "8": AnalyticsMenu(clinic); break;
                case "9": TreatmentPlansMenu(clinic); break;
                case "10": FilesMenu(clinic); break;
            }
        }
    }

    static void ManagePatients(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Пацієнти -------------------------");
            Console.WriteLine("1. Усі пацієнти");
            Console.WriteLine("2. Додати пацієнта");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть пункт: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            if (choice == "1")
            {
                clinic.Patients.DisplayAll();
            }
            else if (choice == "2")
            {
                try
                {
                    Console.Write("Ім'я: ");
                    string firstName = Console.ReadLine() ?? "";

                    Console.Write("Прізвище: ");
                    string lastName = Console.ReadLine() ?? "";

                    Console.Write("Дата народження (dd.MM.yyyy): ");
                    string birthInput = Console.ReadLine() ?? "";
                    DateTime birthDate = DateTime.ParseExact(birthInput, "dd.MM.yyyy", CultureInfo.InvariantCulture);

                    Console.Write("Група крові (APositive, ANegative, BPositive, BNegative, ABPositive, ABNegative, OPositive, ONegative): ");
                    string bloodInput = Console.ReadLine() ?? "";
                    BloodType bloodType = (BloodType)Enum.Parse(typeof(BloodType), bloodInput, true);

                    Console.Write("Телефон (10 цифр): ");
                    string phone = Console.ReadLine() ?? "";

                    Patient patient = new Patient(firstName, lastName, birthDate, bloodType, phone);
                    clinic.Patients.Add(patient);
                    clinic.Logger.LogInfo($"Додано пацієнта #{patient.Id}: {patient.FullName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                    clinic.Logger.LogWarning($"Помилка введення: {ex.Message}");
                }
            }
        }
    }

    static void ManageDoctors(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Лікарі -------------------------");
            Console.WriteLine("1. Усі лікарі");
            Console.WriteLine("2. Додати лікаря");
            Console.WriteLine("5. Вільні години лікаря");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть пункт: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            if (choice == "1")
            {
                clinic.Doctors.DisplayAll();
            }
            else if (choice == "2")
            {
                try
                {
                    Console.Write("Ім'я: ");
                    string firstName = Console.ReadLine() ?? "";

                    Console.Write("Прізвище: ");
                    string lastName = Console.ReadLine() ?? "";

                    Console.Write("Спеціальність (Cardiology, Neurology, ...): ");
                    string specInput = Console.ReadLine() ?? "";
                    Speciality speciality = (Speciality)Enum.Parse(typeof(Speciality), specInput, true);

                    Console.Write("Номер ліцензії: ");
                    string license = Console.ReadLine() ?? "";

                    Console.Write("Телефон (10 цифр): ");
                    string phone = Console.ReadLine() ?? "";

                    Doctor doctor = new Doctor(firstName, lastName, speciality, license, phone);
                    clinic.Doctors.Add(doctor);
                    clinic.Logger.LogInfo($"Додано лікаря #{doctor.Id}: {doctor.FullName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                    clinic.Logger.LogWarning($"Помилка введення: {ex.Message}");
                }
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
                    clinic.Logger.LogWarning($"Помилка введення: {ex.Message}");
                }
            }
        }
    }

    static void ManageAppointments(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Записи -------------------------");
            Console.WriteLine("1. Майбутні записи");
            Console.WriteLine("2. Створити запис");
            Console.WriteLine("3. Завершити запис");
            Console.WriteLine("4. Скасувати запис");
            Console.WriteLine("8. Скасувати всі записи пацієнта");
            Console.WriteLine("9. За типом прийому");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть пункт: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            if (choice == "1")
            {
                clinic.Appointments.DisplayList(clinic.Appointments.GetUpcoming());
            }
            else if (choice == "2")
            {
                try
                {
                    Console.Write("ID пацієнта: ");
                    int patId = int.Parse(Console.ReadLine() ?? "");

                    Console.Write("ID лікаря: ");
                    int docId = int.Parse(Console.ReadLine() ?? "");

                    Console.Write("Дата і час (dd.MM.yyyy HH:mm): ");
                    string dateInput = Console.ReadLine() ?? "";
                    DateTime scheduledAt = DateTime.ParseExact(dateInput, "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);

                    Console.Write("Тривалість (хв): ");
                    int duration = int.Parse(Console.ReadLine() ?? "");

                    int countBefore = clinic.Appointments.GetAll().Length;
                    clinic.Appointments.Book(patId, docId, scheduledAt, duration);
                    Appointment[] allAfter = clinic.Appointments.GetAll();
                    if (allAfter.Length > countBefore)
                    {
                        Appointment created = allAfter[allAfter.Length - 1];
                        clinic.Logger.LogInfo($"Запис #{created.Id} створено: пацієнт #{created.PatientId} -> лікар #{created.DoctorId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                    clinic.Logger.LogWarning($"Помилка введення: {ex.Message}");
                }
            }
            else if (choice == "3")
            {
                Console.Write("ID запису для завершення: ");
                if (!int.TryParse(Console.ReadLine(), out int appId)) continue;

                Appointment? target = null;
                Appointment[] allApps = clinic.Appointments.GetAll();
                for (int i = 0; i < allApps.Length; i++)
                {
                    if (allApps[i].Id == appId)
                    {
                        target = allApps[i];
                        break;
                    }
                }

                if (target != null && target.Complete())
                {
                    Console.WriteLine($"Запис [{appId}] завершено.");
                    clinic.Logger.LogInfo($"Запис #{appId} завершено");
                }
                else
                {
                    Console.WriteLine("Не вдалося завершити запис.");
                }
            }
            else if (choice == "4")
            {
                Console.Write("ID запису для скасування: ");
                if (!int.TryParse(Console.ReadLine(), out int appId)) continue;

                Console.Write("Причина (Enter — без причини): ");
                string reason = Console.ReadLine() ?? "";

                Appointment? target = null;
                Appointment[] allApps = clinic.Appointments.GetAll();
                for (int i = 0; i < allApps.Length; i++)
                {
                    if (allApps[i].Id == appId)
                    {
                        target = allApps[i];
                        break;
                    }
                }

                if (target != null && target.Cancel(reason))
                {
                    Console.WriteLine($"Запис [{appId}] скасовано.");
                    clinic.Logger.LogInfo($"Запис #{appId} скасовано");
                }
                else
                {
                    Console.WriteLine("Не вдалося скасувати запис.");
                }
            }
            else if (choice == "8")
            {
                Console.Write("ID пацієнта: ");
                if (!int.TryParse(Console.ReadLine(), out int patId)) continue;

                Console.Write("Причина (Enter — без причини): ");
                string reason = Console.ReadLine() ?? "";

                Appointment[] patApps = clinic.Appointments.GetByPatient(patId);
                int canceled = ClinicApp.Managers.AppointmentManager.CancelAll(patApps, reason);

                Console.WriteLine($"Скасовано записів: {canceled}");
                if (canceled > 0)
                {
                    clinic.Logger.LogInfo($"Скасовано {canceled} записів пацієнта #{patId}");
                }
            }
            else if (choice == "9")
            {
                Console.WriteLine("1. Термінові");
                Console.WriteLine("2. Консультації спеціаліста");
                Console.WriteLine("3. Звичайні");
                Console.Write("Оберіть: ");

                string? typeChoice = Console.ReadLine()?.Trim();

                if (typeChoice == "1")
                {
                    clinic.Appointments.DisplayList(clinic.Appointments.GetUrgent());
                }
                else if (typeChoice == "2")
                {
                    clinic.Appointments.DisplayList(clinic.Appointments.GetSpecialist());
                }
                else if (typeChoice == "3")
                {
                    clinic.Appointments.DisplayList(clinic.Appointments.GetRegular());
                }
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
                        clinic.Logger.LogInfo($"Оплачено запис #{aId}");
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

static void WaitingRoomMenu(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Черга ------------------------------");
            Console.WriteLine("1. Додати пацієнта до черги");
            Console.WriteLine("2. Прийняти першого");
            Console.WriteLine("3. Хто перший?");
            Console.WriteLine("4. Переглянути всю чергу");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть пункт: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                    Console.Write("ID пацієнта: ");
                    if (int.TryParse(Console.ReadLine(), out int pId))
                    {
                        if (clinic.Patients.TryFindById(pId, out Patient? p) && p != null)
                        {
                            clinic.WaitingRoom.Enqueue(p);
                            Console.WriteLine($"Пацієнта {p.FullName} додано до черги.");
                        }
                        else
                        {
                            Console.WriteLine("Пацієнта не знайдено.");
                        }
                    }
                    break;

                case "2":
                    try
                    {
                        Patient accepted = clinic.WaitingRoom.Dequeue();
                        Console.WriteLine($"Прийнято: {accepted.FullName}. У черзі лишилось: {clinic.WaitingRoom.Count}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;

                case "3":
                    try
                    {
                        Patient first = clinic.WaitingRoom.Peek();
                        Console.WriteLine($"Перший у черзі: {first.FullName}");
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    break;

                case "4":
                    Patient[] queueArray = clinic.WaitingRoom.ToArray();
                    if (queueArray.Length == 0)
                    {
                        Console.WriteLine("Черга порожня.");
                    }
                    else
                    {
                        for (int i = 0; i < queueArray.Length; i++)
                        {
                            Console.WriteLine($"[{i + 1}] {queueArray[i].FullName}");
                        }
                    }
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
    
    static List<ClinicApp.Models.DoctorStats> CollectDoctorStats(Clinic clinic)
    {
        var list = new List<ClinicApp.Models.DoctorStats>();
        foreach (var stat in clinic.Analytics.ComputeDoctorStats())
        {
            list.Add(stat);
        }
        return list;
    }

    static List<ClinicApp.Models.PatientStats> CollectPatientStats(Clinic clinic)
    {
        var list = new List<ClinicApp.Models.PatientStats>();
        foreach (var stat in clinic.Analytics.ComputePatientStats())
        {
            list.Add(stat);
        }
        return list;
    }
    static void AnalyticsMenu(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Аналітика -------------------------");
            Console.WriteLine("1. Лікарі за навантаженням");
            Console.WriteLine("2. Лікарі за виручкою");
            Console.WriteLine("3. Лікарі за іменем");
            Console.WriteLine("4. Пацієнти за кількістю візитів");
            Console.WriteLine("5. Пацієнти за витратами");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть пункт: ");
            string choice = Console.ReadLine() ?? "";

            if (choice == "0") break;

            if (choice == "1" || choice == "2" || choice == "3")
            {
                var docList = CollectDoctorStats(clinic);
                
                if (choice == "1")
                    docList.Sort(); // Природний порядок (IComparable)
                else if (choice == "2")
                    docList.Sort(new DoctorStatsByRevenue());
                else if (choice == "3")
                    docList.Sort(new DoctorStatsByName());

                for (int i = 0; i < docList.Count; i++)
                {
                    Console.WriteLine(docList[i]);
                }
            }
            else if (choice == "4" || choice == "5")
            {
                var patList = CollectPatientStats(clinic);

                if (choice == "4")
                    patList.Sort(); // Природний порядок (IComparable)
                else if (choice == "5")
                    patList.Sort(new PatientStatsBySpent());

                for (int i = 0; i < patList.Count; i++)
                {
                    Console.WriteLine(patList[i]);
                }
            }
        }
    }
    static void TreatmentPlansMenu(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Плани лікування -------------------------");
            Console.WriteLine("1. Показати всі плани");
            Console.WriteLine("2. Додати план лікування");
            Console.WriteLine("3. Плани пацієнта");
            Console.WriteLine("4. Активувати план");
            Console.WriteLine("5. Завершити план");
            Console.WriteLine("6. Скасувати план");
            Console.WriteLine("7. Інформація про тип TreatmentPlan");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                {
                    TreatmentPlan[] allPlans = clinic.TreatmentPlans.GetAll();
                    if (allPlans.Length == 0)
                    {
                        Console.WriteLine("Планів лікування немає.");
                    }
                    else
                    {
                        for (int i = 0; i < allPlans.Length; i++)
                        {
                            Console.WriteLine(allPlans[i]);
                        }
                    }
                    break;
                }
                case "2":
                {
                    TreatmentPlan plan = FormBuilder.Build<TreatmentPlan>();
                    if (clinic.TreatmentPlans.Add(plan))
                    {
                        Console.WriteLine($"План лікування [{plan.Id}] додано.");
                    }
                    else
                    {
                        Console.WriteLine("План не додано.");
                    }
                    break;
                }
                case "3":
                {
                    Console.Write("ID пацієнта: ");
                    if (!int.TryParse(Console.ReadLine(), out int patientId))
                    {
                        Console.WriteLine("Помилка: некоректне число.");
                        break;
                    }

                    TreatmentPlan[] patientPlans = clinic.TreatmentPlans.GetByPatient(patientId);
                    if (patientPlans.Length == 0)
                    {
                        Console.WriteLine("Планів для цього пацієнта не знайдено.");
                    }
                    else
                    {
                        for (int i = 0; i < patientPlans.Length; i++)
                        {
                            Console.WriteLine(patientPlans[i]);
                        }
                    }
                    break;
                }
                case "4":
                {
                    Console.Write("ID плану: ");
                    if (!int.TryParse(Console.ReadLine(), out int planId))
                    {
                        Console.WriteLine("Помилка: некоректне число.");
                        break;
                    }

                    TreatmentPlan? plan = clinic.TreatmentPlans.GetById(planId);
                    if (plan == null)
                    {
                        Console.WriteLine("План не знайдено.");
                    }
                    else if (plan.Activate())
                    {
                        Console.WriteLine($"План [{plan.Id}] активовано.");
                    }
                    else
                    {
                        Console.WriteLine($"Неможливо активувати план у статусі {plan.Status}.");
                    }
                    break;
                }
                case "5":
                {
                    Console.Write("ID плану: ");
                    if (!int.TryParse(Console.ReadLine(), out int planId))
                    {
                        Console.WriteLine("Помилка: некоректне число.");
                        break;
                    }

                    TreatmentPlan? plan = clinic.TreatmentPlans.GetById(planId);
                    if (plan == null)
                    {
                        Console.WriteLine("План не знайдено.");
                    }
                    else if (plan.Complete())
                    {
                        Console.WriteLine($"План [{plan.Id}] завершено.");
                    }
                    else
                    {
                        Console.WriteLine($"Неможливо завершити план у статусі {plan.Status}.");
                    }
                    break;
                }
                case "6":
                {
                    Console.Write("ID плану: ");
                    if (!int.TryParse(Console.ReadLine(), out int planId))
                    {
                        Console.WriteLine("Помилка: некоректне число.");
                        break;
                    }

                    TreatmentPlan? plan = clinic.TreatmentPlans.GetById(planId);
                    if (plan == null)
                    {
                        Console.WriteLine("План не знайдено.");
                    }
                    else if (plan.Cancel())
                    {
                        Console.WriteLine($"План [{plan.Id}] скасовано.");
                    }
                    else
                    {
                        Console.WriteLine($"Неможливо скасувати план у статусі {plan.Status}.");
                    }
                    break;
                }
                case "7":
                {
                    ModelValidator.PrintInfo(typeof(TreatmentPlan));
                    break;
                }
                default:
                {
                    Console.WriteLine("Невідомий пункт меню.");
                    break;
                }
            }
        }
    }
    static void FilesMenu(Clinic clinic)
    {
        while (true)
        {
            Console.WriteLine("\n--- Файли -------------------------");
            Console.WriteLine("1. Останні рядки лога");
            Console.WriteLine("2. Очистити лог");
            Console.WriteLine("3. Експортувати всі звіти");
            Console.WriteLine("4. Імпорт пацієнтів з CSV");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть: ");

            string? choice = Console.ReadLine()?.Trim();
            if (choice == "0") break;

            switch (choice)
            {
                case "1":
                {
                    Console.Write("Скільки останніх рядків показати: ");
                    if (!int.TryParse(Console.ReadLine(), out int n) || n <= 0)
                    {
                        Console.WriteLine("Помилка: введіть додатне число.");
                        break;
                    }

                    string[] lines = clinic.Logger.GetLastLines(n);
                    if (lines.Length == 0)
                    {
                        Console.WriteLine("Лог порожній або файл не існує.");
                    }
                    else
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            Console.WriteLine(lines[i]);
                        }
                    }
                    break;
                }
                case "2":
                {
                    clinic.Logger.Clear();
                    Console.WriteLine("Лог очищено.");
                    break;
                }
                case "3":
                {
                    string[] paths = clinic.Exporter.ExportAll();
                    Console.WriteLine("Звіти збережено:");
                    for (int i = 0; i < paths.Length; i++)
                    {
                        Console.WriteLine($"  {paths[i]}");
                    }
                    clinic.Logger.LogInfo($"Експортовано {paths.Length} звітів");
                    break;
                }
                case "4":
                {
                    Console.Write("Шлях до CSV-файлу: ");
                    string filePath = (Console.ReadLine() ?? "").Trim().Trim('"');
                    ImportResult result = CsvImporter.ImportPatients(clinic, filePath);
                    result.Print();
                    clinic.Logger.LogInfo($"Імпорт CSV ({filePath}): імпортовано {result.Imported}, пропущено {result.Skipped}");
                    break;
                }
                default:
                {
                    Console.WriteLine("Невідомий пункт меню.");
                    break;
                }
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