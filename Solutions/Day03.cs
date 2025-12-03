namespace AoC2025.Solutions;

/// <summary>
/// Day 3: Lobby - Nalezení maximálního joltage z bank baterií.
/// Každá banka je řádek číslic, kde je třeba zapnout přesně 2 baterie tak,
/// aby výsledný joltage (dvouciferné číslo ze zapnutých baterií) byl maximální.
/// </summary>
public class Day03 : ISolution
{
    /// <summary>
    /// Helper: Finds the maximum joltage by removing 3 positions from a 15-digit bank.
    /// </summary>
    /// <summary>
    /// Helper: Finds the maximum joltage by keeping exactly 12 digits (removing 3) using greedy stack-based approach.
    /// </summary>
    private long FindMaxJoltageGreedy(string bank)
    {
        int toRemove = bank.Length - 12;
        var stack = new System.Collections.Generic.Stack<char>();
        for (int i = 0; i < bank.Length; i++)
        {
            char current = bank[i];
            while (stack.Count > 0 && toRemove > 0 && stack.Peek() < current && (bank.Length - i + stack.Count - 1) >= 12)
            {
                stack.Pop();
                toRemove--;
            }
            stack.Push(current);
        }
        // Stack is in reverse order, so build result
        var result = new System.Text.StringBuilder();
        foreach (var c in stack)
            result.Insert(0, c);
        // Only keep 12 digits (in case stack is longer)
        string max12 = result.ToString().Substring(0, 12);
        return long.Parse(max12);
    }

    /// <summary>
    /// Wrapper: Finds the maximum joltage by keeping exactly 12 digits (removing 3).
    /// </summary>
    private long FindMaxJoltageWithTwelveBatteries(string bank)
    {
        return FindMaxJoltageGreedy(bank);
    }

    /// <summary>
    /// Helper: Creates a number from a bank string with specified positions removed.
    /// </summary>
    private long CreateNumberWithoutPositions(string bank, int[] positionsToRemove)
    {
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < bank.Length; i++)
        {
            if (System.Array.IndexOf(positionsToRemove, i) == -1)
            {
                result.Append(bank[i]);
            }
        }
        return long.Parse(result.ToString());
    }
    public int DayNumber => 3;
    
    public string Title => "Lobby";
    
    /// <summary>
    /// Part 1: Najde maximální joltage pro každou banku baterií a vrátí jejich součet.
    /// </summary>
    /// <param name="input">Vstupní data - každý řádek je banka baterií (string číslic)</param>
    /// <returns>Součet maximálních joltagů jako string</returns>
    public string SolvePart1(string input)
    {
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        long totalJoltage = 0;
        
        foreach (var line in lines)
        {
            string bank = line.Trim();
            if (bank.Length < 2) continue; // Musíme mít alespoň 2 baterie
            
            int maxJoltage = FindMaxJoltage(bank);
            totalJoltage += maxJoltage;
        }
        
        return totalJoltage.ToString();
    }
    
    /// <summary>
    /// Najde maximální možný joltage pro danou banku baterií.
    /// Testuje všechny možné dvojice pozic (i, j) kde i &lt; j.
    /// </summary>
    /// <param name="bank">String číslic reprezentující banku baterií</param>
    /// <returns>Maximální joltage (dvouciferné číslo)</returns>
    private int FindMaxJoltage(string bank)
    {
        int maxJoltage = int.MinValue;
        
        // Projdeme všechny možné dvojice pozic (i, j) kde i < j
        for (int i = 0; i < bank.Length - 1; i++)
        {
            for (int j = i + 1; j < bank.Length; j++)
            {
                // Vypočítáme joltage z této dvojice
                // Převedeme char na int: '7' - '0' = 7
                int digit1 = bank[i] - '0';
                int digit2 = bank[j] - '0';
                int joltage = digit1 * 10 + digit2;
                
                // Sledujeme maximum
                maxJoltage = Math.Max(maxJoltage, joltage);
            }
        }
        
        return maxJoltage;
    }
    
    /// <summary>
    /// Helper: Generates all k-combinations of n positions (0-based).
    /// </summary>
    private IEnumerable<int[]> GetCombinations(int n, int k)
    {
        int[] result = new int[k];
        IEnumerable<int[]> Generate(int start, int depth)
        {
            if (depth == k)
            {
                yield return (int[])result.Clone();
                yield break;
            }
            for (int i = start; i < n; i++)
            {
                result[depth] = i;
                foreach (var combo in Generate(i + 1, depth + 1))
                    yield return combo;
            }
        }
        return Generate(0, 0);
    }

    /// <summary>
    /// Part 2: Najde maximální joltage pro každou banku (12 baterií zapnuto) a vrátí jejich součet.
    /// </summary>
    public string SolvePart2(string input)
    {
        var lines = input.Split(new[] {'\r', '\n'}, System.StringSplitOptions.RemoveEmptyEntries);
        long totalJoltage = 0;
        int lineNum = 0;
        foreach (var line in lines)
        {
            lineNum++;
            string bank = line.Trim();
            if (bank.Length < 15) continue; // must have at least 15 digits for 12 batteries
            long maxJoltage = FindMaxJoltageWithTwelveBatteries(bank);
            totalJoltage += maxJoltage;
        }
        return totalJoltage.ToString();
    }
}
