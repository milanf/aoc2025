namespace AoC2025.Solutions;

public class Day07 : ISolution
{
    public int DayNumber => 7;
    
    public string Title => "Laboratories";
    
    public string SolvePart1(string input)
    {
        var grid = ParseGrid(input);
        int splitCount = CountBeamSplits(grid);
        return splitCount.ToString();
    }

    public string SolvePart2(string input)
    {
        return "Not implemented yet";
    }

    private static char[][] ParseGrid(string input)
    {
        return input.Split('\n')
            .Select(line => line.TrimEnd('\r'))
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => line.ToCharArray())
            .ToArray();
    }

    private static (int row, int col) FindStart(char[][] grid)
    {
        for (int r = 0; r < grid.Length; r++)
        {
            for (int c = 0; c < grid[r].Length; c++)
            {
                if (grid[r][c] == 'S')
                    return (r, c);
            }
        }
        throw new InvalidOperationException("Start position 'S' not found");
    }

    private static int CountBeamSplits(char[][] grid)
    {
        var (startRow, startCol) = FindStart(grid);
        
        var queue = new Queue<Beam>();
        var splitPositions = new HashSet<(int row, int col)>();
        var visitedBeams = new HashSet<(int row, int col)>();
        
        // Začínáme na S, první krok je dolů
        queue.Enqueue(new Beam(startRow, startCol));
        visitedBeams.Add((startRow, startCol));
        
        while (queue.Count > 0)
        {
            var beam = queue.Dequeue();
            
            // Posun dolů
            int newRow = beam.Row + 1;
            int newCol = beam.Col;
            
            // Kontrola hranic
            if (newRow >= grid.Length)
                continue;
            
            if (newCol < 0 || newCol >= grid[newRow].Length)
                continue;
            
            // Kontrola, zda jsme na této pozici již byli
            if (visitedBeams.Contains((newRow, newCol)))
                continue;
            
            visitedBeams.Add((newRow, newCol));
            
            char cell = grid[newRow][newCol];
            
            if (cell == '^')
            {
                // SPLIT! Počítáme pouze pokud jsme tento splitter ještě neviděli
                splitPositions.Add((newRow, newCol));
                
                // Levý paprsek
                int leftCol = newCol - 1;
                if (leftCol >= 0)
                {
                    queue.Enqueue(new Beam(newRow, leftCol));
                }
                
                // Pravý paprsek
                int rightCol = newCol + 1;
                if (rightCol < grid[newRow].Length)
                {
                    queue.Enqueue(new Beam(newRow, rightCol));
                }
            }
            else if (cell == '.')
            {
                // Volné místo - paprsek pokračuje dolů
                queue.Enqueue(new Beam(newRow, newCol));
            }
            // Pokud cell == 'S', je to chyba v logice (neměli bychom se sem dostat znovu)
        }
        
        return splitPositions.Count;
    }

    private record Beam(int Row, int Col);
}
