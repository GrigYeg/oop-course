using System;

public static class Task4
{
    public static void Run()
    {
        int n = int.Parse(Console.ReadLine()!);
        int m = int.Parse(Console.ReadLine()!);

        int[,] matrix = new int[n, m];

        int maxVal = -1;
        int maxRow = 0;
        int maxCol = 0;

        for (int i = 0; i < n; i++)
        {
            string[] parts = Console.ReadLine()!.Split(' ');
            for (int j = 0; j < m; j++)
            {
                matrix[i, j] = int.Parse(parts[j]);
                
                if (matrix[i, j] > maxVal)
                {
                    maxVal = matrix[i, j];
                    maxRow = i;
                    maxCol = j;
                }
            }
        }

        for (int i = 0; i < n; i++)
        {
            int rowSum = 0;
            for (int j = 0; j < m; j++)
            {
                rowSum += matrix[i, j];
            }
            Console.WriteLine($"Лікар {i + 1}: {rowSum} прийомів");
        }

        string[] colSums = new string[m];
        for (int j = 0; j < m; j++)
        {
            int colSum = 0;
            for (int i = 0; i < n; i++)
            {
                colSum += matrix[i, j];
            }
            colSums[j] = colSum.ToString();
        }
        
        Console.WriteLine($"По днях: {string.Join(", ", colSums)}");
        Console.WriteLine($"Максимум: {maxVal} (Лікар {maxRow + 1}, День {maxCol + 1})");
    }
}