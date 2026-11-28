using System;
using System.Collections.Generic;
using System.Linq;
using ClinicApp.Enums;
using ClinicApp.Models;

namespace ClinicApp.Extensions;

public static class PatientExtensions
{
    public static IEnumerable<Patient> Adults(this IEnumerable<Patient> source) =>
        source.Where(p => p.DateOfBirth.Date.AddYears(18) <= DateTime.Today);

    public static IEnumerable<Patient> ByBloodType(this IEnumerable<Patient> source, BloodType bloodType) =>
        source.Where(p => p.BloodType == bloodType);

    public static IEnumerable<Patient> WithAppointments(this IEnumerable<Patient> source, IEnumerable<Appointment> appointments) =>
        source.Where(p => appointments.Any(a => a.PatientId == p.Id));
}