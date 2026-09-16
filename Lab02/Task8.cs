using System;

public static class Task8
{
    public static void Run()
    {
        int d = int.Parse(Console.ReadLine()!);
        int w = int.Parse(Console.ReadLine()!);

        int[,,] data = new int[d, w, 2];
        int[] totals = new int[d];

        for (int dept = 0; dept < d; dept++)
        {
            for (int week = 0; week < w; week++)
            {
                for (int shift = 0; shift < 2; shift++)
                {
                    data[dept, week, shift] = int.Parse(Console.ReadLine()!);
                }
            }
        }

        int maxTotal = -1;
        int busiestDeptIdx = 0;

        for (int dept = 0; dept < d; dept++)
        {
            Console.WriteLine($"Відділення {dept + 1}:");
            int deptTotal = 0;

            for (int week = 0; week < w; week++)
            {
                int morning = data[dept, week, 0];
                int evening = data[dept, week, 1];
                int weekTotal = morning + evening;
                deptTotal += weekTotal;

                Console.WriteLine($"  Тиждень {week + 1}: ранок {morning}, вечір {evening} -> разом {weekTotal}");
            }

            totals[dept] = deptTotal;
            Console.WriteLine($"  Разом: {deptTotal} пацієнтів");

            if (deptTotal > maxTotal)
            {
                maxTotal = deptTotal;
                busiestDeptIdx = dept;
            }
        }

        Console.WriteLine($"Найзавантаженіше: Відділення {busiestDeptIdx + 1} ({maxTotal} пацієнтів)");
    }
}