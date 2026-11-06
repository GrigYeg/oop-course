using System;
using System.Collections.Generic;
using ClinicApp.Models;
using ClinicApp.Enums;

namespace ClinicApp.Managers;

public class PatientManager
{
    private List<Patient> _patients = new List<Patient>();

    public int Count => _patients.Count;

    public void Add(Patient patient)
    {
        _patients.Add(patient);
        Console.WriteLine($"Пацієнта [{patient.Id}] {patient.FullName} додано.");
    }

    public Patient? FindById(int id)
    {
        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].Id == id)
            {
                return _patients[i];
            }
        }
        return null;
    }

    public bool TryFindById(int id, out Patient? patient)
    {
        patient = FindById(id);
        return patient != null;
    }

    public Patient[] FindByName(string query)
    {
        string lowerQuery = query.ToLower();
        List<Patient> results = new List<Patient>();

        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].FirstName.ToLower().Contains(lowerQuery) ||
                _patients[i].LastName.ToLower().Contains(lowerQuery))
            {
                results.Add(_patients[i]);
            }
        }

        return results.ToArray();
    }

    public Patient[] FindByBloodType(BloodType bloodType)
    {
        List<Patient> results = new List<Patient>();

        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].BloodType == bloodType)
            {
                results.Add(_patients[i]);
            }
        }

        return results.ToArray();
    }

    public bool Remove(int id)
    {
        for (int i = 0; i < _patients.Count; i++)
        {
            if (_patients[i].Id == id)
            {
                _patients.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public void DisplayAll()
    {
        if (_patients.Count == 0)
        {
            Console.WriteLine("Порожній список пацієнтів.");
            return;
        }

        Console.WriteLine($"\n=== Пацієнти ({_patients.Count}) ===");
        for (int i = 0; i < _patients.Count; i++)
        {
            Console.WriteLine(_patients[i].ToString());
        }
        Console.WriteLine(new string('=', 30));
    }

    public void DisplayStats()
    {
        if (_patients.Count == 0)
        {
            Console.WriteLine("Немає даних для статистики.");
            return;
        }

        int totalAge = 0;
        int minIdx = 0;
        int maxIdx = 0;
        int adultCount = 0;

        for (int i = 0; i < _patients.Count; i++)
        {
            int currentAge = _patients[i].Age;
            totalAge += currentAge;

            if (currentAge < _patients[minIdx].Age) minIdx = i;
            if (currentAge > _patients[maxIdx].Age) maxIdx = i;

            if (_patients[i].IsAdult) adultCount++;
        }

        double avgAge = (double)totalAge / _patients.Count;

        Console.WriteLine("\n=== Статистика пацієнтів ===");
        Console.WriteLine($"Всього:       {_patients.Count}");
        Console.WriteLine($"Середній вік: {avgAge:F1} р.");
        Console.WriteLine($"Наймолодший:  {_patients[minIdx].FullName} ({_patients[minIdx].Age} р.)");
        Console.WriteLine($"Найстарший:   {_patients[maxIdx].FullName} ({_patients[maxIdx].Age} р.)");
        Console.WriteLine($"Дорослих:     {adultCount} з {_patients.Count}");
        Console.WriteLine(new string('=', 30));
    }

    public Patient[] GetAll()
    {
        return _patients.ToArray();
    }

    public Patient? this[int index]
    {
        get
        {
            if (index < 0 || index >= _patients.Count) return null;
            return _patients[index];
        }
    }
}