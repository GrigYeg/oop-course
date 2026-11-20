using System;
using System.Globalization;
using System.IO;
using System.Text;
using ClinicApp.Enums;
using ClinicApp.Models;

namespace ClinicApp.Utils;

public class SessionManager
{
    private readonly string _sessionPath;

    public SessionManager(string sessionPath = "session.dat")
    {
        _sessionPath = sessionPath;
    }

    public bool Exists()
    {
        return File.Exists(_sessionPath);
    }

    public void Save(Clinic clinic)
    {
        using StreamWriter writer = new StreamWriter(_sessionPath, false, Encoding.UTF8);
        writer.WriteLine("[PATIENTS]");

        foreach (Patient p in clinic.Patients.GetAll())
        {
            writer.WriteLine($"{p.FirstName},{p.LastName},{p.DateOfBirth:dd.MM.yyyy},{p.BloodType},{p.Phone}");
        }

        writer.WriteLine("[END]");
    }

    public int Load(Clinic clinic)
    {
        if (!File.Exists(_sessionPath))
        {
            return 0;
        }

        string[] lines = File.ReadAllLines(_sessionPath, Encoding.UTF8);
        string currentSection = "";
        int addedCount = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            if (line.StartsWith("["))
            {
                if (line == "[END]")
                {
                    break;
                }

                currentSection = line;
                continue;
            }

            if (currentSection == "[PATIENTS]")
            {
                try
                {
                    string[] parts = line.Split(',');
                    if (parts.Length != 5)
                    {
                        continue;
                    }

                    string firstName = parts[0].Trim();
                    string lastName = parts[1].Trim();
                    DateTime dateOfBirth = DateTime.ParseExact(parts[2].Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture);

                    string bloodTypeStr = parts[3].Trim();
                    if (string.IsNullOrWhiteSpace(bloodTypeStr) || int.TryParse(bloodTypeStr, out _))
                    {
                        continue;
                    }

                    BloodType bloodType = (BloodType)Enum.Parse(typeof(BloodType), bloodTypeStr, true);
                    if (!Enum.IsDefined(typeof(BloodType), bloodType))
                    {
                        continue;
                    }

                    string phone = parts[4].Trim();

                    if (PatientExists(clinic, firstName, lastName, dateOfBirth))
                    {
                        continue;
                    }

                    Patient patient = new Patient(firstName, lastName, dateOfBirth, bloodType, phone);
                    clinic.Patients.Add(patient);
                    addedCount++;
                }
                catch (Exception)
                {
                }
            }
        }

        return addedCount;
    }

    private bool PatientExists(Clinic clinic, string firstName, string lastName, DateTime dateOfBirth)
    {
        foreach (Patient existing in clinic.Patients.GetAll())
        {
            if (string.Equals(existing.FirstName, firstName, StringComparison.OrdinalIgnoreCase)
                && string.Equals(existing.LastName, lastName, StringComparison.OrdinalIgnoreCase)
                && existing.DateOfBirth.Date == dateOfBirth.Date)
            {
                return true;
            }
        }

        return false;
    }
}