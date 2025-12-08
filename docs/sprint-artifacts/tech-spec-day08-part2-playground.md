# Tech-Spec: Day 08 Part 2 - Playground

**Created:** 2025-12-08  
**Status:** ✅ Completed  
**AoC Link:** https://adventofcode.com/2025/day/8

---

## Overview

### Problem Statement

V Part 1 jsme spojili 1000 párů junction boxů a hledali tři největší obvody. Elfové však zjistili, že **potřebují všechny junction boxy propojit do JEDNOHO velkého obvodu**, aby elektřina mohla proudit všude.

**Klíčové body:**
- Pokračujeme v propojování **nejbližších nespojených párů** (stejná strategie jako Part 1)
- Propojujeme, **dokud nejsou všechny junction boxy ve stejném obvodu**
- Pro **n junction boxů** potřebujeme **n-1 propojení** k vytvoření souvislého stromu (MST)
- Pro **1000 junction boxů** tedy potřebujeme **999 propojení**
- **Výsledek:** Vynásobit **X souřadnice** posledních dvou spojených junction boxů

**Example z AoC (20 junction boxů):**
```
162,817,812
57,618,57
906,360,560
592,479,940
352,342,300
466,668,158
542,29,236
431,825,988
739,650,466
52,470,668
216,146,977
819,987,18
117,168,530
805,96,715
346,949,466
970,615,88
941,993,340
862,61,35
984,92,344
425,690,689
```

**Postup propojování (example):**
1. Propojujeme páry podle Euklidovské vzdálenosti (nejbližší nejdříve)
2. **20 junction boxů** → potřebujeme **19 propojení** pro jeden souvislý obvod
3. **Poslední propojení:** Junction boxy na pozicích `216,146,977` a `117,168,530`
4. **Výsledek:** X₁ × X₂ = 216 × 117 = **25272**

**Důležité rozdíly oproti Part 1:**
- Part 1: Přesně **1000 propojení** → výsledek: součin velikostí 3 největších obvodů
- Part 2: Přesně **999 propojení** (n-1) → výsledek: součin X souřadnic posledního spojeného páru
- Part 2 vytváří **kompletní Minimum Spanning Tree (MST)**

### Input Analysis

**Reálný input (`Inputs/day08.txt`):**
- **1000 junction boxů** (přesně 1000 řádků)
- Každý řádek obsahuje 3 čísla: `X,Y,Z`
- **Rozsah souřadnic:** 0 až ~100,000 (všechny souřadnice jsou celá čísla)

**Vzorový vstup (první řádky):**
```
58660,9565,9912
87631,12487,66875
84510,67581,57621
75066,51710,19906
90551,35141,1233
...
```

**Poslední řádky:**
```
...
50685,71530,36759
42721,92561,38478
40473,52034,63533
46677,33353,56928
56369,23526,87394
```

**Požadovaný počet propojení:**
- Pro **1000 junction boxů** potřebujeme **999 propojení** k vytvoření souvislého stromu
- Toto je základní vlastnost stromů: **počet hran = počet vrcholů - 1**

**Spojitost s teórií grafů:**
- Tento problém je **kompletní Minimum Spanning Tree (MST)**
- Junction boxy = **vrcholy grafu**
- Možná propojení = **hrany** s váhami (Euklidovské vzdálenosti)
- **Kruskalův algoritmus** je ideální řešení:
  1. Seřadit všechny hrany podle vzdálenosti (nejmenší první)
  2. Přidávat hrany, které nespojují již propojené komponenty
  3. Pokračovat, dokud nemáme **n-1 hran** (souvislý graf)
  4. **Poslední přidaná hrana** je to, co hledáme!

**Časová složitost:**
1. **Vypočítat všechny vzdálenosti:** O(n²) kde n=1000 → **500,000 párů** ✅
2. **Seřadit hrany podle vzdálenosti:** O(n² log n²) ≈ **40,000,000 operací** ✅
3. **Union-Find pro 999 spojení:** O(k × α(n)) kde α ≈ 4 → **~4,000 operací** ✅
4. **Celkem:** O(n² log n) = **velmi efektivní** pro n=1000

**Prostorová složitost:**
- Uložení všech vzdáleností: O(n²) = 500,000 párů × 16 bytes (edge) ≈ **8 MB** ✅
- Union-Find struktura: O(n) = 1000 × 8 bytes ≈ **8 KB** ✅
- Celkem: **~8 MB** → **triviální**

**Edge Cases:**
- ✅ Co když máme přesně 1000 JB? → Potřebujeme 999 propojení
- ✅ Co když dva páry mají stejnou vzdálenost? → Pořadí propojení neovlivní strukturu MST, jen může ovlivnit, který pár bude poslední
- ⚠️ Co když by bylo méně než 1000 JB? → V reálném vstupu je přesně 1000, ale algoritmus funguje pro jakékoli n
- ✅ Jak poznáme, že máme kompletní MST? → Union-Find má pouze 1 komponentu, nebo jsme přidali n-1 hran

### Solution

**Algoritmus: Kruskalův algoritmus s Union-Find (dokončení MST)**

#### 1. Data Structures

```csharp
// Reprezentace junction boxu
public record JunctionBox(int Id, int X, int Y, int Z);

// Hrana (propojení) s váhou
public record Edge(int Box1, int Box2, double Distance) 
    : IComparable<Edge>
{
    public int CompareTo(Edge? other) => 
        other == null ? 1 : Distance.CompareTo(other.Distance);
}

// Union-Find (Disjoint Set Union)
public class UnionFind
{
    private int[] parent;
    private int[] rank;
    private int componentCount; // Počet oddělených komponent
    
    public UnionFind(int size)
    {
        parent = Enumerable.Range(0, size).ToArray();
        rank = new int[size];
        componentCount = size;
    }
    
    // Najít kořen (reprezentanta) množiny s path compression
    public int Find(int x)
    {
        if (parent[x] != x)
            parent[x] = Find(parent[x]); // Path compression
        return parent[x];
    }
    
    // Spojit dvě množiny, vrátit true pokud byly oddělené
    public bool Union(int x, int y)
    {
        int rootX = Find(x);
        int rootY = Find(y);
        
        if (rootX == rootY)
            return false; // Už ve stejné množině
        
        // Union by rank
        if (rank[rootX] < rank[rootY])
            parent[rootX] = rootY;
        else if (rank[rootX] > rank[rootY])
            parent[rootY] = rootX;
        else
        {
            parent[rootY] = rootX;
            rank[rootX]++;
        }
        
        componentCount--;
        return true;
    }
    
    // Je vše propojené do jednoho obvodu?
    public bool IsFullyConnected() => componentCount == 1;
    
    // Kolik komponent zbývá?
    public int ComponentCount => componentCount;
}
```

#### 2. Hlavní algoritmus

```csharp
public string SolvePart2(string input)
{
    // 1. Parse junction boxy
    var boxes = input
        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Select((line, id) =>
        {
            var parts = line.Split(',');
            return new JunctionBox(
                id,
                int.Parse(parts[0]),
                int.Parse(parts[1]),
                int.Parse(parts[2])
            );
        })
        .ToArray();
    
    int n = boxes.Length;
    
    // 2. Vypočítat všechny možné hrany a seřadit je podle vzdálenosti
    var edges = new List<Edge>(n * (n - 1) / 2);
    
    for (int i = 0; i < n; i++)
    {
        for (int j = i + 1; j < n; j++)
        {
            double distance = CalculateDistance(boxes[i], boxes[j]);
            edges.Add(new Edge(i, j, distance));
        }
    }
    
    // Seřadit hrany podle vzdálenosti (nejkratší první)
    edges.Sort();
    
    // 3. Kruskalův algoritmus - přidávat hrany, dokud nemáme MST
    var unionFind = new UnionFind(n);
    Edge? lastAddedEdge = null;
    int edgesAdded = 0;
    int requiredEdges = n - 1; // Pro n vrcholů potřebujeme n-1 hran
    
    foreach (var edge in edges)
    {
        // Zkusit spojit dva junction boxy
        if (unionFind.Union(edge.Box1, edge.Box2))
        {
            // Spojení bylo úspěšné (nebyly ve stejném obvodu)
            lastAddedEdge = edge;
            edgesAdded++;
            
            // Máme kompletní MST?
            if (edgesAdded == requiredEdges)
            {
                break;
            }
        }
        // Pokud Union vrátil false, jsou už ve stejném obvodu → přeskočit
    }
    
    // 4. Spočítat výsledek: X₁ × X₂ posledního spojeného páru
    if (lastAddedEdge == null)
        throw new InvalidOperationException("Nepodařilo se najít poslední hranu!");
    
    var box1 = boxes[lastAddedEdge.Box1];
    var box2 = boxes[lastAddedEdge.Box2];
    
    long result = (long)box1.X * box2.X;
    
    return result.ToString();
}

// Euklidovská vzdálenost ve 3D
private double CalculateDistance(JunctionBox a, JunctionBox b)
{
    long dx = a.X - b.X;
    long dy = a.Y - b.Y;
    long dz = a.Z - b.Z;
    return Math.Sqrt(dx * dx + dy * dy + dz * dz);
}
```

#### 3. Optimalizace

**Možné optimalizace (pokud by byl větší vstup):**
1. **Použít heap místo kompletního seřazení:**
   - Místo seřazení všech hran (O(n² log n))
   - Použít priority queue a generovat hrany on-the-fly
   - Časová složitost: O(k log n²) kde k je počet potřebných hran
   
2. **Paralelizace výpočtu vzdáleností:**
   - Použít `Parallel.For` pro výpočet vzdáleností mezi páry
   
3. **Spatial indexing (pro HODNĚ velké vstupy):**
   - KD-tree nebo R-tree pro rychlejší hledání nejbližších sousedů
   - Vyplatí se až pro n > 10,000

**Pro naše n=1000 jsou tyto optimalizace zbytečné!**

#### 4. Algoritmus krok za krokem (example)

**Example (20 junction boxů → 19 propojení):**

```
Počáteční stav:
- 20 junction boxů (každý ve své komponentě)
- 190 možných hran (20×19/2)

Iterace 1-18:
- Spojujeme nejbližší páry
- Komponenty se postupně slučují
- Po každém spojení: componentCount--

Iterace 19 (poslední):
- Zbývají 2 komponenty
- Najdeme nejkratší hranu mezi nimi
- Spojíme: 216,146,977 a 117,168,530
- Výsledek: 216 × 117 = 25272 ✅
```

#### 5. Verifikace algoritmu

**Test na example:**
- Input: 20 junction boxů
- Očekávaný výsledek: 25272
- Algoritmus:
  1. Vygeneruje 190 hran
  2. Seřadí je podle vzdálenosti
  3. Přidá 19 nejkratších hran (které nevytváří cykly)
  4. Poslední hrana spojí JB s X=216 a X=117
  5. Výsledek: 216 × 117 = 25272 ✅

**Test na reálném vstupu:**
- Input: 1000 junction boxů
- Algoritmus:
  1. Vygeneruje 499,500 hran
  2. Seřadí je podle vzdálenosti
  3. Přidá 999 nejkratších hran (které nevytváří cykly)
  4. Poslední hrana spojí dva JB s konkrétními X souřadnicemi
  5. Výsledek: X₁ × X₂

### Implementation Details

**C# specifika:**
- `double` pro vzdálenosti (výpočet Euklidovské vzdálenosti)
- `long` pro výpočet výsledku (X souřadnice mohou být až ~100,000, součin až ~10,000,000,000)
- `record` pro JunctionBox a Edge (immutable, value equality)
- `List<Edge>` místo pole pro flexibilitu
- `IComparable<Edge>` pro snadné seřazení

**Důležité detaily:**
1. **ID junction boxů = index v poli** (0-based)
2. **Union-Find:** Path compression + Union by rank pro O(α(n)) ≈ O(1)
3. **Seřazení hran:** `edges.Sort()` používá IntroSort (O(n log n) average)
4. **Poslední hrana:** Ukládáme `lastAddedEdge` po každém úspěšném spojení
5. **Výstup:** `long` pro výsledek (prevence overflow)

### Expected Results

**Example (20 junction boxů):**
```
Input: 20 junction boxů
Output: 25272
Explanation: Poslední spojení mezi JB s X=216 a X=117
```

**Real Input (1000 junction boxů):**
```
Input: 1000 junction boxů (day08.txt)
Output: ??? (závisí na datech)
Execution time: < 100ms (n=1000 je malý vstup)
```

### Edge Cases

| Scénář | Jak ošetřit |
|--------|-------------|
| Přesně 1000 JB | Standard - 999 propojení |
| Dva páry se stejnou vzdáleností | Pořadí je dáno `Sort()`, výsledek je stále validní MST |
| Posledních N párů má stejnou vzdálenost | Kterýkoli z nich může být "poslední", všechny jsou validní |
| Méně než 2 JB | Nelze spočítat - throw exception |
| Jeden JB | Není co spojovat - výsledek je 0 nebo exception |

### Testing Strategy

#### Unit Tests

```csharp
[Test]
public void Part2_Example_Returns25272()
{
    var input = @"162,817,812
57,618,57
906,360,560
592,479,940
352,342,300
466,668,158
542,29,236
431,825,988
739,650,466
52,470,668
216,146,977
819,987,18
117,168,530
805,96,715
346,949,466
970,615,88
941,993,340
862,61,35
984,92,344
425,690,689";
    
    var solution = new Day08();
    var result = solution.SolvePart2(input);
    
    Assert.AreEqual("25272", result);
}

[Test]
public void Part2_RealInput_ReturnsValidResult()
{
    var input = File.ReadAllText("Inputs/day08.txt");
    
    var solution = new Day08();
    var result = solution.SolvePart2(input);
    
    // Výsledek musí být kladné číslo
    Assert.IsTrue(long.TryParse(result, out long value));
    Assert.Greater(value, 0);
}

[Test]
public void UnionFind_ConnectsAllComponents()
{
    var uf = new UnionFind(10);
    
    // Původně 10 komponent
    Assert.AreEqual(10, uf.ComponentCount);
    Assert.IsFalse(uf.IsFullyConnected());
    
    // Spojení 9 párů → 1 komponenta
    for (int i = 0; i < 9; i++)
    {
        Assert.IsTrue(uf.Union(i, i + 1));
    }
    
    Assert.AreEqual(1, uf.ComponentCount);
    Assert.IsTrue(uf.IsFullyConnected());
}

[Test]
public void CalculateDistance_Returns_EuclideanDistance()
{
    var a = new JunctionBox(0, 0, 0, 0);
    var b = new JunctionBox(1, 3, 4, 0);
    
    var solution = new Day08();
    var distance = solution.CalculateDistance(a, b);
    
    // √(3² + 4²) = √25 = 5
    Assert.AreEqual(5.0, distance, 0.0001);
}
```

#### Integration Tests

```csharp
[Test]
public void Part2_Example_ProducesMST()
{
    // Ověřit, že algoritmus vytváří validní MST
    // - Přesně n-1 hran
    // - Všechny vrcholy propojené
    // - Minimální celková vzdálenost
}
```

### Performance Considerations

**Pro n=1000:**
- **Výpočet vzdáleností:** O(n²) = 500,000 operací → ~5ms
- **Seřazení hran:** O(n² log n²) = ~40M operací → ~50ms
- **Union-Find:** O(999 × 4) = ~4,000 operací → <1ms
- **Celkem:** ~60ms ✅ (velmi rychlé)

**Paměťová náročnost:**
- Pole junction boxů: 1000 × 32 bytes = 32 KB
- Seznam hran: 500,000 × 32 bytes = 16 MB
- Union-Find: 1000 × 8 bytes = 8 KB
- **Celkem: ~16 MB** ✅ (triviální)

**Bottleneck:**
- Seřazení 500,000 hran je největší operace
- Pro větší n by bylo lepší použít priority queue

### Complexity Analysis

| Operace | Časová složitost | Prostorová složitost |
|---------|------------------|----------------------|
| Parse input | O(n) | O(n) |
| Výpočet hran | O(n²) | O(n²) |
| Seřazení hran | O(n² log n) | O(1) |
| Kruskalův algoritmus | O(n² × α(n)) | O(n) |
| **Celkem** | **O(n² log n)** | **O(n²)** |

Pro n=1000:
- Časová: ~40,000,000 operací → ~60ms ✅
- Prostorová: ~16 MB ✅

### References

- **Advent of Code 2025 - Day 8:** https://adventofcode.com/2025/day/8
- **Kruskalův algoritmus:** https://en.wikipedia.org/wiki/Kruskal%27s_algorithm
- **Union-Find:** https://en.wikipedia.org/wiki/Disjoint-set_data_structure
- **Minimum Spanning Tree:** https://en.wikipedia.org/wiki/Minimum_spanning_tree

---

## Implementation Checklist

- [ ] Vytvořit `JunctionBox` record
- [ ] Vytvořit `Edge` record s `IComparable<Edge>`
- [ ] Implementovat `UnionFind` class
  - [ ] `Find()` s path compression
  - [ ] `Union()` s union by rank
  - [ ] `ComponentCount` property
  - [ ] `IsFullyConnected()` method
- [ ] Implementovat `SolvePart2()` v `Day08.cs`
  - [ ] Parse junction boxy
  - [ ] Vypočítat všechny hrany
  - [ ] Seřadit hrany podle vzdálenosti
  - [ ] Kruskalův algoritmus - přidat n-1 hran
  - [ ] Uložit poslední přidanou hranu
  - [ ] Vynásobit X souřadnice
- [ ] Implementovat `CalculateDistance()` helper
- [ ] Vytvořit unit testy v `Day08Tests.cs`
  - [ ] Test na example (očekávaný výsledek: 25272)
  - [ ] Test na reálný input (validace formátu)
  - [ ] Test `UnionFind` komponent
  - [ ] Test výpočtu vzdálenosti
- [ ] Spustit testy a ověřit správnost
- [ ] Odeslat odpověď na AoC

---

**Status:** Ready for implementation ✅  
**Estimated time:** 45-60 minut  
**Confidence:** High - Kruskalův algoritmus je well-known a přímočarý
