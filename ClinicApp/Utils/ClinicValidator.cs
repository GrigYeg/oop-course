using System.Text.RegularExpressions;

namespace ClinicApp.Utils;

public static class ClinicValidator
{
    private static readonly Regex PhoneRegex = new Regex(@"^[0-9]{10}\z", RegexOptions.Compiled);
    private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+\z", RegexOptions.Compiled);

    public static void ValidateName(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
        {
            throw new ArgumentException("Поле не може бути порожнім і має бути <= 50 символів", fieldName);
        }
    }

    public static void ValidatePhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || !PhoneRegex.IsMatch(phone))
        {
            throw new ArgumentException("Номер телефону має містити рівно 10 цифр", nameof(phone));
        }
    }

    public static void ValidateEmail(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return;
        }

        if (!EmailRegex.IsMatch(email))
        {
            throw new ArgumentException("Некоректний формат email", nameof(email));
        }
    }

    public static void ValidateDate(DateTime value, string fieldName)
    {
        if (value > DateTime.Today || value.Year < 1900)
        {
            throw new ArgumentOutOfRangeException(fieldName, "Дата не може бути пізнішою за сьогодні та ранішою за 1900 рік");
        }
    }

    public static void ValidatePositive(int value, string fieldName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(fieldName, "Значення має бути більшим за 0");
        }
    }
}