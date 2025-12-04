namespace AoC2025.Solutions;

/// <summary>
/// Day 4: Printing Department - Nalezení přístupných rolí papíru na mřížce.
/// Role papíru je přístupná vysokozdvižnému vozíku pouze pokud má méně než 4 role
/// v osmi sousedních polích (horizontálně, vertikálně, diagonálně).
/// </summary>
public class Day04 : ISolution
{
    /// <summary>
    /// Směry pro kontrolu 8 sousedních polí (horizontal, vertical, diagonal).
    /// </summary>
    private static readonly (int dr, int dc)[] Directions = 
    {
        (-1, -1), (-1, 0), (-1, 1),  // horní řada
        (0, -1),           (0, 1),   // střední (vlevo, vpravo)
        (1, -1),  (1, 0),  (1, 1)    // dolní řada
    };

    public int DayNumber => 4;
    
    public string Title => "Printing Department";

    /// <summary>
    /// Part 1: Spočítá kolik rolí papíru (@) je přístupných (má méně než 4 role v sousedních polích).
    /// </summary>
    public string SolvePart1(string input)
    {
        var grid = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        int accessibleRolls = 0;
        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] == '@')
                {
                    int neighbors = CountNeighbors(grid, row, col);
                    if (neighbors < 4)
                    {
                        accessibleRolls++;
                    }
                }
            }
        }
        
        return accessibleRolls.ToString();
    }

    /// <summary>
    /// Spočítá počet sousedních rolí papíru (@) pro danou pozici na mřížce.
    /// Kontroluje všech 8 směrů s boundary checkingem.
    /// </summary>
    /// <param name="grid">Mřížka jako pole stringů</param>
    /// <param name="row">Řádek pozice</param>
    /// <param name="col">Sloupec pozice</param>
    /// <returns>Počet sousedních rolí (0-8)</returns>
    private int CountNeighbors(string[] grid, int row, int col)
    {
        int count = 0;
        
        foreach (var (dr, dc) in Directions)
        {
            int newRow = row + dr;
            int newCol = col + dc;
            
            // Boundary check
            if (newRow >= 0 && newRow < grid.Length &&
                newCol >= 0 && newCol < grid[newRow].Length)
            {
                if (grid[newRow][newCol] == '@')
                {
                    count++;
                }
            }
        }
        
        return count;
    }

    /// <summary>
    /// Spočítá počet sousedních rolí papíru (@) pro danou pozici na mřížce.
    /// Verze pro char[][] (mutable grid).
    /// </summary>
    /// <param name="grid">Mřížka jako 2D pole znaků</param>
    /// <param name="row">Řádek pozice</param>
    /// <param name="col">Sloupec pozice</param>
    /// <returns>Počet sousedních rolí (0-8)</returns>
    private int CountNeighborsCharArray(char[][] grid, int row, int col)
    {
        int count = 0;
        
        foreach (var (dr, dc) in Directions)
        {
            int newRow = row + dr;
            int newCol = col + dc;
            
            // Boundary check
            if (newRow >= 0 && newRow < grid.Length &&
                newCol >= 0 && newCol < grid[newRow].Length)
            {
                if (grid[newRow][newCol] == '@')
                {
                    count++;
                }
            }
        }
        
        return count;
    }

    /// <summary>
    /// Najde všechny aktuálně přístupné role na mřížce.
    /// Role je přístupná, pokud má méně než 4 role v 8 sousedních polích.
    /// </summary>
    /// <param name="grid">Mřížka jako 2D pole znaků</param>
    /// <returns>Seznam pozic (row, col) přístupných rolí</returns>
    private List<(int row, int col)> FindAccessibleRolls(char[][] grid)
    {
        var accessible = new List<(int row, int col)>();
        
        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] == '@')
                {
                    int neighbors = CountNeighborsCharArray(grid, row, col);
                    if (neighbors < 4)
                    {
                        accessible.Add((row, col));
                    }
                }
            }
        }
        
        return accessible;
    }

    /// <summary>
    /// Part 2: Simuluje postupné odstraňování přístupných rolí papíru.
    /// Role je přístupná pokud má méně než 4 sousedy. Po odstranění se přepočítají
    /// přístupné role a proces pokračuje dokud existují přístupné role.
    /// </summary>
    public string SolvePart2(string input)
    {
        // Parse do mutable 2D pole
        char[][] grid = input.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                             .Select(line => line.ToCharArray())
                             .ToArray();
        
        int totalRemoved = 0;
        
        // Simulační smyčka
        while (true)
        {
            var accessible = FindAccessibleRolls(grid);
            
            if (accessible.Count == 0)
                break;  // Žádné další role k odstranění
            
            // Odstranění všech přístupných rolí
            foreach (var (row, col) in accessible)
            {
                grid[row][col] = '.';
                totalRemoved++;
            }
        }
        
        return totalRemoved.ToString();
    }
}
