using System;
using ClinicApp.Managers;
using ClinicApp.Models;

namespace ClinicApp;

public class Clinic
{
    public string Name { get; set; }
    public PatientManager Patients { get; }
    public DoctorManager Doctors { get; }
    public AppointmentManager Appointments { get; }
    public MedicalRecordManager MedicalRecords { get; }
    public BillingManager Billing { get; }

    public Clinic(string name)
    {
        Name = name;
        Patients = new PatientManager();
        Doctors = new DoctorManager();
        Appointments = new AppointmentManager(Patients, Doctors);
        MedicalRecords = new MedicalRecordManager();
        Billing = new BillingManager(Appointments);
    }

    public void DisplaySchedule(DateTime date)
    {
        Console.WriteLine($"\n=== Розклад на {date:dd.MM.yyyy} ===");
        Appointment[] dailyAppointments = Appointments.GetByDate(date);
        Appointments.DisplayList(dailyAppointments);
    }

    public void GenerateReport()
    {
        Console.WriteLine("\n╔══════════════════════════════════════════════╗");
        Console.WriteLine($"║ Звіт — {Name,-33} ║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine($"║ Пацієнтів: {Patients.Count,-31} ║");
        Console.WriteLine($"║ Лікарів:   {Doctors.Count,-31} ║");

        Appointment[] upcoming = Appointments.GetUpcoming();
        Console.WriteLine($"║ Майбутніх записів: {upcoming.Length,-23} ║");

        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine("║ Навантаження лікарів (майбутні записи):      ║");

        Doctor[] allDoctors = Doctors.GetAll();
        for (int i = 0; i < allDoctors.Length; i++)
        {
            int doctorCount = 0;
            for (int j = 0; j < upcoming.Length; j++)
            {
                if (upcoming[j].DoctorId == allDoctors[i].Id)
                {
                    doctorCount++;
                }
            }
            string docInfo = $"{allDoctors[i].FullName} ({allDoctors[i].Speciality}): {doctorCount} записів";
            Console.WriteLine($"║   {docInfo,-42} ║");
        }
        Console.WriteLine("╚══════════════════════════════════════════════╝");
    }
}