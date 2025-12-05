using AoC2025.Solutions;

namespace AoC2025.Tests;

public class Day05Tests
{
    private readonly Day05 _solution;

    public Day05Tests()
    {
        _solution = new Day05();
    }

    [Fact]
    public void Part1_WithExampleInput_Returns3()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day05_example.txt");

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("3", result);
    }

    [Fact]
    public void Part1_WithRealInput_ReturnsCorrectAnswer()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day05.txt");

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("0", result);
        // Expected result will be verified after first run
    }

    [Theory]
    [InlineData("3-5\n\n5", "1")] // ID on upper bound
    [InlineData("3-5\n\n3", "1")] // ID on lower bound
    [InlineData("3-5\n\n4", "1")] // ID in middle
    [InlineData("3-5\n\n2", "0")] // ID below range
    [InlineData("3-5\n\n6", "0")] // ID above range
    public void Part1_WithBoundaryValues_ReturnsCorrectCount(string input, string expected)
    {
        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Part1_WithSinglePointRange_WorksCorrectly()
    {
        // Arrange
        var input = "5-5\n\n5";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1", result);
    }

    [Fact]
    public void Part1_WithOverlappingRanges_CountsOnlyOnce()
    {
        // Arrange
        var input = "10-20\n15-25\n\n17";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1", result); // ID 17 is in both ranges but counted once
    }

    [Fact]
    public void Part1_WithMultipleIds_CountsCorrectly()
    {
        // Arrange
        var input = "3-5\n10-14\n\n4\n5\n11\n20";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("3", result); // 4, 5, 11 are fresh; 20 is not
    }

    [Fact]
    public void Part1_WithLargeNumbers_HandlesCorrectly()
    {
        // Arrange - using actual values from real input
        var input = "169486974574545-170251643963353\n\n169486974574545\n170251643963353\n500000000000000";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("2", result); // First two are in range, third is not
    }

    [Theory]
    [InlineData("3-5\n10-14\n\n1\n8\n32", "0")] // All spoiled
    [InlineData("3-5\n10-14\n\n3\n4\n5\n10\n11\n12\n13\n14", "8")] // All fresh
    public void Part1_WithAllFreshOrAllSpoiled_ReturnsCorrectCount(string input, string expected)
    {
        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal(expected, result);
    }

    // ==================== Part 2 Tests ====================

    [Fact]
    public void Part2_WithExampleInput_Returns14()
    {
        // Arrange - from AoC example
        var input = """
            3-5
            10-14
            16-20
            12-18

            """;

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        // Merged ranges: [3-5], [10-20]
        // Count: (5-3+1) + (20-10+1) = 3 + 11 = 14
        Assert.Equal("14", result);
    }

    [Theory]
    [InlineData("5-5\n\n", "1")] // Single-point range
    [InlineData("1-5\n6-10\n\n", "10")] // Adjacent ranges (merge to 1-10)
    [InlineData("1-10\n5-15\n\n", "15")] // Overlapping ranges (merge to 1-15)
    [InlineData("1-20\n5-10\n\n", "20")] // Range within range (merge to 1-20)
    [InlineData("1-5\n1-5\n\n", "5")] // Duplicate ranges
    [InlineData("1-5\n7-10\n\n", "9")] // Non-overlapping gaps
    public void Part2_WithEdgeCases_ReturnsCorrectCount(string input, string expected)
    {
        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Part2_WithMultipleOverlaps_MergesCorrectly()
    {
        // Arrange
        var input = """
            1-5
            3-8
            6-10
            15-20

            """;

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        // Merged: [1-10], [15-20]
        // Count: 10 + 6 = 16
        Assert.Equal("16", result);
    }

    [Fact]
    public void Part2_WithLargeNumbers_HandlesCorrectly()
    {
        // Arrange - using large numbers similar to real input
        var input = """
            169486974574545-170251643963353
            230409669398989-230409669398989

            """;

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        // Range 1: 170251643963353 - 169486974574545 + 1 = 764,669,388,809
        // Range 2: 230409669398989 - 230409669398989 + 1 = 1
        // Total: 764,669,388,810
        Assert.Equal("764669388810", result);
    }

    [Fact]
    public void Part2_WithRealInput_ComputesCorrectly()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day05.txt");

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("0", result);
        
        // Result should be a positive number less than sum of all ranges without merge
        var count = long.Parse(result);
        Assert.True(count > 0);
        Assert.True(count < 452125362963180L); // Theoretical max without merging
    }

    [Fact]
    public void Part2_MergeAlgorithm_HandlesAllCases()
    {
        // Arrange - comprehensive test of merge scenarios
        var input = """
            1-3
            2-5
            7-10
            9-15
            20-25
            23-28
            30-35

            """;

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        // Merged: [1-5], [7-15], [20-28], [30-35]
        // Count: 5 + 9 + 9 + 6 = 29
        Assert.Equal("29", result);
    }
}
