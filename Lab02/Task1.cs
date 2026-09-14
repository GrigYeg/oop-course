using System;

public static class Task1
{
    public static void Run()
    {
        int n = int.Parse(Console.ReadLine()!);
        double[] weights = new double[n];

        for (int i = 0; i < n; i++)
        {
            weights[i] = double.Parse(Console.ReadLine()!);
        }

        double sum = 0;
        double min = weights[0];
        double max = weights[0];

        foreach (double weight in weights)
        {
            sum += weight;
            if (weight < min) min = weight;
            if (weight > max) max = weight;
        }

        double average = sum / n;

        int aboveAvgCount = 0;
        for (int i = 0; i < n; i++)
        {
            if (weights[i] > average)
            {
                aboveAvgCount++;
            }
        }

        Console.WriteLine($"Кількість: {n} / Середня вага: {average:F1} кг / Мін / Макс: {min:F1} / {max:F1} кг / Вище середнього: {aboveAvgCount} з {n}");
    }
}