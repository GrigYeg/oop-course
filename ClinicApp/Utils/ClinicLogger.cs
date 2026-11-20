using System;
using System.IO;
using System.Text;

namespace ClinicApp.Utils;

public class ClinicLogger
{
    private readonly string _logPath;

    public ClinicLogger(string logPath = "clinic.log")
    {
        _logPath = logPath;
    }

    public void LogInfo(string message)
    {
        Write("INFO", message);
    }

    public void LogWarning(string message)
    {
        Write("WARN", message);
    }

    public void LogError(string message)
    {
        Write("ERROR", message);
    }

    public string[] GetLastLines(int n)
    {
        if (!File.Exists(_logPath) || n <= 0)
        {
            return Array.Empty<string>();
        }

        string[] allLines = File.ReadAllLines(_logPath, Encoding.UTF8);
        int skip = allLines.Length - n;
        if (skip < 0)
        {
            skip = 0;
        }

        int count = allLines.Length - skip;
        string[] result = new string[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = allLines[skip + i];
        }

        return result;
    }

    public void Clear()
    {
        if (File.Exists(_logPath))
        {
            File.Delete(_logPath);
        }
    }

    public bool Exists()
    {
        return File.Exists(_logPath);
    }

    private void Write(string level, string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string paddedLevel = level.PadRight(5);
        string line = $"[{timestamp}] [{paddedLevel}] {message}{Environment.NewLine}";
        File.AppendAllText(_logPath, line, Encoding.UTF8);
    }
}