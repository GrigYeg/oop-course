using System;
using System.IO;
using System.Text;
using ClinicApp.Events;

namespace ClinicApp.Utils;

public class ClinicLogger
{
    private readonly string _logPath;

    public ClinicLogger(string logPath = "clinic.log")
    {
        _logPath = logPath;
    }

    public void LogInfo(string message)
    {
        Write("INFO", message);
    }

    public void LogWarning(string message)
    {
        Write("WARN", message);
    }

    public void LogError(string message)
    {
        Write("ERROR", message);
    }

    public string[] GetLastLines(int n)
    {
        if (!File.Exists(_logPath) || n <= 0)
        {
            return Array.Empty<string>();
        }

        string[] allLines = File.ReadAllLines(_logPath, Encoding.UTF8);
        int skip = allLines.Length - n;
        if (skip < 0)
        {
            skip = 0;
        }

        int count = allLines.Length - skip;
        string[] result = new string[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = allLines[skip + i];
        }

        return result;
    }

    public void Clear()
    {
        if (File.Exists(_logPath))
        {
            File.Delete(_logPath);
        }
    }

    public bool Exists()
    {
        return File.Exists(_logPath);
    }

    private void Write(string level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string paddedLevel = level.PadRight(5);
        string line = $"[{timestamp}] [{paddedLevel}] {message}{Environment.NewLine}";
        File.AppendAllText(_logPath, line, Encoding.UTF8);
    }
    
    public void OnAppointmentBooked(object? sender, AppointmentEventArgs e) =>
        LogInfo($"Запис #{e.AppointmentId} створено: пацієнт #{e.PatientId} → лікар #{e.DoctorId}");

    public void OnAppointmentCancelled(object? sender, AppointmentEventArgs e)
    {
        string details = string.IsNullOrWhiteSpace(e.Notes) ? "" : $": {e.Notes}";
        LogInfo($"Запис #{e.AppointmentId} скасовано{details}");
    }

    public void OnAppointmentCompleted(object? sender, AppointmentEventArgs e) =>
        LogInfo($"Запис #{e.AppointmentId} завершено");

    public void OnUrgentAppointmentBooked(object? sender, AppointmentEventArgs e)
    {
        string details = string.IsNullOrWhiteSpace(e.Notes) ? "без примітки" : e.Notes;
        string message = $"ТЕРМІНОВИЙ запис #{e.AppointmentId}: {details}";
        LogWarning(message);

        Directory.CreateDirectory("alerts");
        string alertPath = Path.Combine("alerts", $"urgent_{DateTime.Now:yyyy-MM-dd}.txt");
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string line = $"[{timestamp}] {message} (пацієнт #{e.PatientId}, лікар #{e.DoctorId}){Environment.NewLine}";
        File.AppendAllText(alertPath, line, Encoding.UTF8);
    }

    public void OnPatientAdded(object? sender, PatientEventArgs e) =>
        LogInfo($"Новий пацієнт #{e.PatientId}: {e.FullName}");

    public void OnPaymentReceived(object? sender, PaymentEventArgs e) =>
        LogInfo($"Оплата запису #{e.AppointmentId}: {e.Amount:F2} грн");

    public void OnPlanCompleted(object? sender, TreatmentPlanEventArgs e) =>
        LogInfo($"План лікування #{e.PlanId} завершено (пацієнт #{e.PatientId}): {e.Diagnosis}");
}