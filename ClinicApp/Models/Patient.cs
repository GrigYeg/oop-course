using ClinicApp.Enums;
using ClinicApp.Utils;

namespace ClinicApp.Models;

public class Patient
{
    private static int _nextId = 1;

    public int Id { get; }

    private string _firstName = "";
    public string FirstName
    {
        get => _firstName;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
            {
                throw new ArgumentException("Ім'я не може бути порожнім і має бути <= 50 символів", nameof(FirstName));
            }
            _firstName = value;
        }
    }

    private string _lastName = "";
    public string LastName
    {
        get => _lastName;
        set
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
            {
                throw new ArgumentException("Прізвище не може бути порожнім і має бути <= 50 символів", nameof(LastName));
            }
            _lastName = value;
        }
    }

    private DateTime _dateOfBirth;
    public DateTime DateOfBirth
    {
        get => _dateOfBirth;
        set
        {
            if (value > DateTime.Today || value.Year < 1900)
            {
                throw new ArgumentOutOfRangeException(nameof(DateOfBirth), "Дата народження має бути в межах від 1900 року до сьогодні");
            }
            _dateOfBirth = value;
        }
    }

    public BloodType BloodType { get; private set; }

    private string _phone = "";
    public string Phone
    {
        get => _phone;
        set
        {
            if (value == null || value.Length != 10)
            {
                throw new ArgumentException("Номер телефону має містити рівно 10 цифр", nameof(Phone));
            }
            foreach (char c in value)
            {
                if (!char.IsDigit(c))
                {
                    throw new ArgumentException("Номер телефону має містити лише цифри", nameof(Phone));
                }
            }
            _phone = value;
        }
    }

    public string Email { get; set; } = "";

    public string FullName => $"{FirstName} {LastName}";

    public int Age
    {
        get
        {
            int age = DateTime.Today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
            return age;
        }
    }

    public bool IsAdult => Age >= 18;

    public Patient() : this("Невідомий", "Пацієнт")
    {
    }

    public Patient(string firstName, string lastName) : this(firstName, lastName, new DateTime(2000, 1, 1), BloodType.Unknown, "0000000000")
    {
    }

    public string GetAgeCategory()
    {
        if (Age < 18) return "дитина";
        if (Age < 60) return "дорослий";
        return "літній";
    }

    public override string ToString()
    {
        return $"[{Id}] {FullName} | Вік: {ClinicFormatter.FormatAge(Age)} ({GetAgeCategory()}) | Кров: {ClinicFormatter.FormatBloodType(BloodType)} | Тел: {ClinicFormatter.FormatPhone(Phone)}";
    }
    
    public Patient(string firstName, string lastName, DateTime dateOfBirth, BloodType bloodType, string phone)
    {
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        BloodType = bloodType;
        Phone = phone;
        Id = _nextId++;
    }
}