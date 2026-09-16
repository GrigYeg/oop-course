using System;

public static class Task5
{
    public static void Run()
    {
        int n = int.Parse(Console.ReadLine()!);
        int[,] matrix = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            string[] parts = Console.ReadLine()!.Split(' ');
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = int.Parse(parts[j]);
            }
        }

        string[] mainDiag = new string[n];
        string[] secDiag = new string[n];
        int mainSum = 0;
        int secSum = 0;

        for (int i = 0; i < n; i++)
        {
            int mainVal = matrix[i, i];
            int secVal = matrix[i, n - 1 - i];

            mainDiag[i] = mainVal.ToString();
            secDiag[i] = secVal.ToString();

            mainSum += mainVal;
            secSum += secVal;
        }

        Console.WriteLine($"Головна діагональ: {string.Join(", ", mainDiag)} (сума = {mainSum})");
        Console.WriteLine($"Побічна діагональ: {string.Join(", ", secDiag)} (сума = {secSum})");
    }
}