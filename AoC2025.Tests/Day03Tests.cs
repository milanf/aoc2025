using AoC2025.Solutions;

namespace AoC2025.Tests;

/// <summary>
/// Unit testy pro Day 3 - validace pomocí example inputů z AoC zadání.
/// </summary>
public class Day03Tests
{
    private readonly Day03 _solution;

    public Day03Tests()
    {
        _solution = new Day03();
    }
        [Fact]
        public void Part2_ExampleInput_ReturnsExpectedResult()
        {
            // Arrange
            var input = File.ReadAllText(Path.Combine("TestData", "day03_example.txt"));

            // Act
            var result = _solution.SolvePart2(input);

            // Assert
            Assert.Equal("3121910778619", result);
        }

    [Fact]
    public void Part1_ExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = File.ReadAllText(Path.Combine("TestData", "day03_example.txt"));

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("357", result);
    }
    
    [Fact]
    public void Part2_SingleLine_FirstRealInput()
    {
        // Arrange - first line from real input (first 15 digits)
        var input = "736432424122543";

        // Act
        var result = _solution.SolvePart2(input);

        // Assert - just log to see what we get
        Assert.NotEqual("0", result);
    }
    
    [Fact]
    public void Part2_SingleLine_ExampleFromSpec()
    {
        // Arrange - example from spec
        var input = "987654321111111";

        // Act
        var result = _solution.SolvePart2(input);

        // Assert - should be 987654321111
        Assert.Equal("987654321111", result);
    }
}
