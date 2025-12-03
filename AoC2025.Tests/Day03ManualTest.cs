using AoC2025.Solutions;
using Xunit.Abstractions;

namespace AoC2025.Tests;

public class Day03ManualTest
{
    private readonly ITestOutputHelper _output;
    private readonly Day03 _solution;

    public Day03ManualTest(ITestOutputHelper output)
    {
        _output = output;
        _solution = new Day03();
    }

    [Fact]
    public void DebugFirstLine()
    {
        // First line from real input: 736432424122543
        string bank = "736432424122543";
        
        _output.WriteLine($"Bank: {bank}");
        _output.WriteLine($"Length: {bank.Length}");
        
        // Manually check: to maximize, we want to keep the largest digits
        // Keep 12 out of 15 = remove 3
        // Optimal: remove smallest/leftmost bad digits
        
        // Let's see what the method returns
        var method = typeof(Day03).GetMethod("FindMaxJoltageWithTwelveBatteries", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (method != null)
        {
            var result = (long)method.Invoke(_solution, new object[] { bank });
            _output.WriteLine($"Result: {result}");
            
            // Manual check: 736432424122543
            // To maximize: we want 7,7,6,5,4,4,4,3,3,3,3,2,2,2,2,2,1
            // We keep 12, so remove 3 smallest or strategic positions
            // Greedy: Remove positions that don't hurt the leftmost large digits
            // Expected: Remove last 3 digits (543) -> 736432424122
            // Or remove 1,2,2 from positions to keep 7,7,6,5,4,4,4,3,3,3,3,2 = 776544433332
            
            _output.WriteLine($"Expected approximately: 776544433332 or similar");
        }
    }
}
