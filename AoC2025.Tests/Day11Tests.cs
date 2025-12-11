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

    [Fact]
    public void Part2_Example_Returns2Paths()
    {
        // Arrange - Example from AoC Part 2 with 8 total paths, 2 valid
        // Valid paths must visit BOTH dac AND fft
        var input = File.ReadAllText("TestData/day11_part2_example.txt");

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.Equal("2", result);
    }

    [Fact]
    public void Part2_DirectPathWithBothRequired_Returns1()
    {
        // Arrange - Direct path through both required nodes
        var input = @"svr: dac
dac: fft
fft: out";

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.Equal("1", result);
    }

    [Fact]
    public void Part2_DirectPathMissingOneRequired_Returns0()
    {
        // Arrange - Path only goes through dac, missing fft
        var input = @"svr: dac
dac: out";

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.Equal("0", result);
    }

    [Fact]
    public void Part2_RequiredNodeInReverse_Returns1()
    {
        // Arrange - Path visits fft first, then dac (order doesn't matter)
        var input = @"svr: fft
fft: dac
dac: out";

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.Equal("1", result);
    }

    [Fact]
    public void Part2_MultipleBranchesOnlyOneValid_ReturnsCorrectCount()
    {
        // Arrange - Two branches, only one has both required nodes
        var input = @"svr: aaa bbb
aaa: dac
dac: fft
fft: out
bbb: out";

        // Act
        var result = _solution.SolvePart2(input);

        // Assert
        Assert.Equal("1", result); // Only path through aaa->dac->fft->out is valid
    }
}
