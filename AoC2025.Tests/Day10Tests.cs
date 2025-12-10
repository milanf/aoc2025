using AoC2025.Solutions;

namespace AoC2025.Tests;

public class Day10Tests
{
    private readonly Day10 _solution = new();

    [Fact]
    public void Test1_Example_Machine1()
    {
        // Arrange
        string input = "[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("2", result);
    }

    [Fact]
    public void Test2_Example_Machine2()
    {
        // Arrange
        string input = "[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("3", result);
    }

    [Fact]
    public void Test3_Example_Machine3()
    {
        // Arrange
        string input = "[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("2", result);
    }

    [Fact]
    public void Test4_Example_AllThreeMachines()
    {
        // Arrange
        string input = @"[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("7", result);
    }

    [Fact]
    public void Test5_TrivialMachine_AlreadyOff()
    {
        // Arrange
        string input = "[....] (0,1,2,3)";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("0", result);
    }

    [Fact]
    public void Test6_OneLight_OneButton()
    {
        // Arrange
        string input = "[#] (0)";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("1", result);
    }

    [Fact]
    public void Test7_OneLight_OneButton_AlreadyOff()
    {
        // Arrange
        string input = "[.] (0)";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("0", result);
    }

    [Fact]
    public void Test8_RealInput_FirstMachine()
    {
        // Arrange
        string input = "[#...##] (0,1,3,4,5) (0,4,5) (1,2,3,4) (0,1,2) {132,30,23,13,121,115}";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        // Výsledek není znám předem, ale musí být platné číslo
        Assert.True(int.TryParse(result, out int value));
        Assert.True(value >= 0);
    }

    [Fact]
    public void Test9_MultipleButtons_SameEffect()
    {
        // Arrange - dva buttony dělají totéž
        string input = "[#.] (0) (0)";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("1", result); // Stačí jeden button
    }

    [Fact]
    public void Test10_TwoLights_SimpleToggle()
    {
        // Arrange
        string input = "[##] (0) (1)";
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("2", result); // Jeden button pro každé světlo
    }

    [Fact]
    public void Test11_RealInput_FullFile()
    {
        // Arrange
        string input = File.ReadAllText("TestData/day10_example.txt");
        
        // Act
        string result = _solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("7", result); // Příklad z AoC: 2+3+2 = 7
    }
    
    // ==================== Part 2 Tests ====================
    
    [Fact]
    public void Part2_Test1_Example_Machine1()
    {
        // Arrange
        string input = "[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}";
        
        // Act
        string result = _solution.SolvePart2(input);
        
        // Assert
        Assert.Equal("10", result);
    }

    [Fact]
    public void Part2_Test2_Example_Machine2()
    {
        // Arrange
        string input = "[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}";
        
        // Act
        string result = _solution.SolvePart2(input);
        
        // Assert
        Assert.Equal("12", result);
    }

    [Fact]
    public void Part2_Test3_Example_Machine3()
    {
        // Arrange
        string input = "[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}";
        
        // Act
        string result = _solution.SolvePart2(input);
        
        // Assert
        Assert.Equal("11", result);
    }

    [Fact]
    public void Part2_Test4_Example_AllThreeMachines()
    {
        // Arrange
        string input = @"[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}";
        
        // Act
        string result = _solution.SolvePart2(input);
        
        // Assert
        Assert.Equal("33", result); // 10 + 12 + 11 = 33
    }

    [Fact]
    public void Part2_Test5_TrivialMachine_AllZeros()
    {
        // Arrange
        string input = "[....] (0,1,2,3) {0,0,0,0}";
        
        // Act
        string result = _solution.SolvePart2(input);
        
        // Assert
        Assert.Equal("0", result);
    }

    [Fact]
    public void Part2_Test6_SimpleCounter()
    {
        // Arrange - jeden counter, jedno tlačítko
        string input = "[#] (0) {5}";
        
        // Act
        string result = _solution.SolvePart2(input);
        
        // Assert
        Assert.Equal("5", result); // Stiskni tlačítko 5×
    }

    [Fact]
    public void Part2_Test7_TwoCounters_SeparateButtons()
    {
        // Arrange
        string input = "[##] (0) (1) {3,7}";
        
        // Act
        string result = _solution.SolvePart2(input);
        
        // Assert
        Assert.Equal("10", result); // (0) 3× + (1) 7× = 10
    }

    [Fact]
    public void Part2_Test8_SharedButton()
    {
        // Arrange - jedno tlačítko ovlivňuje oba countery stejně
        string input = "[##] (0,1) {5,5}";
        
        // Act
        string result = _solution.SolvePart2(input);
        
        // Assert
        Assert.Equal("5", result); // Stiskni (0,1) 5×
    }
}
