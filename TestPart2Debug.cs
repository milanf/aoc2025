using System;
using System.Text;
using AoC2025.Solutions;

class TestPart2Debug
{
    static void Main()
    {
        var day03 = new Day03();
        
        // First line from real input
        string firstLine = "7364324241225433445422232322233434224835321253334532333323166227675321322533332522422446233324232434";
        string bank15 = firstLine.Substring(0, 15);
        
        Console.WriteLine($"First 15 digits: {bank15}");
        Console.WriteLine($"Expected: remove 3 smallest or strategically to maximize");
        
        // Test with example line
        string exampleLine = "987654321111111";
        Console.WriteLine($"\nExample line: {exampleLine}");
        
        // Use reflection to call private method
        var method = typeof(Day03).GetMethod("FindMaxJoltageWithTwelveBatteries", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (method != null)
        {
            var result1 = (long)method.Invoke(day03, new object[] { bank15 });
            Console.WriteLine($"Max joltage for first line (first 15): {result1}");
            
            var result2 = (long)method.Invoke(day03, new object[] { exampleLine });
            Console.WriteLine($"Max joltage for example: {result2} (expected: 987654321111)");
        }
    }
}
