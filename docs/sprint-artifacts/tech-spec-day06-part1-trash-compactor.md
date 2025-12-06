# Tech-Spec: Day 06 Part 1 - Trash Compactor

**Created:** 2025-12-06  
**Status:** Completed  
**AoC Link:** https://adventofcode.com/2025/day/6

---

## Overview

### Problem Statement

Po pomoci elfům v kuchyni jste skočili do odpadu a ocitli se v odpadkovém lisovači. Zatímco čekáte na otevření dveří, pomáháte mladému chobotničkovi s matikářskou domácí úlohou.

**Klíčové body:**
- Matikářský úkol je uspořádán jako **dlouhý horizontální seznam problémů**
- Každý problém má **čísla uspořádaná vertikálně** a **operaci dole** (`+` nebo `*`)
- Problémy jsou odděleny **celým sloupcem mezer**
- **Cíl: vyřešit všechny problémy a sečíst všechny výsledky (grand total)**

**Example z AoC:**
```
 123 328  51 64 
 45 64  387 23 
  6 98  215 314
*   +   *   +  
```

Tento úkol obsahuje **4 problémy**:
- **Problém 1:** `123 * 45 * 6 = 33210`
- **Problém 2:** `328 + 64 + 98 = 490`
- **Problém 3:** `51 * 387 * 215 = 4243455`
- **Problém 4:** `64 + 23 + 314 = 401`

**Grand Total:** `33210 + 490 + 4243455 + 401 = 4277556`

### Input Analysis

**Reálný input (`Inputs/day06.txt`):**
- **4 řádky s čísly** (vertikálně zarovnaná čísla jednotlivých problémů)
- **1 řádek s operacemi** (`+` nebo `*`)
- **1000 sloupců = 1000 problémů** k vyřešení
- Čísla jsou oddělena různým počtem mezer (zarovnání může být různé)
- Hodnoty čísel: **1 až ~9999** (standardní rozsah pro `int`)

**Porovnání s example:**
- Example: 4 problémy, 3 čísla na problém, jednoduché hodnoty
- Reálný vstup: **1000 problémů**, 4 čísla na problém, větší hodnoty → **výrazně větší!**

**Důsledky pro algoritmus:**
- ❌ **Manuální parsing** by byl náchylný k chybám (různé mezery)
- ✅ **Musíme správně extrahovat sloupce** - čísla na stejné pozici ve sloupci patří do jednoho problému
- ✅ **Split by whitespace** + zpracování po sloupcích
- ⚠️ **Pozor na mezinásobení velkých čísel** - `int` může přetéct, použít `long`
  - Např: `9999 * 9999 * 9999 * 9999 ≈ 10^16` → potřebujeme `long` (do ~9×10^18)

**Časová složitost:**
- Parsing: O(n × m) kde n = počet řádků (4), m = počet sloupců (1000)
- Výpočet: O(m × k) kde m = počet problémů (1000), k = počet čísel na problém (4)
- Celkem: O(4000) operací → **triviální**

**Prostorová složitost:**
- O(m × n) pro uložení všech čísel: 4000 čísel → **triviální**

### Solution

**Algoritmus:**

1. **Parse vstup:**
   ```csharp
   // Rozdělit řádky
   var lines = input.Split('\n').Select(l => l.TrimEnd()).ToArray();
   var numberRows = lines.Take(4).ToArray();
   var operationRow = lines[4];
   ```

2. **Extrakce sloupců (problémů):**
   ```csharp
   // Pro každý sloupec: extrahuj čísla ze všech 4 řádků + operaci
   // Použít regex nebo split by whitespace pro každý řádek
   
   var problems = new List<Problem>();
   var columns = ExtractColumns(numberRows, operationRow);
   
   foreach (var column in columns)
   {
       problems.Add(new Problem 
       { 
           Numbers = column.Numbers, 
           Operation = column.Operation 
       });
   }
   ```

3. **Způsob extrakce sloupců:**
   
   **Možnost A: Split každý řádek zvlášť**
   ```csharp
   var row1Numbers = numberRows[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
   var row2Numbers = numberRows[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
   // ... pro každý řádek
   var operations = operationRow.Split(' ', StringSplitOptions.RemoveEmptyEntries);
   
   // Sloupec i = [row1Numbers[i], row2Numbers[i], row3Numbers[i], row4Numbers[i]]
   ```
   
   **Možnost B: Regex pro pozice sloupců**
   - Složitější, ale robustnější
   - Není nutné pro tento typ vstupu

4. **Vyřešit každý problém:**
   ```csharp
   long grandTotal = 0;
   
   foreach (var problem in problems)
   {
       long result = problem.Numbers[0];
       
       for (int i = 1; i < problem.Numbers.Count; i++)
       {
           if (problem.Operation == '+')
               result += problem.Numbers[i];
           else if (problem.Operation == '*')
               result *= problem.Numbers[i];
       }
       
       grandTotal += result;
   }
   
   return grandTotal;
   ```

**Edge cases:**
- ✅ **Různé počty mezer** mezi čísly → použít `StringSplitOptions.RemoveEmptyEntries`
- ✅ **Velké výsledky násobení** → použít `long` místo `int`
- ✅ **Leading/trailing mezery** → použít `Trim()` nebo `TrimEnd()`
- ✅ **Jednoznačnost operace** → každý sloupec má vždy právě jednu operaci
- ⚠️ **Přetečení `long`** - teoreticky možné při 4× násobení ~9999
  - Pro Part 1 pravděpodobně není problém
  - Při přetečení v Part 2 zvážit `BigInteger`

**Datové struktury:**
```csharp
record Problem(List<long> Numbers, char Operation);
```

### Scope

**In Scope (Part 1):**
- ✅ Parsing vstupních řádků (4 řádky čísel + 1 řádek operací)
- ✅ Extrakce sloupců (problémů) ze vstupních dat
- ✅ Vyhodnocení každého problému (součet nebo součin)
- ✅ Sečtení všech výsledků (grand total)
- ✅ Použití `long` pro velké hodnoty
- ✅ Ošetření různých mezer a zarovnání

**Out of Scope (Part 1):**
- ❌ Podpora jiných operací kromě `+` a `*`
- ❌ Validace konzistence vstupu (počet čísel v řádcích)
- ❌ `BigInteger` (není nutné pro Part 1)
- ❌ Složité regex parsing

**Nice to Have:**
- 💡 Unit test s příkladem z AoC
- 💡 Validace vstupu (stejný počet čísel v každém řádku)
- 💡 Logging pro debugging (výsledky jednotlivých problémů)

---

## Implementation Plan

### 1. Data Structures

```csharp
// Reprezentace jednoho problému
public record Problem(List<long> Numbers, char Operation);
```

### 2. Parsing

```csharp
public static (List<long>[] numberRows, char[] operations) ParseInput(string input)
{
    var lines = input.Split('\n')
        .Select(l => l.TrimEnd())
        .Where(l => !string.IsNullOrWhiteSpace(l))
        .ToArray();
    
    // První 4 řádky = čísla
    var numberRows = new List<long>[4];
    for (int i = 0; i < 4; i++)
    {
        numberRows[i] = lines[i]
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToList();
    }
    
    // Pátý řádek = operace
    var operations = lines[4]
        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s[0])
        .ToArray();
    
    return (numberRows, operations);
}
```

### 3. Extract Problems

```csharp
public static List<Problem> ExtractProblems(List<long>[] numberRows, char[] operations)
{
    var problems = new List<Problem>();
    int columnCount = operations.Length;
    
    for (int col = 0; col < columnCount; col++)
    {
        var numbers = new List<long>();
        for (int row = 0; row < 4; row++)
        {
            numbers.Add(numberRows[row][col]);
        }
        
        problems.Add(new Problem(numbers, operations[col]));
    }
    
    return problems;
}
```

### 4. Solve Problems

```csharp
public static long SolveProblem(Problem problem)
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
```

### 5. Main Solution

```csharp
public static long Solve(string input)
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
    
    return grandTotal;
}
```

---

## Test Cases

### Example from AoC

**Input:**
```
 123 328  51 64 
 45 64  387 23 
  6 98  215 314
*   +   *   +  
```

**Expected Output:** `4277556`

**Breakdown:**
- Problém 1: `123 * 45 * 6 = 33210`
- Problém 2: `328 + 64 + 98 = 490`
- Problém 3: `51 * 387 * 215 = 4243455`
- Problém 4: `64 + 23 + 314 = 401`
- Grand Total: `33210 + 490 + 4243455 + 401 = 4277556`

### Unit Test

```csharp
[Fact]
public void Example_Should_Return_4277556()
{
    var input = File.ReadAllText("TestData/day06_example.txt");
    var result = Day06.SolvePart1(input);
    Assert.Equal(4277556, result);
}
```

---

## Complexity Analysis

### Time Complexity
- **Parsing:** O(n × m) kde n = 4 (řádky), m = 1000 (sloupce) → O(4000)
- **Problem solving:** O(m × k) kde m = 1000 (problémy), k = 4 (čísla) → O(4000)
- **Total:** O(n × m + m × k) → **O(8000)** → triviální

### Space Complexity
- **Storage:** O(n × m) pro uložení všech čísel → O(4000)
- **Working memory:** O(m) pro seznam problémů → O(1000)
- **Total:** **O(4000)** → triviální

---

## Risk Assessment

### Low Risk
- ✅ Jednoduchý parsing (split by whitespace)
- ✅ Jednoduché operace (+ a *)
- ✅ Malý objem dat (4000 čísel)
- ✅ Standardní datové typy (`long` stačí)

### Medium Risk
- ⚠️ **Parsing edge cases:** různé mezery, zarovnání
  - **Mitigace:** použít `StringSplitOptions.RemoveEmptyEntries`
- ⚠️ **Integer overflow:** násobení velkých čísel
  - **Mitigace:** použít `long` (až 9×10^18)

### High Risk
- ❌ Žádná významná rizika

---

## Notes

- Advent of Code Day 6 je **parsing challenge** - správné extrahování sloupců je klíčové
- Pozor na **různé mezery** a zarovnání v inputu
- **Použít `long`** pro výsledky násobení
- Example je **velmi jednoduchý** - reálný input má 1000 problémů
- Part 2 pravděpodobně přidá další složitost (např. priority operací, závorky, nebo jiné operace)
