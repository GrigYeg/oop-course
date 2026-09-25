using System;

namespace ClinicApp;

class Program
{
    static void Main(string[] args)
    {
        PatientManager patientManager = new PatientManager();
        
        PatientsMenu(patientManager);
    }

    static void PatientsMenu(PatientManager manager)
    {
        manager.Add(new Patient("Іван", "Петренко", new DateTime(1985, 5, 12), "A+", "0501234567"));
        manager.Add(new Patient("Олена", "Коваль", new DateTime(1993, 8, 24), "B-", "0672345678"));
        manager.Add(new Patient("Максим", "Бойко", new DateTime(2010, 2, 15), "O+", "0933456789"));
        manager.Add(new Patient("Марія", "Ткач"));
        
        manager.DisplayAll();
        manager.DisplayStats();
    }
}