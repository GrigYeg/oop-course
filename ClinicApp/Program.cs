using System;

namespace ClinicApp;

class Program
{
    static void Main(string[] args)
    {
        PatientManager patientManager = new PatientManager();
        DoctorManager doctorManager = new DoctorManager();
        AppointmentManager appointmentManager = new AppointmentManager(patientManager, doctorManager);

        AppointmentsMenu(patientManager, doctorManager, appointmentManager);
    }

    static void AppointmentsMenu(PatientManager patients, DoctorManager doctors, AppointmentManager appointments)
    {
        patients.Add(new Patient("Іван", "Петренко", new DateTime(1985, 5, 12), "A+", "0501234567"));
        patients.Add(new Patient("Олена", "Коваль", new DateTime(1993, 8, 24), "B-", "0672345678"));
        patients.Add(new Patient("Максим", "Бойко", new DateTime(2010, 2, 15), "O+", "0933456789"));

        doctors.Add(new Doctor("Олег", "Сидоренко", "Кардіологія", "LIC-001", "0441234567"));
        doctors.Add(new Doctor("Наталія", "Мороз", "Неврологія", "LIC-002", "0442345678"));
        doctors.Add(new Doctor("Андрій", "Власенко", "Педіатрія", "LIC-003", "0443456789"));

        Console.WriteLine();
        appointments.Book(1, 1, new DateTime(2026, 9, 25, 10, 0, 0));
        appointments.Book(99, 1, new DateTime(2026, 9, 25, 12, 0, 0)); // Помилка

        appointments.Book(2, 2, new DateTime(2026, 9, 25, 11, 0, 0), 45);
        appointments.Book(3, 3, new DateTime(2026, 9, 26, 9, 0, 0), 20);

        Console.WriteLine("\nМайбутні записи:");
        appointments.DisplayList(appointments.GetUpcoming());

        Console.WriteLine();
        if (appointments.Cancel(1))
        {
            Console.WriteLine("Запис [1] скасовано.");
        }

        Console.WriteLine("\nЗаписи пацієнта #2:");
        appointments.DisplayList(appointments.GetByPatient(2));
    }
}