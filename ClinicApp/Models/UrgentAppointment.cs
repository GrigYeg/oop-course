using System;

namespace ClinicApp.Models;

public class UrgentAppointment : Appointment
{
    public string UrgencyNote { get; }

    public UrgentAppointment(int patientId, int doctorId, DateTime scheduledAt, string urgencyNote = "", int durationMinutes = 30, decimal discountMultiplier = 1m)
        : base(patientId, doctorId, scheduledAt, durationMinutes, 1.5m * discountMultiplier)
    {
        UrgencyNote = urgencyNote;
    }
    
    public sealed override string GetDescription()
    {
        if (string.IsNullOrWhiteSpace(UrgencyNote))
        {
            return "Терміновий";
        }
        return $"Терміновий ({UrgencyNote})";
    }

    public new int GetPriority()
    {
        return 1;
    }
}