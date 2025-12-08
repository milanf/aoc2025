namespace AoC2025.Tests;

public class Day08Tests
{
    private readonly Solutions.Day08 _solution = new();

    [Fact]
    public void Part1_Example_Returns40()
    {
        // Arrange
        string input = File.ReadAllText("TestData/day08_example.txt");

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // Expected: After 10 connections with 20 boxes
        // Should have: 5, 4, 2, 2, 1, 1, 1, 1, 1, 1, 1 
        // Top 3: 5 × 4 × 2 = 40
        Assert.Equal("40", result);
    }

    [Fact]
    public void Debug_Example_ShowCircuits()
    {
        // Arrange
        string input = File.ReadAllText("TestData/day08_example.txt");
        
        // Act - Let's see what we actually get
        string result = _solution.SolvePart1(input);
        
        // This test is just for debugging - let's see what result we get
        Assert.NotEmpty(result);
    }

    [Fact]
    public void Part1_RealInput_ReturnsCorrectAnswer()
    {
        // Arrange
        string inputPath = Path.Combine("..", "..", "..", "..", "Inputs", "day08.txt");
        
        // Skip if file doesn't exist
        if (!File.Exists(inputPath))
        {
            return;
        }
        
        string input = File.ReadAllText(inputPath);

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        Assert.NotEmpty(result);
        // Uncomment and set expected value once known:
        // Assert.Equal("expected_value", result);
    }

    [Fact]
    public void Part1_MinimalInput_ThreeBoxesOneConnection()
    {
        // Arrange
        string input = @"0,0,0
1,0,0
0,1,0";

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // After 1 connection: circuits [2, 1, 1]
        // Product: 2 × 1 × 1 = 2
        Assert.Equal("2", result);
    }

    [Fact]
    public void Part1_NineBoxesThreeConnections()
    {
        // Arrange
        // Create 9 boxes in 3 distinct groups
        string input = @"0,0,0
1,0,0
2,0,0
10,0,0
11,0,0
12,0,0
20,0,0
21,0,0
22,0,0";

        // Act
        string result = _solution.SolvePart1(input);

        // Assert
        // After 3 connections, we should have 3 circuits of 2 boxes each + 3 singles
        // Top three sizes could vary, but let's verify it doesn't crash
        Assert.NotEmpty(result);
    }
}
