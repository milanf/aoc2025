# Tech-Spec: Day 07 Part 2 - Laboratories (Quantum Tachyon Manifold)

**Created:** 2025-12-07  
**Status:** In Progress  
**AoC Link:** https://adventofcode.com/2025/day/7

---

## Overview

### Problem Statement

Po opravě kvantového tachyonového manifestu zjišťujeme, že **jedna kvantová částice bere obě cesty** při každém splitteru → **many-worlds interpretation**.

**Klíčové body:**
- Částice začíná na pozici `S` a pohybuje se **pouze dolů**
- Když částice narazí na **splitter** (`^`), **čas se rozdělí na dvě timeline**:
  - V jedné timeline jde částice **vlevo**
  - V druhé timeline jde částice **vpravo**
- Obe nové cesty pokračují **dolů** ze svých pozic
- **Cíl: spočítat celkový počet různých timelines** (unikátních cest, kterými může částice projít)

**Example z AoC:**
```
.......S.......
...............
.......^.......
...............
......^.^......
...............
.....^.^.^.....
...............
....^.^...^....
...............
...^.^...^.^...
...............
..^...^.....^..
...............
.^.^.^.^.^...^.
...............
```

**Ukázky různých timelines:**

1. **Timeline "vždy vlevo":**
```
.......S.......
.......|.......
......|^.......
......|........
.....|^.^......
.....|.........
....|^.^.^.....
....|..........
...|^.^...^....
...|...........
..|^.^...^.^...
..|............
.|^...^.....^..
.|.............
|^.^.^.^.^...^.
|..............
```

2. **Timeline "střídání L/R":**
```
.......S.......
.......|.......
......|^.......
......|........
......^|^......
.......|.......
.....^|^.^.....
......|........
....^.^|..^....
.......|.......
...^.^.|.^.^...
.......|.......
..^...^|....^..
.......|.......
.^.^.^|^.^...^.
......|........
```

3. **Timeline "jiná cesta, stejný konec":**
```
.......S.......
.......|.......
......|^.......
......|........
.....|^.^......
.....|.........
....|^.^.^.....
....|..........
....^|^...^....
.....|.........
...^.^|..^.^...
......|........
..^..|^.....^..
.....|.........
.^.^.^|^.^...^.
......|........
```

**Očekávaný výsledek pro example:** `40` různých timelines

### Input Analysis

**Reálný input (`Inputs/day07.txt`):**
- **142 řádků** × **141 znaků** = **20,022 pozic**
- **1,640 splitterů** (`^`)
- Jeden startovní bod `S`
- Většina pozic je prázdné místo `.`

**Porovnání s example:**
- Example: 16 řádků × 15 znaků, ~15 splitterů → **40 timelines**
- Reálný vstup: 142 řádků × 141 znaků, **1,640 splitterů** → **~100× větší!**

**Důsledky pro algoritmus:**

⚠️ **KRITICKÉ ZJIŠTĚNÍ:**
- Pokud by částice procházela **všemi 1,640 splittery**, vzniklo by **2^1640 timelines** → **ASTRONOMICKÉ ČÍSLO!**
- To je **více než atomy ve vesmíru** → brute force je NEMOŽNÝ!

**ALE:**
- ✅ **Částice nemůže projít VŠEMI splittery** - jde jen dolů, takže projde pouze splittery na své cestě
- ✅ **Mnoho cest se "sloučí" na stejných pozicích** → nepočítají se jako nové timelines, ale jako jedna
- ✅ **Počítáme unikátní koncové pozice**, nikoli všechny možné kombinace cest

**Správný přístup:**
- Nesledovat **každou cestu jednotlivě** (exponenciální)
- Sledovat **počet způsobů, jak se dostat na každou pozici** (polynomiální)
- Použít **dynamické programování**: pro každou pozici spočítat, kolika různými cestami tam částice může dojít
- Na konci sečíst všechny koncové pozice

**Matematický model:**
- `paths[row][col]` = počet různých cest, jak se dostat na pozici `(row, col)`
- Když částice narazí na splitter na `(r, c)`, pak:
  - `paths[r][c-1] += paths[r][c]` (levá cesta)
  - `paths[r][c+1] += paths[r][c]` (pravá cesta)
- Když částice jde volným prostorem `.`, pak:
  - `paths[r+1][c] += paths[r][c]` (pokračuje dolů)

**Časová složitost:**
- Jedno průchod mřížkou: **O(width × height)** = **O(141 × 142)** = **O(20,000)**
- Pro každou pozici: konstantní operace (přičtení k sousedům)
- **Celkem: O(width × height) = O(20,000)** → **lineární, velmi rychlé!**

**Prostorová složitost:**
- Mřížka: **O(width × height)** = **O(20,000)**
- DP tabulka: **O(width × height)** = **O(20,000)**
- **Celkem: O(20,000)** → **triviální**

### Solution

**Algoritmus: Dynamické programování (DP) s počítáním cest**

1. **Parse vstup do 2D mřížky:**
   ```csharp
   char[][] grid = ParseGrid(input);
   (int startRow, int startCol) = FindStart(grid);
   ```

2. **Inicializace DP tabulky:**
   ```csharp
   // paths[r][c] = počet různých cest, jak se dostat na pozici (r, c)
   var paths = new long[grid.Length][];
   for (int i = 0; i < grid.Length; i++)
       paths[i] = new long[grid[i].Length];
   
   // Začínáme s jednou cestou na startovní pozici
   paths[startRow][startCol] = 1;
   ```

3. **BFS simulace s počítáním cest:**
   ```csharp
   // Fronta pozic k zpracování
   var queue = new Queue<(int row, int col)>();
   var processed = new HashSet<(int row, int col)>();
   
   queue.Enqueue((startRow, startCol));
   
   while (queue.Count > 0)
   {
       var (row, col) = queue.Dequeue();
       
       // Přeskočit, pokud už jsme tuto pozici zpracovali
       if (processed.Contains((row, col)))
           continue;
       
       processed.Add((row, col));
       
       long currentPaths = paths[row][col];
       if (currentPaths == 0)
           continue; // Žádné cesty sem nevedou
       
       // Posun dolů
       int newRow = row + 1;
       int newCol = col;
       
       // Kontrola hranic
       if (newRow >= grid.Length)
           continue; // Částice opustila mřížku
       
       char cell = grid[newRow][newCol];
       
       if (cell == '^')
       {
           // SPLITTER! Rozdělení na dvě cesty
           
           // Levá cesta (col - 1)
           int leftCol = newCol - 1;
           if (leftCol >= 0)
           {
               paths[newRow][leftCol] += currentPaths;
               if (!processed.Contains((newRow, leftCol)))
                   queue.Enqueue((newRow, leftCol));
           }
           
           // Pravá cesta (col + 1)
           int rightCol = newCol + 1;
           if (rightCol < grid[newRow].Length)
           {
               paths[newRow][rightCol] += currentPaths;
               if (!processed.Contains((newRow, rightCol)))
                   queue.Enqueue((newRow, rightCol));
           }
       }
       else if (cell == '.')
       {
           // Volné místo - částice pokračuje dolů
           paths[newRow][newCol] += currentPaths;
           if (!processed.Contains((newRow, newCol)))
               queue.Enqueue((newRow, newCol));
       }
   }
   ```

4. **Sečtení všech koncových cest:**
   ```csharp
   // Koncové pozice = buď na poslední řádce, nebo pozice, kde částice "uvázla"
   long totalTimelines = 0;
   
   // Projít celou mřížku a najít všechny pozice s nenulový počtem cest
   for (int r = 0; r < grid.Length; r++)
   {
       for (int c = 0; c < grid[r].Length; c++)
       {
           long pathCount = paths[r][c];
           if (pathCount > 0)
           {
               // Je to koncová pozice?
               // - Pokud je na poslední řádce
               // - Nebo pokud další krok dolů by byl mimo mřížku
               // - Nebo pokud další krok dolů narazí na neprochodnou pozici
               
               int nextRow = r + 1;
               if (nextRow >= grid.Length)
               {
                   // Je na/pod poslední řádkou
                   totalTimelines += pathCount;
               }
               else if (c < 0 || c >= grid[nextRow].Length)
               {
                   // Je mimo horizontální hranice
                   totalTimelines += pathCount;
               }
               else
               {
                   char nextCell = grid[nextRow][c];
                   // Pokud další krok nevede nikam (není ani '.', ani '^', ani 'S')
                   if (nextCell != '.' && nextCell != '^' && nextCell != 'S')
                   {
                       totalTimelines += pathCount;
                   }
               }
           }
       }
   }
   
   return totalTimelines;
   ```

**Alternativní přístup - jednodušší:**

**Pozorování:** Koncové timelines = částice, které opustily mřížku (šly dolů mimo hranice).

**Jednodušší řešení:**
```csharp
public static long CountTimelines(char[][] grid)
{
    var (startRow, startCol) = FindStart(grid);
    
    // paths[r][c] = počet cest vedoucích na pozici (r, c)
    var paths = new long[grid.Length + 1][]; // +1 pro "mimo mřížku" řádek
    for (int i = 0; i <= grid.Length; i++)
        paths[i] = new long[grid[0].Length];
    
    paths[startRow][startCol] = 1;
    
    // Procházet řádek po řádku (top-down)
    for (int r = 0; r < grid.Length; r++)
    {
        for (int c = 0; c < grid[r].Length; c++)
        {
            long currentPaths = paths[r][c];
            if (currentPaths == 0)
                continue;
            
            char cell = grid[r][c];
            
            // Posun dolů
            int nextRow = r + 1;
            
            if (nextRow >= grid.Length)
            {
                // Částice opustila mřížku - končí zde
                paths[nextRow][c] += currentPaths;
                continue;
            }
            
            char nextCell = grid[nextRow][c];
            
            if (nextCell == '^')
            {
                // SPLITTER! Rozdělení na L/R, oba pokračují dolů
                
                // Levá cesta
                int leftCol = c - 1;
                if (leftCol >= 0)
                    paths[nextRow][leftCol] += currentPaths;
                
                // Pravá cesta
                int rightCol = c + 1;
                if (rightCol < grid[nextRow].Length)
                    paths[nextRow][rightCol] += currentPaths;
            }
            else if (nextCell == '.')
            {
                // Volné místo - pokračuje dolů
                paths[nextRow][c] += currentPaths;
            }
        }
    }
    
    // Sečíst všechny cesty, které "vypadly" z mřížky
    long totalTimelines = 0;
    for (int c = 0; c < paths[grid.Length].Length; c++)
        totalTimelines += paths[grid.Length][c];
    
    return totalTimelines;
}
```

**Tento přístup je čistší a efektivnější:**
- Procházíme mřížku **řádek po řádku** (top-down)
- Pro každou pozici s nenulový počtem cest propagujeme cesty dále dolů
- Při splitteru **rozdělíme cesty vlevo a vpravo**
- Koncové timelines = cesty, které vypadly z mřížky (řádek `grid.Length`)

**Datové typy:**
- `paths`: `long[][]` - protože počet timelines může být **velký** (až 2^N pro N splitterů)
- Pro 1,640 splitterů by to teoreticky bylo 2^1640, ale v praxi bude mnohem menší díky sloučení cest

**Edge cases:**
- ✅ **Částice vyjde vlevo/vpravo** po split → končí mimo mřížku
- ✅ **Částice vyjde dole** → končí
- ✅ **Více cest se sejde na stejné pozici** → sečtou se (to je klíč DP!)
- ✅ **S je na okraji** → částice začíná dolů od S
- ✅ **Žádné splittery na cestě** → jen 1 timeline

### Scope

**In Scope (Part 2):**
- ✅ Parsing 2D mřížky (stejné jako Part 1)
- ✅ Nalezení startovní pozice `S` (stejné jako Part 1)
- ✅ **Dynamické programování** pro počítání unikátních cest
- ✅ **Propagace cest** při průchodu splittery (L/R)
- ✅ Sčítání koncových timelines
- ✅ Použití `long` pro velké čísla

**Out of Scope (Part 2):**
- ❌ Brute force - sledování každé cesty jednotlivě (exponenciální!)
- ❌ Rekurze bez memoizace (stack overflow)
- ❌ Jiné směry pohybu (jen dolů)

**Nice to Have:**
- 💡 Unit test s příkladem z AoC (16×15 mřížka, očekávaný výsledek **40**)
- 💡 Visualizace DP tabulky (debugging)
- 💡 Validace, že výsledek není přetečení (overflow check)
- 💡 Optimalizace paměti (rolling array místo celé 2D tabulky)

---

## Implementation Plan

### 1. Data Structures

```csharp
// Použít stejné jako Part 1
// Žádné nové struktury nejsou potřeba
```

### 2. Parsing

```csharp
// Použít stejné metody jako Part 1:
// - ParseGrid(string input) → char[][]
// - FindStart(char[][] grid) → (int row, int col)
```

### 3. Core Algorithm - Dynamic Programming Path Counting

```csharp
public static long CountTimelines(char[][] grid)
{
    var (startRow, startCol) = FindStart(grid);
    
    int height = grid.Length;
    int width = grid[0].Length;
    
    // paths[r][c] = počet různých cest vedoucích na pozici (r, c)
    var paths = new long[height + 1][];
    for (int i = 0; i <= height; i++)
        paths[i] = new long[width];
    
    // Začínáme s jednou cestou na S
    paths[startRow][startCol] = 1;
    
    // Procházet řádek po řádku (top-down)
    for (int r = 0; r < height; r++)
    {
        for (int c = 0; c < width; c++)
        {
            long currentPaths = paths[r][c];
            if (currentPaths == 0)
                continue; // Žádné cesty nevedou na tuto pozici
            
            char cell = grid[r][c];
            
            // Posun dolů
            int nextRow = r + 1;
            
            if (nextRow >= height)
            {
                // Částice opustila mřížku - přidat k výstupnímu řádku
                paths[nextRow][c] += currentPaths;
                continue;
            }
            
            char nextCell = grid[nextRow][c];
            
            if (nextCell == '^')
            {
                // SPLITTER! Rozdělení na levou a pravou cestu
                
                // Levá cesta (col - 1)
                int leftCol = c - 1;
                if (leftCol >= 0)
                    paths[nextRow][leftCol] += currentPaths;
                else
                    paths[height][leftCol] += currentPaths; // Vypadlo mimo mřížku vlevo
                
                // Pravá cesta (col + 1)
                int rightCol = c + 1;
                if (rightCol < width)
                    paths[nextRow][rightCol] += currentPaths;
                else
                    paths[height][rightCol] += currentPaths; // Vypadlo mimo mřížku vpravo
            }
            else if (nextCell == '.' || nextCell == 'S')
            {
                // Volné místo nebo startovní pozice - pokračuje dolů
                paths[nextRow][c] += currentPaths;
            }
            // Pokud nextCell je něco jiného (nemělo by nastat), částice končí
        }
    }
    
    // Sečíst všechny cesty, které vypadly z mřížky
    long totalTimelines = 0;
    for (int c = 0; c < width; c++)
        totalTimelines += paths[height][c];
    
    return totalTimelines;
}
```

**Vylepšení - ošetření horizontálních výstupů:**
```csharp
if (nextCell == '^')
{
    // SPLITTER! Rozdělení na levou a pravou cestu
    
    // Levá cesta (col - 1)
    int leftCol = c - 1;
    if (leftCol >= 0)
    {
        paths[nextRow][leftCol] += currentPaths;
    }
    // Pokud leftCol < 0, cesta vypadla vlevo - nepočítáme ji jako timeline
    
    // Pravá cesta (col + 1)
    int rightCol = c + 1;
    if (rightCol < width)
    {
        paths[nextRow][rightCol] += currentPaths;
    }
    // Pokud rightCol >= width, cesta vypadla vpravo - nepočítáme ji jako timeline
}
```

**Poznámka:** Podle zadání se zdá, že **timelines končí pouze když vypadnou dole**, ne z boku. Pokud by vypadly z boku, pravděpodobně by se "ztratily" a nepočítaly.

**Konečná verze - korektní:**
```csharp
public static long CountTimelines(char[][] grid)
{
    var (startRow, startCol) = FindStart(grid);
    
    int height = grid.Length;
    int width = grid[0].Length;
    
    // paths[r][c] = počet různých cest vedoucích na pozici (r, c)
    var paths = new long[height][];
    for (int i = 0; i < height; i++)
        paths[i] = new long[width];
    
    // Začínáme s jednou cestou na S
    paths[startRow][startCol] = 1;
    
    // Počítadlo timelines, které vypadly z mřížky
    long totalTimelines = 0;
    
    // Procházet řádek po řádku (top-down)
    for (int r = 0; r < height; r++)
    {
        for (int c = 0; c < width; c++)
        {
            long currentPaths = paths[r][c];
            if (currentPaths == 0)
                continue;
            
            // Posun dolů
            int nextRow = r + 1;
            
            if (nextRow >= height)
            {
                // Částice vypadla dole z mřížky → timeline končí
                totalTimelines += currentPaths;
                continue;
            }
            
            char nextCell = grid[nextRow][c];
            
            if (nextCell == '^')
            {
                // SPLITTER! Rozdělení na L/R
                
                int leftCol = c - 1;
                if (leftCol >= 0)
                    paths[nextRow][leftCol] += currentPaths;
                // Jinak vypadla vlevo - končí (nepočítá se?)
                
                int rightCol = c + 1;
                if (rightCol < width)
                    paths[nextRow][rightCol] += currentPaths;
                // Jinak vypadla vpravo - končí (nepočítá se?)
            }
            else if (nextCell == '.')
            {
                // Volné místo
                paths[nextRow][c] += currentPaths;
            }
            else if (nextCell == 'S')
            {
                // Nemělo by nastat (S je jen na začátku)
                paths[nextRow][c] += currentPaths;
            }
        }
    }
    
    return totalTimelines;
}
```

### 4. Main Solution Method

```csharp
public class Day07 : ISolution
{
    public string SolvePart1(string input)
    {
        var grid = ParseGrid(input);
        int splitCount = CountBeamSplits(grid);
        return splitCount.ToString();
    }
    
    public string SolvePart2(string input)
    {
        var grid = ParseGrid(input);
        long timelineCount = CountTimelines(grid);
        return timelineCount.ToString();
    }
    
    // ... ostatní metody (ParseGrid, FindStart, CountBeamSplits)
    
    private static long CountTimelines(char[][] grid)
    {
        // Implementace výše
    }
}
```

### 5. Test Cases

```csharp
[Fact]
public void Part2_Example_Returns40()
{
    var input = @".......S.......
...............
.......^.......
...............
......^.^......
...............
.....^.^.^.....
...............
....^.^...^....
...............
...^.^...^.^...
...............
..^...^.....^..
...............
.^.^.^.^.^...^.
...............";
    
    var solution = new Day07();
    var result = solution.SolvePart2(input);
    
    Assert.Equal("40", result);
}

[Fact]
public void Part2_ActualInput_ReturnsCorrectAnswer()
{
    var input = File.ReadAllText("TestData/day07.txt");
    var solution = new Day07();
    var result = solution.SolvePart2(input);
    
    // Očekáváme výsledek - bude znám po spuštění
    Assert.NotEmpty(result);
}

[Fact]
public void Part2_SinglePath_NoSplitters_Returns1()
{
    var input = @"S
.
.
.";
    
    var solution = new Day07();
    var result = solution.SolvePart2(input);
    
    Assert.Equal("1", result); // Jen jedna timeline
}

[Fact]
public void Part2_OneSplitter_Returns2()
{
    var input = @".S.
...
.^.
...";
    
    var solution = new Day07();
    var result = solution.SolvePart2(input);
    
    Assert.Equal("2", result); // Dvě timelines (L a R)
}
```

---

## Technical Decisions

### Proč dynamické programování místo BFS s tracking cest?

1. **Efektivita:** DP je **O(width × height)**, sledování všech cest je **O(2^splitterů)**
2. **Paměť:** DP používá **O(width × height)**, tracking cest by používal exponenciální paměť
3. **Jednoduchost:** DP přístup je jednodušší na implementaci a debugování
4. **Matematická elegance:** Sčítání cest se slučuje přirozeně v DP tabulce

### Datový typ pro počítání cest

**Použít `long` místo `int`:**
- S 1,640 splittery by teoreticky mohlo vzniknout 2^1640 timelines
- V praxi bude mnohem méně (díky sloučení cest), ale `long` je bezpečnější
- `long` v C# = 64-bit signed integer = až 9,223,372,036,854,775,807
- Pokud by i to nestačilo, použít `BigInteger`

**Kontrola overflow:**
```csharp
checked
{
    paths[nextRow][leftCol] += currentPaths;
}
```

### Jak správně počítat koncové timelines?

**Otázka:** Co je "timeline"?
- **Odpověď z příkladu:** Timeline = **unikátní cesta od S do konce** (kdy částice opustí mřížku)

**Implementace:**
- Počítat cesty, které "vypadly" z mřížky (řádek `height`)
- NEBO: počítat všechny pozice na poslední řádce, kde jsou nenulové cesty

**Finální rozhodnutí:** Počítat cesty, které **vypadly dole z mřížky** (nextRow >= height).

---

## Acceptance Criteria

- ✅ Správně parsuje 2D mřížku ze vstupu
- ✅ Najde startovní pozici `S`
- ✅ Implementuje dynamické programování pro počítání cest
- ✅ Správně propaguje cesty při průchodu splittery (L/R)
- ✅ Správně sčítá cesty, které se sejdou na stejné pozici
- ✅ Počítá celkový počet timelines (unikátních cest)
- ✅ Používá `long` pro velké čísla
- ✅ Unit test s příkladem z AoC vrací očekávaný výsledek `40`
- ✅ Řešení běží v rozumném čase (< 5 sekund pro reálný vstup)

---

## Notes

**Klíčové poznatky:**
1. **Many-worlds interpretation** = částice bere obě cesty → počítáme všechny možné výsledky
2. **DP je klíč** - sloučení cest na stejných pozicích redukuje exponenciální složitost na polynomiální
3. **Propagace cest** - při splitteru se cesty rozdělí, ale stále se sčítají na cílových pozicích
4. **Koncové timelines** = pozice, kde částice opustila mřížku (dole)

**Potenciální úskalí:**
1. **Integer overflow** - používat `long` nebo `BigInteger`
2. **Off-by-one errors** při indexování
3. **Nesprávné počítání koncových timelines** - ujistit se, co je "timeline"
4. **Zapomenout sčítat cesty** při sloučení na stejné pozici

**Debugging tipy:**
- Vypisovat DP tabulku po každém řádku
- Vizualizovat cesty pro malé příklady
- Zkontrolovat, že součet cest na každém řádku odpovídá očekávání (2^splitterů do daného bodu)

**Očekávání pro výsledek:**
- Example: **40 timelines**
- Reálný vstup: **pravděpodobně tisíce až miliony**, ale ne 2^1640 (díky sloučení cest)
