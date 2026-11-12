using System;
using System.Collections.Generic;
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
        var allDoctors = _doctors.GetAll();
        var allAppointments = _appointments.GetAll();

        for (int i = 0; i < allDoctors.Length; i++)
        {
            var doc = allDoctors[i];
            int count = 0;
            decimal revenue = 0;
            DateTime lastDate = DateTime.MinValue;

            for (int j = 0; j < allAppointments.Length; j++)
            {
                var app = allAppointments[j];
                if (app.DoctorId == doc.Id)
                {
                    count++;
                    revenue += app.GetCost();
                    if (app.ScheduledAt > lastDate)
                    {
                        lastDate = app.ScheduledAt;
                    }
                }
            }
            yield return new DoctorStats(doc.Id, doc.FullName, count, revenue, lastDate);
        }
    }

    public IEnumerable<PatientStats> ComputePatientStats()
    {
        var allPatients = _patients.GetAll();
        var allAppointments = _appointments.GetAll();

        for (int i = 0; i < allPatients.Length; i++)
        {
            var pat = allPatients[i];
            int count = 0;
            decimal spent = 0;
            DateTime lastDate = DateTime.MinValue;

            for (int j = 0; j < allAppointments.Length; j++)
            {
                var app = allAppointments[j];
                if (app.PatientId == pat.Id)
                {
                    count++;
                    spent += app.GetCost();
                    if (app.ScheduledAt > lastDate) 
                    {
                        lastDate = app.ScheduledAt;
                    }
                }
            }

            yield return new PatientStats(pat.Id, pat.FullName, count, spent, lastDate);
        }
    }
}