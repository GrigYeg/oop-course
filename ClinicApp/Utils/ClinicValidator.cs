namespace ClinicApp.Utils;

public static class ClinicValidator
{
    public static void ValidateName(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 50)
        {
            throw new ArgumentException("Поле не може бути порожнім і має бути <= 50 символів", fieldName);
        }
    }

    public static void ValidatePhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone) || phone.Length != 10)
        {
            throw new ArgumentException("Номер телефону має містити рівно 10 символів", nameof(phone));
        }

        foreach (char c in phone)
        {
            if (!char.IsDigit(c))
            {
                throw new ArgumentException("Номер телефону має містити лише цифри", nameof(phone));
            }
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