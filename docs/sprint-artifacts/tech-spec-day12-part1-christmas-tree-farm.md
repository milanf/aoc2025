# Tech-Spec: Day 12 Part 1 - Christmas Tree Farm

**Created:** 2025-12-12  
**Status:** 📝 **Draft**  
**AoC Link:** https://adventofcode.com/2025/day/12

---

## Overview

### Problem Statement

Nacházíme se ve vánoční stromové farmě na Severním pólu. Elfové se snaží umístit dárky pod stromečky, ale musí se ujistit, že všechny dárky se vejdou do přidělených regionů. Dárky mají různé standardizované tvary a musí být umístěny přesně na 2D mřížce bez překrývání.

**Cíl:** Určit **počet regionů**, do kterých se vejdou **všechny** přiřazené dárky.

**Klíčové body:**
- Dárky mají definované tvary (jako tetris bloky)
- Dárky lze **otáčet a převracet** (všechny možné transformace)
- Dárky **nesmí překrývat** své `#` pozice, ale mohou sdílet `.` pozice
- Region má pevnou velikost (šířka × výška)
- Pro každý region musíme zjistit, zda se do něj vejdou **všechny** zadané dárky

**Příklad z AoC:**

**Definice tvarů dárků:**
```
0:
###
##.
##.

1:
###
.##
..#

2:
.##
###
##.

3:
###
.#.
###

4:
###
#..
###

5:
###
.#.
###
```

**Regiony k ověření:**
```
4x4: 0 0 0 0 2 0  → region 4×4, potřebuje: 4× shape[4], 2× shape[2]
12x5: 1 0 1 0 2 2  → region 12×5, potřebuje: 1× shape[0], 1× shape[1], 2× shape[3], 2× shape[4]
12x5: 1 0 1 0 3 2  → region 12×5, potřebuje: 1× shape[0], 1× shape[1], 3× shape[3], 2× shape[4]
```

**Výsledek:**
- **Region 1 (4×4):** ✅ Všechny dárky se vejdou
- **Region 2 (12×5):** ✅ Všechny dárky se vejdou
- **Region 3 (12×5):** ❌ Dárky se **nevejdou** (o 1 dárek více než v Regionu 2)

**Odpověď: 2 regiony se vejdou**

### Input Analysis

**Reálný input (`Inputs/day12.txt`):**

**Sekce 1: Definice tvarů (6 tvarů, indexy 0-5)**
```
0: 3×3 shape (7 bloků)
1: 3×3 shape (7 bloků)
2: 3×3 shape (8 bloků)
3: 3×3 shape (7 bloků)
4: 3×3 shape (7 bloků)
5: 3×3 shape (7 bloků)
```

**Sekce 2: Regiony (500 regionů)**
- **Počet regionů:** 500
- **Velikosti regionů:**
  - Nejmenší: 35×35 = 1,225 buněk
  - Největší: 50×50 = 2,500 buněk
  - Průměrná velikost: ~43×43 = ~1,849 buněk

**Analýza požadavků na dárky:**
- **Počet dárků na region:** 138–472 dárků celkem
- **Průměrný počet dárků:** ~231 dárků na region
- **Největší požadavek:** 81× jeden typ dárku (shape 4) v regionu 50×48

**KRITICKÁ ANALÝZA SLOŽITOSTI:**

**Proč brute-force nebude fungovat:**
- Pro 231 dárků v regionu 43×43:
  - Každý dárek má ~8 možných orientací (4 rotace × 2 flips)
  - Každý dárek může být umístěn na ~1,800 pozicích
  - **Celkový prostor řešení:** $(8 \times 1800)^{231} \approx 10^{800}$ možností ⚠️ **NEMOŽNÉ**

**Důležité pozorování:**
1. Každý tvar má **~7-8 bloků** (`#`)
2. Region má průměrně **1,849 buněk**
3. Potřebujeme umístit **~231 dárků × 7 bloků = 1,617 bloků**
4. **Využití prostoru:** 1,617 / 1,849 ≈ **87.5%** ✅ Vysoce zaplněné!

**Klíčové zjištění:**
⚠️ **Tento problém je NP-complete** (varianta 2D Bin Packing / Tetris Packing)
- Neexistuje polynomiální algoritmus pro obecné řešení
- Musíme použít:
  - **Backtracking s pruningem** (DFS s heuristikami)
  - **Constraint Satisfaction Problem (CSP)** přístup
  - **Greedy heuristiky** + ověření

### Algorithm Analysis

#### Přístup 1: Backtracking DFS s inteligentním pruningem ✅ **DOPORUČENÝ**

**High-level algoritmus:**
```csharp
function CanFitAllPresents(region, presentCounts):
    grid = CreateEmptyGrid(region.width, region.height)
    presentList = ExpandPresentCounts(presentCounts) // [shape0, shape0, ..., shape1, ...]
    return Backtrack(grid, presentList, 0)

function Backtrack(grid, presents, index):
    if index == presents.Count:
        return true  // Všechny dárky umístěny!
    
    shape = presents[index]
    
    // Zkusíme všechny transformace (rotace + flipy)
    foreach transformation in GetAllTransformations(shape):
        // Zkusíme všechny pozice v gridu
        foreach position in grid.GetPossiblePositions():
            if CanPlaceAt(grid, transformation, position):
                PlacePresent(grid, transformation, position)
                
                if Backtrack(grid, presents, index + 1):
                    return true
                
                RemovePresent(grid, transformation, position) // Backtrack
    
    return false  // Tento dárek nejde umístit
```

**Klíčové optimalizace (pruning):**

1. **Early termination:** Pokud zbývající dárky nemusí fyzicky sedět
   ```csharp
   int requiredBlocks = GetRemainingBlockCount(presents, index);
   int availableSpace = CountEmptySpace(grid);
   if (requiredBlocks > availableSpace) return false;
   ```

2. **MRV (Most Restrictive Variable):** Začni od nejtěžších dárků
   ```csharp
   // Seřaď dárky podle "obtížnosti" umístění
   presentList = presentList.OrderByDescending(p => GetDifficulty(p));
   ```

3. **Cache transformací:** Předpočítej všechny rotace/flipy
   ```csharp
   Dictionary<int, List<Shape>> transformationCache;
   ```

4. **Bit-based grid:** Použij `BitArray` nebo `ulong[]` pro rychlé ověření překrytí
   ```csharp
   bool CanPlaceAt(ulong[] grid, ulong[] shape, int x, int y):
       return (grid[y] & (shape[0] << x)) == 0;
   ```

**Časová složitost:** $O(k \times n \times m \times 8)$ kde:
- $k$ = počet dárků (~231)
- $n \times m$ = velikost regionu (~1,849)
- $8$ = počet transformací

**Best case:** $O(k)$ - všechny dárky sednou na první pokus
**Worst case:** $O((8 \times nm)^k)$ - exponenciální

**S pruningem očekáváme:** $O(k^2 \times nm)$ - kvadratická až kubická

#### Přístup 2: Greedy + Verification 🚀 **RYCHLÉ ŘEŠENÍ**

**Algoritmus:**
```csharp
function TryFitGreedy(region, presentCounts):
    grid = CreateEmptyGrid(region.width, region.height)
    presentList = ExpandAndSort(presentCounts) // Seřaď podle velikosti
    
    foreach present in presentList:
        bestFit = null
        bestScore = infinity
        
        // Najdi nejlepší pozici pro tento dárek
        foreach transformation in GetTransformations(present):
            foreach position in GetValidPositions(grid, transformation):
                score = EvaluatePlacement(grid, transformation, position)
                if score < bestScore:
                    bestFit = (transformation, position)
                    bestScore = score
        
        if bestFit == null:
            return false  // Dárek se nevejde
        
        PlacePresent(grid, bestFit.transformation, bestFit.position)
    
    return true
```

**Heuristiky pro `EvaluatePlacement`:**
- **Bottom-left heuristic:** Preferuj levý dolní roh
- **Tight fit:** Preferuj pozice, kde je dárek těsně u jiných
- **Corner filling:** Preferuj zaplnění rohů

**Výhody:**
- Rychlé: $O(k \times nm \times 8)$
- Deterministické
- Snadná implementace

**Nevýhody:**
- Nemusí najít řešení, i když existuje
- Pro AoC často stačí ✅

#### Přístup 3: Dancing Links (DLX) Algorithm 🎯 **PROFESIONÁLNÍ**

**Pokročilá technika** pro Exact Cover Problem:
- Reprezentuj problém jako matici 0/1
- Použij Donald Knuth's Algorithm X s Dancing Links

**Výhody:**
- Garantované nalezení řešení (pokud existuje)
- Extrémně rychlý pruning

**Nevýhody:**
- Složitá implementace
- Možná overkill pro AoC

#### Porovnání přístupů:

| Přístup | Rychlost | Úspěšnost | Složitost impl. | Doporučení |
|---------|----------|-----------|-----------------|------------|
| Backtracking + pruning | Střední | 100% | Střední | ✅ **BEST** |
| Greedy | Velmi rychlé | ~80% | Nízká | ⚠️ Fallback |
| Dancing Links | Rychlé | 100% | Vysoká | 🎯 Pro pokročilé |

**Doporučení pro implementaci:**
1. ✅ **Start:** Backtracking s pruningem
2. Pokud je příliš pomalé → přidej víc heuristik
3. Pokud stále pomalé → použij Greedy jako pre-filter

---

## Requirements

### Functional Requirements

1. **RF1: Parsování vstupních dat**
   - **RF1.1:** Načíst definice tvarů dárků
     - Formát: `index:\n` následované řádky s `.` a `#`
     - Uložit jako `Dictionary<int, char[,]>`
   - **RF1.2:** Načíst definice regionů
     - Formát: `width×height: count0 count1 count2 count3 count4 count5`
     - Uložit jako `List<(int width, int height, int[] counts)>`

2. **RF2: Generování transformací tvarů**
   - **RF2.1:** Pro každý tvar vygenerovat:
     - 4 rotace (0°, 90°, 180°, 270°)
     - 2 flipy (horizontální, vertikální)
     - Celkem až **8 unikátních transformací** (některé mohou být duplicitní)
   - **RF2.2:** Odstranit duplicitní transformace
   - **RF2.3:** Cachovat transformace pro znovupoužití

3. **RF3: Kontrola umístění dárku**
   - **RF3.1:** Ověřit, že dárek:
     - Nezasahuje mimo hranice regionu
     - Nepřekrývá `#` bloky jiných dárků
     - `.` bloky dárku mohou překrývat cokoliv
   - **RF3.2:** Použít efektivní reprezentaci (bit arrays)

4. **RF4: Backtracking algoritmus**
   - **RF4.1:** Zkusit umístit všechny dárky do regionu
   - **RF4.2:** Použít backtracking při neúspěchu
   - **RF4.3:** Aplikovat pruning heuristiky:
     - Early termination (nedostatek místa)
     - MRV (nejtěžší dárky první)
     - Forward checking

5. **RF5: Počítání úspěšných regionů**
   - **RF5.1:** Pro každý region zkusit umístit všechny dárky
   - **RF5.2:** Započítat region, pokud se všechny dárky vejdou
   - **RF5.3:** Vrátit celkový počet úspěšných regionů

### Non-Functional Requirements

1. **NFR1: Výkon**
   - **NFR1.1:** Zpracování jednoho regionu: < 5 sekund (průměrně)
   - **NFR1.2:** Celkový čas pro 500 regionů: < 15 minut
   - **NFR1.3:** Použít časové limity pro prevenci nekonečných výpočtů

2. **NFR2: Paměť**
   - **NFR2.1:** Použít efektivní reprezentaci gridu (bit arrays)
   - **NFR2.2:** Cachovat transformace, ne rekalkulovat
   - **NFR2.3:** Paměťová spotřeba: < 500 MB

3. **NFR3: Správnost**
   - **NFR3.1:** Ověřit řešení na příkladech z AoC
   - **NFR3.2:** Unit testy pro transformace
   - **NFR3.3:** Edge cases: prázdné regiony, přeplněné regiony

---

## Design

### Data Structures

#### 1. Present Shape
```csharp
public class PresentShape
{
    public int Id { get; set; }
    public bool[,] Grid { get; set; }  // true = #, false = .
    public int Width { get; set; }
    public int Height { get; set; }
    public int BlockCount { get; set; }  // Počet '#' bloků
    
    private List<PresentShape>? _transformations;
    public List<PresentShape> GetTransformations()
    {
        if (_transformations == null)
        {
            _transformations = GenerateAllTransformations();
        }
        return _transformations;
    }
}
```

#### 2. Region
```csharp
public class Region
{
    public int Width { get; set; }
    public int Height { get; set; }
    public int[] PresentCounts { get; set; }  // [count0, count1, ..., count5]
    
    public int GetTotalPresentCount() => PresentCounts.Sum();
    public int GetTotalBlockCount(Dictionary<int, PresentShape> shapes)
        => PresentCounts.Select((c, i) => c * shapes[i].BlockCount).Sum();
}
```

#### 3. Grid (efektivní reprezentace)
```csharp
public class Grid
{
    private readonly ulong[] _rows;  // Každý řádek jako bitová maska
    public int Width { get; }
    public int Height { get; }
    
    public bool CanPlaceAt(PresentShape shape, int x, int y)
    {
        for (int dy = 0; dy < shape.Height; dy++)
        {
            for (int dx = 0; dx < shape.Width; dx++)
            {
                if (shape.Grid[dy, dx])  // '#' blok
                {
                    if (x + dx >= Width || y + dy >= Height) return false;
                    if (IsOccupied(x + dx, y + dy)) return false;
                }
            }
        }
        return true;
    }
    
    public void PlacePresent(PresentShape shape, int x, int y) { /* ... */ }
    public void RemovePresent(PresentShape shape, int x, int y) { /* ... */ }
}
```

### Algorithm Implementation

#### Core Backtracking Function

```csharp
public class ChristmasTreeFarmSolver
{
    private Dictionary<int, PresentShape> _shapes;
    
    public int SolvePartOne(string input)
    {
        var (shapes, regions) = ParseInput(input);
        _shapes = shapes;
        
        int successfulRegions = 0;
        
        foreach (var region in regions)
        {
            if (CanFitAllPresents(region))
            {
                successfulRegions++;
            }
        }
        
        return successfulRegions;
    }
    
    private bool CanFitAllPresents(Region region)
    {
        var grid = new Grid(region.Width, region.Height);
        var presents = ExpandPresents(region.PresentCounts);
        
        // Seřaď dárky podle obtížnosti (větší první)
        presents = presents.OrderByDescending(p => _shapes[p].BlockCount).ToList();
        
        return Backtrack(grid, presents, 0);
    }
    
    private bool Backtrack(Grid grid, List<int> presents, int index)
    {
        if (index == presents.Count)
            return true;  // Všechny dárky úspěšně umístěny
        
        int shapeId = presents[index];
        var shape = _shapes[shapeId];
        
        // Early termination: zkontroluj, zda se zbývající dárky vejdou
        int remainingBlocks = GetRemainingBlockCount(presents, index);
        if (remainingBlocks > grid.GetEmptySpace())
            return false;
        
        // Zkus všechny transformace
        foreach (var transformation in shape.GetTransformations())
        {
            // Zkus všechny pozice (s heuristikami)
            foreach (var (x, y) in GetCandidatePositions(grid, transformation))
            {
                if (grid.CanPlaceAt(transformation, x, y))
                {
                    grid.PlacePresent(transformation, x, y);
                    
                    if (Backtrack(grid, presents, index + 1))
                        return true;
                    
                    grid.RemovePresent(transformation, x, y);
                }
            }
        }
        
        return false;
    }
    
    private IEnumerable<(int x, int y)> GetCandidatePositions(Grid grid, PresentShape shape)
    {
        // Heuristika: zkus nejdřív levý horní roh, pak postupuj řádek po řádku
        for (int y = 0; y <= grid.Height - shape.Height; y++)
        {
            for (int x = 0; x <= grid.Width - shape.Width; x++)
            {
                yield return (x, y);
            }
        }
    }
}
```

#### Shape Transformations

```csharp
public static class ShapeTransformations
{
    public static List<PresentShape> GenerateAllTransformations(PresentShape original)
    {
        var transformations = new HashSet<string>();  // Pro detekci duplicit
        var results = new List<PresentShape>();
        
        var current = original;
        
        // 4 rotace
        for (int rotation = 0; rotation < 4; rotation++)
        {
            // Přidej aktuální rotaci
            AddIfUnique(current, transformations, results);
            
            // Přidej horizontální flip
            var flipped = FlipHorizontal(current);
            AddIfUnique(flipped, transformations, results);
            
            // Rotuj pro další iteraci
            current = Rotate90(current);
        }
        
        return results;
    }
    
    private static PresentShape Rotate90(PresentShape shape)
    {
        int newWidth = shape.Height;
        int newHeight = shape.Width;
        var newGrid = new bool[newHeight, newWidth];
        
        for (int y = 0; y < shape.Height; y++)
        {
            for (int x = 0; x < shape.Width; x++)
            {
                newGrid[x, shape.Height - 1 - y] = shape.Grid[y, x];
            }
        }
        
        return new PresentShape
        {
            Id = shape.Id,
            Width = newWidth,
            Height = newHeight,
            Grid = newGrid,
            BlockCount = shape.BlockCount
        };
    }
    
    private static PresentShape FlipHorizontal(PresentShape shape)
    {
        var newGrid = new bool[shape.Height, shape.Width];
        
        for (int y = 0; y < shape.Height; y++)
        {
            for (int x = 0; x < shape.Width; x++)
            {
                newGrid[y, shape.Width - 1 - x] = shape.Grid[y, x];
            }
        }
        
        return new PresentShape
        {
            Id = shape.Id,
            Width = shape.Width,
            Height = shape.Height,
            Grid = newGrid,
            BlockCount = shape.BlockCount
        };
    }
    
    private static string GetShapeSignature(PresentShape shape)
    {
        var sb = new StringBuilder();
        for (int y = 0; y < shape.Height; y++)
        {
            for (int x = 0; x < shape.Width; x++)
            {
                sb.Append(shape.Grid[y, x] ? '#' : '.');
            }
        }
        return sb.ToString();
    }
}
```

---

## Testing Strategy

### Unit Tests

1. **Test parsing**
   ```csharp
   [Fact]
   public void ParseInput_ShouldExtractShapesAndRegions()
   {
       var input = File.ReadAllText("day12_example.txt");
       var (shapes, regions) = solver.ParseInput(input);
       
       Assert.Equal(6, shapes.Count);
       Assert.Equal(3, regions.Count);
   }
   ```

2. **Test transformations**
   ```csharp
   [Fact]
   public void GenerateTransformations_ShouldHandleSymmetry()
   {
       var square = new PresentShape { /* 2x2 square */ };
       var transformations = square.GetTransformations();
       
       Assert.Equal(1, transformations.Count);  // Čtverec má jen 1 unikátní tvar
   }
   ```

3. **Test placement**
   ```csharp
   [Fact]
   public void CanPlaceAt_ShouldDetectCollisions()
   {
       var grid = new Grid(5, 5);
       var shape = CreateTestShape();
       
       grid.PlacePresent(shape, 0, 0);
       Assert.False(grid.CanPlaceAt(shape, 0, 0));  // Překrytí
       Assert.True(grid.CanPlaceAt(shape, 2, 2));   // Volné místo
   }
   ```

### Integration Tests

```csharp
[Theory]
[InlineData("day12_example.txt", 2)]
public void SolvePartOne_Example(string filename, int expected)
{
    var input = File.ReadAllText($"TestData/{filename}");
    var result = solver.SolvePartOne(input);
    Assert.Equal(expected, result);
}
```

---

## Edge Cases

1. **Prázdný region** (0×0)
   - Očekávaný výsledek: `true` pokud nejsou požadovány žádné dárky

2. **Region přesně odpovídá dárkům**
   - Využití prostoru: 100%
   - Musí najít exact fit

3. **Příliš malý region**
   - Více dárků než místa
   - Early termination

4. **Všechny dárky stejného typu**
   - Optimalizace: batch placement?

5. **Symetrické tvary**
   - Méně transformací → rychlejší

---

## Performance Considerations

### Optimization Techniques

1. **Bit Operations**
   ```csharp
   // Místo bool[,] použij ulong[] pro rychlejší operace
   ulong rowMask = _rows[y];
   bool isOccupied = (rowMask & (1UL << x)) != 0;
   ```

2. **Transformation Caching**
   ```csharp
   private static readonly Dictionary<int, List<PresentShape>> _transformCache = new();
   ```

3. **Parallel Processing** (pro více regionů)
   ```csharp
   int successfulRegions = regions
       .AsParallel()
       .Count(r => CanFitAllPresents(r));
   ```

4. **Timeout Protection**
   ```csharp
   var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
   if (!TryBacktrack(grid, presents, 0, cts.Token))
       return false;  // Timeout → považuj za neúspěch
   ```

### Expected Performance

Pro 500 regionů s průměrně 231 dárky:
- **Best case:** 5-10 minut (většina regionů sedne rychle)
- **Worst case:** 30-60 minut (mnoho backtrackingu)
- **Optimistic target:** < 15 minut s dobrými heuristikami

---

## Implementation Checklist

- [ ] Implementovat `PresentShape` a parsování tvarů
- [ ] Implementovat `Region` a parsování regionů
- [ ] Implementovat transformace (rotace + flipy)
- [ ] Implementovat efektivní `Grid` s bit operations
- [ ] Implementovat základní backtracking
- [ ] Přidat pruning heuristiky (early termination, MRV)
- [ ] Implementovat heuristiky pro pořadí pozic
- [ ] Přidat caching transformací
- [ ] Napsat unit testy
- [ ] Ověřit na AoC příkladech
- [ ] Optimalizovat výkon (profiling)

---

## References

- [AoC 2025 Day 12](https://adventofcode.com/2025/day/12)
- [2D Bin Packing Problem](https://en.wikipedia.org/wiki/Bin_packing_problem)
- [Exact Cover Problem](https://en.wikipedia.org/wiki/Exact_cover)
- [Donald Knuth's Dancing Links](https://arxiv.org/abs/cs/0011047)
- [Backtracking Algorithms](https://en.wikipedia.org/wiki/Backtracking)
