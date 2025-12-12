using AoC2025.Solutions;

namespace AoC2025.Tests;

public class Day12Tests
{
    private readonly Day12 _solution = new();

    [Fact]
    public void Part1_Example_Returns2()
    {
        // Arrange - Example from AoC with 3 regions, 2 should fit
        var input = File.ReadAllText("TestData/day12_example.txt");

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("2", result);
    }

    [Fact]
    public void PresentShape_GeneratesTransformations()
    {
        // Arrange - Create a simple L-shape
        var shape = new PresentShape
        {
            Id = 0,
            Width = 2,
            Height = 2,
            Grid = new bool[,] { { true, true }, { true, false } },
            BlockCount = 3
        };

        // Act
        var transformations = shape.GetTransformations();

        // Assert
        Assert.NotEmpty(transformations);
        Assert.True(transformations.Count <= 8); // Max 8 transformations
    }

    [Fact]
    public void Grid_CanPlaceAndRemovePresent()
    {
        // Arrange
        var grid = new Grid(5, 5);
        var shape = new PresentShape
        {
            Id = 0,
            Width = 2,
            Height = 2,
            Grid = new bool[,] { { true, true }, { true, true } },
            BlockCount = 4
        };

        // Act - Place at (0, 0)
        Assert.True(grid.CanPlaceAt(shape, 0, 0));
        grid.PlacePresent(shape, 0, 0);

        // Assert - Cannot place at same position
        Assert.False(grid.CanPlaceAt(shape, 0, 0));
        Assert.Equal(21, grid.GetEmptySpace()); // 25 - 4 = 21

        // Act - Remove and check again
        grid.RemovePresent(shape, 0, 0);

        // Assert - Can place again
        Assert.True(grid.CanPlaceAt(shape, 0, 0));
        Assert.Equal(25, grid.GetEmptySpace());
    }

    [Fact]
    public void Grid_DetectsCollisions()
    {
        // Arrange
        var grid = new Grid(5, 5);
        var shape = new PresentShape
        {
            Id = 0,
            Width = 3,
            Height = 3,
            Grid = new bool[,]
            {
                { true, true, true },
                { true, false, false },
                { true, false, false }
            },
            BlockCount = 4
        };

        // Act - Place at (0, 0)
        grid.PlacePresent(shape, 0, 0);

        // Assert - Overlapping positions should fail
        Assert.False(grid.CanPlaceAt(shape, 0, 0)); // Exact overlap
        Assert.False(grid.CanPlaceAt(shape, 1, 0)); // Partial overlap
        Assert.True(grid.CanPlaceAt(shape, 2, 2));  // No overlap
    }

    [Fact]
    public void Grid_ChecksBounds()
    {
        // Arrange
        var grid = new Grid(5, 5);
        var shape = new PresentShape
        {
            Id = 0,
            Width = 3,
            Height = 3,
            Grid = new bool[,]
            {
                { true, true, true },
                { true, false, false },
                { true, false, false }
            },
            BlockCount = 4
        };

        // Assert - Out of bounds
        Assert.False(grid.CanPlaceAt(shape, 3, 0)); // Would extend to x=6 (>5)
        Assert.False(grid.CanPlaceAt(shape, 0, 3)); // Would extend to y=6 (>5)
        Assert.False(grid.CanPlaceAt(shape, 5, 5)); // Completely out
        Assert.True(grid.CanPlaceAt(shape, 2, 2));  // Exactly fits (2,2 to 4,4)
    }

    [Fact]
    public void Region_CalculatesTotalPresentCount()
    {
        // Arrange
        var region = new Region
        {
            Width = 10,
            Height = 10,
            PresentCounts = new[] { 1, 2, 3, 0, 1, 2 }
        };

        // Act
        var total = region.GetTotalPresentCount();

        // Assert
        Assert.Equal(9, total); // 1+2+3+0+1+2 = 9
    }

    [Fact]
    public void PresentShape_Rotate90Works()
    {
        // Arrange - L-shape
        var shape = new PresentShape
        {
            Id = 0,
            Width = 2,
            Height = 3,
            Grid = new bool[,]
            {
                { true, false },
                { true, false },
                { true, true }
            },
            BlockCount = 4
        };

        // Act
        var transformations = shape.GetTransformations();

        // Assert - Should have at least 4 unique rotations
        Assert.NotEmpty(transformations);
        
        // Verify we have transformations with different dimensions
        var hasDifferentDimensions = transformations.Any(t => t.Width != shape.Width || t.Height != shape.Height);
        Assert.True(hasDifferentDimensions);
    }

    [Fact]
    public void Part1_EmptyRegion_ReturnsCorrectCount()
    {
        // Arrange - Region with no presents required
        var input = @"0:
##
##

4×4: 0 0 0 0 0 0";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1", result); // Empty requirement should succeed
    }

    [Fact]
    public void Part1_SinglePresentFitsExactly()
    {
        // Arrange - 2x2 shape in 2x2 region
        var input = @"0:
##
##

2×2: 1 0 0 0 0 0";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1", result);
    }

    [Fact]
    public void Part1_TooManyPresents_Fails()
    {
        // Arrange - 2x2 region but need 2 shapes of 4 blocks = 8 blocks > 4 available
        var input = @"0:
##
##

2×2: 2 0 0 0 0 0";

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("0", result); // Cannot fit
    }
}
