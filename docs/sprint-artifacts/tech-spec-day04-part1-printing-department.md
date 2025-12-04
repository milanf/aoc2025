# Tech-Spec: Day 04 Part 1 - Printing Department

**Created:** 2025-12-04  
**Status:** Ready for Development  
**AoC Link:** https://adventofcode.com/2025/day/4

---

## Overview

### Problem Statement

V tiskárně je třeba optimalizovat práci vysokozdvižných vozíků, které manipulují s velkými rolemi papíru. Role jsou uspořádány na velké mřížce a vozíky mohou přistoupit pouze k těm rolím, které nejsou příliš obklopeny ostatními rolemi.

**Klíčové body:**
- Mřížka obsahuje role papíru (`@`) a prázdná místa (`.`)
- Role je **přístupná**, pokud má **méně než 4 role** v 8 sousedních polích
- 8 sousedních polí = 8-souvislost (horizontálně, vertikálně, diagonálně)
- **Cíl: spočítat kolik rolí je přístupných vysokozdvižnými vozíky**

**Example z AoC:**
```
..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.
```

Výsledek: **13 přístupných rolí** (označených jako `x` v zadání):
```
..xx.xx@x.
x@@.@.@.@@
@@@@@.x.@@
@.@@@@..@.
x@.@@@@.@x
.@@@@@@@.@
.@.@.@.@@@
x.@@@.@@@@
.@@@@@@@@.
x.x.@@@.x.
```

### Input Analysis

**Reálný input (`Inputs/day04.txt`):**
- 136 řádků
- 140 sloupců
- **19 040 polí celkem**
- Hustota rolí papíru: přibližně 70-75% polí obsahuje `@`

**Porovnání s example:**
- Example: 10×10 = 100 polí
- Reálný vstup: 136×140 = 19 040 polí → **190× větší!**

**Důsledky pro algoritmus:**
- Bruteforce přístup je stále efektivní: O(rows × cols × 8) = O(n)
- Pro 19 040 polí × 8 kontrol = ~152 000 operací (triviální)
- Žádná optimalizace není potřeba

### Solution

**Algoritmus:**

1. **Parse mřížku** do 2D struktury (pole stringů nebo char[][])
2. **Pro každé pole na mřížce:**
   - Pokud obsahuje roli (`@`), spočítej sousedy
   - Spočítej kolik z 8 sousedních polí obsahuje `@`
   - Pokud je počet < 4, role je přístupná
3. **Vrať celkový počet přístupných rolí**

**Směry pro 8 sousedů:**
```csharp
(x-1, y-1)  (x, y-1)  (x+1, y-1)  // horní řada
(x-1, y)    [x, y]    (x+1, y)    // střední řada
(x-1, y+1)  (x, y+1)  (x+1, y+1)  // dolní řada
```

**Edge cases:**
- **Okraje mřížky:** role na okraji mají méně než 8 sousedů → musíme kontrolovat boundaries
- **Rohy mřížky:** mají pouze 3 sousedy
- **Prázdná pole:** ignorujeme (pouze role `@` se počítají)

**Časová složitost:** O(rows × cols) - lineární
**Prostorová složitost:** O(rows × cols) - pro uložení mřížky

### Scope

**In Scope (Part 1):**
- ✅ Parsing mřížky ze vstupního textu
- ✅ Reprezentace mřížky jako 2D struktura (pole stringů)
- ✅ Iterace přes všechna pole mřížky
- ✅ Kontrola 8 sousedních polí pro každou roli
- ✅ Boundary checking (okraje a rohy mřížky)
- ✅ Počítání přístupných rolí
- ✅ Unit test s example inputem (expected: 13)

**Out of Scope:**
- ❌ Part 2 (bude řešeno samostatně po odemčení)
- ❌ Optimalizace (není potřeba pro velikost vstupu)
- ❌ Grafická vizualizace mřížky
- ❌ Validace formátu inputu (předpokládáme validní data)

---

## Context for Development

### Codebase Patterns

**Struktura projektu:**
- `Solutions/Day04.cs` - implementace řešení (implementuje `ISolution`)
- `Inputs/day04.txt` - reálný input z AoC (136 řádků × 140 sloupců)
- `AoC2025.Tests/Day04Tests.cs` - xUnit testy
- `AoC2025.Tests/TestData/day04_example.txt` - example input z AoC zadání

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
| `Solutions/Day04.cs` | Hlavní implementační soubor - vytvořit nový |
| `AoC2025.Tests/Day04Tests.cs` | Test - vytvořit nový |
| `AoC2025.Tests/TestData/day04_example.txt` | Vytvořit s example inputem |
| `Inputs/day04.txt` | Reálný input (už existuje, 136×140 mřížka) |

### Technical Decisions

**1. Reprezentace mřížky:**
```csharp
string[] grid = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
// Přístup: grid[row][col]
// row = řádek (y-ová souřadnice)
// col = sloupec (x-ová souřadnice)
```

**2. Směry pro 8 sousedů:**
```csharp
private static readonly (int dr, int dc)[] Directions = 
{
    (-1, -1), (-1, 0), (-1, 1),  // horní řada
    (0, -1),           (0, 1),   // střední (vlevo, vpravo)
    (1, -1),  (1, 0),  (1, 1)    // dolní řada
};
```

**3. Počítání sousedů:**
```csharp
private int CountNeighbors(string[] grid, int row, int col)
{
    int count = 0;
    foreach (var (dr, dc) in Directions)
    {
        int newRow = row + dr;
        int newCol = col + dc;
        
        // Boundary check
        if (newRow >= 0 && newRow < grid.Length &&
            newCol >= 0 && newCol < grid[newRow].Length)
        {
            if (grid[newRow][newCol] == '@')
                count++;
        }
    }
    return count;
}
```

**4. Hlavní logika:**
```csharp
int accessibleRolls = 0;
for (int row = 0; row < grid.Length; row++)
{
    for (int col = 0; col < grid[row].Length; col++)
    {
        if (grid[row][col] == '@')
        {
            int neighbors = CountNeighbors(grid, row, col);
            if (neighbors < 4)
                accessibleRolls++;
        }
    }
}
return accessibleRolls.ToString();
```

**5. Edge cases:**
- **Boundary checking:** vždy kontrolovat `newRow >= 0 && newRow < grid.Length`
- **Prázdné řádky:** filtrovat pomocí `StringSplitOptions.RemoveEmptyEntries`
- **Různé délky řádků:** teoreticky by nemělo nastat, ale použít `grid[newRow].Length`
- **Role na okraji:** mají přirozeně méně sousedů → správné chování

---

## Implementation Plan

### Tasks

- [ ] **Task 1:** Vytvořit `Solutions/Day04.cs`
  - Implementovat `ISolution` interface
  - `DayNumber = 4`
  - `Title = "Printing Department"`

- [ ] **Task 2:** Implementovat pomocnou strukturu pro směry
  - Definovat pole `Directions` s 8 směry (delta row, delta col)

- [ ] **Task 3:** Implementovat `CountNeighbors(string[] grid, int row, int col)` helper metodu
  - Input: mřížka, pozice role
  - Output: počet sousedících rolí (0-8)
  - Implementovat boundary checking
  - Iterovat přes všech 8 směrů

- [ ] **Task 4:** Implementovat `SolvePart1(string input)`
  - Parse input na řádky (string array)
  - Iterovat přes všechna pole mřížky
  - Pro každou roli (`@`) zavolat `CountNeighbors()`
  - Pokud neighbors < 4, inkrementovat počítadlo
  - Return počet jako string

- [ ] **Task 5:** Vytvořit `AoC2025.Tests/TestData/day04_example.txt`
  - Obsahuje 10 řádků example z AoC:
    ```
    ..@@.@@@@.
    @@@.@.@.@@
    @@@@@.@.@@
    @.@@@@..@.
    @@.@@@@.@@
    .@@@@@@@.@
    .@.@.@.@@@
    @.@@@.@@@@
    .@@@@@@@@.
    @.@.@@@.@.
    ```

- [ ] **Task 6:** Vytvořit `AoC2025.Tests/Day04Tests.cs`
  - Test s example inputem
  - Expected result: `"13"`

- [ ] **Task 7:** Implementovat `SolvePart2()` jako placeholder
  - Vrátit `"Not implemented yet"`

### Test Cases

**Example test:**
```
Input (10×10 mřížka):
..@@.@@@@.
@@@.@.@.@@
@@@@@.@.@@
@.@@@@..@.
@@.@@@@.@@
.@@@@@@@.@
.@.@.@.@@@
@.@@@.@@@@
.@@@@@@@@.
@.@.@@@.@.

Expected Output: 13
```

**Detailní analýza example (pro verifikaci):**

Přístupné role (neighbors < 4):
```
Row 0: [0,2], [0,3], [0,5], [0,6], [0,9]    → 5 rolí
Row 1: [1,0]                                → 1 role
Row 2: [2,5]                                → 1 role
Row 4: [4,0], [4,9]                         → 2 role
Row 7: [7,0]                                → 1 role
Row 9: [9,0], [9,2], [9,8]                  → 3 role

Total: 5 + 1 + 1 + 2 + 1 + 3 = 13 ✓
```

**Edge cases k otestování manuálně:**

1. **Roh mřížky:**
```
@..
...
...
```
Role na [0,0] má 3 sousedy (pouze dolní a pravé oblasti) → všichni prázdní → neighbors = 0 < 4 → přístupná

2. **Úplně obklopená role:**
```
@@@
@@@
@@@
```
Role uprostřed [1,1] má 8 sousedů, všichni jsou role → neighbors = 8 ≥ 4 → **nepřístupná**

3. **Práh 3 vs 4 sousedů:**
```
..@
.@@
@@@
```
Role na [1,2] má právě 3 sousedy → neighbors = 3 < 4 → **přístupná**

```
@@@
@@@
.@@
```
Role na [1,1] má právě 4 sousedy → neighbors = 4 ≥ 4 → **nepřístupná**

---

## Definition of Done

- [ ] `Solutions/Day04.cs` existuje a implementuje `ISolution`
- [ ] `SolvePart1()` vrací správný výsledek pro example input (13)
- [ ] `SolvePart1()` vrací správný výsledek pro reálný input (zatím neznáme)
- [ ] Unit test v `AoC2025.Tests/Day04Tests.cs` prochází (zelený)
- [ ] Example data v `TestData/day04_example.txt` jsou správně naformátovaná
- [ ] Kód je čitelný s XML dokumentačními komentáři
- [ ] `CountNeighbors()` správně počítá sousedy včetně boundary checkingu
- [ ] `SolvePart2()` vrací placeholder message

---

## Notes

**Proč není potřeba optimalizace:**
- Lineární časová složitost O(n) kde n = počet polí
- Pro 19 040 polí: ~152 000 operací (8 kontrol × 19 040)
- Moderní CPU zvládne miliony operací za sekundu
- Žádné rekurzivní hledání, žádné složité výpočty
- Bruteforce je naprosto dostačující

**Možné optimalizace (POUZE pokud by bylo třeba):**
- Early exit pokud najdeme 4+ sousedy (ale není to nutné)
- Paralelizace s `Parallel.For` (overkill pro tento problém)

**Potenciální nástrahy:**
- Špatné boundary checking → IndexOutOfRangeException
- Záměna row/col indexů → špatné výsledky
- Zapomenutí na diagonální sousedy → nesprávný počet
- Off-by-one chyba v podmínce `< 4` vs `<= 3` (jsou ekvivalentní, ale doporučuji `< 4`)

---
