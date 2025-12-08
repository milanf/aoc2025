# Tech-Spec: Day 08 Part 1 - Playground

**Created:** 2025-12-08  
**Status:** Draft  
**AoC Link:** https://adventofcode.com/2025/day/8

---

## Overview

### Problem Statement

Po opravě teleporteru se ocitáte na obrovském podzemním hřišti, kde elfové připravují vánoční dekorace pomocí junction boxů (elektrických spojovacích skříněk) zavěšených v prostoru. Plánují je propojit světelnými řetězy tak, aby elektřina mohla dosáhnout každého junction boxu.

**Klíčové body:**
- Každý junction box má pozici v **3D prostoru** (X, Y, Z souřadnice)
- Většina junction boxů neposkytuje elektřinu
- Když dva junction boxy propojíte řetězem světel, mohou si **předávat elektřinu** → stávají se součástí **stejného obvodu (circuit)**
- **Strategie propojování:** Vždy propojit dva junction boxy, které jsou **nejblíže sobě** (Euklidovská vzdálenost) a **ještě nejsou přímo propojené**
- Po propojení 1000 párů najít **tři největší obvody** a vynásobit jejich velikosti

**Example z AoC (20 junction boxů, 10 propojení):**
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

**Postup propojování:**
1. Najít dva nejbližší junction boxy (podle Euklidovské vzdálenosti)
2. Propojit je → stanou se součástí stejného obvodu
3. Opakovat, dokud neprovedeme 1000 propojení
4. Po 10 propojeních v example máme:
   - 1 obvod s 5 junction boxy
   - 1 obvod se 4 junction boxy
   - 2 obvody s 2 junction boxy každý
   - 7 obvodů s 1 junction boxem každý
5. **Výsledek:** 5 × 4 × 2 = **40**

**Klíčové poznatky:**
- Propojení dvou junction boxů, které jsou **již ve stejném obvodu**, **nic nedělá**
- Propojení dvou junction boxů z **různých obvodů** → **sloučí oba obvody do jednoho**
- Musíme efektivně sledovat, které junction boxy patří do jakého obvodu → **Union-Find (Disjoint Set Union)**

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

**Porovnání s example:**
- Example: **20 junction boxů**, **10 propojení** → výsledek 40
- Reálný vstup: **1000 junction boxů**, **1000 propojení** → **50× větší!**

**Důsledky pro algoritmus:**
- **Musíme provést 1000 iterací**, v každé najít nejbližší pár
- V každé iteraci musíme spočítat vzdálenosti mezi všemi páry → **O(n²) na iteraci**
- **Celkem: 1000 × O(n²)** kde n klesá od 1000 směrem k 1 (už propojené páry ignorujeme)
- ❌ **Naivní přístup:** Pro každou iteraci procházet všechny páry → **O(k × n²)** kde k=1000, n=1000 → **~1 miliarda operací** → příliš pomalé!
- ✅ **Optimalizace 1:** Předpočítat **všechny vzdálenosti** jednou → O(n²) = 500,000 operací
- ✅ **Optimalizace 2:** Seřadit páry podle vzdálenosti → O(n² log n)
- ✅ **Optimalizace 3:** Použít **Union-Find** pro rychlé zjištění, zda jsou v jednom obvodu
- ✅ **Optimální algoritmus:** Kruskalův algoritmus pro MST s Union-Find!

**Spojitost s teórií grafů:**
- Tento problém je **Minimum Spanning Tree (MST)** s limitovaným počtem hran!
- Junction boxy = **vrcholy grafu**
- Možná propojení = **hrany** s váhami (vzdálenosti)
- Chceme přidat **1000 nejkratších hran**, které nevytváří cykly v rámci jednoho obvodu
- **Kruskalův algoritmus** je přesně to, co potřebujeme!

**Časová složitost optimálního řešení:**
1. **Vypočítat všechny vzdálenosti:** O(n²) = O(1,000,000) ✅
2. **Seřadit hrany podle vzdálenosti:** O(n² log n²) = O(2,000,000 log 1,000,000) ≈ O(40,000,000) ✅
3. **Union-Find pro 1000 spojení:** O(k × α(n)) kde α je inverzní Ackermannova funkce ≈ O(1000 × 4) ≈ O(4,000) ✅
4. **Celkem:** O(n² log n) = **velmi efektivní** pro n=1000

**Prostorová složitost:**
- Uložení všech vzdáleností: O(n²) = 500,000 párů × 8 bytes ≈ **4 MB** ✅
- Union-Find struktura: O(n) = 1000 × 8 bytes ≈ **8 KB** ✅
- Celkem: **~4 MB** → **triviální**

**Edge Cases:**
- ✅ Co když dva páry mají stejnou vzdálenost? → Pořadí propojení neovlivní výsledek
- ✅ Co když propojíme dva junction boxy, které jsou už ve stejném obvodu? → Union-Find to detekuje, spojení se neprovede
- ✅ Co když po 1000 propojeních zůstane více než 3 obvody? → Můžeme mít i desítky obvodů, vynásobíme tři největší
- ⚠️ Co když jsou méně než 3 obvody? → Nepravděpodobné s 1000 JB a 1000 propojení, ale ošetřit

### Solution

**Algoritmus: Kruskalův algoritmus s Union-Find**

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
    
    public UnionFind(int size)
    {
        parent = Enumerable.Range(0, size).ToArray();
        rank = new int[size];
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
        
        return true;
    }
}
```

#### 2. Hlavní algoritmus

```csharp
public string SolvePart1(string input)
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
    
    // 2. Vypočítat všechny vzdálenosti a vytvořit hrany
    var edges = new List<Edge>(n * (n - 1) / 2);
    
    for (int i = 0; i < n; i++)
    {
        for (int j = i + 1; j < n; j++)
        {
            double distance = CalculateDistance(boxes[i], boxes[j]);
            edges.Add(new Edge(i, j, distance));
        }
    }
    
    // 3. Seřadit hrany podle vzdálenosti (vzestupně)
    edges.Sort();
    
    // 4. Kruskalův algoritmus - přidat 1000 nejkratších hran
    var uf = new UnionFind(n);
    int connectionsAdded = 0;
    
    foreach (var edge in edges)
    {
        if (connectionsAdded >= 1000)
            break;
        
        // Pokusit se propojit - pokud jsou v různých obvodech, propojit
        if (uf.Union(edge.Box1, edge.Box2))
        {
            connectionsAdded++;
        }
    }
    
    // 5. Spočítat velikosti všech obvodů
    var circuitSizes = new Dictionary<int, int>();
    
    for (int i = 0; i < n; i++)
    {
        int root = uf.Find(i);
        circuitSizes[root] = circuitSizes.GetValueOrDefault(root, 0) + 1;
    }
    
    // 6. Najít tři největší obvody
    var topThree = circuitSizes.Values
        .OrderByDescending(size => size)
        .Take(3)
        .ToArray();
    
    // 7. Vynásobit velikosti tří největších obvodů
    long result = topThree[0] * topThree[1] * topThree[2];
    
    return result.ToString();
}

private double CalculateDistance(JunctionBox a, JunctionBox b)
{
    long dx = a.X - b.X;
    long dy = a.Y - b.Y;
    long dz = a.Z - b.Z;
    return Math.Sqrt(dx * dx + dy * dy + dz * dz);
}
```

#### 3. Optimalizace a poznámky

**Union-Find optimalizace:**
- ✅ **Path compression** v metodě `Find()` → O(α(n)) ≈ O(1)
- ✅ **Union by rank** → zajišťuje vyváženost stromu
- ✅ Obě optimalizace dohromady dávají **amortizovanou konstantní složitost**

**Paměťové optimalizace:**
- ❓ Ukládáme všechny hrany (500,000 objektů) → 4 MB
- ✅ Alternativa: použít **priority queue** a generovat hrany on-the-fly
  - Ale: seřazení je jednorázové, pak jen procházíme → efektivnější předpočítat

**Numerická přesnost:**
- ⚠️ Používáme `double` pro vzdálenost → potenciální floating-point chyby
- ✅ Pro porovnání vzdáleností můžeme použít **druhou mocninu** (bez sqrt) → rychlejší a přesnější!
- ✅ Upravená verze:

```csharp
private long CalculateDistanceSquared(JunctionBox a, JunctionBox b)
{
    long dx = a.X - b.X;
    long dy = a.Y - b.Y;
    long dz = a.Z - b.Z;
    return dx * dx + dy * dy + dz * dz; // Bez sqrt!
}

public record Edge(int Box1, int Box2, long DistanceSquared) 
    : IComparable<Edge>
{
    public int CompareTo(Edge? other) => 
        other == null ? 1 : DistanceSquared.CompareTo(other.DistanceSquared);
}
```

**Výhody použití druhé mocniny:**
- ✅ Rychlejší (bez volání `Math.Sqrt`)
- ✅ Přesnější (žádné floating-point zaokrouhlení)
- ✅ Stále korektní pro porovnání (pokud a < b, pak a² < b²)

---

## Test Cases

### Example Test (z AoC)

**Input:**
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

**Očekávaný výstup:** `40`

**Postup:**
- 20 junction boxů, 10 propojení
- Po 10 propojeních:
  - 1 obvod: 5 boxů
  - 1 obvod: 4 boxy
  - 2 obvody: 2 boxy každý
  - 7 obvodů: 1 box každý
- Tři největší: 5 × 4 × 2 = **40**

### Edge Cases

**Test 1: Minimální vstup (3 boxy, 1 propojení)**
```
0,0,0
1,0,0
0,1,0
```
- Po 1 propojení: obvody [2, 1, 1]
- Výsledek: 2 × 1 × 1 = **2**

**Test 2: Všechny boxy propojeny do jednoho obvodu**
- Pokud máme n boxů a provedeme n-1 propojení → jeden obvod s n boxy
- Tři největší: [n, 1, 1] → **n**

**Test 3: Rovnoměrné rozdělení**
- 9 boxů, 3 propojení → mohou vzniknout 3 obvody po 3 boxech
- Výsledek: 3 × 3 × 3 = **27**

---

## Implementation Plan

### Class Structure

```
Day08.cs
├── SolvePart1(string input) : string
├── ParseJunctionBoxes(string input) : JunctionBox[]
├── CalculateDistanceSquared(JunctionBox a, JunctionBox b) : long
├── BuildAndSortEdges(JunctionBox[] boxes) : List<Edge>
├── PerformKruskal(List<Edge> edges, int n, int connectionsToAdd) : UnionFind
├── GetCircuitSizes(UnionFind uf, int n) : Dictionary<int, int>
└── GetTopThreeProduct(Dictionary<int, int> circuitSizes) : long

UnionFind.cs (helper class)
├── UnionFind(int size)
├── Find(int x) : int
└── Union(int x, int y) : bool

JunctionBox.cs (record)
└── JunctionBox(int Id, int X, int Y, int Z)

Edge.cs (record)
└── Edge(int Box1, int Box2, long DistanceSquared) : IComparable<Edge>
```

### Testing Strategy

1. **Unit testy pro pomocné metody:**
   - `CalculateDistanceSquared()` → ověřit správný výpočet
   - `UnionFind.Find()` a `Union()` → ověřit path compression a union by rank

2. **Integration test s example:**
   - Celý example z AoC → očekáváme `40`

3. **Edge case testy:**
   - Minimální vstupy (3 boxy)
   - Rovnoměrné rozdělení
   - Všechny boxy v jednom obvodu

4. **Performance test:**
   - Měření času na reálném vstupu (1000 boxů)
   - Očekávaný čas: < 100 ms

---

## Performance Analysis

### Časová složitost

| Krok | Složitost | Počet operací (n=1000) |
|------|-----------|------------------------|
| Parse input | O(n) | 1,000 |
| Vypočítat hrany | O(n²) | 500,000 |
| Seřadit hrany | O(n² log n) | ~9,000,000 |
| Kruskal (1000 spojení) | O(k × α(n)) | ~4,000 |
| Spočítat obvody | O(n) | 1,000 |
| Najít top 3 | O(n log n) | ~10,000 |
| **Celkem** | **O(n² log n)** | **~9,000,000** ✅ |

**Závěr:** Algoritmus je **velmi efektivní** i pro n=1000. Předpokládaný čas běhu < 100 ms.

### Prostorová složitost

| Struktura | Složitost | Velikost (n=1000) |
|-----------|-----------|-------------------|
| Junction boxy | O(n) | ~8 KB |
| Hrany | O(n²) | ~4 MB |
| Union-Find | O(n) | ~8 KB |
| Circuit sizes | O(n) | ~8 KB |
| **Celkem** | **O(n²)** | **~4 MB** ✅ |

**Závěr:** Paměťové nároky jsou **triviální** pro moderní systémy.

---

## Additional Notes

### Proč Union-Find?

Union-Find (Disjoint Set Union) je ideální datová struktura pro tento problém, protože:
1. ✅ **Rychle zjistí, zda jsou dva prvky ve stejné množině** → O(α(n)) ≈ O(1)
2. ✅ **Rychle spojí dvě množiny** → O(α(n)) ≈ O(1)
3. ✅ **Podporuje dynamické přidávání spojení** → ideální pro Kruskalův algoritmus
4. ✅ **Jednoduchá implementace** → ~30 řádků kódu

### Proč Kruskalův algoritmus?

Kruskalův algoritmus je klasický algoritmus pro hledání Minimum Spanning Tree (MST):
1. ✅ Seřadí hrany podle váhy
2. ✅ Postupně přidává nejkratší hrany, které nevytváří cyklus
3. ✅ Používá Union-Find pro detekci cyklů
4. ✅ **Přesně odpovídá zadání:** propojit nejbližší páry, které ještě nejsou propojené!

### Alternativní přístupy

❌ **Naivní simulace:**
- V každé iteraci hledat nejbližší pár → O(k × n²) = O(1,000,000,000) → příliš pomalé

❌ **Primův algoritmus:**
- Také MST algoritmus, ale roste strom z jednoho vrcholu
- Nevhodný, protože chceme přidat **přesně 1000 hran**, ne celý MST

✅ **Kruskal s Union-Find:**
- Optimální pro tento problém
- Jednoduchý, elegantní, efektivní

---

## Summary

### Key Takeaways

1. **Problém = Kruskalův MST s limitovaným počtem hran**
2. **Union-Find je klíčová datová struktura** pro rychlé spojování obvodů
3. **Optimalizace:** Použít druhou mocninu vzdálenosti místo sqrt
4. **Časová složitost: O(n² log n)** → velmi efektivní pro n=1000
5. **Prostorová složitost: O(n²)** → triviální (~4 MB)

### Implementation Checklist

- [ ] Implementovat `UnionFind` třídu s path compression a union by rank
- [ ] Implementovat `JunctionBox` a `Edge` recordy
- [ ] Napsat `CalculateDistanceSquared()` metodu
- [ ] Implementovat hlavní `SolvePart1()` metodu s Kruskalovým algoritmem
- [ ] Napsat unit testy pro `UnionFind`
- [ ] Napsat integration test s example (očekáváme 40)
- [ ] Otestovat na reálném vstupu
- [ ] Ověřit performance (< 100 ms)

---

**Status:** ✅ Ready for implementation
