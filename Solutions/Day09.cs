namespace AoC2025.Solutions;

/// <summary>
/// Day 9: Movie Theater - Finding maximum rectangle area
/// </summary>
public class Day09 : ISolution
{
    public int DayNumber => 9;
    public string Title => "Movie Theater";

    public string SolvePart1(string input)
    {
        // Parse all red tile coordinates
        var points = ParsePoints(input);
        
        // Calculate maximum rectangle area
        long maxArea = CalculateMaxRectangleArea(points);
        
        return maxArea.ToString();
    }

    public string SolvePart2(string input)
    {
        // Part 2 not yet implemented
        return "0";
    }

    /// <summary>
    /// Parse input file to extract (X, Y) coordinates
    /// </summary>
    private Point[] ParsePoints(string input)
    {
        return input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Trim().Split(',');
                return new Point(
                    long.Parse(parts[0]),
                    long.Parse(parts[1])
                );
            })
            .ToArray();
    }

    /// <summary>
    /// Calculate maximum rectangle area between any two points
    /// Area is calculated as (width + 1) × (height + 1) because we count tiles, not distance
    /// Uses brute force O(n²) approach - acceptable for n=496
    /// </summary>
    private long CalculateMaxRectangleArea(Point[] points)
    {
        if (points.Length < 2)
            return 0;

        long maxArea = 0;

        // Try all pairs of points
        for (int i = 0; i < points.Length; i++)
        {
            for (int j = i + 1; j < points.Length; j++)
            {
                // Calculate rectangle dimensions (number of tiles, not distance)
                // Width is the distance + 1 (inclusive)
                long width = Math.Abs(points[i].X - points[j].X) + 1;
                long height = Math.Abs(points[i].Y - points[j].Y) + 1;
                long area = width * height;

                // Update maximum
                if (area > maxArea)
                    maxArea = area;
            }
        }

        return maxArea;
    }

    /// <summary>
    /// Represents a point with X, Y coordinates
    /// Using long to handle coordinates up to ~100,000
    /// </summary>
    private record Point(long X, long Y);
}
