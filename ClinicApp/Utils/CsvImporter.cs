using System;
using System.Globalization;
using System.IO;
using System.Text;
using ClinicApp.Enums;
using ClinicApp.Models;

namespace ClinicApp.Utils;

public static class CsvImporter
{
    public static ImportResult ImportPatients(Clinic clinic, string filePath)
    {
        ImportResult result = new ImportResult();

        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            result.AddError(0, $"Файл не знайдено: {filePath}");
            return result;
        }

        string[] lines;
        try
        {
            lines = File.ReadAllLines(filePath, Encoding.UTF8);
        }
        catch (Exception ex)
        {
            result.AddError(0, $"Не вдалося прочитати файл: {ex.Message}");
            return result;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int lineNumber = i + 1;

            try
            {
                string[] parts = line.Split(',');
                if (parts.Length != 5)
                {
                    result.AddError(lineNumber, $"Рядок має {parts.Length} {GetFieldWord(parts.Length)} замість 5.");
                    continue;
                }

                string firstName = parts[0].Trim();
                string lastName = parts[1].Trim();
                string birthDateStr = parts[2].Trim();
                string bloodTypeStr = parts[3].Trim();
                string phone = parts[4].Trim();

                if (string.IsNullOrWhiteSpace(firstName))
                {
                    result.AddError(lineNumber, "Ім'я не може бути порожнім.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(lastName))
                {
                    result.AddError(lineNumber, "Прізвище не може бути порожнім.");
                    continue;
                }

                DateTime dateOfBirth;
                try
                {
                    dateOfBirth = DateTime.ParseExact(birthDateStr, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    result.AddError(lineNumber, $"Некоректна дата народження '{birthDateStr}' (очікується dd.MM.yyyy).");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(bloodTypeStr) || int.TryParse(bloodTypeStr, out _))
                {
                    result.AddError(lineNumber, $"Некоректна група крові '{bloodTypeStr}'.");
                    continue;
                }

                BloodType bloodType;
                try
                {
                    bloodType = (BloodType)Enum.Parse(typeof(BloodType), bloodTypeStr, true);
                    if (!Enum.IsDefined(typeof(BloodType), bloodType))
                    {
                        result.AddError(lineNumber, $"Невідома група крові '{bloodTypeStr}'.");
                        continue;
                    }
                }
                catch (ArgumentException)
                {
                    result.AddError(lineNumber, $"Невідома група крові '{bloodTypeStr}'.");
                    continue;
                }

                Patient patient = new Patient(firstName, lastName, dateOfBirth, bloodType, phone);
                clinic.Patients.Add(patient);
                result.AddSuccess();
            }
            catch (Exception ex)
            {
                result.AddError(lineNumber, ex.Message);
            }
        }

        return result;
    }

    private static string GetFieldWord(int count)
    {
        int mod100 = count % 100;
        int mod10 = count % 10;

        if (mod100 >= 11 && mod100 <= 14)
        {
            return "полів";
        }
        if (mod10 == 1)
        {
            return "поле";
        }
        if (mod10 >= 2 && mod10 <= 4)
        {
            return "поля";
        }
        return "полів";
    }
}