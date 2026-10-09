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

    private DateTime _dateOfBirth;
    public DateTime DateOfBirth
    {
        get => _dateOfBirth;
        set
        {
            ClinicValidator.ValidateDate(value, nameof(DateOfBirth));
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
            ClinicValidator.ValidatePhone(value);
            _phone = value;
        }
    }

    private string _email = "";
    public string Email
    {
        get => _email;
        set
        {
            ClinicValidator.ValidateEmail(value);
            _email = value;
        }
    }

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