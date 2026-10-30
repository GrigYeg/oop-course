using System;

namespace ClinicApp.Models;

public class RegularAppointment : Appointment
{
    public RegularAppointment(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30, decimal priceMultiplier = 1m)
        : base(patientId, doctorId, scheduledAt, durationMinutes, priceMultiplier)
    {
    }

    public override string GetDescription()
    {
        return "Звичайний прийом";
    }
}