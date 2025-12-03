using System;
using System.Linq;

class QuickTest
{
    static void Main()
    {
        // Test the logic manually
        string bank = "736432424122543";
        
        Console.WriteLine($"Bank: {bank}");
        Console.WriteLine($"We need to remove 3 positions to keep 12 digits");
        Console.WriteLine($"Goal: maximize the resulting 12-digit number");
        Console.WriteLine();
        
        // Generate all combinations manually and find max
        long maxValue = 0;
        int[] bestPositions = new int[3];
        
        int count = 0;
        for (int i = 0; i < 15; i++)
        {
            for (int j = i + 1; j < 15; j++)
            {
                for (int k = j + 1; k < 15; k++)
                {
                    count++;
                    var result = new System.Text.StringBuilder();
                    for (int pos = 0; pos < 15; pos++)
                    {
                        if (pos != i && pos != j && pos != k)
                        {
                            result.Append(bank[pos]);
                        }
                    }
                    long value = long.Parse(result.ToString());
                    if (value > maxValue)
                    {
                        maxValue = value;
                        bestPositions[0] = i;
                        bestPositions[1] = j;
                        bestPositions[2] = k;
                    }
                }
            }
        }
        
        Console.WriteLine($"Total combinations checked: {count}");
        Console.WriteLine($"Max value: {maxValue}");
        Console.WriteLine($"Remove positions: {bestPositions[0]}, {bestPositions[1]}, {bestPositions[2]}");
        
        // Show what was removed
        Console.Write("Removed digits: ");
        for (int i = 0; i < 3; i++)
        {
            Console.Write($"{bank[bestPositions[i]]} ");
        }
        Console.WriteLine();
        
        // Show the result
        var finalResult = new System.Text.StringBuilder();
        for (int pos = 0; pos < 15; pos++)
        {
            if (pos != bestPositions[0] && pos != bestPositions[1] && pos != bestPositions[2])
            {
                finalResult.Append(bank[pos]);
            }
        }
        Console.WriteLine($"Final number: {finalResult}");
    }
}
