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
        var lines = input.Split('\n')
            .Select(l => l.Replace("\r", ""))
            .Where(l => l.Length > 0)
            .ToArray();
        
        // Split into number rows (g0) and operation row (g2)
        var g0 = lines[..^1];
        var g2 = lines[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        // Python: rotated = list(zip(*g0))[::-1]
        var rotated = Rotate90Clockwise(g0);
        
        // Python: groupby(rotated, key=lambda row: ''.join(row).strip() != '')
        var result = new List<List<long>>();
        var currentGroup = new List<long>();
        
        foreach (var row in rotated)
        {
            var rowStr = new string(row).Trim();
            bool isNonEmpty = !string.IsNullOrEmpty(rowStr);
            
            if (isNonEmpty && long.TryParse(rowStr, out var num))
            {
                currentGroup.Add(num);
            }
            else if (currentGroup.Count > 0)
            {
                result.Add(new List<long>(currentGroup));
                currentGroup.Clear();
            }
        }
        
        if (currentGroup.Count > 0)
        {
            result.Add(currentGroup);
        }
        
        // Python: g2_r = g2[::-1]
        var g2_r = g2.Reverse().ToArray();
        
        // Python: sum(reduce(ops[g2_r[idx]], col) for idx, col in enumerate(result))
        var ops = new Dictionary<string, Func<long, long, long>>
        {
            ["+"] = (a, b) => a + b,
            ["*"] = (a, b) => a * b
        };
        
        long grandTotal = 0;
        for (int idx = 0; idx < result.Count && idx < g2_r.Length; idx++)
        {
            var col = result[idx];
            var op = g2_r[idx];
            
            if (ops.TryGetValue(op, out var operation))
            {
                long colResult = col.Aggregate(operation);
                grandTotal += colResult;
            }
        }
        
        return grandTotal.ToString();
    }
    
    private static char[][] Rotate90Clockwise(string[] lines)
    {
        // Find max length
        int maxLength = lines.Max(l => l.Length);
        
        // Pad each line to LEFT-align numbers (pad RIGHT with spaces)
        var paddedLines = lines.Select(l => l.PadRight(maxLength)).ToArray();
        
        // Transpose: zip(*g0)
        var transposed = new List<char[]>();
        for (int col = 0; col < maxLength; col++)
        {
            var column = new char[paddedLines.Length];
            for (int row = 0; row < paddedLines.Length; row++)
            {
                column[row] = paddedLines[row][col];
            }
            transposed.Add(column);
        }
        
        // Reverse to complete 90° clockwise rotation: [::-1]
        transposed.Reverse();
        
        return transposed.ToArray();
    }
    
    private static List<MathProblem> GetMathProblemsPart2(string[] input)
    {
        var mathProblems = new List<MathProblem>();
        var unparsedMathProblems = GetUnparsedMathProblems(input);
        
        foreach (var unparsedMathProblem in unparsedMathProblems)
        {
            int maxDigitCount = unparsedMathProblem.NumberStrings.Max(n => n.Length);
            var numberStringsRightToLeft = new string[maxDigitCount];
            for (int i = 0; i < maxDigitCount; i++)
            {
                numberStringsRightToLeft[i] = "";
            }
            
            for (int j = 0; j < unparsedMathProblem.NumberStrings.Count; j++)
            {
                for (int i = 0; i < maxDigitCount; i++)
                {
                    string n = unparsedMathProblem.NumberStrings[j];
                    if (n.Length > i)
                    {
                        numberStringsRightToLeft[i] += n[i];
                    }
                }
            }
            
            mathProblems.Add(new MathProblem
            {
                Numbers = numberStringsRightToLeft
                    .Select(s => s.Replace(" ", ""))
                    .Where(s => s.Length > 0)
                    .Select(long.Parse)
                    .ToList(),
                Operation = unparsedMathProblem.Operation
            });
        }
        
        return mathProblems;
    }
    
    private static List<UnparsedProblem> GetUnparsedMathProblems(string[] input)
    {
        var unparsedmathProblems = new List<UnparsedProblem>();
        List<string>? numberStrings = null;
        UnparsedProblem? unparsedMathProblem = null;
        int maxLineLength = input.Max(l => l.Length);
        
        for (int i = 0; i < maxLineLength; i++)
        {
            if (unparsedMathProblem == null)
            {
                char operand = i < input[^1].Length ? input[^1][i] : ' ';
                unparsedMathProblem = new UnparsedProblem
                {
                    NumberStrings = new List<string>(),
                    Operation = operand
                };
                numberStrings = Enumerable.Range(0, input.Length - 1).Select(_ => "").ToList();
            }
            
            bool onlySpacesFound = true;
            for (int j = 0; j < input.Length - 1; j++)
            {
                char ch = i < input[j].Length ? input[j][i] : ' ';
                if (ch != ' ')
                {
                    onlySpacesFound = false;
                }
                numberStrings![j] += ch;
            }
            
            if (onlySpacesFound)
            {
                for (int j = 0; j < numberStrings!.Count; j++)
                {
                    if (numberStrings[j].Length > 0)
                    {
                        numberStrings[j] = numberStrings[j].Substring(0, numberStrings[j].Length - 1);
                    }
                }
            }
            
            if (onlySpacesFound || i == maxLineLength - 1)
            {
                unparsedmathProblems.Add(new UnparsedProblem
                {
                    NumberStrings = numberStrings!.ToList(),
                    Operation = unparsedMathProblem.Operation
                });
                unparsedMathProblem = null;
            }
        }
        
        return unparsedmathProblems;
    }
    
    private static List<long> SolveMathProblems(List<MathProblem> mathProblems)
    {
        return mathProblems.Select(problem =>
        {
            if (problem.Operation == '*')
            {
                return problem.Numbers.Aggregate(1L, (product, n) => product * n);
            }
            return problem.Numbers.Aggregate(0L, (sum, n) => sum + n);
        }).ToList();
    }
    
    private class MathProblem
    {
        public required List<long> Numbers { get; init; }
        public required char Operation { get; init; }
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
    
    private record UnparsedProblem
    {
        public required List<string> NumberStrings { get; set; }
        public required char Operation { get; set; }
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
