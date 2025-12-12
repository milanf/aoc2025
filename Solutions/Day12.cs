namespace AoC2025.Solutions;

using System.Text;

/// <summary>
/// Day 12: Christmas Tree Farm
/// Place presents (tetris-like shapes) into regions on a 2D grid.
/// Presents can be rotated and flipped. Find how many regions can fit all their required presents.
/// </summary>
public class Day12 : ISolution
{
    public int DayNumber => 12;
    
    public string Title => "Christmas Tree Farm";
    
    public string SolvePart1(string input)
    {
        var (shapes, regions) = ParseInput(input);
        
        int successfulRegions = 0;
        
        foreach (var region in regions)
        {
            if (CanFitAllPresents(region, shapes))
            {
                successfulRegions++;
            }
        }
        
        return successfulRegions.ToString();
    }
    
    public string SolvePart2(string input)
    {
        return "Not implemented yet";
    }
    
    /// <summary>
    /// Parse input into shape definitions and regions to check.
    /// Format:
    /// - First section: shape definitions (0-5)
    /// - Second section: regions (width×height: count0 count1 count2 count3 count4 count5)
    /// </summary>
    private (Dictionary<int, PresentShape> shapes, List<Region> regions) ParseInput(string input)
    {
        var shapes = new Dictionary<int, PresentShape>();
        var regions = new List<Region>();
        
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        
        int i = 0;
        
        // Parse shape definitions
        while (i < lines.Length && lines[i].EndsWith(':'))
        {
            int shapeId = int.Parse(lines[i].TrimEnd(':'));
            i++;
            
            var shapeLines = new List<string>();
            while (i < lines.Length && !lines[i].EndsWith(':') && !lines[i].Contains('×') && !lines[i].Contains('x'))
            {
                shapeLines.Add(lines[i]);
                i++;
            }
            
            shapes[shapeId] = CreateShape(shapeId, shapeLines);
        }
        
        // Parse regions
        while (i < lines.Length)
        {
            var line = lines[i];
            if (line.Contains('×') || line.Contains('x'))
            {
                var parts = line.Split(':');
                var dimensions = parts[0].Split(new[] { '×', 'x' });
                int width = int.Parse(dimensions[0]);
                int height = int.Parse(dimensions[1]);
                
                var counts = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToArray();
                
                regions.Add(new Region
                {
                    Width = width,
                    Height = height,
                    PresentCounts = counts
                });
            }
            i++;
        }
        
        return (shapes, regions);
    }
    
    /// <summary>
    /// Create a PresentShape from text lines
    /// </summary>
    private PresentShape CreateShape(int id, List<string> lines)
    {
        int height = lines.Count;
        int width = lines.Max(l => l.Length);
        
        var grid = new bool[height, width];
        int blockCount = 0;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (x < lines[y].Length && lines[y][x] == '#')
                {
                    grid[y, x] = true;
                    blockCount++;
                }
            }
        }
        
        return new PresentShape
        {
            Id = id,
            Width = width,
            Height = height,
            Grid = grid,
            BlockCount = blockCount
        };
    }
    
    /// <summary>
    /// Check if all presents can fit in the given region using backtracking
    /// </summary>
    private bool CanFitAllPresents(Region region, Dictionary<int, PresentShape> shapes)
    {
        var grid = new Grid(region.Width, region.Height);
        var presents = ExpandPresents(region.PresentCounts, shapes);
        
        // Sort presents by block count (largest first) - MRV heuristic
        presents = presents.OrderByDescending(p => p.BlockCount).ToList();
        
        return Backtrack(grid, presents, 0);
    }
    
    /// <summary>
    /// Expand present counts into a list of present shapes
    /// </summary>
    private List<PresentShape> ExpandPresents(int[] counts, Dictionary<int, PresentShape> shapes)
    {
        var presents = new List<PresentShape>();
        
        for (int i = 0; i < counts.Length; i++)
        {
            for (int j = 0; j < counts[i]; j++)
            {
                presents.Add(shapes[i]);
            }
        }
        
        return presents;
    }
    
    /// <summary>
    /// Backtracking algorithm to place all presents
    /// </summary>
    private bool Backtrack(Grid grid, List<PresentShape> presents, int index)
    {
        if (index == presents.Count)
        {
            return true; // All presents placed successfully
        }
        
        var shape = presents[index];
        
        // Early termination: check if remaining presents can physically fit
        int remainingBlocks = 0;
        for (int i = index; i < presents.Count; i++)
        {
            remainingBlocks += presents[i].BlockCount;
        }
        
        if (remainingBlocks > grid.GetEmptySpace())
        {
            return false;
        }
        
        // Try all transformations
        foreach (var transformation in shape.GetTransformations())
        {
            // Try all positions
            for (int y = 0; y <= grid.Height - transformation.Height; y++)
            {
                for (int x = 0; x <= grid.Width - transformation.Width; x++)
                {
                    if (grid.CanPlaceAt(transformation, x, y))
                    {
                        grid.PlacePresent(transformation, x, y);
                        
                        if (Backtrack(grid, presents, index + 1))
                        {
                            return true;
                        }
                        
                        grid.RemovePresent(transformation, x, y);
                    }
                }
            }
        }
        
        return false;
    }
}

/// <summary>
/// Represents a present shape (tetris-like block)
/// </summary>
public class PresentShape
{
    public int Id { get; set; }
    public bool[,] Grid { get; set; } = null!; // true = #, false = .
    public int Width { get; set; }
    public int Height { get; set; }
    public int BlockCount { get; set; } // Number of '#' blocks
    
    private List<PresentShape>? _transformations;
    
    /// <summary>
    /// Get all unique transformations (rotations + flips) of this shape
    /// </summary>
    public List<PresentShape> GetTransformations()
    {
        if (_transformations is null)
        {
            _transformations = GenerateAllTransformations();
        }
        return _transformations;
    }
    
    /// <summary>
    /// Generate all unique transformations (4 rotations × 2 flips)
    /// </summary>
    private List<PresentShape> GenerateAllTransformations()
    {
        var transformations = new HashSet<string>();
        var results = new List<PresentShape>();
        
        var current = this;
        
        // Try 4 rotations
        for (int rotation = 0; rotation < 4; rotation++)
        {
            // Add current rotation
            AddIfUnique(current, transformations, results);
            
            // Add horizontal flip
            var flipped = FlipHorizontal(current);
            AddIfUnique(flipped, transformations, results);
            
            // Rotate for next iteration
            current = Rotate90(current);
        }
        
        return results;
    }
    
    /// <summary>
    /// Add shape to results if it's unique
    /// </summary>
    private void AddIfUnique(PresentShape shape, HashSet<string> signatures, List<PresentShape> results)
    {
        string signature = GetShapeSignature(shape);
        if (signatures.Add(signature))
        {
            results.Add(shape);
        }
    }
    
    /// <summary>
    /// Get a unique signature for this shape configuration
    /// </summary>
    private string GetShapeSignature(PresentShape shape)
    {
        var sb = new StringBuilder();
        for (int y = 0; y < shape.Height; y++)
        {
            for (int x = 0; x < shape.Width; x++)
            {
                sb.Append(shape.Grid[y, x] ? '#' : '.');
            }
            sb.Append('|');
        }
        return sb.ToString();
    }
    
    /// <summary>
    /// Rotate shape 90 degrees clockwise
    /// </summary>
    private PresentShape Rotate90(PresentShape shape)
    {
        int newWidth = shape.Height;
        int newHeight = shape.Width;
        var newGrid = new bool[newHeight, newWidth];
        
        for (int y = 0; y < shape.Height; y++)
        {
            for (int x = 0; x < shape.Width; x++)
            {
                newGrid[x, shape.Height - 1 - y] = shape.Grid[y, x];
            }
        }
        
        return new PresentShape
        {
            Id = shape.Id,
            Width = newWidth,
            Height = newHeight,
            Grid = newGrid,
            BlockCount = shape.BlockCount
        };
    }
    
    /// <summary>
    /// Flip shape horizontally
    /// </summary>
    private PresentShape FlipHorizontal(PresentShape shape)
    {
        var newGrid = new bool[shape.Height, shape.Width];
        
        for (int y = 0; y < shape.Height; y++)
        {
            for (int x = 0; x < shape.Width; x++)
            {
                newGrid[y, shape.Width - 1 - x] = shape.Grid[y, x];
            }
        }
        
        return new PresentShape
        {
            Id = shape.Id,
            Width = shape.Width,
            Height = shape.Height,
            Grid = newGrid,
            BlockCount = shape.BlockCount
        };
    }
}

/// <summary>
/// Represents a region where presents should be placed
/// </summary>
public class Region
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int[] PresentCounts { get; set; } = null!; // [count0, count1, ..., count5]
    
    public int GetTotalPresentCount() => PresentCounts.Sum();
}

/// <summary>
/// Efficient grid representation using boolean array
/// </summary>
public class Grid
{
    private readonly bool[,] _cells;
    public int Width { get; }
    public int Height { get; }
    private int _emptySpaceCache;
    
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _cells = new bool[height, width];
        _emptySpaceCache = width * height;
    }
    
    /// <summary>
    /// Check if a present can be placed at the given position
    /// </summary>
    public bool CanPlaceAt(PresentShape shape, int x, int y)
    {
        // Check bounds
        if (x + shape.Width > Width || y + shape.Height > Height)
        {
            return false;
        }
        
        // Check for collisions (only '#' blocks matter)
        for (int dy = 0; dy < shape.Height; dy++)
        {
            for (int dx = 0; dx < shape.Width; dx++)
            {
                if (shape.Grid[dy, dx]) // '#' block
                {
                    if (_cells[y + dy, x + dx])
                    {
                        return false; // Collision
                    }
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Place a present at the given position
    /// </summary>
    public void PlacePresent(PresentShape shape, int x, int y)
    {
        for (int dy = 0; dy < shape.Height; dy++)
        {
            for (int dx = 0; dx < shape.Width; dx++)
            {
                if (shape.Grid[dy, dx])
                {
                    _cells[y + dy, x + dx] = true;
                    _emptySpaceCache--;
                }
            }
        }
    }
    
    /// <summary>
    /// Remove a present from the given position (for backtracking)
    /// </summary>
    public void RemovePresent(PresentShape shape, int x, int y)
    {
        for (int dy = 0; dy < shape.Height; dy++)
        {
            for (int dx = 0; dx < shape.Width; dx++)
            {
                if (shape.Grid[dy, dx])
                {
                    _cells[y + dy, x + dx] = false;
                    _emptySpaceCache++;
                }
            }
        }
    }
    
    /// <summary>
    /// Get the number of empty cells in the grid
    /// </summary>
    public int GetEmptySpace()
    {
        return _emptySpaceCache;
    }
}
