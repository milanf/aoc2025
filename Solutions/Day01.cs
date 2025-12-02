namespace AoC2025.Solutions;

/// <summary>
/// Day 1: Template/Example implementace.
/// Tento soubor slouží jako vzor pro další dny.
/// </summary>
public class Day01 : ISolution
{
    public int DayNumber => 1;
    
    public string Title => "Secret Entrance";
    
    public string SolvePart1(string input)
    {
        // Part 1: Secret Entrance
        int position = 50;
        int zeroCount = 0;
        var lines = input.Split('\n');
        foreach (var rawLine in lines)
        {
            var instr = ParseInstruction(rawLine);
            if (instr == null) continue;
            position = RotateDial(position, instr.Value.direction, instr.Value.distance);
            if (position == 0) zeroCount++;
        }
        return zeroCount.ToString();
    }
    
    public string SolvePart2(string input)
    {
        // Part 2: CLICK Method - Count ALL zero crossings during rotation
        int position = 50;
        int zeroCount = 0;
        var lines = input.Split('\n');
        
        foreach (var rawLine in lines)
        {
            var instr = ParseInstruction(rawLine);
            if (instr == null) continue;
            
            // Count zero crossings during this rotation
            zeroCount += CountZeroCrossings(position, instr.Value.direction, instr.Value.distance);
            
            // Update position
            position = RotateDial(position, instr.Value.direction, instr.Value.distance);
        }
        
        return zeroCount.ToString();
    }
    
    // Pomocné metody můžeš přidávat zde
    /// <summary>
    /// Counts how many times the dial passes through 0 during a rotation.
    /// Uses explicit step-by-step simulation for clarity.
    /// </summary>
    /// <param name="start">Starting dial position</param>
    /// <param name="dir">Direction ('L' or 'R')</param>
    /// <param name="dist">Distance to rotate</param>
    /// <returns>Number of times dial passes through 0</returns>
    private static int CountZeroCrossings(int start, char dir, int dist)
    {
        int count = 0;
        int pos = start;
        int step = dir == 'L' ? -1 : 1;
        
        for (int i = 0; i < dist; i++)
        {
            pos = ((pos + step) % 100 + 100) % 100;
            if (pos == 0) count++;
        }
        
        return count;
    }
    
    /// <summary>
    /// Rotates the dial according to direction and distance, with cyclic wrap (0-99).
    /// </summary>
    /// <param name="current">Current dial position</param>
    /// <param name="dir">Direction ('L' or 'R')</param>
    /// <param name="dist">Distance to rotate</param>
    /// <returns>New dial position</returns>
    private static int RotateDial(int current, char dir, int dist)
    {
        int offset = dir == 'L' ? -dist : dist;
        return ((current + offset) % 100 + 100) % 100;
    }
        /// <summary>
        /// Parses a single instruction line into direction and distance.
        /// </summary>
        /// <param name="line">Instruction line (e.g. "L68")</param>
        /// <returns>Tuple of direction (L/R) and distance</returns>
        private static (char direction, int distance)? ParseInstruction(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;
            line = line.Trim();
            if (line.Length < 2) return null;
            char direction = line[0];
            if (direction != 'L' && direction != 'R') return null;
            if (!int.TryParse(line[1..], out int distance)) return null;
            return (direction, distance);
        }
    // private int ParseNumber(string line) { ... }
}

