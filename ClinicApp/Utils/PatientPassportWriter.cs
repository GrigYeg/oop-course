using System;
using System.IO;
using System.Text;
using ClinicApp.Enums;
using ClinicApp.Events;
using ClinicApp.Models;

namespace ClinicApp.Utils;

public class PatientPassportWriter
{
    private readonly Clinic _clinic;
    private readonly string _outputDir;

    public PatientPassportWriter(Clinic clinic, string outputDir = "patients")
    {
        _clinic = clinic;
        _outputDir = outputDir;
        Directory.CreateDirectory(_outputDir);
    }

    public void OnPatientAdded(object? sender, PatientEventArgs e) =>
        WritePassport(e.PatientId);

    public void OnAppointmentCompleted(object? sender, AppointmentEventArgs e) =>
        WritePassport(e.PatientId);

    public void OnPlanCompleted(object? sender, TreatmentPlanEventArgs e) =>
        WritePassport(e.PatientId);

    private void WritePassport(int patientId)
    {
        Patient? patient = _clinic.Patients.FindById(patientId);
        if (patient == null)
        {
            return;
        }

        string path = Path.Combine(_outputDir, $"passport_{patientId}.txt");
        using StreamWriter writer = new StreamWriter(path, false, Encoding.UTF8);

        int age = CalculateAge(patient.DateOfBirth);

        writer.WriteLine($"=== Паспорт пацієнта #{patient.Id} ===");
        writer.WriteLine($"Згенеровано: {DateTime.Now:dd.MM.yyyy HH:mm}");
        writer.WriteLine();
        writer.WriteLine($"Ім'я: {patient.FullName}");
        writer.WriteLine($"Дата народження: {patient.DateOfBirth:dd.MM.yyyy} ({age} {GetAgeWord(age)})");
        writer.WriteLine($"Група крові: {FormatBloodType(patient.BloodType)}   Телефон: {FormatPhone(patient.Phone)}");
        writer.WriteLine();

        MedicalRecord[] records = _clinic.MedicalRecords.GetByPatient(patientId);

        writer.WriteLine("-- Діагнози --");
        bool hasDiagnoses = false;
        for (int i = 0; i < records.Length; i++)
        {
            if (records[i] is Diagnosis d)
            {
                writer.WriteLine(d);
                hasDiagnoses = true;
            }
        }
        if (!hasDiagnoses)
        {
            writer.WriteLine("(немає)");
        }

        writer.WriteLine("-- Аналізи --");
        bool hasLabs = false;
        for (int i = 0; i < records.Length; i++)
        {
            if (records[i] is LabResult l)
            {
                writer.WriteLine(l);
                hasLabs = true;
            }
        }
        if (!hasLabs)
        {
            writer.WriteLine("(немає)");
        }

        writer.WriteLine("-- Рецепти --");
        bool hasPrescriptions = false;
        for (int i = 0; i < records.Length; i++)
        {
            if (records[i] is Prescription p)
            {
                writer.WriteLine(p);
                hasPrescriptions = true;
            }
        }
        if (!hasPrescriptions)
        {
            writer.WriteLine("(немає)");
        }

        writer.WriteLine();
        writer.WriteLine("-- Записи --");
        Appointment[] appointments = _clinic.Appointments.GetByPatient(patientId);
        if (appointments.Length == 0)
        {
            writer.WriteLine("(немає)");
        }
        else
        {
            for (int i = 0; i < appointments.Length; i++)
            {
                Appointment a = appointments[i];
                writer.WriteLine($"[{a.Id}] {a.GetDescription()} | {a.ScheduledAt:dd.MM.yyyy HH:mm} | {a.Status} | {a.GetCost():F2} грн");
            }
        }

        writer.WriteLine();
        writer.WriteLine("-- Плани лікування --");
        bool hasPlans = false;
        foreach (TreatmentPlan plan in _clinic.TreatmentPlans.GetAll())
        {
            if (plan.PatientId == patientId)
            {
                writer.WriteLine(plan);
                hasPlans = true;
            }
        }
        if (!hasPlans)
        {
            writer.WriteLine("(немає)");
        }

        writer.WriteLine();
        writer.WriteLine($"Заборгованість: {_clinic.Billing.GetPatientDebt(patientId):F2} грн");
    }

    private int CalculateAge(DateTime dateOfBirth)
    {
        DateTime today = DateTime.Today;
        int age = today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > today.AddYears(-age))
        {
            age--;
        }
        return age;
    }

    private string GetAgeWord(int age)
    {
        int mod100 = age % 100;
        int mod10 = age % 10;
        if (mod100 >= 11 && mod100 <= 14) return "років";
        if (mod10 == 1) return "рік";
        if (mod10 >= 2 && mod10 <= 4) return "роки";
        return "років";
    }

    private string FormatBloodType(BloodType bloodType)
    {
        switch (bloodType)
        {
            case BloodType.APositive: return "A+";
            case BloodType.ANegative: return "A-";
            case BloodType.BPositive: return "B+";
            case BloodType.BNegative: return "B-";
            case BloodType.ABPositive: return "AB+";
            case BloodType.ABNegative: return "AB-";
            case BloodType.OPositive: return "O+";
            case BloodType.ONegative: return "O-";
            default: return bloodType.ToString();
        }
    }

    private string FormatPhone(string phone)
    {
        if (phone.Length == 10)
        {
            return $"({phone.Substring(0, 3)}) {phone.Substring(3, 3)}-{phone.Substring(6, 4)}";
        }
        return phone;
    }
}