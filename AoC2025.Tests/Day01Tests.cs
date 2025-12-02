using AoC2025.Solutions;

namespace AoC2025.Tests;

/// <summary>
/// Unit testy pro Day 1 - validace pomocí example inputů z AoC zadání.
/// </summary>
public class Day01Tests
{
    private readonly Day01 _solution;

    public Day01Tests()
    {
        _solution = new Day01();
    }

    [Fact]
    public void Part1_ExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = File.ReadAllText(Path.Combine("TestData", "day01_example.txt"));

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("3", result);
    }

    [Fact]
    public void Part2_ExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = File.ReadAllText(Path.Combine("TestData", "day01_example.txt"));

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.Equal("6", result);
    }
}

