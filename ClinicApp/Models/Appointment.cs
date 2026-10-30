using ClinicApp.Enums;
using ClinicApp.Utils;
using ClinicApp.Interfaces;

namespace ClinicApp.Models;

public class Appointment : IPayable, ICancellable
{
    protected readonly decimal _priceMultiplier;
    private static int _nextId = 1;

    public int Id { get; }
    public int PatientId { get; }
    public int DoctorId { get; }
    public DateTime ScheduledAt { get; }
    
    private int _durationMinutes;
    public int DurationMinutes
    {
        get => _durationMinutes;
        set
        {
            ClinicValidator.ValidatePositive(value, nameof(DurationMinutes));
            _durationMinutes = value;
        }
    }
    
    public AppointmentStatus Status { get; set; }
    public string Notes { get; private set; }

    public DateTime EndsAt => ScheduledAt.AddMinutes(DurationMinutes);
    public bool IsUpcoming => ScheduledAt > DateTime.Now && Status == AppointmentStatus.Scheduled;
    
    public bool IsCancelled => Status == AppointmentStatus.Cancelled;
    public string CancellationReason => IsCancelled ? Notes : "";
    
    /* Відповідь до Задачі 5:
       Обрано Варіант Б (коефіцієнт у конструкторі підкласу).
       Чому не нові підкласи: створення підкласів для кожної комбінації (VIP-терміновий, Дитячий-звичайний) призведе до комбінаторного вибуху кількості класів. До того ж, клас SpecialistAppointment є sealed, від нього не можна успадкуватись.
       Чому Варіант Б: це класичний приклад принципу Open/Closed (відкритий для розширення, закритий для змін). Ми використовуємо композицію параметрів замість успадкування. Кожен підклас тепер сам контролює свою базову націнку через параметр конструктора, а знижка просто множиться на неї при створенні об'єкта.
    */

    public Appointment(int patientId, int doctorId, DateTime scheduledAt, int durationMinutes = 30, decimal priceMultiplier = 1m)
    {
        PatientId = patientId;
        DoctorId = doctorId;
        ScheduledAt = scheduledAt;
        DurationMinutes = durationMinutes;
        Status = AppointmentStatus.Scheduled;
        Notes = "";
        Id = _nextId++;
        _priceMultiplier = priceMultiplier;
    }
    
    public virtual string GetDescription()
    {
        return "Прийом";
    }

    public int GetPriority()
    {
        return 3;
    }

    public bool Cancel(string reason = "")
    {
        if (Status != AppointmentStatus.Scheduled) return false;
        Status = AppointmentStatus.Cancelled;
        if (reason.Length > 0) Notes = reason;
        return true;
    }

    public bool Complete()
    {
        if (Status != AppointmentStatus.Scheduled) return false;
        Status = AppointmentStatus.Completed;
        return true;
    }
    
    public bool IsPaid { get; private set; }

    public virtual decimal GetCost()
    {
        return (DurationMinutes * 10m) * _priceMultiplier;
    }

    public void MarkPaid()
    {
        if (IsCancelled)
        {
            return;
        }
        IsPaid = true;
    }

    public override string ToString()
    {
        string baseInfo = $"[{Id}] {GetDescription()} | Пацієнт #{PatientId} -> Лікар #{DoctorId} | {ScheduledAt:dd.MM.yyyy HH:mm}-{EndsAt:HH:mm} | {Status} | {GetCost():F2} грн";

        if (Notes.Length > 0)
        {
            return $"{baseInfo} | {Notes}";
        }

        return baseInfo;
    }
}