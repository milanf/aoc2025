# Tech-Spec: AoC 2025 C# Console Runner

**Created:** 2025-12-01  
**Status:** ✅ Completed  
**Author:** Barry (Quick Flow Solo Dev)
**Completed:** 2025-12-01

## Overview

### Problem Statement

Potřebujeme aplikaci pro Advent of Code 2025, která umožní:
- Spouštět řešení jednotlivých dnů (Day 1, Day 2, ...)
- Postupně přidávat nová řešení bez změn v core logice
- Načítat input data pro každý den
- Zobrazovat výsledky Part 1 a Part 2

### Solution

C# Console aplikace s pluginovým systémem pro jednotlivé dny. Každý den bude samostatná třída implementující společné rozhraní. Runner app dynamicky načte a spustí požadovaný den.

### Scope

**In Scope:**
- ✅ Console runner aplikace (.NET 10)
- ✅ Interface pro jednotlivé dny (ISolution)
- ✅ Automatické discovery řešení (reflection)
- ✅ Input loading mechanismus (textové soubory)
- ✅ CLI interface pro výběr dne
- ✅ Example Day 1 implementace jako template
- ✅ xUnit test projekt pro vzorové příklady z AoC zadání
- ✅ Test fixtures s example inputs a expected outputs

**Out of Scope:**
- ❌ Web interface / API
- ❌ Databáze nebo persistence
- ❌ Automatické stahování inputů z AoC webu
- ❌ Visualization výsledků
- ❌ Performance benchmarking (zatím)

## Context for Development

### Codebase Patterns

**Greenfield projekt** - vytváříme od nuly s těmito principy:

1. **Plugin Architecture**: Každý den = samostatná třída
2. **Interface Segregation**: `ISolution` interface pro konzistenci
3. **Reflection-based Discovery**: Automatické načítání řešení
4. **Convention over Configuration**: Standardní umístění inputů a řešení

### Project Structure

```
aoc2025/
├── AoC2025.Console/          # Main console application
│   ├── Program.cs            # Entry point, runner logic
│   ├── ISolution.cs          # Interface pro řešení
│   └── AoC2025.Console.csproj
├── AoC2025.Tests/            # xUnit test projekt
│   ├── Day01Tests.cs         # Unit testy pro Day 1
│   ├── TestData/             # Vzorové vstupy z AoC zadání
│   │   ├── day01_example.txt
│   │   └── ...
│   └── AoC2025.Tests.csproj
├── Solutions/                # Složka pro řešení jednotlivých dnů
│   ├── Day01.cs              # Template/example
│   ├── Day02.cs              # (přidáme postupně)
│   └── ...
├── Inputs/                   # Input data pro každý den (reálná data)
│   ├── day01.txt
│   ├── day02.txt
│   └── ...
└── aoc2025.sln               # Solution file
```

### Technical Decisions

| Decision | Rationale |
|----------|-----------|
| .NET 10 Console App | Požadavek uživatele, moderní C#, jednoduché spouštění |
| Interface-based design | Umožňuje dynamické načítání a jednotnou strukturu |
| Reflection pro discovery | Automatické nalezení všech Day* tříd, škálovatelné |
| Text files pro input | Standardní AoC formát, jednoduché ukládání |
| Namespace: `AoC2025.Solutions` | Logické oddělení řešení od infrastruktury |
| xUnit pro testing | Industry standard, výborná podpora v .NET, jednoduché assertions |
| Test data oddělená od produkčních | `TestData/` vs `Inputs/` - vzorové vs reálné inputy |

## Implementation Plan

### Tasks

- [x] **Task 1**: Vytvořit .NET 10 solution a console projekt
  - Použít `dotnet new sln` a `dotnet new console`
  - Nastavit target framework na `net10.0`
  - Vytvořit základní strukturu složek

- [x] **Task 2**: Implementovat `ISolution` interface
  - Metody: `SolvePart1(string input)` a `SolvePart2(string input)`
  - Properties: `DayNumber`, `Title`
  - Dokumentovat s XML comments

- [x] **Task 3**: Vytvořit Runner logiku v `Program.cs`
  - CLI menu pro výběr dne (nebo parameter z command line)
  - Reflection-based discovery všech tříd implementujících `ISolution`
  - Input loading z `Inputs/dayXX.txt`
  - Spuštění řešení a výpis výsledků
  - Error handling (missing input, missing solution)

- [x] **Task 4**: Implementovat `Day01.cs` jako template
  - Placeholder implementace s TODOs
  - Ukázat strukturu a best practices
  - Testovací input v `Inputs/day01.txt`

- [x] **Task 5**: Vytvořit xUnit test projekt
  - `dotnet new xunit -n AoC2025.Tests`
  - Reference na AoC2025.Console projekt
  - Vytvořit `TestData/` složku s `day01_example.txt`
  - Implementovat `Day01Tests.cs` s testy pro example input z AoC zadání
  - Každý test ověří Part1 a Part2 oproti známým výsledkům

- [x] **Task 6**: Přidat README s usage instructions
  - Jak přidat nový den
  - Jak spustit aplikaci
  - Jak spustit testy (`dotnet test`)
  - Konvence pro input files a test data

### Acceptance Criteria

- [x] **AC 1**: Console app se spustí a zobrazí menu s dostupnými dny
  - **Given**: Aplikace je zkompilována
  - **When**: Spustím `dotnet run` v AoC2025.Console
  - **Then**: Vidím seznam dostupných dnů a prompt pro výběr

- [x] **AC 2**: Mohu spustit řešení konkrétního dne
  - **Given**: Day01.cs je implementován a má input v Inputs/day01.txt
  - **When**: Vyberu "1" v menu nebo spustím `dotnet run -- 1`
  - **Then**: Aplikace načte input, spustí Part1 a Part2, zobrazí výsledky

- [x] **AC 3**: Nový den lze přidat bez změny runner logiky
  - **Given**: Mám hotový runner
  - **When**: Přidám `Solutions/Day02.cs` implementující `ISolution`
  - **Then**: Runner automaticky detekuje Day 2 a nabídne ho v menu

- [x] **AC 4**: Error handling funguje korektně
  - **Given**: Vybrán den bez input souboru
  - **When**: Spustím řešení
  - **Then**: Aplikace zobrazí user-friendly error message (ne crash)

- [x] **AC 5**: Unit testy validují example inputy z AoC zadání
  - **Given**: Day01Tests.cs s example input a expected outputs
  - **When**: Spustím `dotnet test`
  - **Then**: Všechny testy projdou (zelené), ověřují Part1 a Part2

## Additional Context

### Dependencies

- **.NET 10 SDK**: Musí být nainstalováno (https://dotnet.microsoft.com/download)
- **xUnit**: Automaticky přidáno přes `dotnet new xunit`
- **Žádné další external NuGet packages** - pouze built-in System.* libraries

### File Naming Conventions

- **Solutions**: `DayXX.cs` kde XX je padded number (01, 02, ..., 25)
- **Inputs**: `dayXX.txt` (lowercase, padded)
- **Class names**: `Day01`, `Day02`, etc.
- **Namespace**: `AoC2025.Solutions`

### Example ISolution Implementation

```csharp
namespace AoC2025.Solutions;

public class Day01 : ISolution
{
    public int DayNumber => 1;
    public string Title => "Trebuchet?!"; // AoC puzzle title
    
    public string SolvePart1(string input)
    {
        // TODO: Implement Part 1
        return "Not implemented yet";
    }
    
    public string SolvePart2(string input)
    {
        // TODO: Implement Part 2
        return "Not implemented yet";
    }
}
```

### Testing Strategy

**Pro MVP (tento tech-spec):**
- xUnit test projekt s testy pro example inputs z AoC zadání
- Každý den bude mít vlastní test class (`Day01Tests.cs`, `Day02Tests.cs`, ...)
- Test data v `TestData/dayXX_example.txt`
- Testy ověřují Part1 a Part2 oproti známým výsledkům z AoC

**Example test struktura:**
```csharp
public class Day01Tests
{
    [Fact]
    public void Part1_ExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var solution = new Day01();
        var input = File.ReadAllText("TestData/day01_example.txt");
        
        // Act
        var result = solution.SolvePart1(input);
        
        // Assert
        Assert.Equal("142", result); // Expected z AoC example
    }
    
    [Fact]
    public void Part2_ExampleInput_ReturnsExpectedResult()
    {
        var solution = new Day01();
        var input = File.ReadAllText("TestData/day01_example.txt");
        var result = solution.SolvePart2(input);
        Assert.Equal("281", result); // Expected z AoC example
    }
}
```

**Budoucí rozšíření:**
- Benchmark measurements (optional)
- Performance regression tests

### Notes

- **Input files**: Uživatel bude manuálně kopírovat input z adventofcode.com do `Inputs/dayXX.txt`
- **Performance**: Pro AoC není kritická, ale můžeme později přidat měření času
- **Extensibility**: Design umožňuje snadno přidat:
  - Benchmark mode
  - Visualization helpers
  - Sample/test input support
  - Command line arguments pro automatizaci

---

## Next Steps

Po schválení tohoto tech-spec:

1. **Doporučený workflow**: Spustit Quick Dev v čerstvém contextu
   ```
   Příkaz: *quick-dev
   ```

2. **Nebo použít tento spec** pro implementaci ve stávajícím contextu (méně efektivní)

---

**Ready for review! 🚀**

