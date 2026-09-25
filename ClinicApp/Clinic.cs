using System;

namespace ClinicApp;

public class Clinic
{
    public string Name { get; }
    public PatientManager Patients { get; }
    public DoctorManager Doctors { get; }
    public AppointmentManager Appointments { get; }

    public Clinic(string name)
    {
        Name = name;
        Patients = new PatientManager();
        Doctors = new DoctorManager();
        Appointments = new AppointmentManager(Patients, Doctors);
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
        Console.WriteLine($"║  Звіт — {Name,-33}║");
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine($"║  Пацієнтів:          {Patients.Count,-24}║");
        Console.WriteLine($"║  Лікарів:            {Doctors.Count,-24}║");
        
        Appointment[] upcoming = Appointments.GetUpcoming();
        Console.WriteLine($"║  Майбутніх записів:  {upcoming.Length,-24}║");
        
        Console.WriteLine("╠══════════════════════════════════════════════╣");
        Console.WriteLine("║  Навантаження лікарів (майбутні записи):     ║");

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
            Console.WriteLine($"║    {docInfo,-42}║");
        }
        Console.WriteLine("╚══════════════════════════════════════════════╝");
    }
}