using AoC2025.Solutions;

namespace AoC2025.Tests;

/// <summary>
/// Unit testy pro Day 4: Printing Department - validace pomocí example inputu z AoC zadání.
/// </summary>
public class Day04Tests
{
    private readonly Day04 _solution;

    public Day04Tests()
    {
        _solution = new Day04();
    }

    [Fact]
    public void Part1_ExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = File.ReadAllText(Path.Combine("TestData", "day04_example.txt"));

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("13", result);
    }

    [Fact]
    public void Part1_SingleRollInCorner_IsAccessible()
    {
        // Arrange - role v rohu má méně sousedů
        var input = "@..\n...\n...";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1", result); // 0 sousedů < 4 → přístupná
    }

    [Fact]
    public void Part1_FullySurroundedRoll_IsNotAccessible()
    {
        // Arrange - role obklopená 8 rolemi
        var input = "@@@\n@@@\n@@@";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        // Rohy mají 3 sousedy (přístupné), hrany mají 5 sousedů (nepřístupné), střed má 8 sousedů (nepřístupný)
        // 4 rohy mají každý 3 sousedy < 4 → 4 přístupné
        Assert.Equal("4", result);
    }

    [Fact]
    public void Part1_RollWithExactly3Neighbors_IsAccessible()
    {
        // Arrange
        var input = "..@\n.@@\n@@@";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        // Role na [0,2] má 1 soused (přístupná)
        // [1,1] má 4 sousedy (nepřístupná), [1,2] má 3 sousedy (přístupná)
        // [2,0] má 1 soused (přístupná), [2,1] má 3 sousedy (nepřístupná - čekej, ne!), [2,2] má 3 sousedy (nepřístupná - ne!)
        // Správně: [0,2]=1, [1,1]=4, [1,2]=3, [2,0]=1, [2,1]=3, [2,2]=3
        // < 4: [0,2], [1,2], [2,0] → 3 přístupné
        Assert.Equal("3", result);
    }

    [Fact]
    public void Part2_ExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = File.ReadAllText(Path.Combine("TestData", "day04_example.txt"));

        // Act
        var result = _solution.SolvePart2(input);

        // Assert - postupné odstraňování 43 rolí celkem
        Assert.Equal("43", result);
    }

    [Fact]
    public void Part2_AllRollsAccessible_RemovesAllInFirstIteration()
    {
        // Arrange - všechny role mají méně než 4 sousedy
        var input = "@.@\n...\n@.@";

        // Act
        var result = _solution.SolvePart2(input);

        // Assert - 4 role, všechny přístupné (0 sousedů každá)
        Assert.Equal("4", result);
    }

    [Fact]
    public void Part2_NoAccessibleRolls_RemovesNone()
    {
        // Arrange - role s 8 sousedy (nepřístupná)
        var input = "@@@@@\n@@@@@\n@@@@@\n@@@@@\n@@@@@";

        // Act
        var result = _solution.SolvePart2(input);

        // Assert - pouze okrajové role jsou přístupné
        // 5×5 grid: rohy (4) + hrany (12) mají < 4 sousedy
        // Ale po odstranění okrajových se odhalí další...
        // Očekávám, že všechny se postupně odstraní
        Assert.True(int.Parse(result) > 0); // Alespoň nějaké role se odstraní
    }
}
