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
        // Parse all red tile coordinates (they form a polygon in order)
        var points = ParsePoints(input);
        
        // Use coordinate compression and scanline fill for efficient validation
        long maxArea = CalculateMaxValidRectangleAreaOptimized(points);
        
        return maxArea.ToString();
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

    // ============= Part 2: Coordinate Compression + Scanline Fill =============

    /// <summary>
    /// Calculate maximum rectangle area using coordinate compression and scanline fill
    /// This is O(n^2) for coordinates + O(n^2) for pairs, but validation is O(1) with prefix sums
    /// </summary>
    private long CalculateMaxValidRectangleAreaOptimized(Point[] points)
    {
        int n = points.Length;
        if (n < 2)
            return 0;

        // Extract and compress coordinates
        var xs = points.Select(p => p.X).Distinct().OrderBy(x => x).ToList();
        var ys = points.Select(p => p.Y).Distinct().OrderBy(y => y).ToList();

        int xn = xs.Count;
        int yn = ys.Count;

        if (xn <= 1 || yn <= 1)
            return 0;

        // Build coordinate index maps
        var xIndex = xs.Select((val, idx) => (val, idx)).ToDictionary(x => x.val, x => x.idx);
        var yIndex = ys.Select((val, idx) => (val, idx)).ToDictionary(y => y.val, y => y.idx);

        // Build scanline structure: for each Y-band, store X-indices of vertical edges
        var rowXs = Enumerable.Range(0, yn - 1).Select(_ => new List<int>()).ToArray();

        for (int i = 0; i < n; i++)
        {
            var p1 = points[i];
            var p2 = points[(i + 1) % n];

            if (p1.X == p2.X)
            {
                // Vertical edge at x = p1.X
                int xi = xIndex[p1.X];
                int yi1 = yIndex[p1.Y];
                int yi2 = yIndex[p2.Y];
                
                if (yi1 > yi2)
                    (yi1, yi2) = (yi2, yi1);

                // This vertical edge crosses Y-bands [yi1, yi2)
                for (int j = yi1; j < yi2; j++)
                {
                    rowXs[j].Add(xi);
                }
            }
            // Horizontal edges don't contribute to scanline fill
        }

        // Build inside/outside grid using even-odd rule
        int yb = yn - 1;
        int xb = xn - 1;
        var inside = new bool[yb, xb];

        for (int j = 0; j < yb; j++)
        {
            var xsList = rowXs[j];
            if (xsList.Count == 0)
                continue;

            xsList.Sort();
            
            // Remove duplicates
            var uniqueXs = xsList.Distinct().ToList();

            // Even-odd rule: pairs of crossings define inside regions
            for (int k = 0; k + 1 < uniqueXs.Count; k += 2)
            {
                int xL = uniqueXs[k];
                int xR = uniqueXs[k + 1];

                for (int i = xL; i < xR && i < xb; i++)
                {
                    inside[j, i] = true;
                }
            }
        }

        // Build 2D prefix sum for "outside" cells (0 = inside, 1 = outside)
        var pref = new int[yb + 1, xb + 1];
        for (int j = 0; j < yb; j++)
        {
            for (int i = 0; i < xb; i++)
            {
                int outside = inside[j, i] ? 0 : 1;
                pref[j + 1, i + 1] = pref[j + 1, i] + pref[j, i + 1] - pref[j, i] + outside;
            }
        }

        long bestArea = 0;

        // Try all pairs of red points as opposite corners
        for (int a = 0; a < n; a++)
        {
            for (int b = a + 1; b < n; b++)
            {
                long x1 = points[a].X;
                long y1 = points[a].Y;
                long x2 = points[b].X;
                long y2 = points[b].Y;

                if (x1 == x2 && y1 == y2)
                    continue; // Same point

                long xl = Math.Min(x1, x2);
                long xr = Math.Max(x1, x2);
                long yl = Math.Min(y1, y2);
                long yr = Math.Max(y1, y2);

                // Map to compressed coordinates
                int ixL = BinarySearchLowerBound(xs, xl);
                int ixR = BinarySearchLowerBound(xs, xr);
                int iyL = BinarySearchLowerBound(ys, yl);
                int iyR = BinarySearchLowerBound(ys, yr);

                // Calculate rectangle area
                long area = (xr - xl + 1) * (yr - yl + 1);

                if (area <= bestArea)
                    continue; // Can't beat current best

                // Check if any outside cells in this region
                int outsideCells = GetOutsideCount(pref, ixL, iyL, ixR, iyR);
                if (outsideCells == 0)
                {
                    // All cells inside polygon - valid rectangle
                    bestArea = area;
                }
            }
        }

        return bestArea;
    }

    private int BinarySearchLowerBound(List<long> sorted, long value)
    {
        int left = 0, right = sorted.Count;
        while (left < right)
        {
            int mid = left + (right - left) / 2;
            if (sorted[mid] < value)
                left = mid + 1;
            else
                right = mid;
        }
        return left;
    }

    private int GetOutsideCount(int[,] pref, int x1, int y1, int x2, int y2)
    {
        if (x1 >= x2 || y1 >= y2)
            return 0;
        return pref[y2, x2] - pref[y1, x2] - pref[y2, x1] + pref[y1, x1];
    }
}
