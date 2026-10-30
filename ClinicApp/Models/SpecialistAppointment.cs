using System;

namespace ClinicApp.Models;

public sealed class SpecialistAppointment : Appointment
{
    public string ConsultationTopic { get; }

    public SpecialistAppointment(int patientId, int doctorId, DateTime scheduledAt, string topic = "", int durationMinutes = 45, decimal discountMultiplier = 1m)
        : base(patientId, doctorId, scheduledAt, durationMinutes, 1.3m * discountMultiplier)
    {
        ConsultationTopic = topic;
    }

    public override string GetDescription()
    {
        if (string.IsNullOrWhiteSpace(ConsultationTopic))
        {
            return "Консультація спеціаліста";
        }
        return $"Консультація спеціаліста: {ConsultationTopic}";
    }
}