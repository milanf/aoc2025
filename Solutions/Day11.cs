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
        var graph = ParseGraph(input);
        var pathCount = CountPathsWithRequiredNodes(graph, "svr", "out", "dac", "fft");
        return pathCount.ToString();
    }

    /// <summary>
    /// Count paths from start to target that visit BOTH required nodes (in any order).
    /// Uses DFS with state tracking: (node, hasVisitedRequired1, hasVisitedRequired2).
    /// Time complexity: O(V * E) with 4 possible states per node.
    /// Space complexity: O(V * 4) for memoization cache.
    /// </summary>
    private long CountPathsWithRequiredNodes(
        Dictionary<string, List<string>> graph,
        string start,
        string target,
        string required1,
        string required2)
    {
        // Validate all required nodes exist
        if (!graph.ContainsKey(start) && start != target)
            return 0;
        
        // Check if required nodes exist anywhere in the graph (as keys or values)
        var allNodes = new HashSet<string>(graph.Keys);
        foreach (var neighbors in graph.Values)
        {
            foreach (var neighbor in neighbors)
            {
                allNodes.Add(neighbor);
            }
        }
        
        if (!allNodes.Contains(required1) || !allNodes.Contains(required2) || !allNodes.Contains(target))
            return 0;

        var memo = new Dictionary<(string node, bool visited1, bool visited2), long>();
        
        // Check if start node is one of the required nodes
        bool startIsRequired1 = start == required1;
        bool startIsRequired2 = start == required2;
        
        return CountPathsWithRequiredNodesHelper(
            graph, start, target, required1, required2,
            startIsRequired1, startIsRequired2, memo);
    }

    /// <summary>
    /// Recursive DFS helper with state tracking for visited required nodes.
    /// Returns count of paths that visit BOTH required nodes before reaching target.
    /// </summary>
    private long CountPathsWithRequiredNodesHelper(
        Dictionary<string, List<string>> graph,
        string current,
        string target,
        string required1,
        string required2,
        bool hasVisitedRequired1,
        bool hasVisitedRequired2,
        Dictionary<(string, bool, bool), long> memo)
    {
        // Base case: reached target
        if (current == target)
        {
            // Valid path only if visited BOTH required nodes
            return (hasVisitedRequired1 && hasVisitedRequired2) ? 1 : 0;
        }

        // Check memo cache
        var state = (current, hasVisitedRequired1, hasVisitedRequired2);
        if (memo.ContainsKey(state))
            return memo[state];

        // Update visited status for current node
        bool visitedRequired1 = hasVisitedRequired1 || (current == required1);
        bool visitedRequired2 = hasVisitedRequired2 || (current == required2);

        // If node has no outgoing edges, no path exists
        if (!graph.ContainsKey(current))
        {
            memo[state] = 0;
            return 0;
        }

        // Count paths through all neighbors
        long totalPaths = 0;
        foreach (var neighbor in graph[current])
        {
            totalPaths += CountPathsWithRequiredNodesHelper(
                graph, neighbor, target, required1, required2,
                visitedRequired1, visitedRequired2, memo);
        }

        // Cache the result
        memo[state] = totalPaths;
        return totalPaths;
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
