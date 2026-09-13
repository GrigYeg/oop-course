using System;

public static class Task1
{
    public static void Run()
    {
        double weight = double.Parse(Console.ReadLine()!);
        double height = double.Parse(Console.ReadLine()!);

        double bmi = weight / (height * height);

        Console.WriteLine($"IMT: {bmi:F2}");
    }
}