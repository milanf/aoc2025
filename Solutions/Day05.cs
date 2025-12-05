namespace AoC2025.Solutions;

public class Day05 : ISolution
{
    public int DayNumber => 5;
    
    public string Title => "Cafeteria";
    
    public string SolvePart1(string input)
    {
        var (ranges, ids) = ParseInput(input);
        
        int freshCount = 0;
        foreach (var id in ids)
        {
            if (ranges.Any(range => range.Contains(id)))
            {
                freshCount++;
            }
        }
        
        return freshCount.ToString();
    }

    public string SolvePart2(string input)
    {
        // Parse only the first section (ranges), second section is irrelevant for Part 2
        input = input.Replace("\r\n", "\n");
        var sections = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        
        // Parse and sort ranges by Start position
        var ranges = sections[0]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var parts = line.Split('-');
                return new Range(long.Parse(parts[0]), long.Parse(parts[1]));
            })
            .OrderBy(r => r.Start)
            .ToList();
        
        // Merge overlapping/adjacent ranges
        var merged = MergeRanges(ranges);
        
        // Calculate total count of unique IDs
        long totalCount = merged.Sum(r => r.Count);
        
        return totalCount.ToString();
    }

    /// <summary>
    /// Merges overlapping or adjacent ranges into a minimal set of non-overlapping ranges.
    /// Input must be sorted by Start position.
    /// Time complexity: O(n)
    /// </summary>
    private List<Range> MergeRanges(List<Range> sortedRanges)
    {
        if (sortedRanges.Count == 0)
            return new List<Range>();
        
        var merged = new List<Range>();
        var current = sortedRanges[0];
        
        foreach (var next in sortedRanges.Skip(1))
        {
            // Check if ranges can be merged (overlapping or adjacent)
            // Adjacent means next.Start == current.End + 1
            if (next.Start <= current.End + 1)
            {
                // Merge - extend current range
                // Use Math.Max to handle cases where next is contained within current
                current = new Range(
                    current.Start,
                    Math.Max(current.End, next.End)
                );
            }
            else
            {
                // Ranges don't overlap - save current and start new
                merged.Add(current);
                current = next;
            }
        }
        
        // Don't forget to add the last range
        merged.Add(current);
        
        return merged;
    }
    
    private (List<Range> ranges, List<long> ids) ParseInput(string input)
    {
        // Normalize line endings
        input = input.Replace("\r\n", "\n");
        
        var sections = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);
        
        if (sections.Length != 2)
        {
            throw new ArgumentException("Input must contain two sections separated by blank line");
        }
        
        // Parse ranges
        var ranges = sections[0]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                var parts = line.Split('-');
                if (parts.Length != 2)
                {
                    throw new ArgumentException($"Invalid range format: {line}");
                }
                return new Range(long.Parse(parts[0]), long.Parse(parts[1]));
            })
            .ToList();
        
        // Parse IDs
        var ids = sections[1]
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(long.Parse)
            .ToList();
        
        return (ranges, ids);
    }

    public record Range(long Start, long End)
    {
        public bool Contains(long value) => value >= Start && value <= End;
        
        /// <summary>
        /// Returns the count of values in this range (inclusive)
        /// </summary>
        public long Count => End - Start + 1;
    }
}
