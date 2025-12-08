# Tech-Spec: Day 02 Part 1 - Gift Shop

**Created:** 2025-12-02  
**Status:** ✅ Completed  
**AoC Link:** https://adventofcode.com/2025/day/2

---

## Overview

### Problem Statement

V gift shopu na Severním pólu se mladý Elf "zapařil" na počítači a přidal do databáze produktů spoustu nevalidních product ID. Tvým úkolem je identifikovat všechny nevalidní product IDs v daných rozsazích.

**Klíčové body:**
- Input = řádek s rozsahy oddělené čárkami, formát: `start-end`
- Nevalidní ID = číslo tvořené opakováním nějakého vzorce **právě dvakrát**
  - Příklady: `55` (5 dvakrát), `6464` (64 dvakrát), `123123` (123 dvakrát)
- **Vedoucí nuly neexistují** - `0101` není ID vůbec (ale `101` je validní)
- **Výsledek = součet všech nevalidních IDs** v rozsazích

**Example z AoC:**
```
11-22,95-115,998-1012,1188511880-1188511890,222220-222224,
1698522-1698528,446443-446449,38593856-38593862,565653-565659,
824824821-824824827,2121212118-2121212124
```

**Analýza rozsahů:**
- `11-22`: **11, 22** (oba nevalidní)
- `95-115`: **99** (9 dvakrát)
- `998-1012`: **1010** (10 dvakrát)
- `1188511880-1188511890`: **1188511885** (11885 dvakrát)
- `222220-222224`: **222222** (222 dvakrát)
- `1698522-1698528`: žádné nevalidní
- `446443-446449`: **446446** (446 dvakrát)
- `38593856-38593862`: **38593859** (3859 dvakrát)
- `565653-565659`: žádné nevalidní
- `824824821-824824827`: žádné nevalidní
- `2121212118-2121212124`: žádné nevalidní

**Výsledek pro example:** 1227775554

### Solution

Implementace detektoru nevalidních product IDs s pattern matchingem pro duplicitní sekvence.

**Algoritmus:**
1. **Parse inputu**: split podle čárek, z každého rozsahu získat `start-end`
2. **Pro každý rozsah**: iterovat všechna čísla od start do end (včetně)
3. **Pro každé číslo**: kontrola, zda je "repeating pattern dvakrát"
   - Převést číslo na string
   - Pokud má sudou délku, rozdělit na dvě poloviny
   - Porovnat, zda jsou obě poloviny identické
   - Pokud ano → nevalidní ID
4. **Suma**: sečíst všechna nevalidní IDs
5. **Return**: součet jako string

**Pattern Detection:**
```csharp
bool IsInvalidId(long number)
{
    string str = number.ToString();
    int len = str.Length;
    
    // Musí mít sudou délku (jinak nemůže být složeno z 2x stejného vzorce)
    if (len % 2 != 0) return false;
    
    int half = len / 2;
    string firstHalf = str.Substring(0, half);
    string secondHalf = str.Substring(half, half);
    
    return firstHalf == secondHalf;
}
```

### Scope

**In Scope (Part 1):**
- ✅ Parsing rozsahů z jednoho dlouhého řádku (split by comma)
- ✅ Iterace všech čísel v každém rozsahu
- ✅ Detekce nevalidních IDs (repeating pattern dvakrát)
- ✅ Součet všech nevalidních IDs
- ✅ Unit test s example inputem (expected: `1227775554`)

**Out of Scope:**
- ❌ Part 2 (bude řešeno samostatně po odemčení)
- ❌ Optimalizace pro extrémně velké rozsahy (generování vzorců místo iterace)
- ❌ Handling multiline inputů (zadání říká single long line)

---

## Context for Development

### Codebase Patterns

**Struktura projektu:**
- `Solutions/Day02.cs` - implementace řešení (implementuje `ISolution`)
- `Inputs/day02.txt` - reálný input z AoC (single line s rozsahy)
- `AoC2025.Tests/Day02Tests.cs` - xUnit testy
- `AoC2025.Tests/TestData/day02_example.txt` - example input z AoC zadání

**ISolution Interface:**
```csharp
public interface ISolution
{
    int DayNumber { get; }
    string Title { get; }
    string SolvePart1(string input);
    string SolvePart2(string input);
}
```

**Konvence:**
- Parsování: `input.Split(',')` pro rozsahy, `range.Split('-')` pro start-end
- Return type: vždy `string` (i když výsledek je číslo)
- Use `long` pro ID numbers (rozsahy mohou být velké, např. `1188511880`)
- XML dokumentační komentáře pro public API
- Private pomocné metody pro čitelnost (`IsInvalidId`, `ParseRanges`)

### Files to Reference

| File | Purpose |
|------|---------|
| `Solutions/Day02.cs` | Hlavní implementační soubor - vytvořit nový |
| `AoC2025.Tests/Day02Tests.cs` | Test file - vytvořit nový |
| `AoC2025.Tests/TestData/day02_example.txt` | Example input z AoC - vytvořit |
| `Inputs/day02.txt` | Reálný input - doplnit po stažení z AoC |

### Technical Decisions

**1. Data Types:**
- **`long` pro ID numbers** - některé rozsahy mají hodnoty přes 1 miliardu
- **`string` pro pattern matching** - nejjednodušší způsob detekce "dvakrát stejný pattern"

**2. Algoritmus detekce:**
- **String split approach**: rozdělit číslo na dvě poloviny a porovnat
- **Alternativa (zamítnuto)**: regex `^(\d+)\1$` - méně čitelné, zbytečně komplexní

**3. Performance considerations:**
- Rozsahy mohou být velké (např. 1 miliarda čísel), ale pro AoC je brute-force iterace OK
- Pokud by to bylo moc pomalé, lze optimalizovat generováním pouze kandidátů (sudá délka atd.)

**4. Edge cases:**
- Čísla s lichým počtem číslic **nemohou** být nevalidní (55 má 2, ale 555 má 3 → nelze rozdělit)
- Jednociferná čísla jsou validní (např. 5 je validní, není to "55")
- Leading zeros neexistují podle zadání

---

## Implementation Stories

### Story 1: Vytvoření kostry řešení pro Day 02

**Acceptance Criteria:**
- [ ] Soubor `Solutions/Day02.cs` existuje
- [ ] Implementuje `ISolution` interface
- [ ] `DayNumber` returns `2`
- [ ] `Title` returns `"Gift Shop"`
- [ ] `SolvePart1` a `SolvePart2` mají placeholder implementaci (např. `return "Not implemented";`)

**Implementation Notes:**
- Zkopírovat strukturu z `Day01.cs`
- Namespace: pokud je `AoC2025.Console` nebo podobně, zachovat konzistenci

---

### Story 2: Parser rozsahů

**Acceptance Criteria:**
- [ ] Private metoda `List<(long start, long end)> ParseRanges(string input)`
- [ ] Split podle čárky, trim whitespace
- [ ] Split každý rozsah podle `-` na start/end
- [ ] Parse `long.Parse()`
- [ ] Return list of tuples `(start, end)`

**Implementation Notes:**
```csharp
private List<(long start, long end)> ParseRanges(string input)
{
    var ranges = new List<(long, long)>();
    var parts = input.Split(',');
    
    foreach (var part in parts)
    {
        var trimmed = part.Trim();
        var bounds = trimmed.Split('-');
        long start = long.Parse(bounds[0]);
        long end = long.Parse(bounds[1]);
        ranges.Add((start, end));
    }
    
    return ranges;
}
```

**Edge Cases:**
- Whitespace kolem range stringů (trim)
- Single line input (podle zadání)

---

### Story 3: Detektor nevalidních IDs

**Acceptance Criteria:**
- [ ] Private metoda `bool IsInvalidId(long number)`
- [ ] Převede číslo na string
- [ ] Zkontroluje sudou délku (jinak return false)
- [ ] Rozdělí na dvě poloviny
- [ ] Porovná obě poloviny
- [ ] Return true pokud jsou stejné

**Implementation Notes:**
```csharp
/// <summary>
/// Detekuje, zda je product ID nevalidní (složeno z dvou identických částí).
/// </summary>
/// <param name="number">Product ID k testování</param>
/// <returns>True pokud je ID nevalidní (repeating pattern dvakrát)</returns>
private bool IsInvalidId(long number)
{
    string str = number.ToString();
    int len = str.Length;
    
    // Musí mít sudou délku
    if (len % 2 != 0) return false;
    
    int half = len / 2;
    string firstHalf = str.Substring(0, half);
    string secondHalf = str.Substring(half);
    
    return firstHalf == secondHalf;
}
```

**Test Cases:**
- `55` → true
- `6464` → true
- `123123` → true
- `101` → false (lichý počet číslic)
- `1234` → false (12 != 34)
- `11` → true
- `99` → true

---

### Story 4: Hlavní logika SolvePart1

**Acceptance Criteria:**
- [ ] `SolvePart1` parsuje rozsahy pomocí `ParseRanges`
- [ ] Iteruje každý rozsah
- [ ] Pro každé číslo v rozsahu volá `IsInvalidId`
- [ ] Pokud je nevalidní, přidá do sumy
- [ ] Return suma jako string

**Implementation Notes:**
```csharp
public string SolvePart1(string input)
{
    var ranges = ParseRanges(input.Trim());
    long sum = 0;
    
    foreach (var (start, end) in ranges)
    {
        for (long id = start; id <= end; id++)
        {
            if (IsInvalidId(id))
            {
                sum += id;
            }
        }
    }
    
    return sum.ToString();
}
```

**Performance Notes:**
- Pro example input je to rychlé (rozsahy malé)
- Pro reálný input může trvat déle, ale mělo by být OK (AoC není o micro-optimalizacích)

---

### Story 5: Unit test s example inputem

**Acceptance Criteria:**
- [ ] Soubor `AoC2025.Tests/Day02Tests.cs` existuje
- [ ] Test `Part1_WithExampleInput_ReturnsExpectedResult`
- [ ] Example input v `TestData/day02_example.txt`
- [ ] Expected result: `"1227775554"`
- [ ] Test prochází zeleně

**Implementation Notes:**

**`AoC2025.Tests/TestData/day02_example.txt`:**
```
11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124
```

**`AoC2025.Tests/Day02Tests.cs`:**
```csharp
using Xunit;
using AoC2025.Console;

namespace AoC2025.Tests;

public class Day02Tests
{
    private readonly Day02 _solution = new();

    [Fact]
    public void Part1_WithExampleInput_ReturnsExpectedResult()
    {
        // Arrange
        var input = File.ReadAllText("TestData/day02_example.txt");

        // Act
        var result = _solution.SolvePart1(input);

        // Assert
        Assert.Equal("1227775554", result);
    }

    [Fact]
    public void IsInvalidId_WithRepeatingPattern_ReturnsTrue()
    {
        // Test jednotlivých případů z example
        var solution = new Day02();
        
        // Use reflection to call private method for testing
        var method = typeof(Day02).GetMethod("IsInvalidId", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        Assert.True((bool)method.Invoke(solution, new object[] { 11L }));
        Assert.True((bool)method.Invoke(solution, new object[] { 22L }));
        Assert.True((bool)method.Invoke(solution, new object[] { 99L }));
        Assert.True((bool)method.Invoke(solution, new object[] { 1010L }));
        Assert.True((bool)method.Invoke(solution, new object[] { 123123L }));
        
        Assert.False((bool)method.Invoke(solution, new object[] { 101L }));
        Assert.False((bool)method.Invoke(solution, new object[] { 1234L }));
    }
}
```

---

## Definition of Done

**Code:**
- [x] `Solutions/Day02.cs` implementuje `ISolution`
- [x] `SolvePart1` vrací správný výsledek pro example input
- [x] Všechny private metody mají XML komentáře
- [x] Kód je čitelný a dodržuje C# conventions

**Tests:**
- [x] `Day02Tests.cs` obsahuje test pro Part 1 s example inputem
- [x] Test prochází zeleně
- [x] Optional: unit testy pro `IsInvalidId` metodu

**Integration:**
- [x] `Program.cs` umožňuje spustit Day 02 (pokud je dynamické načítání, automaticky)
- [x] Reálný input `Inputs/day02.txt` je vyplněný (stažený z AoC)
- [x] Spuštění vrací correct answer pro reálný input

**Documentation:**
- [x] Tato spec je kompletní a ready
- [x] README.md obsahuje Day 02 v seznamu (pokud existuje)

---

## Notes & Considerations

**Proč string matching místo matematiky?**
- Detekce "dvakrát stejný pattern" je triviální se stringy
- Matematické řešení by vyžadovalo exponenty a modulo → složitější
- Pro AoC je čitelnost důležitější než mikro-optimalizace

**Co když jsou rozsahy moc velké?**
- Pro example funguje brute-force
- Pokud by reálný input byl extrémní (miliardy čísel), lze optimalizovat:
  - Generovat pouze kandidáty (čísla se sudou délkou)
  - Pattern generation: pro délku N generovat všechny možné "repeating" patterns
- Nejdřív zkusit naive approach, optimalizovat až když je to nutné

**Edge cases které nemusíme řešit:**
- Leading zeros neexistují podle zadání
- Záporná čísla neexistují v zadání
- Empty input - nebude v AoC

---

## Ready for Development 🚀

Tato specifikace je ready k implementaci. Všechny stories jsou atomic a mají jasné AC. 

**Next Steps:**
1. Vytvořit kostru `Day02.cs`
2. Implementovat helper metody (`ParseRanges`, `IsInvalidId`)
3. Implementovat `SolvePart1`
4. Vytvořit testy a ověřit s example inputem
5. Stáhnout reálný input a získat hvězdičku 🌟
