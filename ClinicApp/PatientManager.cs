using System;

namespace ClinicApp;

public class PatientManager
{
    private const int MaxPatients = 100;
    private Patient[] _patients = new Patient[MaxPatients];
    private int _count = 0;

    public int Count => _count;

    public void Add(Patient patient)
    {
        if (_count >= MaxPatients)
        {
            Console.WriteLine("Ліміт пацієнтів досягнуто.");
            return;
        }
        
        _patients[_count] = patient;
        _count++;
        Console.WriteLine($"Пацієнта [{patient.Id}] {patient.FullName} додано.");
    }

    public Patient? FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].Id == id)
            {
                return _patients[i];
            }
        }
        return null;
    }

    public Patient[] FindByName(string query)
    {
        string lowerQuery = query.ToLower();
        int matchCount = 0;

        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].FirstName.ToLower().Contains(lowerQuery) || 
                _patients[i].LastName.ToLower().Contains(lowerQuery))
            {
                matchCount++;
            }
        }

        Patient[] results = new Patient[matchCount];
        int resultIndex = 0;
        
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].FirstName.ToLower().Contains(lowerQuery) || 
                _patients[i].LastName.ToLower().Contains(lowerQuery))
            {
                results[resultIndex] = _patients[i];
                resultIndex++;
            }
        }

        return results;
    }

    public bool Remove(int id)
    {
        int indexToRemove = -1;
        for (int i = 0; i < _count; i++)
        {
            if (_patients[i].Id == id)
            {
                indexToRemove = i;
                break;
            }
        }

        if (indexToRemove == -1) return false;

        for (int i = indexToRemove; i < _count - 1; i++)
        {
            _patients[i] = _patients[i + 1];
        }

        _patients[_count - 1] = null!;
        _count--;
        return true;
    }

    public void DisplayAll()
    {
        if (_count == 0)
        {
            Console.WriteLine("Порожній список пацієнтів.");
            return;
        }

        Console.WriteLine($"\n=== Пацієнти ({_count} / {MaxPatients}) ===");
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine(_patients[i].ToString());
        }
        Console.WriteLine(new string('=', 30));
    }

    public void DisplayStats()
    {
        if (_count == 0)
        {
            Console.WriteLine("Немає даних для статистики.");
            return;
        }

        int totalAge = 0;
        int minIdx = 0;
        int maxIdx = 0;
        int adultCount = 0;

        for (int i = 0; i < _count; i++)
        {
            int currentAge = _patients[i].Age;
            totalAge += currentAge;

            if (currentAge < _patients[minIdx].Age) minIdx = i;
            if (currentAge > _patients[maxIdx].Age) maxIdx = i;
            
            if (_patients[i].IsAdult) adultCount++;
        }

        double avgAge = (double)totalAge / _count;

        Console.WriteLine("\n=== Статистика пацієнтів ===");
        Console.WriteLine($"Всього:      {_count}");
        Console.WriteLine($"Середній вік: {avgAge:F1} р.");
        Console.WriteLine($"Наймолодший: {_patients[minIdx].FullName} ({_patients[minIdx].Age} р.)");
        Console.WriteLine($"Найстарший:  {_patients[maxIdx].FullName} ({_patients[maxIdx].Age} р.)");
        Console.WriteLine($"Дорослих:    {adultCount} з {_count}");
        Console.WriteLine(new string('=', 30));
    }
}