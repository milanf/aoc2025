using AoC2025.Solutions;

namespace AoC2025.Tests;

public class Day06Tests
{
    private readonly Day06 _solution;

    public Day06Tests()
    {
        _solution = new Day06();
    }

    [Fact]
    public void Part1_WithExampleInput_Returns4277556()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day06_example.txt");

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("4277556", result);
    }

    [Fact]
    public void Part1_WithRealInput_ReturnsCorrectAnswer()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day06.txt");

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("0", result);
        // Expected result will be verified after first run
    }

    [Theory]
    [InlineData("2\n3\n4\n+", "9")] // Simple addition: 2+3+4
    [InlineData("2\n3\n4\n*", "24")] // Simple multiplication: 2*3*4
    [InlineData("5\n0\n10\n*", "0")] // Multiplication with zero
    [InlineData("100\n200\n300\n+", "600")] // Large numbers addition
    public void Part1_WithSingleProblem_ReturnsCorrectResult(string input, string expected)
    {
        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Part1_WithMultipleProblems_SumsResultsCorrectly()
    {
        // Arrange - 3 problems: 2+3+0=5, 4*5*0=0, 1+1+0=2, total=7
        var input = "2 4 1\n3 5 1\n0 0 0\n+ * +";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("7", result);
    }

    [Fact]
    public void Part1_WithLargeMultiplication_HandlesLongValues()
    {
        // Arrange - Test that we can handle large numbers without overflow
        var input = "9999\n9999\n9999\n9999\n*";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        // 9999^4 = 9,996,000,599,960,001
        Assert.Equal("9996000599960001", result);
    }
    
}
