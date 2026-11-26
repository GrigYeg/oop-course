using ClinicApp.Models;
using ClinicApp.Enums;
using ClinicApp.Interfaces;
using ClinicApp.Events;

namespace ClinicApp.Managers;

public class AppointmentManager
{
    private const int MaxAppointments = 500;
    private Appointment[] _appointments = new Appointment[MaxAppointments];
    private int _count = 0;
    private PatientManager _patients;
    private DoctorManager _doctors;

    public int Count => _count;

    public AppointmentManager(PatientManager patients, DoctorManager doctors)
    {
        _patients = patients;
        _doctors = doctors;
    }

    private Appointment? FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].Id == id) return _appointments[i];
        }
        return null;
    }
    
    private bool AddAppointment(Appointment appointment, string notes = "")
    {
        Patient? patient = _patients.FindById(appointment.PatientId);
        if (patient == null)
        {
            Console.WriteLine($"Помилка: пацієнта з ID {appointment.PatientId} не знайдено.");
            return false;
        }

        Doctor? doctor = _doctors.FindById(appointment.DoctorId);
        if (doctor == null)
        {
            Console.WriteLine($"Помилка: лікаря з ID {appointment.DoctorId} не знайдено.");
            return false;
        }

        if (_count >= MaxAppointments)
        {
            Console.WriteLine("Ліміт записів досягнуто.");
            return false;
        }

        _appointments[_count++] = appointment;
        Console.WriteLine($"Запис [{appointment.Id}] створено: {patient.FullName} -> {doctor.FullName} о {appointment.ScheduledAt:dd.MM.yyyy HH:mm}");
        RaiseAppointmentBooked(appointment, notes);
        return true;
    }

    public bool Book(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30)
    {
        RegularAppointment newApp = new RegularAppointment(patientId, doctorId, scheduledAt, durationMinutes);
        return AddAppointment(newApp);
    }

    public bool BookUrgent(int patientId, int doctorId, DateTime scheduledAt, string urgencyNote = "", int durationMinutes = 30)
    {
        UrgentAppointment newApp = new UrgentAppointment(patientId, doctorId, scheduledAt, urgencyNote, durationMinutes);
        if (AddAppointment(newApp, urgencyNote))
        {
            UrgentAppointmentBooked?.Invoke(
                this,
                new AppointmentEventArgs(newApp.Id, newApp.PatientId, newApp.DoctorId, newApp.ScheduledAt, urgencyNote));
            return true;
        }
        return false;
    }

    public bool BookSpecialist(int patientId, int doctorId, DateTime scheduledAt, string topic = "", int durationMinutes = 45)
    {
        SpecialistAppointment newApp = new SpecialistAppointment(patientId, doctorId, scheduledAt, topic, durationMinutes);
        return AddAppointment(newApp, topic);
    }

    public bool Cancel(int id, string reason = "")
    {
        Appointment? app = FindById(id);
        if (app != null && app.Cancel(reason))
        {
            AppointmentCancelled?.Invoke(
                this,
                new AppointmentEventArgs(app.Id, app.PatientId, app.DoctorId, app.ScheduledAt, reason));
            return true;
        }
        return false;
    }

    public bool Complete(int id)
    {
        Appointment? app = FindById(id);
        if (app != null && app.Complete())
        {
            AppointmentCompleted?.Invoke(
                this,
                new AppointmentEventArgs(app.Id, app.PatientId, app.DoctorId, app.ScheduledAt));
            return true;
        }
        return false;
    }
    
    public int CancelAll(ICancellable[] items, string reason = "")
    {
        int canceledCount = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].Cancel(reason))
            {
                canceledCount++;
                if (items[i] is Appointment app)
                {
                    AppointmentCancelled?.Invoke(
                        this,
                        new AppointmentEventArgs(app.Id, app.PatientId, app.DoctorId, app.ScheduledAt, reason));
                }
            }
        }
        return canceledCount;
    }

    public Appointment[] GetByPatient(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].PatientId == patientId) matchCount++;
        }

        Appointment[] results = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].PatientId == patientId) results[index++] = _appointments[i];
        }
        return results;
    }

    public Appointment[] GetByDoctor(int doctorId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].DoctorId == doctorId) matchCount++;
        }

        Appointment[] results = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].DoctorId == doctorId) results[index++] = _appointments[i];
        }
        return results;
    }

    public Appointment[] GetByDate(DateTime date)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].ScheduledAt.Date == date.Date) matchCount++;
        }

        Appointment[] results = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].ScheduledAt.Date == date.Date) results[index++] = _appointments[i];
        }
        return results;
    }
    
    public Appointment[] GetByDate(int year, int month, int day)
    {
        return GetByDate(new DateTime(year, month, day));
    }

    public Appointment[] GetUpcoming()
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].IsUpcoming) matchCount++;
        }

        Appointment[] results = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i].IsUpcoming) results[index++] = _appointments[i];
        }
        return results;
    }
    
    public Appointment[] GetUrgent()
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i] is UrgentAppointment) matchCount++;
        }

        Appointment[] results = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i] is UrgentAppointment) results[index++] = _appointments[i];
        }
        return results;
    }

    public Appointment[] GetSpecialist()
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i] is SpecialistAppointment) matchCount++;
        }

        Appointment[] results = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i] is SpecialistAppointment) results[index++] = _appointments[i];
        }
        return results;
    }

    public Appointment[] GetRegular()
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i] is RegularAppointment) matchCount++;
        }

        Appointment[] results = new Appointment[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_appointments[i] is RegularAppointment) results[index++] = _appointments[i];
        }
        return results;
    }
    
    public Appointment[] GetAll()
    {
        Appointment[] copy = new Appointment[_count];
        for (int i = 0; i < _count; i++)
        {
            copy[i] = _appointments[i];
        }
        return copy;
    }

    public void DisplayAppointment(Appointment a)
    {
        Patient? patient = _patients.FindById(a.PatientId);
        string patientName = patient != null ? patient.FullName : $"Пацієнт #{a.PatientId}";

        Doctor? doctor = _doctors.FindById(a.DoctorId);
        string doctorName = doctor != null ? doctor.FullName : $"Лікар #{a.DoctorId}";

        string baseInfo = $"[{a.Id}] {a.GetDescription()} | {patientName} -> {doctorName} | {a.ScheduledAt:dd.MM.yyyy HH:mm}-{a.EndsAt:HH:mm} | {a.Status} | {a.GetCost():F2} грн";

        if (a.Notes.Length > 0)
        {
            Console.WriteLine($"{baseInfo} | {a.Notes}");
        }
        else
        {
            Console.WriteLine(baseInfo);
        }
    }

    public void DisplayList(Appointment[] list)
    {
        if (list.Length == 0)
        {
            Console.WriteLine("Не знайдено жодного запису.");
            return;
        }

        for (int i = 0; i < list.Length; i++)
        {
            DisplayAppointment(list[i]);
        }
    }
    
    public Appointment? this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) return null;
            return _appointments[index];
        }
    }
    
    public event EventHandler<AppointmentEventArgs>? AppointmentBooked;
    public event EventHandler<AppointmentEventArgs>? AppointmentCancelled;
    public event EventHandler<AppointmentEventArgs>? AppointmentCompleted;
    public event EventHandler<AppointmentEventArgs>? UrgentAppointmentBooked;

    private void RaiseAppointmentBooked(Appointment appointment, string notes = "")
    {
        AppointmentBooked?.Invoke(
            this,
            new AppointmentEventArgs(
                appointment.Id,
                appointment.PatientId,
                appointment.DoctorId,
                appointment.ScheduledAt,
                notes));
    }
}