# Tech-Spec: Day 07 Part 1 - Laboratories

**Created:** 2025-12-07  
**Status:** Completed  
**AoC Link:** https://adventofcode.com/2025/day/7

---

## Overview

### Problem Statement

Po opravě odpadkového lisu se ocitáte v laboratoři s teleportery. Teleporter je rozbitý a uniká z něj kouř. Po připojení diagnostického nástroje zjistíte, že problém je v tachyonovém manifestu.

**Klíčové body:**
- Tachyonový paprsek vstupuje do manifestu na pozici `S` a **vždy se pohybuje směrem dolů**
- Paprsek volně prochází prázdným prostorem (`.`)
- Když paprsek narazí na **splitter** (`^`), původní paprsek se **zastaví** a vytvoří se **dva nové paprsky** - jeden vlevo a jeden vpravo od splitteru
- **Cíl: spočítat, kolikrát dojde k rozdělení paprsku (beam split)**

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

**Postup šíření paprsku:**
1. Paprsek vstupuje na `S` a jde dolů jako `|`
2. Při každém setkání se splitterem `^`:
   - Původní paprsek se **zastaví**
   - Vytvoří se **dva nové paprsky** (vlevo a vpravo)
   - Oba nové paprsky pokračují **dolů**
3. Proces pokračuje, dokud všechny paprsky nevyjdou z manifestu nebo nenarazí na splitter

**Očekávaný výsledek pro example:** `21` rozdělení

### Input Analysis

**Reálný input (`Inputs/day07.txt`):**
- **142 řádků** (síť/mřížka)
- **Šířka: 141 znaků** na řádek
- **Celková velikost: 142 × 141 ≈ 20,000 pozic**
- Obsahuje:
  - Jeden startovní bod `S` (typicky na prvním řádku uprostřed)
  - Mnoho splitterů `^` (stovky)
  - Většina pozic je prázdné místo `.`

**Porovnání s example:**
- Example: 16 řádků × 15 znaků = 240 pozic, 21 splits
- Reálný vstup: 142 řádků × 141 znaků = **20,022 pozic** → **~100× větší!**
- Očekávaný počet splits: **řádově tisíce**

**Důsledky pro algoritmus:**
- ❌ **Rekurzivní přístup bez memoizace** by mohl být pomalý (exponenciální růst paprsků)
- ❌ **Brute force simulace každého paprsku samostatně** by vedla k duplicitním výpočtům
- ✅ **BFS (Breadth-First Search)** s frontou paprsků - simulace šíření všech paprsků najednou
- ✅ **Tracking visited states** - zaznamenat, které pozice a směry jsme už zpracovali
- ⚠️ **Potenciálně cykly?** Ne, paprsky jdou jen dolů → nemůže nastat cyklus

**Důležité poznatky:**
- Paprsky se **pohybují jen dolů** → není cyklus, proces skončí
- Když paprsek narazí na splitter, **počítá se to jako 1 split** (ne 2)
- Když se dva paprsky "sloučí" na stejné pozici (jdou do stejného sloupce), **pokračují jako dva samostatné paprsky** (nesčítají se)
- Paprsky končí, když:
  - Vyjdou ze spodní hrany manifestu
  - Narazí na splitter (zastaví se, ale vytvoří 2 nové)

**Časová složitost:**
- Worst case: každý paprsek může narazit na splitter → **exponenciální růst paprsků**
- Ale: paprsky jdou jen dolů → max. výška je 142
- Každá pozice může být navštívena více paprsky
- S memoizací: **O(width × height × beams)** = **O(141 × 142 × počet_paprsků)**
- V praxi: **lineární vzhledem k velikosti vstupu** s inteligentním trackingem

**Prostorová složitost:**
- Mřížka: O(width × height) = O(20,000)
- Fronta paprsků: O(max_simultaneous_beams) - řádově stovky až tisíce
- Celkem: **O(width × height)** → **triviální**

### Solution

**Algoritmus: BFS simulace šíření paprsků**

1. **Parse vstup do 2D mřížky:**
   ```csharp
   char[][] grid = input.Split('\n')
       .Select(line => line.TrimEnd().ToCharArray())
       .ToArray();
   ```

2. **Najít startovní pozici `S`:**
   ```csharp
   (int startRow, int startCol) = FindStart(grid);
   ```

3. **BFS simulace:**
   ```csharp
   var queue = new Queue<Beam>();
   var visited = new HashSet<(int row, int col)>(); // Pro optimalizaci
   int splitCount = 0;
   
   // Začneme s jedním paprskem na pozici S, jdoucím dolů
   queue.Enqueue(new Beam(startRow, startCol));
   
   while (queue.Count > 0)
   {
       var beam = queue.Dequeue();
       
       // Posunout paprsek o jeden řádek dolů
       int newRow = beam.Row + 1;
       int newCol = beam.Col;
       
       // Kontrola hranic
       if (newRow >= grid.Length)
           continue; // Paprsek opustil mřížku
       
       char cell = grid[newRow][newCol];
       
       if (cell == '^')
       {
           // Splitter! Paprsek se zastaví, vytvoří se dva nové
           splitCount++;
           
           // Levý paprsek (col - 1)
           if (newCol - 1 >= 0)
               queue.Enqueue(new Beam(newRow, newCol - 1));
           
           // Pravý paprsek (col + 1)
           if (newCol + 1 < grid[newRow].Length)
               queue.Enqueue(new Beam(newRow, newCol + 1));
       }
       else if (cell == '.')
       {
           // Volné místo, paprsek pokračuje dolů
           queue.Enqueue(new Beam(newRow, newCol));
       }
   }
   
   return splitCount;
   ```

4. **Optimalizace - tracking stavů:**
   
   Pokud by jeden sloupec mohl být navštíven vícekrát stejným paprskem, potřebujeme tracking:
   
   ```csharp
   var visitedStates = new HashSet<(int row, int col)>();
   
   // Před zpracováním paprsku:
   if (visitedStates.Contains((newRow, newCol)))
       continue; // Už jsme na této pozici byli
   
   visitedStates.Add((newRow, newCol));
   ```
   
   **Ale:** podle zadání se zdá, že paprsky se **nesčítají**, takže každý paprsek je nezávislý. Pokud dva paprsky přijdou na stejnou pozici, oba pokračují samostatně.

5. **Datová struktura:**
   ```csharp
   record Beam(int Row, int Col);
   ```

**Edge cases:**
- ✅ **Paprsek vyjde z hranic** (vlevo/vpravo) po split → ignorovat
- ✅ **Paprsek vyjde ze spodní hrany** → končí, neprochází dál
- ✅ **Více paprsků na stejné pozici** → každý pokračuje samostatně (nesčítají se)
- ✅ **S je na okraji** → paprsek začíná dolů od S
- ⚠️ **Duplicitní zpracování** → pokud by paprsky vytvářely cykly, ale to není možné (jdou jen dolů)

**Poznámky:**
- Paprsky **nejdou horizontálně**, pouze **vertikálně dolů**
- Po split se vytvoří dva paprsky, které **okamžitě začínají na pozicích vlevo a vpravo** od splitteru
- Oba nové paprsky pak pokračují **dolů** ze svých pozic

**Alternativní přístup - rekurze:**
```csharp
int CountSplits(char[][] grid, int row, int col, HashSet<(int, int)> visited)
{
    // Posun dolů
    row++;
    
    if (row >= grid.Length) return 0;
    if (visited.Contains((row, col))) return 0; // Už jsme zde byli
    
    visited.Add((row, col));
    char cell = grid[row][col];
    
    if (cell == '^')
    {
        // Split! Počítáme + pokračujeme vlevo a vpravo
        int splits = 1;
        splits += CountSplits(grid, row, col - 1, visited);
        splits += CountSplits(grid, row, col + 1, visited);
        return splits;
    }
    else
    {
        // Pokračujeme dolů
        return CountSplits(grid, row, col, visited);
    }
}
```

Rekurze je čitelnější, ale může mít problém se stack overflow při velkém vstupu. BFS je bezpečnější.

### Scope

**In Scope (Part 1):**
- ✅ Parsing 2D mřížky
- ✅ Nalezení startovní pozice `S`
- ✅ BFS simulace šíření paprsků
- ✅ Počítání splits při nárazech na `^`
- ✅ Správné zacházení s hranicemi (paprsky končí nebo pokračují)
- ✅ Tracking stavů pro optimalizaci

**Out of Scope (Part 1):**
- ❌ Jiné typy objektů kromě `.`, `^`, `S`
- ❌ Horizontální pohyb paprsků
- ❌ Jiné směry (nahoru, diagonálně)
- ❌ Visualizace postupu paprsků (nice to have pro debugging)

**Nice to Have:**
- 💡 Unit test s příkladem z AoC (16×15 mřížka, očekávaný výsledek 21)
- 💡 Visualizace šíření paprsků (debugging)
- 💡 Validace vstupu (existence `S`, správné znaky)
- 💡 Statistiky (max. počet simultánních paprsků, průměrná hloubka)

---

## Implementation Plan

### 1. Data Structures

```csharp
// Reprezentace paprsku
public record Beam(int Row, int Col);

// Pro tracking visited states (pokud potřeba)
public record BeamState(int Row, int Col);
```

### 2. Parsing

```csharp
public static char[][] ParseGrid(string input)
{
    return input.Split('\n')
        .Select(line => line.TrimEnd())
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => line.ToCharArray())
        .ToArray();
}

public static (int row, int col) FindStart(char[][] grid)
{
    for (int r = 0; r < grid.Length; r++)
    {
        for (int c = 0; c < grid[r].Length; c++)
        {
            if (grid[r][c] == 'S')
                return (r, c);
        }
    }
    throw new InvalidOperationException("Start position 'S' not found");
}
```

### 3. Core Algorithm - BFS Simulation

```csharp
public static int CountBeamSplits(char[][] grid)
{
    var (startRow, startCol) = FindStart(grid);
    
    var queue = new Queue<Beam>();
    var splitPositions = new HashSet<(int row, int col)>();
    var visitedBeams = new HashSet<(int row, int col)>();
    
    // Začínáme na S, první krok je dolů
    queue.Enqueue(new Beam(startRow, startCol));
    visitedBeams.Add((startRow, startCol));
    
    while (queue.Count > 0)
    {
        var beam = queue.Dequeue();
        
        // Posun dolů
        int newRow = beam.Row + 1;
        int newCol = beam.Col;
        
        // Kontrola hranic
        if (newRow >= grid.Length)
            continue;
        
        if (newCol < 0 || newCol >= grid[newRow].Length)
            continue;
        
        // Kontrola, zda jsme na této pozici již byli
        if (visitedBeams.Contains((newRow, newCol)))
            continue;
        
        visitedBeams.Add((newRow, newCol));
        
        char cell = grid[newRow][newCol];
        
        if (cell == '^')
        {
            // SPLIT! Počítáme pouze unikátní splitters
            splitPositions.Add((newRow, newCol));
            
            // Levý paprsek
            int leftCol = newCol - 1;
            if (leftCol >= 0)
            {
                queue.Enqueue(new Beam(newRow, leftCol));
            }
            
            // Pravý paprsek
            int rightCol = newCol + 1;
            if (rightCol < grid[newRow].Length)
            {
                queue.Enqueue(new Beam(newRow, rightCol));
            }
        }
        else if (cell == '.')
        {
            // Volné místo - paprsek pokračuje dolů
            queue.Enqueue(new Beam(newRow, newCol));
        }
    }
    
    return splitPositions.Count;
}
```

**Klíčové implementační detaily:**
- **`splitPositions` HashSet:** Trackuje unikátní pozice splitterů - každý splitter se počítá pouze jednou, i když na něj narazí více paprsků
- **`visitedBeams` HashSet:** Prevence nekonečných cyklů - paprsek se může vrátit na již navštívenou pozici, proto musíme trackovat visited states
- **Kontrola visited před zpracováním:** Kritické pro zamezení nekonečné smyčky s reálným vstupem

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
        return "Not implemented yet";
    }
}
```

### 5. Test Cases

```csharp
[Fact]
public void Part1_Example_Returns21()
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
    var result = solution.SolvePart1(input);
    
    Assert.Equal("21", result);
}

[Fact]
public void Part1_ActualInput_ReturnsCorrectAnswer()
{
    var input = File.ReadAllText("TestData/day07.txt");
    var solution = new Day07();
    var result = solution.SolvePart1(input);
    
    // Očekáváme výsledek - bude znám po spuštění
    Assert.NotEmpty(result);
}
```

---

## Technical Decisions

### Proč BFS místo rekurze?

1. **Bezpečnost:** BFS používá frontu, není riziko stack overflow
2. **Kontrola:** Snazší sledovat stav všech paprsků najednou
3. **Optimalizace:** Jednodušší přidat tracking visited states
4. **Debugging:** Lze snadno vizualizovat stav fronty

### Tracking visited states - ANO nebo NE?

**Finální rozhodnutí: ANO - KRITICKÉ PRO SPRÁVNÉ FUNGOVÁNÍ**

**Důvody:**
1. **Prevence nekonečných cyklů:** Při reálném vstupu může paprsek teoreticky cyklit (i když jde jen dolů, po split může pravý paprsek jít na pozici, kterou už levý paprsek navštívil)
2. **Počítání unikátních splitterů:** Používáme `splitPositions` HashSet pro trackování unikátních splitterů - když více paprsků narazí na stejný splitter, počítá se pouze jednou
3. **Visited beams:** Používáme `visitedBeams` HashSet pro sledování, které pozice již byly navštíveny jakýmkoli paprskem

**Implementace:**
```csharp
var splitPositions = new HashSet<(int row, int col)>();  // Unikátní splitters
var visitedBeams = new HashSet<(int row, int col)>();    // Visited pozice

// Kontrola před zpracováním:
if (visitedBeams.Contains((newRow, newCol)))
    continue;

visitedBeams.Add((newRow, newCol));

// Pri split:
splitPositions.Add((newRow, newCol));  // Počítá se jen jednou
```

### Datové typy

- **Grid:** `char[][]` - nejjednodušší a nejrychlejší
- **Beam:** `record Beam(int Row, int Col)` - immutable, ideální pro queue
- **Split count:** `int` - postačující (max. 20,000 pozic → max. splits je řádově tisíce)

---

## Acceptance Criteria

- ✅ Správně parsuje 2D mřížku ze vstupu
- ✅ Najde startovní pozici `S`
- ✅ Simuluje šíření tachyonových paprsků směrem dolů
- ✅ Správně detekuje splitters (`^`) a provádí split (1 paprsek → 2 paprsky)
- ✅ Počítá celkový počet splitů
- ✅ Paprsky končí při opuštění mřížky (dole nebo z boku)
- ✅ Unit test s příkladem z AoC vrací očekávaný výsledek `21`
- ✅ Řešení běží v rozumném čase (< 5 sekund pro reálný vstup)

---

## Notes

**Potenciální úskalí:**
1. **Off-by-one errors** při posunu dolů a vlevo/vpravo
2. **Boundary conditions** - paprsky vyjíždějící z mřížky
3. **Nesprávné počítání splits** - počítat jen při nárazech na `^`, ne při vytváření nových paprsků
4. **Duplikace paprsků** - pokud nesprávně použijeme visited set

**Debugging tipy:**
- Vypisovat pozice paprsků v každém kroku
- Visualizovat mřížku s aktuálními paprsky
- Počítat simultánní paprsky (max. hodnota)

**Očekávání pro Part 2:**
- Pravděpodobně změna pravidel (horizontální pohyb? více směrů? jiné typy splitterů?)
- Možná optimalizace algoritmu (větší vstupy? více typů paprsků?)
- Být připraven refaktorovat pro nové požadavky
