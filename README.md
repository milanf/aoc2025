# 🎄 Advent of Code 2025 - C# Runner

Console aplikace pro spouštění řešení Advent of Code 2025 v C#.

## 🚀 Quick Start

### Prerekvizity

- **.NET 10 SDK** - [stáhnout zde](https://dotnet.microsoft.com/download/dotnet/10.0)

### Spuštění

```bash
cd AoC2025.Console
dotnet run
```

Nebo spusť konkrétní den přímo:

```bash
dotnet run -- 1  # Spustí Day 1
```

### Testy

```bash
dotnet test
```

---

## 📁 Struktura projektu

```
aoc2025/
├── AoC2025.Console/          # Main console aplikace
│   ├── Program.cs            # Runner logika, menu, reflection
│   ├── ISolution.cs          # Interface pro všechna řešení
│   └── AoC2025.Console.csproj
├── AoC2025.Tests/            # xUnit testy
│   ├── Day01Tests.cs         # Unit testy pro Day 1
│   ├── TestData/             # Example inputy z AoC zadání
│   │   └── day01_example.txt
│   └── AoC2025.Tests.csproj
├── Solutions/                # Řešení jednotlivých dnů
│   ├── Day01.cs              # Day 1 implementace
│   ├── Day02.cs              # Day 2 implementace
│   └── ...
├── Inputs/                   # Input data (tvoje osobní inputy z AoC)
│   ├── day01.txt
│   ├── day02.txt
│   └── ...
└── aoc2025.sln               # Solution file
```

---

## ➕ Jak přidat nový den

### 1. Vytvoř řešení

Vytvoř nový soubor v `Solutions/` složce (např. `Day02.cs`):

```csharp
namespace AoC2025.Solutions;

public class Day02 : ISolution
{
    public int DayNumber => 2;
    public string Title => "Název puzzle z AoC"; // Z zadání
    
    public string SolvePart1(string input)
    {
        // Tvoje implementace Part 1
        var lines = input.Split('\n');
        // ...
        return result.ToString();
    }
    
    public string SolvePart2(string input)
    {
        // Tvoje implementace Part 2
        return result.ToString();
    }
}
```

### 2. Přidej input data

Stáhni svůj osobní input z [adventofcode.com](https://adventofcode.com/2025) a ulož ho do:

```
Inputs/day02.txt
```

### 3. Spusť!

```bash
dotnet run
```

Runner automaticky detekuje nový den pomocí reflection a nabídne ho v menu. **Žádné změny v `Program.cs` nejsou potřeba!**

---

## 🧪 Testování

### Přidání testů pro nový den

1. Vytvoř `AoC2025.Tests/Day02Tests.cs`:

```csharp
using AoC2025.Solutions;

namespace AoC2025.Tests;

public class Day02Tests
{
    private readonly Day02 _solution;

    public Day02Tests()
    {
        _solution = new Day02();
    }

    [Fact]
    public void Part1_ExampleInput_ReturnsExpectedResult()
    {
        var input = File.ReadAllText(Path.Combine("TestData", "day02_example.txt"));
        var result = _solution.SolvePart1(input);
        Assert.Equal("EXPECTED_VALUE", result); // Z AoC example
    }

    [Fact]
    public void Part2_ExampleInput_ReturnsExpectedResult()
    {
        var input = File.ReadAllText(Path.Combine("TestData", "day02_example.txt"));
        var result = _solution.SolvePart2(input);
        Assert.Equal("EXPECTED_VALUE", result); // Z AoC example
    }
}
```

2. Přidej example input do `AoC2025.Tests/TestData/day02_example.txt`
   - Použij **example input z AoC zadání** (ne tvůj osobní input)
   - Example inputy mají známé výsledky uvedené v zadání

3. Spusť testy:

```bash
dotnet test
```

---

## 🎯 Konvence

### Soubory

- **Řešení**: `Solutions/DayXX.cs` (např. `Day01.cs`, `Day02.cs`)
- **Inputy**: `Inputs/dayXX.txt` (např. `day01.txt`, `day02.txt`)
- **Test data**: `AoC2025.Tests/TestData/dayXX_example.txt`

### Čísla dnů

Používej **dvouciferná čísla s nulou** pro soubory (`day01.txt`, `day02.txt`), ale třídy pojmenuj normálně (`Day1`, `Day2`).

### Return values

Metody `SolvePart1` a `SolvePart2` vrací `string`. Konvertuj výsledky:

```csharp
return result.ToString(); // int, long, atd. → string
```

---

## 📝 Tips & Tricks

### Sdílení kódu mezi Part 1 a Part 2

```csharp
public class Day05 : ISolution
{
    public string SolvePart1(string input)
    {
        var data = ParseInput(input);
        return CalculatePart1(data).ToString();
    }
    
    public string SolvePart2(string input)
    {
        var data = ParseInput(input); // Reuse parsing
        return CalculatePart2(data).ToString();
    }
    
    private List<int> ParseInput(string input)
    {
        return input.Split('\n')
                    .Select(int.Parse)
                    .ToList();
    }
}
```

### Debugging

- Nastav breakpoint v `Program.cs` nebo ve svém řešení
- Spusť s debuggerem v IDE (F5) nebo `dotnet run`

### Error handling

Runner automaticky zachytává výjimky a zobrazuje user-friendly error messages. V řešení můžeš házet výjimky normálně:

```csharp
if (lines.Length == 0)
    throw new InvalidOperationException("Input je prázdný!");
```

---

## 🏆 Best Practices

1. **Nejdřív testy** - Implementuj test s example inputem z AoC, pak řešení
2. **Parsing odděleně** - Vytvoř pomocnou metodu `ParseInput()`
3. **Reuse kód** - Sdílej logiku mezi Part 1 a Part 2
4. **Komentuj algoritmy** - Pokud je logika složitá, přidej komentář
5. **Clean code** - Používej smysluplné názvy proměnných a metod

---

## 🎅 Happy Coding!

Hodně štěstí s Advent of Code 2025! 🌟

Pro více informací navštiv [adventofcode.com](https://adventofcode.com/2025).

