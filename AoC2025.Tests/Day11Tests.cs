using AoC2025.Solutions;

namespace AoC2025.Tests;

public class Day11Tests
{
    private readonly Day11 _solution = new();

    [Fact]
    public void Part1_Example_Returns5Paths()
    {
        // Arrange - Example from AoC with 5 distinct paths
        var input = File.ReadAllText("TestData/day11_example.txt");

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("5", result);
    }

    [Fact]
    public void Part1_DirectPath_Returns1()
    {
        // Arrange - Direct connection from you to out
        var input = "you: out";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1", result);
    }

    [Fact]
    public void Part1_TwoParallelPaths_Returns2()
    {
        // Arrange - Two separate paths to out
        var input = @"you: aaa bbb
aaa: out
bbb: out";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("2", result);
    }

    [Fact]
    public void Part1_NoPath_Returns0()
    {
        // Arrange - No path leads to 'out'
        var input = @"you: aaa
aaa: bbb
bbb: ccc";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("0", result);
    }

    [Fact]
    public void Part1_ComplexTree_ReturnsCorrectCount()
    {
        // Arrange - More complex graph with shared nodes
        var input = @"you: a b
a: c d
b: c e
c: out
d: out
e: out";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        // Paths: you->a->c->out, you->a->d->out, you->b->c->out, you->b->e->out
        Assert.Equal("4", result);
    }
}
