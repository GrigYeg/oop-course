using System;

namespace ClinicApp;

class Program
{
    static void Main(string[] args)
    {
        Clinic clinic = new Clinic("Медична Клініка");
        
        clinic.Patients.Add(new Patient("Іван", "Петренко", new DateTime(1985, 5, 12), "A+", "0501234567"));
        clinic.Patients.Add(new Patient("Олена", "Коваль", new DateTime(1993, 8, 24), "B-", "0672345678"));
        clinic.Patients.Add(new Patient("Максим", "Бойко", new DateTime(2010, 2, 15), "O+", "0933456789"));
        clinic.Patients.Add(new Patient("Марія", "Ткач"));

        clinic.Doctors.Add(new Doctor("Олег", "Сидоренко", "Кардіологія", "LIC-001", "0441234567"));
        clinic.Doctors.Add(new Doctor("Наталія", "Мороз", "Неврологія", "LIC-002", "0442345678"));
        clinic.Doctors.Add(new Doctor("Андрій", "Власенко", "Педіатрія", "LIC-003", "0443456789"));

        clinic.Appointments.Book(1, 1, new DateTime(2026, 9, 25, 10, 0, 0));
        clinic.Appointments.Book(2, 2, new DateTime(2026, 9, 25, 11, 0, 0), 45);
        clinic.Appointments.Book(3, 3, new DateTime(2026, 9, 26, 9, 0, 0), 20);
        
        clinic.DisplaySchedule(new DateTime(2026, 9, 25));
        clinic.GenerateReport();
    }
}