using AoC2025.Solutions;

namespace AoC2025.Tests;

/// <summary>
/// Unit testy pro Day 2 - Gift Shop - validace detekce nevalidních product IDs.
/// </summary>
public class Day02Tests
{
    private readonly Day02 _solution;

    public Day02Tests()
    {
        _solution = new Day02();
    }

    [Fact]
    public void Part1_ExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = File.ReadAllText(Path.Combine("TestData", "day02_example.txt"));

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1227775554", result);
    }

    [Fact]
    public void Part2_WithExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = File.ReadAllText(Path.Combine("TestData", "day02_example.txt"));

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.Equal("4174379265", result);
    }

    [Theory]
    [InlineData(11, true)]
    [InlineData(22, true)]
    [InlineData(55, true)]
    [InlineData(99, true)]
    [InlineData(1010, true)]
    [InlineData(6464, true)]
    [InlineData(123123, true)]
    [InlineData(1188511885, true)]
    [InlineData(222222, true)]
    [InlineData(446446, true)]
    [InlineData(38593859, true)]
    [InlineData(101, false)]
    [InlineData(1234, false)]
    [InlineData(12345, false)]
    [InlineData(100, false)]
    public void IsInvalidId_WithVariousNumbers_ReturnsExpectedResult(long number, bool expected)
    {
        // Use reflection to call private method for testing
        var method = typeof(Day02).GetMethod("IsInvalidId", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Act
        var result = (bool)method!.Invoke(_solution, new object[] { number })!;

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(55, true)]
    [InlineData(555, true)]
    [InlineData(1111111, true)]
    [InlineData(12341234, true)]
    [InlineData(123123123, true)]
    [InlineData(1212121212, true)]
    [InlineData(565656, true)]
    [InlineData(824824824, true)]
    [InlineData(2121212121, true)]
    [InlineData(99, true)]
    [InlineData(111, true)]
    [InlineData(999, true)]
    [InlineData(1010, true)]
    [InlineData(1234, false)]
    [InlineData(101, false)]
    [InlineData(1, false)]
    [InlineData(12, false)]
    public void IsInvalidIdPart2_DetectsPatterns(long number, bool expected)
    {
        // Use reflection to call private method for testing
        var method = typeof(Day02).GetMethod("IsInvalidIdPart2", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        // Act
        var result = (bool)method!.Invoke(_solution, new object[] { number })!;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Part1_StillWorksAfterPart2Implementation()
    {
        // Arrange
        var input = File.ReadAllText(Path.Combine("TestData", "day02_example.txt"));

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1227775554", result);
    }
}
