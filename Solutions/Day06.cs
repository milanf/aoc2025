namespace AoC2025.Solutions;

public class Day06 : ISolution
{
    public int DayNumber => 6;
    
    public string Title => "Trash Compactor";
    
    public string SolvePart1(string input)
    {
        // 1. Parse input
        var (numberRows, operations) = ParseInput(input);
        
        // 2. Extract problems
        var problems = ExtractProblems(numberRows, operations);
        
        // 3. Solve all problems and sum results
        long grandTotal = 0;
        foreach (var problem in problems)
        {
            long result = SolveProblem(problem);
            grandTotal += result;
        }
        
        return grandTotal.ToString();
    }

    public string SolvePart2(string input)
    {
        return "Not implemented";
    }
    
    private static (List<long>[] numberRows, char[] operations) ParseInput(string input)
    {
        var lines = input.Split('\n')
            .Select(l => l.TrimEnd())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();
        
        // Poslední řádek = operace
        var operationLine = lines[^1];
        var operations = operationLine
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s[0])
            .ToArray();
        
        // Všechny řádky kromě posledního = čísla
        int numberRowCount = lines.Length - 1;
        var numberRows = new List<long>[numberRowCount];
        for (int i = 0; i < numberRowCount; i++)
        {
            numberRows[i] = lines[i]
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .ToList();
        }
        
        return (numberRows, operations);
    }
    
    private static List<Problem> ExtractProblems(List<long>[] numberRows, char[] operations)
    {
        var problems = new List<Problem>();
        int columnCount = operations.Length;
        int rowCount = numberRows.Length;
        
        for (int col = 0; col < columnCount; col++)
        {
            var numbers = new List<long>();
            for (int row = 0; row < rowCount; row++)
            {
                numbers.Add(numberRows[row][col]);
            }
            
            problems.Add(new Problem(numbers, operations[col]));
        }
        
        return problems;
    }
    
    private static long SolveProblem(Problem problem)
    {
        long result = problem.Numbers[0];
        
        for (int i = 1; i < problem.Numbers.Count; i++)
        {
            result = problem.Operation switch
            {
                '+' => result + problem.Numbers[i],
                '*' => result * problem.Numbers[i],
                _ => throw new InvalidOperationException($"Unknown operation: {problem.Operation}")
            };
        }
        
        return result;
    }
    
    private record Problem(List<long> Numbers, char Operation);
}
