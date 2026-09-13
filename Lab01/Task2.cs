using System;

public static class Task2
{
    public static void Run()
    {
        double price = double.Parse(Console.ReadLine()!);
        int visits = int.Parse(Console.ReadLine()!);
        int discount = int.Parse(Console.ReadLine()!);
        
        double total = price * visits * (1 - discount / 100.0);
        
        Console.WriteLine($"Сума: {total:F2} грн");
    }
}