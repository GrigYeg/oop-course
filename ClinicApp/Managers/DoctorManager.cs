using ClinicApp.Models;
using ClinicApp.Enums;
using ClinicApp.Utils;

namespace ClinicApp.Managers;

public class DoctorManager
{
    private const int MaxDoctors = 50;
    private Doctor[] _doctors = new Doctor[MaxDoctors];
    private int _count = 0;

    public int Count => _count;

    public void Add(Doctor doctor)
    {
        if (_count >= MaxDoctors)
        {
            Console.WriteLine("Ліміт лікарів досягнуто.");
            return;
        }

        _doctors[_count] = doctor;
        _count++;
    }

    public Doctor? FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Id == id)
            {
                return _doctors[i];
            }
        }
        return null;
    }

    public Doctor[] FindBySpeciality(Speciality speciality)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Speciality == speciality) matchCount++;
        }

        Doctor[] results = new Doctor[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Speciality == speciality) results[index++] = _doctors[i];
        }
        return results;
    }
    
    public Doctor[] FindBySpeciality(string query)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (ClinicFormatter.FormatSpeciality(_doctors[i].Speciality).Contains(query, StringComparison.OrdinalIgnoreCase)) matchCount++;
        }

        Doctor[] results = new Doctor[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (ClinicFormatter.FormatSpeciality(_doctors[i].Speciality).Contains(query, StringComparison.OrdinalIgnoreCase)) results[index++] = _doctors[i];
        }
        return results;
    }
    
    public bool TryFindById(int id, out Doctor? doctor)
    {
        doctor = FindById(id);
        return doctor != null;
    }

    public Doctor[] GetAll()
    {
        Doctor[] copy = new Doctor[_count];
        Array.Copy(_doctors, copy, _count);
        return copy;
    }

    public bool Remove(int id)
    {
        int indexToRemove = -1;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].Id == id)
            {
                indexToRemove = i;
                break;
            }
        }

        if (indexToRemove == -1) return false;

        for (int i = indexToRemove; i < _count - 1; i++)
        {
            _doctors[i] = _doctors[i + 1];
        }

        _doctors[_count - 1] = null!;
        _count--;
        return true;
    }

    public void DisplayAll()
    {
        if (_count == 0)
        {
            Console.WriteLine("Порожній список лікарів.");
            return;
        }

        Console.WriteLine($"\n=== Лікарі ({_count} / {MaxDoctors}) ===");
        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine(_doctors[i].ToString());
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

        int availableNow = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_doctors[i].IsAvailableNow)
            {
                availableNow++;
            }
        }

        Console.WriteLine("\n=== Статистика лікарів ===");
        Console.WriteLine($"Всього:         {_count}");
        Console.WriteLine($"Доступні зараз: {availableNow}");
        Console.WriteLine("По спеціальностях:");

        for (int i = 0; i < _count; i++)
        {
            bool isDuplicate = false;
            for (int j = 0; j < i; j++)
            {
                if (_doctors[i].Speciality == _doctors[j].Speciality)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                int specCount = 0;
                for (int k = 0; k < _count; k++)
                {
                    if (_doctors[k].Speciality == _doctors[i].Speciality)
                    {
                        specCount++;
                    }
                }
                Console.WriteLine($"  {_doctors[i].Speciality}: {specCount}");
            }
        }
        Console.WriteLine(new string('=', 30));
    }
    
    public Doctor? this[int index]
    {
        get
        {
            if (index < 0 || index >= _count) return null;
            return _doctors[index];
        }
    }
}