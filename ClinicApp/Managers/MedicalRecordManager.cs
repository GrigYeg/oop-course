using ClinicApp.Models;

namespace ClinicApp.Managers;

public class MedicalRecordManager
{
    private const int MaxRecords = 1000;
    private readonly MedicalRecord[] _records = new MedicalRecord[MaxRecords];
    private int _count = 0;

    public int Count => _count;

    public MedicalRecord? this[int index]
    {
        get
        {
            if (index < 0 || index >= _count)
            {
                return null;
            }
            return _records[index];
        }
    }

    public void Add(MedicalRecord record)
    {
        if (_count >= MaxRecords)
        {
            Console.WriteLine("Помилка: досягнуто ліміт медичних записів.");
            return;
        }

        _records[_count] = record;
        _count++;
        Console.WriteLine($"Запис [{record.Id}] {record.GetRecordType()} додано.");
    }

    public MedicalRecord? FindById(int id)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].Id == id)
            {
                return _records[i];
            }
        }
        return null;
    }

    public MedicalRecord[] GetByPatient(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId)
            {
                matchCount++;
            }
        }

        MedicalRecord[] result = new MedicalRecord[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId)
            {
                result[index++] = _records[i];
            }
        }
        return result;
    }

    public MedicalRecord[] GetByDoctor(int doctorId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].DoctorId == doctorId)
            {
                matchCount++;
            }
        }

        MedicalRecord[] result = new MedicalRecord[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].DoctorId == doctorId)
            {
                result[index++] = _records[i];
            }
        }
        return result;
    }

    public void DisplayAll()
    {
        if (_count == 0)
        {
            Console.WriteLine("Медичних записів немає.");
            return;
        }

        for (int i = 0; i < _count; i++)
        {
            Console.WriteLine(_records[i]);
        }
    }

    public void DisplayList(MedicalRecord[] records)
    {
        if (records.Length == 0)
        {
            Console.WriteLine("Записів не знайдено.");
            return;
        }

        for (int i = 0; i < records.Length; i++)
        {
            Console.WriteLine(records[i]);
        }
    }
    
    public Diagnosis[] GetDiagnoses(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Diagnosis)
            {
                matchCount++;
            }
        }

        Diagnosis[] result = new Diagnosis[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Diagnosis d)
            {
                result[index++] = d;
            }
        }
        return result;
    }

    public LabResult[] GetLabResults(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is LabResult)
            {
                matchCount++;
            }
        }

        LabResult[] result = new LabResult[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is LabResult lr)
            {
                result[index++] = lr;
            }
        }
        return result;
    }

    public Prescription[] GetPrescriptions(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Prescription)
            {
                matchCount++;
            }
        }

        Prescription[] result = new Prescription[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Prescription rx)
            {
                result[index++] = rx;
            }
        }
        return result;
    }

    public Diagnosis[] GetChronicDiagnoses(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Diagnosis d && d.IsChronic)
            {
                matchCount++;
            }
        }

        Diagnosis[] result = new Diagnosis[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Diagnosis d && d.IsChronic)
            {
                result[index++] = d;
            }
        }
        return result;
    }

    public Prescription[] GetActivePrescriptions(int patientId)
    {
        int matchCount = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Prescription rx && rx.IsActive())
            {
                matchCount++;
            }
        }

        Prescription[] result = new Prescription[matchCount];
        int index = 0;
        for (int i = 0; i < _count; i++)
        {
            if (_records[i].PatientId == patientId && _records[i] is Prescription rx && rx.IsActive())
            {
                result[index++] = rx;
            }
        }
        return result;
    }

    public void DisplayPatientSummary(int patientId)
    {
        MedicalRecord[] patientRecords = GetByPatient(patientId);
        if (patientRecords.Length == 0)
        {
            Console.WriteLine($"У пацієнта #{patientId} немає медичних записів.");
            return;
        }

        int diagnosesCount = 0;
        int labResultsCount = 0;
        int prescriptionsCount = 0;

        for (int i = 0; i < patientRecords.Length; i++)
        {
            if (patientRecords[i] is Diagnosis)
            {
                diagnosesCount++;
            }
            else if (patientRecords[i] is LabResult)
            {
                labResultsCount++;
            }
            else if (patientRecords[i] is Prescription)
            {
                prescriptionsCount++;
            }
        }

        Console.WriteLine($"=== Медична картка пацієнта #{patientId} ===");
        Console.WriteLine($"Всього записів: {patientRecords.Length} (діагнозів: {diagnosesCount}, аналізів: {labResultsCount}, рецептів: {prescriptionsCount})");

        Diagnosis[] chronicDiagnoses = GetChronicDiagnoses(patientId);
        if (chronicDiagnoses.Length > 0)
        {
            Console.WriteLine($"Хронічні діагнози ({chronicDiagnoses.Length}):");
            for (int i = 0; i < chronicDiagnoses.Length; i++)
            {
                Console.WriteLine($"  {chronicDiagnoses[i]}");
            }
        }

        Prescription[] activePrescriptions = GetActivePrescriptions(patientId);
        if (activePrescriptions.Length > 0)
        {
            Console.WriteLine($"Активні рецепти ({activePrescriptions.Length}):");
            for (int i = 0; i < activePrescriptions.Length; i++)
            {
                Console.WriteLine($"  {activePrescriptions[i]} | до {activePrescriptions[i].ExpiresAt:dd.MM.yyyy}");
            }
        }
    }
}