using AoC2025.Solutions;
using Xunit.Abstractions;

namespace AoC2025.Tests;

public class Day07Tests
{
    private readonly Day07 _solution;
    private readonly ITestOutputHelper _output;

    public Day07Tests(ITestOutputHelper output)
    {
        _solution = new Day07();
        _output = output;
    }

    [Fact]
    public void Part1_WithExampleInput_Returns21()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day07_example.txt");

        // Act
        var result = _solution.SolvePart1(input);
        
        // Output for debugging
        _output.WriteLine($"Result: {result}");

        // Assert
        Assert.Equal("21", result);
    }

    [Fact]
    public void Part1_WithRealInput_ReturnsNonZero()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day07_example.txt");

        // Act
        var result = _solution.SolvePart1(input);
        
        // Output for debugging
        _output.WriteLine($"Result: {result}");

        // Assert - Only verify it runs without error and returns a non-zero result
        // Each participant has different input, so we can't assert specific value
        Assert.NotNull(result);
        Assert.NotEqual("0", result);
        int.TryParse(result, out int numericResult);
        Assert.True(numericResult > 0, "Result should be a positive number");
    }

    [Fact]
    public void Part1_SimpleSplit_CountsCorrectly()
    {
        // Arrange - Simple grid with 1 split
        var input = @"...S...
.......
...^...
.......";

        // Act
        var result = _solution.SolvePart1(input);
        
        // Output for debugging
        _output.WriteLine($"Simple split result: {result}");

        // Assert - One beam hits one splitter = 1 split
        Assert.Equal("1", result);
    }

    [Fact]
    public void Part1_TwoSplitsInRow_CountsCorrectly()
    {
        // Arrange - Two splitters in a row, beam goes between them
        var input = @"...S...
.......
..^.^..
.......";

        // Act
        var result = _solution.SolvePart1(input);
        
        // Output for debugging
        _output.WriteLine($"Two splits result: {result}");

        // Assert - Beam goes down between splitters, no split happens
        Assert.Equal("0", result);
    }

    [Fact]
    public void Part1_BeamGoesOutOfBounds_NoError()
    {
        // Arrange - Beam exits left side
        var input = @"S......
.......
^......
.......";

        // Act
        var result = _solution.SolvePart1(input);
        
        // Output for debugging
        _output.WriteLine($"Out of bounds result: {result}");

        // Assert - 1 split, left beam goes out, right beam continues
        Assert.Equal("1", result);
    }

    [Fact]
    public void Part2_WithExampleInput_Returns40()
    {
        // Arrange
        var input = @".......S.......
...............
.......^.......
...............
......^.^......
...............
.....^.^.^.....
...............
....^.^...^....
...............
...^.^...^.^...
...............
..^...^.....^..
...............
.^.^.^.^.^...^.
...............";

        // Act
        var result = _solution.SolvePart2(input);
        
        // Output for debugging
        _output.WriteLine($"Part2 Example Result: {result}");

        // Assert
        Assert.Equal("40", result);
    }

    [Fact]
    public void Part2_SinglePath_NoSplitters_Returns1()
    {
        // Arrange
        var input = @"S
.
.
.";

        // Act
        var result = _solution.SolvePart2(input);
        
        // Output for debugging
        _output.WriteLine($"Part2 Single Path Result: {result}");

        // Assert - One timeline
        Assert.Equal("1", result);
    }

    [Fact]
    public void Part2_OneSplitter_Returns2()
    {
        // Arrange
        var input = @".S.
...
.^.
...";

        // Act
        var result = _solution.SolvePart2(input);
        
        // Output for debugging
        _output.WriteLine($"Part2 One Splitter Result: {result}");

        // Assert - Two timelines (left and right)
        Assert.Equal("2", result);
    }

    [Fact]
    public void Part2_WithRealInput_ReturnsNonZero()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day07_example.txt");

        // Act
        var result = _solution.SolvePart2(input);
        
        // Output for debugging
        _output.WriteLine($"Part2 Real Input Result: {result}");

        // Assert - Only verify it runs without error
        Assert.NotNull(result);
        Assert.NotEqual("0", result);
        long.TryParse(result, out long numericResult);
        Assert.True(numericResult > 0, "Result should be a positive number");
    }
}
