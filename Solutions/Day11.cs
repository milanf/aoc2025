namespace AoC2025.Solutions;

/// <summary>
/// Day 11: Reactor - Count all distinct paths from 'you' to 'out' in a directed acyclic graph.
/// Uses DFS with memoization for O(V + E) time complexity.
/// </summary>
public class Day11 : ISolution
{
    public int DayNumber => 11;
    
    public string Title => "Reactor";
    
    public string SolvePart1(string input)
    {
        var graph = ParseGraph(input);
        var pathCount = CountAllPaths(graph, "you", "out");
        return pathCount.ToString();
    }

    public string SolvePart2(string input)
    {
        return "Not implemented yet";
    }

    /// <summary>
    /// Parse the input into a directed graph representation.
    /// Each line: "device: output1 output2 output3"
    /// </summary>
    private Dictionary<string, List<string>> ParseGraph(string input)
    {
        var graph = new Dictionary<string, List<string>>();
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
                continue;

            var parts = trimmedLine.Split(':', StringSplitOptions.TrimEntries);
            if (parts.Length != 2)
                continue;

            var device = parts[0];
            var outputs = parts[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            graph[device] = outputs;
        }

        return graph;
    }

    /// <summary>
    /// Count all distinct paths from start to target using DFS with memoization.
    /// Time complexity: O(V + E) where V = vertices, E = edges
    /// Space complexity: O(V) for memoization cache
    /// </summary>
    private long CountAllPaths(Dictionary<string, List<string>> graph, string start, string target)
    {
        var memo = new Dictionary<string, long>();
        return CountPathsWithMemo(graph, start, target, memo);
    }

    /// <summary>
    /// Recursive DFS with memoization.
    /// Returns the number of distinct paths from current node to target.
    /// </summary>
    private long CountPathsWithMemo(
        Dictionary<string, List<string>> graph,
        string current,
        string target,
        Dictionary<string, long> memo)
    {
        // Base case: reached target
        if (current == target)
            return 1;

        // Check if already computed
        if (memo.ContainsKey(current))
            return memo[current];

        // If node has no outgoing edges, no path exists
        if (!graph.ContainsKey(current))
        {
            memo[current] = 0;
            return 0;
        }

        // Count paths through all neighbors
        long totalPaths = 0;
        foreach (var neighbor in graph[current])
        {
            totalPaths += CountPathsWithMemo(graph, neighbor, target, memo);
        }

        // Cache the result
        memo[current] = totalPaths;
        return totalPaths;
    }
}
