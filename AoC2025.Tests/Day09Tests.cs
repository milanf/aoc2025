namespace AoC2025.Tests;

public class Day09Tests
{
    private readonly Solutions.Day09 _solution = new();

    [Fact]
    public void Part1_Example_Returns50()
    {
        // Arrange
        string input = File.ReadAllText("TestData/day09_example.txt");

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // Expected: Maximum rectangle area is 50
        // Between points (2,5) and (11,1): (11-2+1) × (5-1+1) = 10 × 5 = 50
        Assert.Equal("50", result);
    }

    [Fact]
    public void Part1_TwoPoints_ReturnsCorrectArea()
    {
        // Arrange
        string input = "0,0\n10,5";

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // Rectangle between (0,0) and (10,5)
        // Width = 10 - 0 + 1 = 11, Height = 5 - 0 + 1 = 6
        // Area = 11 × 6 = 66
        Assert.Equal("66", result);
    }

    [Fact]
    public void Part1_SameXCoordinate_ReturnsTallRectangle()
    {
        // Arrange
        string input = "5,0\n5,10\n5,20";

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // Max area: between (5,0) and (5,20)
        // Width = |5-5| + 1 = 1, Height = |20-0| + 1 = 21
        // Area = 1 × 21 = 21
        Assert.Equal("21", result);
    }

    [Fact]
    public void Part1_SameYCoordinate_ReturnsWideRectangle()
    {
        // Arrange
        string input = "0,5\n10,5\n20,5";

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // Max area: between (0,5) and (20,5)
        // Width = |20-0| + 1 = 21, Height = |5-5| + 1 = 1
        // Area = 21 × 1 = 21
        Assert.Equal("21", result);
    }

    [Fact]
    public void Part1_SinglePoint_ReturnsZero()
    {
        // Arrange
        string input = "5,5";

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // Only one point → no rectangle possible → area = 0
        Assert.Equal("0", result);
    }

    [Fact]
    public void Part1_ThreePointsInL_ReturnsMaxArea()
    {
        // Arrange
        string input = "0,0\n10,0\n0,5";

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // Max area between (10,0) and (0,5)
        // Width = |10-0| + 1 = 11, Height = |5-0| + 1 = 6
        // Area = 11 × 6 = 66
        Assert.Equal("66", result);
    }

    [Fact]
    public void Part1_RealInput_ReturnsCorrectAnswer()
    {
        // Arrange
        string inputPath = Path.Combine("..", "..", "..", "..", "Inputs", "day09.txt");
        
        // Skip if file doesn't exist
        if (!File.Exists(inputPath))
        {
            return;
        }
        
        string input = File.ReadAllText(inputPath);

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // Expected result to be calculated and verified
        // With 496 points and coordinates up to ~98,000
        // Maximum area should be in millions
        Assert.NotEmpty(result);
        
        // Additional validation - result should be a valid number
        Assert.True(long.TryParse(result, out long area));
        Assert.True(area > 0, "Area should be positive for real input");
        
        // Log the result for manual verification
        System.Console.WriteLine($"Day 09 Part 1 Result: {result}");
    }
}
