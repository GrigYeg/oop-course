using System;

public static class Task6
{
    public static void Run()
    {
        int cardNumber = int.Parse(Console.ReadLine()!);
        
        int lastDigit = cardNumber % 10;
        string department = lastDigit switch
        {
            0 or 1 => "загальна терапія",
            2 or 3 => "хірургія",
            4 or 5 => "кардіологія",
            6 or 7 => "неврологія",
            8 or 9 => "офтальмологія",
            _ => "невідомо"
        };

        string isDiscount = (cardNumber % 2 == 0) ? "так" : "ні";
        string isCheckup = (cardNumber % 3 == 0) ? "так" : "ні";

        Console.WriteLine($"Відділення: {department}");
        Console.WriteLine($"Пільгова:   {isDiscount}");
        Console.WriteLine($"Огляд:      {isCheckup}");
    }
}