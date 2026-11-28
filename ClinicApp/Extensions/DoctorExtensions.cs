using System.Collections.Generic;
using System.Linq;
using ClinicApp.Enums;
using ClinicApp.Models;

namespace ClinicApp.Extensions;

public static class DoctorExtensions
{
    public static IEnumerable<Doctor> BySpeciality(this IEnumerable<Doctor> source, Speciality speciality) =>
        source.Where(d => d.Speciality == speciality);

    public static IEnumerable<Doctor> Available(this IEnumerable<Doctor> source) =>
        source.Where(d => d.IsAvailableNow);

    public static IEnumerable<Doctor> WithAppointments(this IEnumerable<Doctor> source, IEnumerable<Appointment> appointments) =>
        source.Where(d => appointments.Any(a => a.DoctorId == d.Id));
}