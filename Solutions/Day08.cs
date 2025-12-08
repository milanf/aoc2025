namespace AoC2025.Solutions;

public class Day08 : ISolution
{
    public int DayNumber => 8;
    public string Title => "Playground";

    public string SolvePart1(string input)
    {
        // Parse junction boxes
        var boxes = ParseJunctionBoxes(input);
        int n = boxes.Length;
        
        // Build and sort edges by distance
        var edges = BuildAndSortEdges(boxes);
        
        // Determine number of connections to ATTEMPT (not necessarily successful)
        int connectionsToAttempt = n == 1000 ? 1000 : n / 2;
        
        // Perform Kruskal's algorithm
        var uf = PerformKruskalWithAttempts(edges, n, connectionsToAttempt);
        
        // Get circuit sizes
        var circuitSizes = GetCircuitSizes(uf, n);
        
        // Calculate product of top three circuits
        long result = GetTopThreeProduct(circuitSizes);
        
        return result.ToString();
    }

    public string SolvePart2(string input)
    {
        // Parse junction boxes
        var boxes = ParseJunctionBoxes(input);
        int n = boxes.Length;
        
        // Build and sort edges by distance
        var edges = BuildAndSortEdges(boxes);
        
        // Kruskal's algorithm - connect until we have ONE circuit (n-1 edges)
        var unionFind = new UnionFind(n);
        Edge? lastAddedEdge = null;
        int edgesAdded = 0;
        int requiredEdges = n - 1; // For n vertices, need n-1 edges for spanning tree
        
        foreach (var edge in edges)
        {
            // Try to connect two junction boxes
            if (unionFind.Union(edge.Box1, edge.Box2))
            {
                // Connection successful (weren't in same circuit)
                lastAddedEdge = edge;
                edgesAdded++;
                
                // Have complete MST?
                if (edgesAdded == requiredEdges)
                {
                    break;
                }
            }
            // If Union returned false, they're already in same circuit → skip
        }
        
        // Calculate result: X₁ × X₂ of last connected pair
        if (lastAddedEdge == null)
            throw new InvalidOperationException("Failed to find last edge!");
        
        var box1 = boxes[lastAddedEdge.Box1];
        var box2 = boxes[lastAddedEdge.Box2];
        
        long result = (long)box1.X * box2.X;
        
        return result.ToString();
    }

    private JunctionBox[] ParseJunctionBoxes(string input)
    {
        return input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select((line, id) =>
            {
                var parts = line.Split(',');
                return new JunctionBox(
                    id,
                    int.Parse(parts[0]),
                    int.Parse(parts[1]),
                    int.Parse(parts[2])
                );
            })
            .ToArray();
    }

    private long CalculateDistanceSquared(JunctionBox a, JunctionBox b)
    {
        long dx = a.X - b.X;
        long dy = a.Y - b.Y;
        long dz = a.Z - b.Z;
        return dx * dx + dy * dy + dz * dz;
    }

    private List<Edge> BuildAndSortEdges(JunctionBox[] boxes)
    {
        int n = boxes.Length;
        var edges = new List<Edge>(n * (n - 1) / 2);
        
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                long distanceSquared = CalculateDistanceSquared(boxes[i], boxes[j]);
                edges.Add(new Edge(i, j, distanceSquared));
            }
        }
        
        edges.Sort();
        return edges;
    }

    private UnionFind PerformKruskalWithAttempts(List<Edge> edges, int n, int attemptsToMake)
    {
        var uf = new UnionFind(n);
        int attemptsMade = 0;
        
        foreach (var edge in edges)
        {
            if (attemptsMade >= attemptsToMake)
                break;
            
            // Try to union - whether successful or not, it counts as an attempt
            uf.Union(edge.Box1, edge.Box2);
            attemptsMade++;
        }
        
        return uf;
    }

    private Dictionary<int, int> GetCircuitSizes(UnionFind uf, int n)
    {
        var circuitSizes = new Dictionary<int, int>();
        
        for (int i = 0; i < n; i++)
        {
            int root = uf.Find(i);
            circuitSizes[root] = circuitSizes.GetValueOrDefault(root, 0) + 1;
        }
        
        return circuitSizes;
    }

    private long GetTopThreeProduct(Dictionary<int, int> circuitSizes)
    {
        var sorted = circuitSizes.Values
            .OrderByDescending(size => size)
            .ToArray();
        
        // Ensure we have at least 3 values by padding with 1s if needed
        long first = sorted.Length > 0 ? sorted[0] : 1;
        long second = sorted.Length > 1 ? sorted[1] : 1;
        long third = sorted.Length > 2 ? sorted[2] : 1;
        
        return first * second * third;
    }

    // Inner classes and records
    private record JunctionBox(int Id, int X, int Y, int Z);

    private record Edge(int Box1, int Box2, long DistanceSquared) : IComparable<Edge>
    {
        public int CompareTo(Edge? other) => 
            other == null ? 1 : DistanceSquared.CompareTo(other.DistanceSquared);
    }

    private class UnionFind
    {
        private readonly int[] _parent;
        private readonly int[] _rank;
        
        public UnionFind(int size)
        {
            _parent = Enumerable.Range(0, size).ToArray();
            _rank = new int[size];
        }
        
        public int Find(int x)
        {
            if (_parent[x] != x)
                _parent[x] = Find(_parent[x]); // Path compression
            return _parent[x];
        }
        
        public bool Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);
            
            if (rootX == rootY)
                return false; // Already in same set
            
            // Union by rank
            if (_rank[rootX] < _rank[rootY])
                _parent[rootX] = rootY;
            else if (_rank[rootX] > _rank[rootY])
                _parent[rootY] = rootX;
            else
            {
                _parent[rootY] = rootX;
                _rank[rootX]++;
            }
            
            return true;
        }
    }
}
