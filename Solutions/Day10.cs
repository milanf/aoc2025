using Google.OrTools.LinearSolver;

namespace AoC2025.Solutions;

/// <summary>
/// Day 10: Factory - Minimální počet stisknutí tlačítek pro inicializaci strojů.
/// Part 1: Řeší soustavu lineárních rovnic nad Galois Field GF(2) pomocí Gaussian elimination.
/// Part 2: Řeší Integer Linear Programming (ILP) problém pro joltage countery.
/// </summary>
public class Day10 : ISolution
{
    public int DayNumber => 10;
    
    public string Title => "Factory";
    
    public string SolvePart1(string input)
    {
        var machines = ParseInput(input);
        int totalPresses = 0;
        
        foreach (var machine in machines)
        {
            int minPresses = SolveMachine(machine);
            totalPresses += minPresses;
        }
        
        return totalPresses.ToString();
    }
    
    public string SolvePart2(string input)
    {
        var machines = ParseInputPart2(input);
        long totalPresses = 0;
        int machineIndex = 0;
        
        foreach (var machine in machines)
        {
            machineIndex++;
            try
            {
                int minPresses = SolveJoltageMachine(machine);
                totalPresses += minPresses;                                
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"CHYBA na stroji {machineIndex}: {ex.Message}");
                throw;
            }
        }
                
        return totalPresses.ToString();
    }
    
    /// <summary>
    /// Reprezentuje jeden stroj s indicator lights a tlačítky.
    /// </summary>
    private class Machine
    {
        public bool[] TargetLights { get; set; } = Array.Empty<bool>();
        public List<int[]> Buttons { get; set; } = new();
    }
    
    /// <summary>
    /// Reprezentuje stroj pro Part 2 s joltage countery.
    /// </summary>
    private class MachineJoltage
    {
        public int[] TargetJoltage { get; set; } = Array.Empty<int>();
        public List<int[]> Buttons { get; set; } = new();
    }
    
    /// <summary>
    /// Parsuje vstupní soubor a vrátí seznam strojů.
    /// Formát: [.##.] (3) (1,3) (2) {3,5,4}
    /// </summary>
    private List<Machine> ParseInput(string input)
    {
        var machines = new List<Machine>();
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;
            
            var machine = new Machine();
            
            // Parse diagram světel [.##.]
            int bracketStart = trimmedLine.IndexOf('[');
            int bracketEnd = trimmedLine.IndexOf(']');
            if (bracketStart == -1 || bracketEnd == -1) continue;
            
            string diagram = trimmedLine.Substring(bracketStart + 1, bracketEnd - bracketStart - 1);
            machine.TargetLights = diagram.Select(c => c == '#').ToArray();
            
            // Parse tlačítka (0,3,4) (1,2)
            string afterBracket = trimmedLine.Substring(bracketEnd + 1);
            
            // Najdi všechny výskyty (...)
            int pos = 0;
            while (pos < afterBracket.Length)
            {
                int openParen = afterBracket.IndexOf('(', pos);
                if (openParen == -1) break;
                
                int closeParen = afterBracket.IndexOf(')', openParen);
                if (closeParen == -1) break;
                
                // Kontrola, zda není součástí joltage {...}
                if (openParen > 0 && afterBracket[openParen - 1] == '{')
                {
                    pos = closeParen + 1;
                    continue;
                }
                
                string buttonContent = afterBracket.Substring(openParen + 1, closeParen - openParen - 1);
                
                // Pokud obsahuje čárky, je to tlačítko
                if (buttonContent.Contains(',') || char.IsDigit(buttonContent.FirstOrDefault()))
                {
                    var lightIndices = buttonContent.Split(',')
                        .Select(s => s.Trim())
                        .Where(s => !string.IsNullOrEmpty(s))
                        .Select(int.Parse)
                        .ToArray();
                    
                    if (lightIndices.Length > 0)
                    {
                        machine.Buttons.Add(lightIndices);
                    }
                }
                
                pos = closeParen + 1;
            }
            
            if (machine.TargetLights.Length > 0 && machine.Buttons.Count > 0)
            {
                machines.Add(machine);
            }
        }
        
        return machines;
    }
    
    /// <summary>
    /// Řeší jeden stroj pomocí Gaussian elimination nad GF(2).
    /// Vrací minimální počet stisknutí tlačítek.
    /// </summary>
    private int SolveMachine(Machine machine)
    {
        int numLights = machine.TargetLights.Length;
        int numButtons = machine.Buttons.Count;
        
        // Vytvoř augmentovanou matici [A | b]
        // A[i,j] = 1 pokud tlačítko i ovlivňuje světlo j
        // b[j] = cílový stav světla j
        bool[,] augMatrix = new bool[numButtons, numLights + 1];
        
        // Naplň matici A
        for (int i = 0; i < numButtons; i++)
        {
            foreach (int lightIndex in machine.Buttons[i])
            {
                if (lightIndex < numLights)
                {
                    augMatrix[i, lightIndex] = true;
                }
            }
        }
        
        // Naplň vektor b (cílový stav)
        for (int j = 0; j < numLights; j++)
        {
            augMatrix[numButtons - 1, numLights] = false; // Inicializace
        }
        
        // Přidej cílový stav jako poslední sloupec
        for (int i = 0; i < numButtons; i++)
        {
            augMatrix[i, numLights] = false; // Začátek s nulovým stavem
        }
        
        // Musíme vyřešit A*x = b, kde b je targetLights
        // Pro Gaussian elimination nad GF(2) použijeme transpozici
        // protože hledáme, která tlačítka stisknout
        
        // Zkusíme jednodušší přístup: backtracking pro malé stroje
        // nebo použijeme heuristiku
        
        // Pro tento problém použijeme backtracking s bitmasking
        int minPresses = int.MaxValue;
        
        // Zkusíme všechny kombinace tlačítek (2^numButtons možností)
        // Pro numButtons <= 20 je to proveditelné
        if (numButtons <= 20)
        {
            minPresses = SolveByBruteForce(machine);
        }
        else
        {
            // Pro větší počty použijeme Gaussian elimination
            minPresses = SolveByGaussianElimination(machine);
        }
        
        return minPresses;
    }
    
    /// <summary>
    /// Řeší stroj hrubou silou (pro malé stroje).
    /// Zkouší všechny kombinace stisknutí tlačítek.
    /// </summary>
    private int SolveByBruteForce(Machine machine)
    {
        int numButtons = machine.Buttons.Count;
        int numLights = machine.TargetLights.Length;
        int minPresses = int.MaxValue;
        
        // Zkusíme všechny kombinace (0 = nestisknout, 1 = stisknout jednou)
        for (int mask = 0; mask < (1 << numButtons); mask++)
        {
            bool[] currentState = new bool[numLights];
            int pressCount = 0;
            
            // Aplikuj tlačítka podle masky
            for (int i = 0; i < numButtons; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    pressCount++;
                    // Toggle všechna světla ovlivněná tímto tlačítkem
                    foreach (int lightIndex in machine.Buttons[i])
                    {
                        if (lightIndex < numLights)
                        {
                            currentState[lightIndex] = !currentState[lightIndex];
                        }
                    }
                }
            }
            
            // Kontrola, zda odpovídá cílovému stavu
            bool matches = true;
            for (int j = 0; j < numLights; j++)
            {
                if (currentState[j] != machine.TargetLights[j])
                {
                    matches = false;
                    break;
                }
            }
            
            if (matches)
            {
                minPresses = Math.Min(minPresses, pressCount);
            }
        }
        
        return minPresses == int.MaxValue ? 0 : minPresses;
    }
    
    /// <summary>
    /// Řeší stroj pomocí Gaussian elimination nad GF(2).
    /// Složitější, ale efektivnější pro velké stroje.
    /// </summary>
    private int SolveByGaussianElimination(Machine machine)
    {
        int numLights = machine.TargetLights.Length;
        int numButtons = machine.Buttons.Count;
        
        // Vytvoř matici (transpozice - řádky jsou světla, sloupce jsou tlačítka)
        bool[,] matrix = new bool[numLights, numButtons + 1];
        
        // Naplň matici
        for (int btn = 0; btn < numButtons; btn++)
        {
            foreach (int lightIdx in machine.Buttons[btn])
            {
                if (lightIdx < numLights)
                {
                    matrix[lightIdx, btn] = true;
                }
            }
        }
        
        // Poslední sloupec je cílový stav
        for (int light = 0; light < numLights; light++)
        {
            matrix[light, numButtons] = machine.TargetLights[light];
        }
        
        // Gaussian elimination
        int currentRow = 0;
        for (int col = 0; col < numButtons && currentRow < numLights; col++)
        {
            // Najdi pivot
            int pivotRow = -1;
            for (int row = currentRow; row < numLights; row++)
            {
                if (matrix[row, col])
                {
                    pivotRow = row;
                    break;
                }
            }
            
            if (pivotRow == -1) continue; // Žádný pivot v tomto sloupci
            
            // Vyměň řádky
            if (pivotRow != currentRow)
            {
                for (int c = 0; c <= numButtons; c++)
                {
                    bool temp = matrix[currentRow, c];
                    matrix[currentRow, c] = matrix[pivotRow, c];
                    matrix[pivotRow, c] = temp;
                }
            }
            
            // Eliminuj ostatní řádky
            for (int row = 0; row < numLights; row++)
            {
                if (row != currentRow && matrix[row, col])
                {
                    // XOR operace (sčítání v GF(2))
                    for (int c = 0; c <= numButtons; c++)
                    {
                        matrix[row, c] ^= matrix[currentRow, c];
                    }
                }
            }
            
            currentRow++;
        }
        
        // Zpětná substituce - najdi řešení
        bool[] solution = new bool[numButtons];
        for (int row = numLights - 1; row >= 0; row--)
        {
            // Najdi vedoucí 1
            int leadingCol = -1;
            for (int col = 0; col < numButtons; col++)
            {
                if (matrix[row, col])
                {
                    leadingCol = col;
                    break;
                }
            }
            
            if (leadingCol != -1)
            {
                solution[leadingCol] = matrix[row, numButtons];
            }
        }
        
        // Spočítej počet stisknutí
        int pressCount = solution.Count(x => x);
        
        // Verifikuj řešení
        bool[] resultState = new bool[numLights];
        for (int btn = 0; btn < numButtons; btn++)
        {
            if (solution[btn])
            {
                foreach (int lightIdx in machine.Buttons[btn])
                {
                    if (lightIdx < numLights)
                    {
                        resultState[lightIdx] = !resultState[lightIdx];
                    }
                }
            }
        }
        
        // Kontrola, zda řešení funguje
        bool valid = true;
        for (int i = 0; i < numLights; i++)
        {
            if (resultState[i] != machine.TargetLights[i])
            {
                valid = false;
                break;
            }
        }
        
        if (!valid)
        {
            // Fallback na brute force
            return SolveByBruteForce(machine);
        }
        
        return pressCount;
    }
    
    /// <summary>
    /// Parsuje vstup pro Part 2 - extrahuje joltage hodnoty místo světel.
    /// Formát: [.##.] (3) (1,3) (2) {3,5,4,7}
    /// Ignoruje diagram světel, čte pouze tlačítka a joltage cíle.
    /// </summary>
    private List<MachineJoltage> ParseInputPart2(string input)
    {
        var machines = new List<MachineJoltage>();
        var lines = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine)) continue;
            
            var machine = new MachineJoltage();
            
            // Rozdělíme na mezery (stejně jako Python: line.split(" "))
            var parts = trimmedLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length < 3) continue; // Musíme mít alespoň: [diagram] (button) {joltage}
            
            // První část je diagram světel [.##.] - ignorujeme
            // Poslední část je joltage {3,5,4,7}
            // Prostřední části jsou tlačítka (0,1) (2,3) ...
            
            // Parse joltage z poslední části
            string joltPart = parts[^1]; // Poslední element
            if (joltPart.StartsWith("{") && joltPart.EndsWith("}"))
            {
                string joltageContent = joltPart.Substring(1, joltPart.Length - 2);
                machine.TargetJoltage = joltageContent.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Select(int.Parse)
                    .ToArray();
            }
            else
            {
                continue; // Není validní formát
            }
            
            // Parse tlačítka ze všech částí mezi první a poslední (parts[1] až parts[^2])
            for (int i = 1; i < parts.Length - 1; i++)
            {
                string part = parts[i];
                if (part.StartsWith("(") && part.EndsWith(")"))
                {
                    string buttonContent = part.Substring(1, part.Length - 2);
                    if (!string.IsNullOrWhiteSpace(buttonContent))
                    {
                        var indices = buttonContent.Split(',')
                            .Select(s => s.Trim())
                            .Where(s => !string.IsNullOrEmpty(s) && s.All(char.IsDigit))
                            .Select(int.Parse)
                            .ToArray();
                        
                        if (indices.Length > 0)
                        {
                            machine.Buttons.Add(indices);
                        }
                    }
                }
            }
            
            if (machine.TargetJoltage.Length > 0 && machine.Buttons.Count > 0)
            {
                machines.Add(machine);
            }
        }
        
        return machines;
    }
    
    /// <summary>
    /// Řeší joltage stroj pomocí ILP (Integer Linear Programming) s OR-Tools.
    /// Minimalizuje součet stisknutí tlačítek s omezením, že každý counter dosáhne cílové hodnoty.
    /// </summary>
    private int SolveJoltageMachine(MachineJoltage machine)
    {
        int numButtons = machine.Buttons.Count;
        int numCounters = machine.TargetJoltage.Length;
        
        // Vytvoř SCIP solver
        Solver solver = Solver.CreateSolver("SCIP");
        if (solver == null)
        {
            throw new Exception("SCIP solver nelze vytvořit!");
        }
        
        // Nastavení parametrů solveru
        solver.SetTimeLimit(10000); // 10 sekund timeout
        
        // Proměnné: x[i] = počet stisknutí tlačítka i
        Variable[] x = new Variable[numButtons];
        int maxTarget = machine.TargetJoltage.Max();
        int sumTarget = machine.TargetJoltage.Sum();
        
        // Horní hranice: v nejhorším případě každý counter potřebuje vlastní tlačítko
        // Což může být až sumTarget pro každé tlačítko
        int upperBound = sumTarget * 3; // Zvětšil jsem na 3× kvůli jistotě
        
        for (int i = 0; i < numButtons; i++)
        {
            x[i] = solver.MakeIntVar(0, upperBound, $"button_{i}");
        }
        
        // Omezení: pro každý counter c, Σ(x[i] * A[i,c]) = target[c]
        // kde A[i,c] = 1 pokud tlačítko i ovlivňuje counter c, jinak 0
        for (int c = 0; c < numCounters; c++)
        {
            Constraint constraint = solver.MakeConstraint(machine.TargetJoltage[c], machine.TargetJoltage[c]);
            
            for (int i = 0; i < numButtons; i++)
            {
                // Zjisti, zda tlačítko i ovlivňuje counter c
                if (machine.Buttons[i].Contains(c))
                {
                    constraint.SetCoefficient(x[i], 1);
                }
            }
        }
        
        // Cílová funkce: minimize Σx[i]
        Objective objective = solver.Objective();
        for (int i = 0; i < numButtons; i++)
        {
            objective.SetCoefficient(x[i], 1);
        }
        objective.SetMinimization();
        
        // Řeš
        Solver.ResultStatus status = solver.Solve();
        
        if (status != Solver.ResultStatus.OPTIMAL)
        {
            throw new Exception($"ILP solver nenašel OPTIMÁLNÍ řešení, stav: {status}");
        }
        
        // Spočítej skutečnou sumu stisknutí (místo objective.Value())
        int totalPresses = 0;
        for (int i = 0; i < numButtons; i++)
        {
            totalPresses += (int)x[i].SolutionValue();
        }
        
        return totalPresses;
    }
}
