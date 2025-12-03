# Tech-Spec: Day 03 Part 1 - Lobby

**Created:** 2025-12-03  
**Status:** Ready for Development  
**AoC Link:** https://adventofcode.com/2025/day/3

---

## Overview

### Problem Statement

V lobby jsou baterie, které mohou dodat nouzové napájení pro eskalátor. Každá baterie je označena hodnotou joltage (1-9). Baterie jsou uspořádány do bank - každý řádek vstupních dat představuje jednu banku baterií.

**Klíčové body:**
- Každá banka baterií = jeden řádek číslic (např. `987654321111111`)
- V každé bance musíme zapnout **přesně 2 baterie**
- Joltage banky = číslo tvořené ciframa zapnutých baterií (nelze přeskládat pořadí!)
- **Cíl: najít maximální možný joltage pro každou banku**
- **Výsledek: součet maximálních joltagů ze všech bank**

**Example z AoC:**
```
987654321111111    -> zapneme první dvě (9,8) -> 98
811111111111119    -> zapneme první a poslední (8,9) -> 89
234234234234278    -> zapneme poslední dvě (7,8) -> 78
818181911112111    -> zapneme 9 a 2 (na pozicích 6 a 12) -> 92

Total = 98 + 89 + 78 + 92 = 357
```

### Solution

**Algoritmus pro maximální joltage:**
Pro každou banku projdeme všechny možné dvojice pozic `(i, j)` kde `i < j`:
- Vytvoříme dvouciferné číslo z `battery[i]` a `battery[j]`
- Sledujeme maximum

**Matematicky:**
- Pro řádek délky `n` máme `n * (n-1) / 2` kombinací
- Pro `n = 100` to je 4950 kombinací (zcela přijatelné)

**Optimalizace není nutná** - bruteforce je dostatečně rychlý pro tento problém.

### Scope

**In Scope (Part 1):**
- ✅ Parsing řádků jako stringy číslic
- ✅ Generování všech dvojic pozic (i, j) kde i < j
- ✅ Výpočet dvouciferného čísla pro každou dvojici
- ✅ Nalezení maxima pro každou banku
- ✅ Součet všech maxim
- ✅ Unit test s example inputem

**Out of Scope:**
- ❌ Part 2 (bude řešeno samostatně po odemčení)
- ❌ Optimalizace (není potřeba, bruteforce stačí)
- ❌ Validace inputu (předpokládáme validní data)

---

## Context for Development

### Codebase Patterns

**Struktura projektu:**
- `Solutions/Day03.cs` - implementace řešení (implementuje `ISolution`)
- `Inputs/day03.txt` - reálný input z AoC (200 řádků, každý má 100 číslic)
- `AoC2025.Tests/Day03Tests.cs` - xUnit testy
- `AoC2025.Tests/TestData/day03_example.txt` - example input z AoC zadání

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
- Parsování: `input.Split('\n', StringSplitOptions.RemoveEmptyEntries)`
- Return type: vždy `string` (i když výsledek je číslo)
- XML dokumentační komentáře pro public API
- Private pomocné metody pro čitelnost

### Files to Reference

| File | Purpose |
|------|---------|
| `Solutions/Day03.cs` | Hlavní implementační soubor - vytvořit nový |
| `AoC2025.Tests/Day03Tests.cs` | Test - vytvořit nový |
| `AoC2025.Tests/TestData/day03_example.txt` | Vytvořit s example inputem |
| `Inputs/day03.txt` | Reálný input (už obsahuje 200 řádků po 100 cifrách) |

### Technical Decisions

**1. Reprezentace baterie banky:**
```csharp
string bank = line.Trim(); // Každý řádek je string číslic
```

**2. Generování všech dvojic:**
```csharp
for (int i = 0; i < bank.Length - 1; i++)
{
    for (int j = i + 1; j < bank.Length; j++)
    {
        // Kombinace (i, j)
    }
}
```

**3. Výpočet dvouciferného joltage:**
```csharp
int joltage = (bank[i] - '0') * 10 + (bank[j] - '0');
```
Převod char na int: `'7' - '0' = 7`

**4. Nalezení maxima:**
```csharp
int maxJoltage = int.MinValue;
// ... v nested loop:
maxJoltage = Math.Max(maxJoltage, joltage);
```

**5. Edge cases:**
- Prázdné řádky: filtrovat pomocí `StringSplitOptions.RemoveEmptyEntries`
- Whitespace: trimovat každý řádek
- Řádky kratší než 2 znaky: teoreticky neměly nastat, ale lze kontrolovat

---

## Implementation Plan

### Tasks

- [ ] **Task 1:** Vytvořit `Solutions/Day03.cs`
  - Implementovat `ISolution` interface
  - `DayNumber = 3`
  - `Title = "Lobby"`

- [ ] **Task 2:** Implementovat `FindMaxJoltage(string bank)` helper metodu
  - Input: jeden řádek číslic (např. `"987654321111111"`)
  - Output: maximální možný joltage pro tuto banku
  - Algoritmus: nested loop přes všechny dvojice (i, j)
  - Return maximum

- [ ] **Task 3:** Implementovat `SolvePart1(string input)`
  - Split input na řádky
  - Pro každý řádek volat `FindMaxJoltage()`
  - Sečíst všechny maximální joltage hodnoty
  - Return součet jako string

- [ ] **Task 4:** Vytvořit `AoC2025.Tests/Day03Tests.cs`
  - Vytvořit test s example inputem
  - Expected result: `"357"`

- [ ] **Task 5:** Vytvořit `AoC2025.Tests/TestData/day03_example.txt`
  - Obsahuje 4 řádky z example:
    ```
    987654321111111
    811111111111119
    234234234234278
    818181911112111
    ```

- [ ] **Task 6:** Implementovat `SolvePart2()` jako placeholder
  - Vrátit `"Not implemented yet"`

### Test Cases

**Example test:**
```
Input:
987654321111111
811111111111119
234234234234278
818181911112111

Expected Output: 357

Breakdown:
- Bank 1: 98 (pozice 0,1)
- Bank 2: 89 (pozice 0,14)
- Bank 3: 78 (pozice 12,13)
- Bank 4: 92 (pozice 6,12)
Total: 98 + 89 + 78 + 92 = 357
```

**Edge cases k otestování:**
- Všechny stejné číslice: `"1111"` → max = 11
- Sestupná sekvence: `"9876"` → max = 98
- Vzestupná sekvence: `"1234"` → max = 34
- Maximum uprostřed: `"29841"` → max = 98

---

## Algorithm Analysis

### Complexity

**Time Complexity:** O(n * m²)
- n = počet bank (200 v reálném inputu)
- m = délka každé banky (100 číslic)
- Pro každou banku: O(m²) kombinací = 4950
- Total: 200 * 4950 = 990,000 operací ✅ naprosto přijatelné

**Space Complexity:** O(1)
- Pouze několik proměnných pro tracking maxima a součtu
- Žádné pomocné datové struktury

### Why Bruteforce Works

Pro tento problém **není potřeba optimalizace**:
1. Input size je malý (200 řádků × 100 číslic)
2. Operace jsou jednoduché (porovnání a max)
3. Časová složitost O(n²) pro řádek délky 100 je triviální
4. Advent of Code často preferuje čistý, čitelný kód před mikro-optimalizací

**Alternativní přístupy (nepoužité, ale uvedené pro kontext):**
- Greedy: vybrat dva nejvyšší digity → NEFUNGUJE! (pořadí záleží)
  - Příklad: `"1982"` - greedy by vybral 9,8 = 98 ✓ správně
  - Ale: `"8129"` - greedy by vybral 9,8 = 98, ale správně je 8,9 = 89
- Sliding window: nehledáme souvislé sekvence
- Dynamic programming: není overlapping subproblems

**Závěr:** Bruteforce je optimální řešení pro tento problém.

---

## Dependencies

**Žádné externí dependencies:**
- Používáme pouze standardní C# knihovny
- LINQ pro čitelnost (volitelné)

---

## Notes

**Input charakteristiky:**
- 200 řádků (bank)
- Každý řádek má přesně 100 číslic
- Číslice jsou 1-9 (nikde není 0 v example ani v reálném inputu)

**Doporučení pro implementaci:**
1. Začít s helper metodou `FindMaxJoltage()` a otestovat ji samostatně
2. Pak implementovat `SolvePart1()` s integrací
3. Unit test pro verifikaci správnosti

**Part 2 considerations:**
- Zatím neznámé (bude odemčeno po vyřešení Part 1)
- Pravděpodobně: výběr více než 2 baterií, nebo jiné omezení
- Ponechat kód modulární pro snadné rozšíření
