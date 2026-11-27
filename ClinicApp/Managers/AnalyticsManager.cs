using System;
using System.Collections.Generic;
using System.Linq;
using ClinicApp.Models;

namespace ClinicApp.Managers;

public class AnalyticsManager
{
    private readonly AppointmentManager _appointments;
    private readonly DoctorManager _doctors;
    private readonly PatientManager _patients;

    public AnalyticsManager(AppointmentManager appointments, DoctorManager doctors, PatientManager patients)
    {
        _appointments = appointments;
        _doctors = doctors;
        _patients = patients;
    }

    public IEnumerable<DoctorStats> ComputeDoctorStats()
    {
        var allAppointments = _appointments.GetAll();

        return _doctors.GetAll().Select(d =>
        {
            var docApps = allAppointments.Where(a => a.DoctorId == d.Id);
            int count = docApps.Count();
            decimal revenue = docApps.Sum(a => a.GetCost());
            DateTime lastDate = docApps.Any() ? docApps.Max(a => a.ScheduledAt) : DateTime.MinValue;

            return new DoctorStats(d.Id, d.FullName, count, revenue, lastDate);
        });
    }

    public IEnumerable<PatientStats> ComputePatientStats()
    {
        var allAppointments = _appointments.GetAll();

        return _patients.GetAll().Select(p =>
        {
            var patApps = allAppointments.Where(a => a.PatientId == p.Id);
            int count = patApps.Count();
            decimal spent = patApps.Sum(a => a.GetCost());
            DateTime lastDate = patApps.Any() ? patApps.Max(a => a.ScheduledAt) : DateTime.MinValue;

            return new PatientStats(p.Id, p.FullName, count, spent, lastDate);
        });
    }
}