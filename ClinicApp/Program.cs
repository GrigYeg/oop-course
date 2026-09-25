using System;

namespace ClinicApp;

class Program
{
    static void Main(string[] args)
    {
        Appointment a1 = new Appointment(1, 1, new DateTime(2026, 9, 25, 10, 0, 0));
        Appointment a2 = new Appointment(2, 2, new DateTime(2026, 9, 25, 11, 0, 0), 45);
        Appointment a3 = new Appointment(3, 3, new DateTime(2026, 9, 26, 9, 0, 0), 20);

        Console.WriteLine(a1.ToString());
        Console.WriteLine(a2.ToString());
        Console.WriteLine(a3.ToString());

        Console.WriteLine("\n// Після Cancel та Complete:");
        
        a1.Cancel("Пацієнт не зміг прийти");
        a2.Complete();

        Console.WriteLine(a1.ToString());
        Console.WriteLine(a2.ToString());
    }
}