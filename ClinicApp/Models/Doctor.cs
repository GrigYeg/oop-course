using ClinicApp.Enums;
using ClinicApp.Interfaces;
using ClinicApp.Utils;

namespace ClinicApp.Models;

public class Doctor : ISchedulable
{
    private static int _nextId = 1;

    public int Id { get; }
    
    private string _firstName = "";
    public string FirstName
    {
        get => _firstName;
        set
        {
            ClinicValidator.ValidateName(value, nameof(FirstName));
            _firstName = value;
        }
    }
    
    private string _lastName = "";
    public string LastName
    {
        get => _lastName;
        set
        {
            ClinicValidator.ValidateName(value, nameof(LastName));
            _lastName = value;
        }
    }
    
    public Speciality Speciality { get; private set; }
    
    private string _licenseNumber = "";
    public string LicenseNumber
    {
        get => _licenseNumber;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Номер ліцензії не може бути порожнім", nameof(LicenseNumber));
            }
            _licenseNumber = value;
        }
    }
    
    private string _phone = "";
    public string Phone
    {
        get => _phone;
        set
        {
            ClinicValidator.ValidatePhone(value);
            _phone = value;
        }
    }
    
    public WorkSchedule Schedule { get; set; }

    public string FullName => $"{FirstName} {LastName}";
    public bool IsAvailableNow => Schedule.IsNow;

    public Doctor() : this("Невідомий", "Лікар", Speciality.General)
    {
    }

    public Doctor(string firstName, string lastName, Speciality speciality) 
        : this(firstName, lastName, speciality, "Не вказано", "0000000000")
    {
    }

    public bool CanAcceptAt(int hour)
    {
        return Schedule.Contains(hour);
    }

    public override string ToString()
    {
        string status = IsAvailableNow ? "доступний зараз" : "не в робочий час";
        return $"[{Id}] {FullName} | {ClinicFormatter.FormatSpeciality(Speciality)} | {LicenseNumber} | Тел: {ClinicFormatter.FormatPhone(Phone)} | {Schedule} | {status}";
    }
    
    public Doctor(string firstName, string lastName, Speciality speciality, string licenseNumber, string phone)
    {
        FirstName = firstName;
        LastName = lastName;
        Speciality = speciality;
        LicenseNumber = licenseNumber;
        Phone = phone;
        Schedule = new WorkSchedule(8, 17);
        Id = _nextId++;
    }
    
    public bool CanSchedule(DateTime at)
    {
        return Schedule.Contains(at.Hour);
    }

    public DateTime[] GetAvailableSlots(DateTime date, int slotCount)
    {
        ClinicValidator.ValidatePositive(slotCount, nameof(slotCount));
        
        int availableHoursCount = 0;
        for (int i = 0; i < 24; i++)
        {
            if (Schedule.Contains(i))
            {
                availableHoursCount++;
            }
        }

        int actualSlotCount = Math.Min(slotCount, availableHoursCount);
        DateTime[] slots = new DateTime[actualSlotCount];
        DateTime baseDate = date.Date; 
        int slotsFound = 0;
        
        for (int i = 0; i < 24; i++)
        {
            if (Schedule.Contains(i))
            {
                slots[slotsFound] = baseDate.AddHours(i);
                slotsFound++;
                if (slotsFound == actualSlotCount)
                {
                    break;
                }
            }
        }

        return slots;
    }
}