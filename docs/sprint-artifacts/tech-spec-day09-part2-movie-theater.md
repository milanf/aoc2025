# Tech-Spec: Day 09 Part 2 - Movie Theater (Green Tiles Constraint)

**Created:** 2025-12-09  
**Status:** ✅ **Completed**  
**AoC Link:** https://adventofcode.com/2025/day/9

---

## Overview

### Problem Statement

V Part 2 přichází **nové omezení**: obdélník může obsahovat **pouze červené nebo zelené dlaždice**.

**Definice zelených dlaždic:**
1. **Hrany mezi červenými body** - každý červený bod je spojen s dalším červeným bodem přímkou (horizontální nebo vertikální)
2. **Seznam je cyklický** - první červený bod se spojuje s posledním
3. **Všechny body uvnitř polygonu** tvořeného červenými body jsou také zelené

**Klíčové změny oproti Part 1:**
- Part 1: Jakýkoliv obdélník s červenými rohy
- Part 2: Obdélník s červenými rohy, ale **všechny ostatní body musí být červené nebo zelené**

**Příklad z AoC:**
```
..............
.......#XXX#..
.......XXXXX..
..#XXXX#XXXX..
..XXXXXXXXXX..
..#XXXXXX#XX..
.........XXX..
.........#X#..
..............
```

Kde:
- `#` = červené dlaždice (ze seznamu)
- `X` = zelené dlaždice (hrany + vnitřek polygonu)
- `.` = ostatní dlaždice (NEJSOU povoleny v obdélníku)

**Největší validní obdélník** v příkladu má plochu **24** (např. mezi (9,5) a (2,3)).

### Input Analysis

**Reálný input (`Inputs/day09.txt`):**
- **496 červených dlaždic** (496 řádků)
- Body jsou seřazeny tak, že tvoří **uzavřený polygon**
- Každá dvojice sousedních bodů sdílí buď X nebo Y souřadnici (tvoří horizontální/vertikální hranu)
- **Rozsah X souřadnic:** 1,851 až 98,064
- **Rozsah Y souřadnic:** 1,571 až 98,074
- **Celková oblast:** ~96,213 × 96,503 ≈ **9.3 miliardy** potenciálních bodů

**Analýza struktury:**
```
Body[0] -> Body[1]: (97554,50097) -> (97554,51315) | ΔX=0, ΔY=1218 [vertikální hrana]
Body[1] -> Body[2]: (97554,51315) -> (98014,51315) | ΔX=460, ΔY=0 [horizontální hrana]
Body[2] -> Body[3]: (98014,51315) -> (98014,52516) | ΔX=0, ΔY=1201 [vertikální hrana]
...
Body[495] -> Body[0]: (97735,50097) -> (97554,50097) [uzavření smyčky]
```

**Charakteristika:**
- Polygon má **496 vrcholů** a **496 hran**
- Všechny hrany jsou axis-aligned (horizontální nebo vertikální)
- Polygon může být **konkávní** (mít vnitřní "zuby")
- Zelených dlaždic může být **miliony** (všechny body na hranách + uvnitř)

### Algorithm Analysis

#### Naivní přístup (NEPOUŽITELNÝ)
```
for each red corner pair (i, j):
    for each point (x, y) inside rectangle:
        if point is not red/green:
            reject rectangle
```

**Časová složitost:** $O(n^2 \times W \times H)$ kde:
- $n = 496$ červených bodů → $\binom{496}{2} = 122,760$ kombinací
- $W, H \approx 96,000$ průměrná velikost obdélníku
- **Celkem: ~1.1 trillion operací** ❌ **NEPŘIJATELNÉ**

#### Problém s Point-by-Point validací

**Ray Casting přístup:**
```
1. Sestavit polygon z červených bodů (v pořadí ze vstupu)
2. Pro každou dvojici červených bodů (rohy obdélníku):
   a. Určit bounding box: (minX, minY) až (maxX, maxY)
   b. Pro každý bod (x,y) v bounding boxu:
      - Zkontrolovat: je červený NEBO je zelený (na hraně nebo uvnitř polygonu)
   c. Pokud všechny body jsou červené/zelené → spočítat plochu
3. Vrátit maximální plochu
```

**Časová složitost:** $O(n^2 \times W \times H \times P)$ kde $P$ = počet hran polygonu
- **Příliš pomalé** pro velké obdélníky (některé mohou mít plochu > 50,000)

#### Efektivní řešení: Coordinate Compression + Scanline Fill ⭐

**Klíčové poznatky:**
1. Červené body mají pouze **496 unikátních X souřadnic** a **496 unikátních Y souřadnic**
2. Oblast mezi červenými body můžeme rozdělit na **mřížku buněk** (bands/bands)
3. Každá buňka je buď **celá uvnitř** polygonu nebo **celá vně**
4. Pomocí **2D Prefix Sum** můžeme validovat obdélník v O(1)

**Algoritmus:**

**Fáze 1: Coordinate Compression**
```
1. Extrahuj všechny unikátní X souřadnice → xs[] (seřaď vzestupně)
2. Extrahuj všechny unikátní Y souřadnice → ys[] (seřaď vzestupně)
3. Vytvoř index mapy: coordinate → compressed index
```

**Fáze 2: Scanline Fill (Even-Odd Rule)**
```
4. Pro každý Y-band (mezi ys[j] a ys[j+1]):
   a. Najdi všechny vertikální hrany polygonu, které protínají tento band
   b. Seřaď X-souřadnice těchto hran
   c. Použij even-odd rule: páry průsečíků definují "inside" regiony
   d. Označ všechny X-bandy mezi páry jako "inside"
```

**Fáze 3: 2D Prefix Sum pro Outside buňky**
```
5. Vytvoř 2D pole inside[yBand][xBand] (true = inside polygon)
6. Vytvoř prefix sum pro "outside" buňky:
   pref[j+1][i+1] = pref[j+1][i] + pref[j][i+1] - pref[j][i] + (inside[j][i] ? 0 : 1)
```

**Fáze 4: Validace obdélníků**
```
7. Pro každou dvojici červených bodů (a, b):
   a. Určit (xl, yl) až (xr, yr) - rohy obdélníku
   b. Mapovat na compressed souřadnice: ixL, ixR, iyL, iyR
   c. Spočítat počet "outside" buněk v regionu pomocí prefix sum (O(1))
   d. Pokud outsideCount == 0 → obdélník je validní
   e. Spočítat plochu = (xr - xl + 1) × (yr - yl + 1)
8. Vrátit maximální plochu
```

**Časová složitost:**
- **Fáze 1:** $O(n \log n)$ pro třídění souřadnic
- **Fáze 2:** $O(n \times Y_B)$ kde $Y_B = |ys| - 1 \approx 496$
- **Fáze 3:** $O(X_B \times Y_B) \approx 496 \times 496 = 246,016$ operací
- **Fáze 4:** $O(n^2)$ pro všechny páry, validace O(1) díky prefix sum
- **Celkem:** $O(n^2 + n \times Y_B + X_B \times Y_B) \approx 122,760 + 246,016 + 246,016 = \mathbf{614,792}$ operací ✅

**Prostorová složitost:** $O(X_B \times Y_B) \approx 496 \times 496 = 246,016$ buněk (~ 240 KB)

#### Implementační detaily

**Scanline Fill - Even-Odd Rule:**
```csharp
// Pro každý Y-band sbírej průsečíky s vertikálními hranami
for (int i = 0; i < n; i++)
{
    var p1 = points[i];
    var p2 = points[(i + 1) % n];
    
    if (p1.X == p2.X)  // Vertikální hrana
    {
        int xi = xIndex[p1.X];
        int yi1 = yIndex[p1.Y];
        int yi2 = yIndex[p2.Y];
        if (yi1 > yi2) swap(yi1, yi2);
        
        // Hrana protína Y-bandy [yi1, yi2)
        for (int j = yi1; j < yi2; j++)
        {
            rowXs[j].Add(xi);
        }
    }
}

// Even-odd rule: páry průsečíků = inside
for (int j = 0; j < yBands; j++)
{
    var xsList = rowXs[j].Sorted().Distinct();
    for (int k = 0; k + 1 < xsList.Count; k += 2)
    {
        int xL = xsList[k];
        int xR = xsList[k + 1];
        for (int i = xL; i < xR; i++)
        {
            inside[j, i] = true;
        }
    }
}
```

**2D Prefix Sum pro rychlé regionové dotazy:**
```csharp
// Vytvoř prefix sum pro "outside" buňky
int[,] pref = new int[yBands + 1, xBands + 1];
for (int j = 0; j < yBands; j++)
{
    for (int i = 0; i < xBands; i++)
    {
        int outside = inside[j, i] ? 0 : 1;
        pref[j+1, i+1] = pref[j+1, i] + pref[j, i+1] - pref[j, i] + outside;
    }
}

// Dotaz na region [x1, x2) × [y1, y2) v O(1)
int GetOutsideCount(int x1, int y1, int x2, int y2)
{
    return pref[y2, x2] - pref[y1, x2] - pref[y2, x1] + pref[y1, x1];
}
```

**Binary Search pro mapování souřadnic:**
```csharp
int BinarySearchLowerBound(List<long> sorted, long value)
{
    int left = 0, right = sorted.Count;
    while (left < right)
    {
        int mid = left + (right - left) / 2;
        if (sorted[mid] < value)
            left = mid + 1;
        else
            right = mid;
    }
    return left;
}
```

#### Finální algoritmus

```
1. Parsovat červené body ze vstupu → points[]
2. Coordinate Compression:
   - Extrahuj unikátní xs[] a ys[], seřaď vzestupně
   - Vytvoř index mapy xIndex[x] → i, yIndex[y] → j
3. Scanline Fill:
   - Pro každý Y-band: sbírej průsečíky s vertikálními hranami
   - Použij even-odd rule k označení inside[j, i] buněk
4. 2D Prefix Sum:
   - Vytvoř pref[j+1, i+1] pro rychlé dotazy na "outside" buňky
5. Validace obdélníků:
   - Pro každou dvojici (a, b) červených bodů:
     * Mapuj rohy na compressed souřadnice
     * Dotaz na outsideCount v O(1) pomocí prefix sum
     * Pokud outsideCount == 0 → spočítej plochu
6. Vrátit maximální plochu
```

**Časová složitost:** $O(n^2 + n \times Y_B + X_B \times Y_B) \approx 615,000$ operací ⚡

**Prostorová složitost:** $O(X_B \times Y_B) \approx 246,016$ buněk (~ 240 KB)

---

## Requirements

### Functional Requirements

1. **RF1: Parsování vstupu**
   - Načíst seznam 496 červených bodů
   - Formát: `X,Y` na každém řádku
   - Body jsou v pořadí, které tvoří polygon

2. **RF2: Coordinate Compression**
   - Extrahovat unikátní X a Y souřadnice
   - Seřadit vzestupně a vytvořit index mapy
   - Komprimovat souřadnice z rozsahu ~96k na ~496 indexů

3. **RF3: Scanline Fill s Even-Odd Rule**
   - Pro každý Y-band identifikovat vertikální hrany polygonu
   - Použít even-odd rule k určení inside/outside buněk
   - Označit všechny buňky uvnitř polygonu

4. **RF4: 2D Prefix Sum**
   - Vytvořit prefix sum pro rychlé dotazy na počet "outside" buněk
   - Dotaz na region musí být v O(1)

5. **RF5: Validace obdélníků**
   - Pro každou dvojici červených bodů (jako rohy):
     - Mapovat rohy na compressed souřadnice
     - Spočítat počet "outside" buněk pomocí prefix sum
     - Pokud outsideCount == 0 → obdélník je validní
   - Spočítat plochu = (xr - xl + 1) × (yr - yl + 1)

6. **RF6: Výstup výsledku**
   - Vrátit maximální plochu validního obdélníku

### Non-Functional Requirements

1. **NFR1: Výkon**
   - Řešení musí běžet v < 100 ms pro vstup s 496 body
   - Coordinate compression snižuje složitost z miliard na stovky tisíc operací
   - 2D prefix sum umožňuje O(1) validaci obdélníků

2. **NFR2: Přesnost**
   - Použít celočíselné aritmetiku (bez zaokrouhlování)
   - Správně implementovat even-odd rule pro scanline fill
   - Binary search musí najít správný index v compressed coordinates

3. **NFR3: Paměť**
   - O(X_B × Y_B) pro inside grid a prefix sum (~ 246,016 buněk = 240 KB)
   - O(n) pro polygon, compressed coordinates, index mapy
   - Celkem < 1 MB paměti

---

## Edge Cases

1. **Obdélník celý uvnitř polygonu**
   - Všechny compressed buňky v regionu jsou "inside" → validní ✅
   - outsideCount == 0

2. **Obdélník částečně venku**
   - Některé compressed buňky jsou "outside" → nevalidní ❌
   - outsideCount > 0

3. **Velmi malý obdélník**
   - Může být celý v jedné compressed buňce → validní pokud buňka je "inside" ✅

4. **Velmi velký obdélník**
   - Pokrývá mnoho compressed buněk → rychle validován díky prefix sum ✅

5. **Konkávní polygon**
   - Scanline fill správně vyplní jen vnitřní části ✅
   - Even-odd rule automaticky řeší konkávní tvary

6. **Degenerovaný polygon (< 2 body)**
   - Vrátit 0 (není validní polygon)

7. **Degenerovaný polygon (všechny body na jedné linii)**
   - xn <= 1 nebo yn <= 1 → vrátit 0

8. **Dva shodné červené body**
   - Obdélník s plochou 0 → automaticky přeskočen (area <= bestArea)

9. **Červené body na jedné linii**
   - Obdélník s šířkou nebo výškou 1 → může být validní ✅

---

## Test Cases

### Test 1: Příklad z AoC (Part 2)
**Vstup:**
```
7,1
11,1
11,7
9,7
9,5
2,5
2,3
7,3
```

**Očekávaný výstup:** `24`

**Vysvětlení:** 
- Polygon tvoří "L" tvar s vrcholy 
- Největší validní obdélník je mezi (9,5) a (2,3)
- Šířka: |9 - 2| + 1 = 8
- Výška: |5 - 3| + 1 = 3
- Plocha = 8 × 3 = 24

### Test 2: Obdélník celý uvnitř polygonu
**Vstup:**
```
0,0
10,0
10,10
0,10
```

**Testovaný obdélník:** rohy (2,2) a (8,8)

**Očekávaný výsledek:** Validní ✅
- Všechny 4 rohy jsou uvnitř čtverce
- Plocha = 7 × 7 = 49

### Test 3: Obdélník částečně venku
**Vstup:**
```
5,5
15,5
15,15
5,15
```

**Testovaný obdélník:** rohy (0,0) a (10,10)

**Očekávaný výsledek:** Nevalidní ❌
- Roh (0,0) je mimo polygon
- Obdélník není celý zelený

### Test 4: Obdélník s rohy na hraně
**Vstup:**
```
0,0
10,0
10,10
0,10
```

**Testovaný obdélník:** rohy (0,0) a (10,10)

**Očekávaný výsledek:** Validní ✅
- Všechny rohy jsou červené (vrcholy polygonu)
- Celý obdélník je uvnitř/na polygonu
- Plocha = 11 × 11 = 121

### Test 5: Konkávní polygon
**Vstup (L-tvar):**
```
0,0
5,0
5,5
10,5
10,10
0,10
```

**Testovaný obdélník:** rohy (0,0) a (10,10)

**Očekávaný výsledek:** Nevalidní ❌
- Obdélník pokrývá oblast (5,0) až (10,5), která je mimo polygon
- Část obdélníku je mimo zelené oblasti

### Test 6: Malý obdélník na hraně
**Vstup:**
```
0,0
10,0
10,10
0,10
```

**Testovaný obdélník:** rohy (5,0) a (6,0)

**Očekávaný výsledek:** Validní ✅
- Oba rohy na horní hraně polygonu
- Plocha = 2 × 1 = 2

---

## Implementation Notes

### 1. Coordinate Compression

**Princip:** Převést velký rozsah souřadnic (~96k) na malý počet indexů (~496)

**Implementace:**
```csharp
// Extrahuj unikátní souřadnice
var xs = points.Select(p => p.X).Distinct().OrderBy(x => x).ToList();
var ys = points.Select(p => p.Y).Distinct().OrderBy(y => y).ToList();

// Vytvoř index mapy
var xIndex = xs.Select((val, idx) => (val, idx))
                .ToDictionary(x => x.val, x => x.idx);
var yIndex = ys.Select((val, idx) => (val, idx))
                .ToDictionary(y => y.val, y => y.idx);

// Binary search pro mapování souřadnic
int BinarySearchLowerBound(List<long> sorted, long value)
{
    int left = 0, right = sorted.Count;
    while (left < right)
    {
        int mid = left + (right - left) / 2;
        if (sorted[mid] < value)
            left = mid + 1;
        else
            right = mid;
    }
    return left;
}
```

**Časová složitost:** $O(n \log n)$ pro třídění + $O(\log n)$ per lookup

### 2. Scanline Fill s Even-Odd Rule

**Princip:** Pro každý horizontální "band" (mezi dvěma Y souřadnicemi) najít průsečíky s vertikálními hranami a použít even-odd rule

**Implementace:**
```csharp
// Pro každý Y-band sbírej průsečíky
var rowXs = Enumerable.Range(0, yn - 1)
                      .Select(_ => new List<int>())
                      .ToArray();

for (int i = 0; i < n; i++)
{
    var p1 = points[i];
    var p2 = points[(i + 1) % n];
    
    if (p1.X == p2.X)  // Vertikální hrana
    {
        int xi = xIndex[p1.X];
        int yi1 = yIndex[p1.Y];
        int yi2 = yIndex[p2.Y];
        
        if (yi1 > yi2)
            (yi1, yi2) = (yi2, yi1);
        
        // Hrana protíná Y-bandy [yi1, yi2)
        for (int j = yi1; j < yi2; j++)
        {
            rowXs[j].Add(xi);
        }
    }
}

// Even-odd rule
var inside = new bool[yBands, xBands];
for (int j = 0; j < yBands; j++)
{
    var xsList = rowXs[j];
    if (xsList.Count == 0) continue;
    
    xsList.Sort();
    var uniqueXs = xsList.Distinct().ToList();
    
    // Páry průsečíků = inside regiony
    for (int k = 0; k + 1 < uniqueXs.Count; k += 2)
    {
        int xL = uniqueXs[k];
        int xR = uniqueXs[k + 1];
        
        for (int i = xL; i < xR && i < xBands; i++)
        {
            inside[j, i] = true;
        }
    }
}
```

**Časová složitost:** $O(n \times Y_B)$ kde $Y_B = |ys| - 1$

### 3. 2D Prefix Sum

**Princip:** Pre-compute kumulativní sumy pro rychlé regionové dotazy

**Implementace:**
```csharp
// Vytvoř prefix sum pro "outside" buňky
var pref = new int[yBands + 1, xBands + 1];

for (int j = 0; j < yBands; j++)
{
    for (int i = 0; i < xBands; i++)
    {
        int outside = inside[j, i] ? 0 : 1;
        pref[j + 1, i + 1] = pref[j + 1, i] + pref[j, i + 1] 
                             - pref[j, i] + outside;
    }
}

// Dotaz na region [x1, x2) × [y1, y2)
int GetOutsideCount(int x1, int y1, int x2, int y2)
{
    if (x1 >= x2 || y1 >= y2) return 0;
    return pref[y2, x2] - pref[y1, x2] - pref[y2, x1] + pref[y1, x1];
}
```

**Časová složitost:** 
- Build: $O(X_B \times Y_B)$
- Query: $O(1)$ per rectangle

### 4. Validace obdélníků

**Implementace:**
```csharp
long bestArea = 0;

for (int a = 0; a < n; a++)
{
    for (int b = a + 1; b < n; b++)
    {
        long x1 = points[a].X, y1 = points[a].Y;
        long x2 = points[b].X, y2 = points[b].Y;
        
        if (x1 == x2 && y1 == y2) continue;  // Stejný bod
        
        long xl = Math.Min(x1, x2), xr = Math.Max(x1, x2);
        long yl = Math.Min(y1, y2), yr = Math.Max(y1, y2);
        
        // Mapuj na compressed coordinates
        int ixL = BinarySearchLowerBound(xs, xl);
        int ixR = BinarySearchLowerBound(xs, xr);
        int iyL = BinarySearchLowerBound(ys, yl);
        int iyR = BinarySearchLowerBound(ys, yr);
        
        // Spočítej plochu
        long area = (xr - xl + 1) * (yr - yl + 1);
        
        if (area <= bestArea) continue;  // Early exit
        
        // Validace: žádné "outside" buňky?
        int outsideCells = GetOutsideCount(ixL, iyL, ixR, iyR);
        if (outsideCells == 0)
        {
            bestArea = area;
        }
    }
}

return bestArea;
```

**Časová složitost:** $O(n^2)$ pro všechny páry × $O(1)$ per validation

---

## Data Structures

### Point Record
```csharp
public record Point(long X, long Y);
```

### Compressed Coordinates
```csharp
List<long> xs;           // Seřazené unikátní X souřadnice
List<long> ys;           // Seřazené unikátní Y souřadnice
Dictionary<long, int> xIndex;  // X → compressed index
Dictionary<long, int> yIndex;  // Y → compressed index
```

### Inside Grid
```csharp
bool[,] inside;          // inside[yBand, xBand] = true pokud buňka je uvnitř polygonu
                         // Velikost: (yn-1) × (xn-1) where xn = |xs|, yn = |ys|
```

### 2D Prefix Sum
```csharp
int[,] pref;             // pref[j+1, i+1] = kumulativní suma "outside" buněk
                         // Velikost: yn × xn
                         // pref[0, *] = 0, pref[*, 0] = 0 (base case)
```

### Scanline Data
```csharp
List<int>[] rowXs;       // Pro každý Y-band: seznam X-indexů vertikálních hran
                         // Velikost: yn-1
```

---

## Class Structure

```csharp
public class Day09 : ISolution
{
    public int DayNumber => 9;
    public string Title => "Movie Theater";

    // Part 1: Simple brute-force for any rectangle
    public string SolvePart1(string input)
    {
        var points = ParsePoints(input);
        long maxArea = CalculateMaxRectangleArea(points);
        return maxArea.ToString();
    }

    // Part 2: Coordinate compression + scanline fill
    public string SolvePart2(string input)
    {
        var points = ParsePoints(input);
        long maxArea = CalculateMaxValidRectangleAreaOptimized(points);
        return maxArea.ToString();
    }

    private Point[] ParsePoints(string input)
    {
        return input
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Split(',');
                return new Point(long.Parse(parts[0]), long.Parse(parts[1]));
            })
            .ToArray();
    }

    private long CalculateMaxValidRectangleAreaOptimized(Point[] points)
    {
        int n = points.Length;
        if (n < 2) return 0;

        // 1. Coordinate Compression
        var xs = points.Select(p => p.X).Distinct().OrderBy(x => x).ToList();
        var ys = points.Select(p => p.Y).Distinct().OrderBy(y => y).ToList();

        int xn = xs.Count;
        int yn = ys.Count;

        if (xn <= 1 || yn <= 1) return 0;  // Degenerovaný polygon

        var xIndex = xs.Select((val, idx) => (val, idx))
                       .ToDictionary(x => x.val, x => x.idx);
        var yIndex = ys.Select((val, idx) => (val, idx))
                       .ToDictionary(y => y.val, y => y.idx);

        // 2. Scanline Fill
        var rowXs = Enumerable.Range(0, yn - 1)
                              .Select(_ => new List<int>())
                              .ToArray();

        for (int i = 0; i < n; i++)
        {
            var p1 = points[i];
            var p2 = points[(i + 1) % n];

            if (p1.X == p2.X)  // Vertikální hrana
            {
                int xi = xIndex[p1.X];
                int yi1 = yIndex[p1.Y];
                int yi2 = yIndex[p2.Y];
                
                if (yi1 > yi2)
                    (yi1, yi2) = (yi2, yi1);

                for (int j = yi1; j < yi2; j++)
                {
                    rowXs[j].Add(xi);
                }
            }
        }

        // 3. Even-Odd Rule
        int yb = yn - 1;
        int xb = xn - 1;
        var inside = new bool[yb, xb];

        for (int j = 0; j < yb; j++)
        {
            var xsList = rowXs[j];
            if (xsList.Count == 0) continue;

            xsList.Sort();
            var uniqueXs = xsList.Distinct().ToList();

            for (int k = 0; k + 1 < uniqueXs.Count; k += 2)
            {
                int xL = uniqueXs[k];
                int xR = uniqueXs[k + 1];

                for (int i = xL; i < xR && i < xb; i++)
                {
                    inside[j, i] = true;
                }
            }
        }

        // 4. 2D Prefix Sum
        var pref = new int[yb + 1, xb + 1];
        for (int j = 0; j < yb; j++)
        {
            for (int i = 0; i < xb; i++)
            {
                int outside = inside[j, i] ? 0 : 1;
                pref[j + 1, i + 1] = pref[j + 1, i] + pref[j, i + 1] 
                                     - pref[j, i] + outside;
            }
        }

        // 5. Validace obdélníků
        long bestArea = 0;

        for (int a = 0; a < n; a++)
        {
            for (int b = a + 1; b < n; b++)
            {
                long x1 = points[a].X, y1 = points[a].Y;
                long x2 = points[b].X, y2 = points[b].Y;

                if (x1 == x2 && y1 == y2) continue;

                long xl = Math.Min(x1, x2), xr = Math.Max(x1, x2);
                long yl = Math.Min(y1, y2), yr = Math.Max(y1, y2);

                int ixL = BinarySearchLowerBound(xs, xl);
                int ixR = BinarySearchLowerBound(xs, xr);
                int iyL = BinarySearchLowerBound(ys, yl);
                int iyR = BinarySearchLowerBound(ys, yr);

                long area = (xr - xl + 1) * (yr - yl + 1);

                if (area <= bestArea) continue;

                int outsideCells = GetOutsideCount(pref, ixL, iyL, ixR, iyR);
                if (outsideCells == 0)
                {
                    bestArea = area;
                }
            }
        }

        return bestArea;
    }

    private int BinarySearchLowerBound(List<long> sorted, long value)
    {
        int left = 0, right = sorted.Count;
        while (left < right)
        {
            int mid = left + (right - left) / 2;
            if (sorted[mid] < value)
                left = mid + 1;
            else
                right = mid;
        }
        return left;
    }

    private int GetOutsideCount(int[,] pref, int x1, int y1, int x2, int y2)
    {
        if (x1 >= x2 || y1 >= y2) return 0;
        return pref[y2, x2] - pref[y1, x2] - pref[y2, x1] + pref[y1, x1];
    }
}


public record Point(long X, long Y);
```

---

## Complexity Summary

### Časová složitost

**Fáze algoritmu:**
- **Parsování vstupu:** $O(n)$
- **Coordinate Compression:** $O(n \log n)$ pro třídění
- **Scanline Fill:** $O(n \times Y_B)$ kde $Y_B \approx n$
- **2D Prefix Sum Build:** $O(X_B \times Y_B) \approx O(n^2)$
- **Validace obdélníků:** $O(n^2)$ páry × $O(1)$ validace
- **Celkem:** $O(n^2 + n \times Y_B + X_B \times Y_B) \approx O(n^2)$

**S konkrétními čísly:**
- $n = 496$ červených bodů
- $X_B = Y_B \approx 496$ compressed bands
- Fáze 1-3: $\approx 246,016$ operací
- Fáze 4: $\binom{496}{2} = 122,760$ kombinací
- **Celkem:** $\approx 615,000$ operací ⚡

**Očekávaný čas běhu:** < 100 ms ✅

### Prostorová složitost

- **Compressed coordinates:** $O(n)$ pro xs, ys, xIndex, yIndex
- **Inside grid:** $O(X_B \times Y_B) \approx 496 \times 496 = 246,016$ buněk
- **Prefix sum:** $O(X_B \times Y_B) \approx 246,016$ int values
- **Scanline data:** $O(n \times Y_B)$ worst case
- **Celkem:** $O(n^2) \approx 246,016$ buněk = **~1 MB paměti** ✅

---

## Key Insights

### Proč Coordinate Compression funguje?

1. **Červené body definují mřížku:** Pouze souřadnice červených bodů jsou relevantní
2. **Buňky mezi červenými body:** Každá buňka je buď celá inside nebo celá outside
3. **Redukce složitosti:** Z ~9 miliard potenciálních bodů na ~246k buněk

### Proč Scanline Fill + Even-Odd Rule?

1. **Efektivní vyplnění:** Pouze vertikální hrany jsou relevantní
2. **Even-Odd Rule:** Automaticky řeší konkávní polygony
3. **O(n × Y_B) složitost:** Velmi rychlé pro axis-aligned polygony

### Proč 2D Prefix Sum?

1. **O(1) regionové dotazy:** Každý obdélník validován okamžitě
2. **Trade-off:** Malá paměť (1 MB) za obrovské zrychlení
3. **Škálovatelnost:** Funguje i pro polygony s tisíci body

---

## Alternative Approaches (zamítnuté)

### ❌ Point-by-Point Validation
- **Problém:** O(n² × W × H × P) ≈ 1 trillion operací
- **Výsledek:** Příliš pomalé i pro malé obdélníky

### ❌ Grid Sampling (20×20, 50×50, etc.)
- **Problém:** Nikdy 100% přesné pro konkávní polygony
- **Výsledek:** Vrací špatné odpovědi

### ❌ Corner-Only Validation
- **Problém:** Příliš permisivní, akceptuje nevalidní obdélníky
- **Výsledek:** Odpověď příliš vysoká

### ✅ Coordinate Compression + Scanline Fill
- **Výhoda:** 100% přesné, velmi rychlé, škálovatelné
- **Výsledek:** Správná odpověď v < 100 ms

---

## Conclusion

**Implementovaný přístup:**
1. **Coordinate Compression** - redukce z miliard na stovky tisíc
2. **Scanline Fill s Even-Odd Rule** - efektivní vyplnění polygonu
3. **2D Prefix Sum** - O(1) validace obdélníků
4. **Brute-force páry** - O(n²) všechny kombinace červených bodů

**Klíčové výhody:**
- ✅ 100% přesnost (žádné vzorkování)
- ✅ Velmi rychlé (< 100 ms)
- ✅ Nízká paměť (~1 MB)
- ✅ Škálovatelné pro velké polygony
- ✅ Automaticky řeší konkávní tvary

**Očekávaný výsledek:** Funkční řešení běžící v < 100 ms pro vstup s 496 body.

