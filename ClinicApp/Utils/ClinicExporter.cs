using System;
using System.IO;
using System.Text;
using ClinicApp.Interfaces;
using ClinicApp.Models;

namespace ClinicApp.Utils;

public class ClinicExporter
{
    private readonly Clinic _clinic;
    private readonly string _baseDir;

    public ClinicExporter(Clinic clinic, string baseDir = "reports")
    {
        _clinic = clinic;
        _baseDir = baseDir;
    }

    public string ExportPatients()
    {
        string dir = EnsureReportDirectory();
        string path = Path.Combine(dir, "patients.txt");

        using StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
        WriteHeader(writer, "Пацієнти - звіт");

        int index = 0;
        foreach (Patient patient in _clinic.Patients.GetAll())
        {
            writer.WriteLine($"{index + 1,3}. {patient}");
            index++;
        }

        WriteFooter(writer, index);
        return path;
    }

    public string ExportAppointments()
    {
        string dir = EnsureReportDirectory();
        string path = Path.Combine(dir, "appointments.txt");

        using StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
        WriteHeader(writer, "Записи на прийом - звіт");

        int index = 0;
        foreach (Appointment appointment in _clinic.Appointments.GetAll())
        {
            writer.WriteLine($"{index + 1,3}. {appointment}");
            index++;
        }

        WriteFooter(writer, index);
        return path;
    }

    public string ExportBilling()
    {
        string dir = EnsureReportDirectory();
        string path = Path.Combine(dir, "billing.txt");

        using StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
        WriteHeader(writer, "Рахунки та борги - звіт");

        int index = 0;
        foreach (IPayable item in _clinic.Billing.GetAllUnpaid())
        {
            writer.WriteLine($"{index + 1,3}. {item}");
            index++;
        }

        writer.WriteLine(new string('─', 40));
        writer.WriteLine($"Всього: {index}");
        writer.WriteLine($"Загальний борг: {_clinic.Billing.GetTotalDebt():F2} грн");
        return path;
    }

    public string ExportTreatmentPlans()
    {
        string dir = EnsureReportDirectory();
        string path = Path.Combine(dir, "treatment_plans.txt");

        using StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);
        WriteHeader(writer, "Плани лікування - звіт");

        int index = 0;
        foreach (TreatmentPlan plan in _clinic.TreatmentPlans.GetAll())
        {
            writer.WriteLine($"{index + 1,3}. {plan}");
            index++;
        }

        WriteFooter(writer, index);
        return path;
    }

    public string[] ExportAll()
    {
        return new string[]
        {
            ExportPatients(),
            ExportAppointments(),
            ExportBilling(),
            ExportTreatmentPlans()
        };
    }

    private string EnsureReportDirectory()
    {
        string dateFolder = DateTime.Now.ToString("yyyy-MM-dd");
        string dir = Path.Combine(_baseDir, dateFolder);
        Directory.CreateDirectory(dir);
        return dir;
    }

    private void WriteHeader(StreamWriter writer, string title)
    {
        writer.WriteLine($"=== {title} ===");
        writer.WriteLine($"Згенеровано: {DateTime.Now:dd.MM.yyyy HH:mm}");
        writer.WriteLine(new string('─', 40));
    }

    private void WriteFooter(StreamWriter writer, int count)
    {
        writer.WriteLine(new string('─', 40));
        writer.WriteLine($"Всього: {count}");
    }
}