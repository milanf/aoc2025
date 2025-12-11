# Tech-Spec: Day 11 Part 2 - Reactor (Filtered Path Counting)

**Created:** 2025-12-11  
**Status:** 📝 **Ready for Development**  
**AoC Link:** https://adventofcode.com/2025/day/11#part2

---

## Overview

### Problem Statement

Tento problém rozšiřuje Day 11 Part 1 o **filtrování cest podle navštívených uzlů**.

**Part 1 recap:** Počítali jsme všechny různé cesty z `you` do `out` v toroidním reaktorovém síti.

**Part 2 úkol:**  
Elfové zjistili, že problematická datová cesta prochází přes **DVA specifické uzly**:
- `dac` (digital-to-analog converter)
- `fft` (fast Fourier transform)

**Cíl:** Najít **počet všech různých cest** vedoucích z `svr` (server rack) do `out` (reaktor), které **navštíví OBOJÍ uzly** `dac` **A** `fft` (v **libovolném pořadí**).

**Příklad z AoC:**
```
svr: aaa bbb
aaa: fft
fft: ccc
bbb: tty
tty: ccc
ccc: ddd eee
ddd: hub
hub: fff
eee: dac
dac: fff
fff: ggg hhh
ggg: out
hhh: out
```

**Všechny cesty z `svr` do `out` (celkem 8):**
1. `svr → aaa → fft → ccc → ddd → hub → fff → ggg → out` ✅ (má fft, dac? ne)
2. `svr → aaa → fft → ccc → ddd → hub → fff → hhh → out` ✅ (má fft, dac? ne)
3. `svr → aaa → fft → ccc → eee → dac → fff → ggg → out` ✅ **PLATNÁ** (má fft i dac)
4. `svr → aaa → fft → ccc → eee → dac → fff → hhh → out` ✅ **PLATNÁ** (má fft i dac)
5. `svr → bbb → tty → ccc → ddd → hub → fff → ggg → out` ❌ (nemá fft ani dac)
6. `svr → bbb → tty → ccc → ddd → hub → fff → hhh → out` ❌ (nemá fft ani dac)
7. `svr → bbb → tty → ccc → eee → dac → fff → ggg → out` ❌ (má dac, ale chybí fft)
8. `svr → bbb → tty → ccc → eee → dac → fff → hhh → out` ❌ (má dac, ale chybí fft)

**Výsledek: 2 platné cesty** (cesty 3 a 4)

**Klíčové rozdíly oproti Part 1:**
- ✅ **Jiný startovní uzel:** `svr` místo `you`
- ✅ **Stejný cílový uzel:** `out`
- ✅ **NOVÁ PODMÍNKA:** Cesta musí navštívit **OBA** uzly `dac` **A** `fft`
- ⚠️ **Pořadí není důležité:** `svr → dac → fft → out` i `svr → fft → dac → out` jsou obě platné
- ✅ Stále platí: data proudí pouze **jedním směrem** (DAG)

### Solution

**Algoritmus: Modifikovaný DFS s trackingem navštívených povinných uzlů**

Základní myšlenka:
1. Použít **DFS s memoizací** z Part 1 jako základ
2. Rozšířit stav o **sadu navštívených povinných uzlů**
3. Cesta je **platná** pouze pokud při dosažení `out` jsme navštívili **oba** `dac` i `fft`

**Rozdíl ve stavu memoizace:**
- **Part 1:** `memo[node]` = počet cest z `node` do `out`
- **Part 2:** `memo[(node, visitedRequiredNodes)]` = počet cest z `node` do `out` s aktuálním stavem navštívených povinných uzlů

**Proč složitější memoizace?**
Protože počet cest z uzlu X do `out` **závisí na tom**, jestli už jsme navštívili `dac` a/nebo `fft`:
- Pokud jsme z uzlu X a už jsme navštívili oba povinné uzly → můžeme jít rovnou do `out`
- Pokud jsme navštívili pouze jeden → musíme ještě projít druhým
- Pokud jsme nenavštívili žádný → musíme projít oběma

**Klíčový algoritmus:**
```csharp
// State: (node, hasVisitedDac, hasVisitedFft)
long CountPathsWithRequired(node, hasVisitedDac, hasVisitedFft) {
    // Base case: reached 'out'
    if (node == "out") {
        // Valid path only if visited BOTH required nodes
        return (hasVisitedDac && hasVisitedFft) ? 1 : 0;
    }
    
    // Check memo
    var state = (node, hasVisitedDac, hasVisitedFft);
    if (memo.ContainsKey(state)) return memo[state];
    
    // Update visited status
    bool visitedDac = hasVisitedDac || (node == "dac");
    bool visitedFft = hasVisitedFft || (node == "fft");
    
    // Count paths through neighbors
    long totalPaths = 0;
    foreach (var neighbor in graph[node]) {
        totalPaths += CountPathsWithRequired(neighbor, visitedDac, visitedFft);
    }
    
    memo[state] = totalPaths;
    return totalPaths;
}

// Call: CountPathsWithRequired("svr", false, false)
```

**Časová složitost:**  
- **Stavy:** $O(V \times 2 \times 2) = O(4V)$ (každý uzel × 2 možnosti pro dac × 2 možnosti pro fft)
- **Pro každý stav:** $O(\text{počet sousedů})$
- **Celkem:** $O(V \times E)$ v nejhorším případě
- **Prakticky:** Mnohem rychlejší díky memoizaci (většina stavů se neprozkoumá)

**Prostorová složitost:**  
- $O(V \times 4)$ pro memo cache (4 stavy na uzel)
- Pro 595 uzlů: ~2,380 možných stavů → stále velmi malé

### Scope

**In Scope:**
- ✅ Parsování stejného formátu jako Part 1 (`device: outputs...`)
- ✅ Implementace DFS s rozšířeným stavem (tracking required nodes)
- ✅ Počítání pouze cest které navštíví **OBA** `dac` **A** `fft`
- ✅ Start z `svr`, konec v `out`
- ✅ Validace existence povinných uzlů v grafu

**Out of Scope:**
- ❌ Počítání cest které navštíví **pouze jeden** z povinných uzlů
- ❌ Výpis všech cest (pouze počet)
- ❌ Optimalizace pořadí (libovolné pořadí je OK)
- ❌ Detekce cyklů (předpoklad DAG z Part 1 stále platí)

---

## Context for Development

### Codebase Patterns

**Existující implementace (Day 11 Part 1):**
- ✅ `ParseGraph()` - funkční parsování, **ZNOVU POUŽÍT**
- ✅ `CountAllPaths()` - základní DFS s memoizací
- ✅ Memoizace: `Dictionary<string, long>` (pouze node → paths)
- ✅ Return type: `long` (pro velké počty cest)

**Klíčové soubory:**
- `Solutions/Day11.cs` - obsahuje Part 1 implementaci
- `AoC2025.Tests/Day11Tests.cs` - unit testy
- `Inputs/day11.txt` - reálný input

### Files to Reference

**Primary file:**
- `Solutions/Day11.cs` - modifikovat `SolvePart2()` metodu

**Test file:**
- `AoC2025.Tests/Day11Tests.cs` - přidat test pro Part 2 example

**Test data:**
- `AoC2025.Tests/TestData/day11_example.txt` - ověřit, jestli obsahuje Part 1 example nebo přidat Part 2 example

### Technical Decisions

1. **Reprezentace stavu pro memoizaci:**
   - **Možnost A:** `Dictionary<(string node, bool dac, bool fft), long>`
   - **Možnost B:** `Dictionary<string, long>` kde key je `"node|dac|fft"` (string concat)
   - **Rozhodnutí:** ✅ **Možnost A** - čistější, rychlejší, C# podporuje tuple jako key

2. **Tracking navštívených uzlů:**
   - **Možnost A:** `HashSet<string>` pro sledování všech navštívených uzlů
   - **Možnost B:** Pouze dva `bool` parametry (`hasVisitedDac`, `hasVisitedFft`)
   - **Rozhodnutí:** ✅ **Možnost B** - minimální overhead, potřebujeme pouze 2 uzly

3. **Validace existence uzlů:**
   - Před spuštěním DFS zkontrolovat, že `svr`, `dac`, `fft`, `out` existují v grafu
   - Pokud chybí → vrátit 0 nebo error
   - **Rozhodnutí:** ✅ Validovat a vrátit 0 (fail gracefully)

4. **Reuse vs. nová metoda:**
   - **Možnost A:** Modifikovat existující `CountAllPaths()` s optional parametry
   - **Možnost B:** Nová metoda `CountPathsWithRequiredNodes()`
   - **Rozhodnutí:** ✅ **Možnost B** - čistší separace, Part 1 zůstává nedotčený

---

## Implementation Plan

### Story 1: Příprava a validace
**Estimated Time:** 15 min

**Tasks:**
- [ ] Vytvořit novou metodu `CountPathsWithRequiredNodes(graph, start, target, required1, required2)`
- [ ] Implementovat validaci: zkontrolovat, že všechny povinné uzly existují v grafu
- [ ] Pokud chybí → vrátit 0

**Acceptance Criteria:**
- ✅ Metoda existuje a má správný signature
- ✅ Validace vrací 0 pokud `svr`, `dac`, `fft` nebo `out` chybí v grafu

---

### Story 2: DFS s trackingem povinných uzlů
**Estimated Time:** 30 min

**Tasks:**
- [ ] Vytvořit rekurzivní helper metodu `CountPathsWithRequiredNodesHelper()`
- [ ] Parametry: `(graph, current, target, hasVisitedDac, hasVisitedFft, memo)`
- [ ] Base case: pokud `current == target`, vrátit 1 pouze pokud `hasVisitedDac && hasVisitedFft`
- [ ] Update stavu: `visitedDac = hasVisitedDac || (current == "dac")`
- [ ] Update stavu: `visitedFft = hasVisitedFft || (current == "fft")`
- [ ] Memoizace: použít `(current, visitedDac, visitedFft)` jako key

**Acceptance Criteria:**
- ✅ Rekurze správně propaguje stav navštívených uzlů
- ✅ Base case vrací 1 pouze pokud byly navštíveny OBA povinné uzly
- ✅ Memoizace funguje s tuple key

**Příklad testovacího případu:**
```csharp
// Graph: svr → dac → fft → out
// Expected: 1 cesta (prochází oběma)
var graph = new Dictionary<string, List<string>> {
    ["svr"] = new List<string> { "dac" },
    ["dac"] = new List<string> { "fft" },
    ["fft"] = new List<string> { "out" }
};
var result = CountPathsWithRequiredNodes(graph, "svr", "out", "dac", "fft");
Assert.Equal(1, result);
```

---

### Story 3: Integrace s SolvePart2()
**Estimated Time:** 10 min

**Tasks:**
- [ ] Zavolat `CountPathsWithRequiredNodes(graph, "svr", "out", "dac", "fft")`
- [ ] Vrátit výsledek jako string
- [ ] Otestovat na reálném inputu `Inputs/day11.txt`

**Acceptance Criteria:**
- ✅ `SolvePart2()` vrací počet cest jako string
- ✅ Výsledek není 0 (mělo by existovat alespoň několik cest)

---

### Story 4: Unit testy
**Estimated Time:** 20 min

**Tasks:**
- [ ] Vytvořit test data pro Part 2 example (8 cest, 2 platné)
- [ ] Přidat test `Day11Part2_Example_ReturnsCorrectCount()`
- [ ] Expected result: `"2"`
- [ ] Otestovat edge cases:
  - Žádná cesta nenavštíví oba uzly → 0
  - Přímá cesta `svr → dac → fft → out` → 1
  - Graf kde `dac` a `fft` jsou na rozdílných větvích → kombinatorika

**Acceptance Criteria:**
- ✅ Test prochází s expected result `"2"` pro example input
- ✅ Edge cases mají správné výsledky

---

### Story 5: Edge cases a optimalizace
**Estimated Time:** 15 min

**Edge Cases:**

1. **Povinný uzel neexistuje v grafu**
   - Příklad: `dac` není v seznamu zařízení
   - Expected: 0 cest
   - Ošetřit validací před DFS

2. **Povinný uzel je nedosažitelný z `svr`**
   - Příklad: `svr` nemá cestu k `dac`
   - Expected: 0 cest
   - Automaticky ošetřeno DFS (žádná cesta nebude obsahovat `dac`)

3. **Přímá cesta z `svr` do `out` BEZ `dac` a `fft`**
   - Příklad: `svr: out`
   - Expected: 0 cest (nenavštívili jsme povinné uzly)
   - Ošetřeno base case v DFS

4. **Cesta navštíví jeden uzel vícekrát**
   - Příklad: `svr → dac → fft → dac → out`
   - Expected: Počítá se jako 1 cesta (už jsme navštívili oba)
   - Ošetřeno `||` operátorem (stav se nemění po druhém navštívení)

5. **Povinné uzly jsou shodné se start/end uzly**
   - Příklad: `dac == "svr"` nebo `fft == "out"`
   - Expected: Funguje normálně (započítáme je při průchodu)
   - Edge case: Pokud `svr == "dac"`, začínáme už s `hasVisitedDac = true`? 
   - **Rozhodnutí:** ❌ Ne, započítáváme pouze při průchodu (ne startovní uzel)
   - **Oprava:** ✅ Ano, pokud `start == "dac"`, započítat to hned na začátku

6. **Velký počet stavů v memoizaci**
   - S 595 uzly × 4 stavy = 2,380 možných stavů
   - Expected: Stále velmi rychlé (~10-50 ms)
   - Monitorovat: pokud > 1s, zvážit optimalizaci

**Tasks:**
- [ ] Otestovat edge cases výše
- [ ] Zvážit: pokud `start == required1` nebo `start == required2`, započítat hned
- [ ] Měřit performance (mělo by být < 100 ms)

**Acceptance Criteria:**
- ✅ Všechny edge cases mají správné výsledky
- ✅ Performance < 100 ms pro reálný input

---

## Additional Context

### Algorithm Walkthrough (Part 2 Example)

**Graf:**
```
svr → aaa, bbb
aaa → fft
fft → ccc
bbb → tty
tty → ccc
ccc → ddd, eee
ddd → hub
hub → fff
eee → dac
dac → fff
fff → ggg, hhh
ggg → out
hhh → out
```

**Simulace DFS:**

1. **Start:** `CountPaths("svr", false, false)`
   - Sousedé: `aaa`, `bbb`
   - Rekurze: `CountPaths("aaa", false, false)` + `CountPaths("bbb", false, false)`

2. **Větev 1:** `CountPaths("aaa", false, false)`
   - Sousedé: `fft`
   - Rekurze: `CountPaths("fft", false, false)`

3. **Uzel `fft`:** `CountPaths("fft", false, false)`
   - **Aktualizace stavu:** `visitedFft = true`
   - Sousedé: `ccc`
   - Rekurze: `CountPaths("ccc", false, true)` ← první true = fft navštíven

4. **Uzel `ccc`:** `CountPaths("ccc", false, true)`
   - Sousedé: `ddd`, `eee`
   - Rekurze: `CountPaths("ddd", false, true)` + `CountPaths("eee", false, true)`

5. **Větev `ddd`:** `CountPaths("ddd", false, true)` → ... → `out`
   - Při dosažení `out`: `hasVisitedDac = false`, `hasVisitedFft = true`
   - **Výsledek:** 0 (chybí dac)

6. **Větev `eee`:** `CountPaths("eee", false, true)`
   - Sousedé: `dac`
   - Rekurze: `CountPaths("dac", false, true)`

7. **Uzel `dac`:** `CountPaths("dac", false, true)`
   - **Aktualizace stavu:** `visitedDac = true`
   - Sousedé: `fff`
   - Rekurze: `CountPaths("fff", true, true)` ← OBA jsou true!

8. **Uzel `fff`:** `CountPaths("fff", true, true)`
   - Sousedé: `ggg`, `hhh`
   - Rekurze: `CountPaths("ggg", true, true)` + `CountPaths("hhh", true, true)`

9. **Koncové uzly:**
   - `CountPaths("ggg", true, true)` → `out` → **VRACÍ 1** ✅
   - `CountPaths("hhh", true, true)` → `out` → **VRACÍ 1** ✅

**Celkem z větve `aaa`:** 2 cesty

10. **Větev 2:** `CountPaths("bbb", false, false)`
    - `bbb → tty → ccc → eee → dac → fff → out`
    - Ale chybí `fft`! → **Výsledek:** 0

**Finální součet:** 2 + 0 = **2 cesty** ✅

### Dependencies

**Žádné nové závislosti:**
- ✅ Použití existujících struktur z Part 1
- ✅ Standard C# collections (`Dictionary`, `List`)

### Testing Strategy

**Unit testy:**
1. ✅ Part 2 example (expected: 2)
2. ✅ Simple path test (svr → dac → fft → out, expected: 1)
3. ✅ No valid paths (svr → out without dac/fft, expected: 0)
4. ✅ Multiple branches (kombinatorika, expected: vypočítáno ručně)

**Integration test:**
- ✅ Spustit na reálném inputu `Inputs/day11.txt`
- ✅ Ověřit, že výsledek > 0 a < total paths z Part 1

### Performance Expectations

**Part 1 výsledek:**
- 595 uzlů, ~1,696 hran
- Počet všech cest: pravděpodobně **tisíce až miliony**

**Part 2 očekávání:**
- Počet cest s oběma povinými uzly: **výrazně menší** (filtr je velmi restriktivní)
- Očekávaný výsledek: **stovky až tisíce** (řádově 1-10% z Part 1)
- Runtime: **< 100 ms** (4× více stavů než Part 1, ale pořád O(V×E))

### Notes

**Proč je Part 2 složitější než Part 1?**
- Part 1: Jednoduchá memoizace (`node → path_count`)
- Part 2: Stavová memoizace (`(node, dac_visited, fft_visited) → path_count`)
- **4× více stavů**, ale pořád velmi efektivní díky memoizaci

**Alternativní přístupy (NEPOUŽÍVAT, pokud není nutné):**

1. **Brute-force s filtrováním:**
   - Spočítat všechny cesty z Part 1
   - Filtrovat ty, které obsahují oba uzly
   - **Nevýhoda:** Pokud je miliony cest, paměť exploduje

2. **Dva průchody DFS:**
   - První průchod: cesty z `svr` do `dac` nebo `fft`
   - Druhý průchod: z druhého uzlu do `out`
   - **Nevýhoda:** Složité kombinovat, neefektivní

3. **BFS s trackingem stavu:**
   - Možné, ale složitější implementace
   - **Nevýhoda:** Víc paměti než DFS

**Doporučení:** ✅ Držet se DFS s rozšířeným stavem (nejpřímočařejší a efektivní)

---

## Checklist před implementací

- [ ] Pochopil jsem, že potřebuji trackovat stav dvou povinných uzlů
- [ ] Vím, že memoizace musí zahrnout stav `(node, dac_visited, fft_visited)`
- [ ] Validuji existenci všech povinných uzlů před spuštěním DFS
- [ ] Base case vrací 1 pouze pokud `hasVisitedDac && hasVisitedFft`
- [ ] Mám připravený test pro Part 2 example (expected: 2)
- [ ] Vím, že výsledek by měl být výrazně menší než Part 1

---

**Estimated Total Time:** ~1.5 hodiny

**Ready to implement!** 🚀
