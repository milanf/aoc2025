# Tech-Spec: Day 11 Part 1 - Reactor

**Created:** 2025-12-11  
**Status:** 📝 **Draft**  
**AoC Link:** https://adventofcode.com/2025/day/11

---

## Overview

### Problem Statement

V továrně se nachází toroidní reaktor, který napájí celou továrnu. Reaktor komunikuje s novým serverovým rackem přes síť propojených zařízení. Každé zařízení má výstupy vedoucí k dalším zařízením, data proudí pouze směrem dopředu (nikdy zpět).

**Cíl:** Najít **počet všech různých cest** vedoucích z výchozího zařízení `you` do reaktoru `out`.

**Příklad z AoC:**
```
aaa: you hhh
you: bbb ccc
bbb: ddd eee
ccc: ddd eee fff
ddd: ggg
eee: out
fff: out
ggg: out
hhh: ccc fff iii
iii: out
```

**Všechny cesty z `you` do `out`:**
1. `you → bbb → ddd → ggg → out`
2. `you → bbb → eee → out`
3. `you → ccc → ddd → ggg → out`
4. `you → ccc → eee → out`
5. `you → ccc → fff → out`

**Celkem: 5 různých cest**

**Klíčové body:**
- Každá cesta je **unikátní posloupnost zařízení**
- Data proudí pouze **jedním směrem** (z uzlu k jeho výstupům)
- Cesta může obsahovat **libovolný počet zařízení**
- Hledáme **všechny možné cesty**, ne nejkratší cestu

### Input Analysis

**Reálný input (`Inputs/day11.txt`):**
- **595 zařízení** (595 řádků)
- Každý řádek definuje jedno zařízení a jeho výstupy:
  - Formát: `device_name: output1 output2 output3 ...`
  - Příklad: `you: wpc ckx kuq rgd rzg sox awx abu bhd per zdq ywf nnk opn dur pfw`

**Statistika grafu:**
- **Celkový počet uzlů:** 595
- **Výchozí uzel:** `you` (má 15 výstupů)
- **Cílové uzly:** `out` (objevuje se cca 20× jako výstup různých zařízení)
- **Průměrný počet výstupů na zařízení:** ~2.85
- **Maximální počet výstupů:** 23 (u některých komplexních zařízení)

**Struktura grafu:**
- **Directed Acyclic Graph (DAG)** - orientovaný acyklický graf
  - Podle zadání data proudí pouze dopředu → **nemůže obsahovat cykly**
  - Pokud by obsahoval cykly, počet cest by byl nekonečný
- **Multi-path graph** - z většiny uzlů vede více výstupů
- **Dense vs. Sparse:** průměrně 2.85 výstupů na uzel → **spíše řídký graf**

**Porovnání s příkladem:**
- Příklad: 9 zařízení → 5 cest
- Reálný vstup: 595 zařízení → očekávaný počet cest **mnohem větší**

**KRITICKÁ ANALÝZA ROZSAHU:**
S 595 uzly a průměrně 2.85 výstupy na uzel, počet cest může být **extrémně vysoký**:
- Pokud by každý uzel měl průměrně 3 výstupy a průměrná hloubka byla 10: $3^{10} = 59,049$ cest
- V nejhorším případě (všechny uzly spojeny do širokého stromu): **miliony až miliardy cest**

**Důležité zjištění:** 
⚠️ **Nelze spoléhat na brute-force prohledávání všech cest!** 
- Při takto velkém grafu musíme použít **optimalizované DFS s memoizací** nebo **dynamické programování**
- Možná buď počet cest je přijatelný (tisíce), nebo musíme použít chytřejší přístup

### Algorithm Analysis

Tento problém je klasický **All Paths Problem** v orientovaném grafu.

#### Přístup 1: DFS (Depth-First Search) s počítáním cest ✅ **ZÁKLADNÍ ŘEŠENÍ**

**Algoritmus:**
```
function countPaths(current, target, graph, visited):
    if current == target:
        return 1
    
    if current in visited:
        return 0  // Ochrana proti cyklům
    
    visited.add(current)
    totalPaths = 0
    
    for neighbor in graph[current]:
        totalPaths += countPaths(neighbor, target, graph, visited)
    
    visited.remove(current)  // Backtrack
    return totalPaths
```

**Kroky:**
1. Začni v uzlu `you`
2. Pro každý výstup rekurzivně spočítej cesty do `out`
3. Když dosáhneš `out`, vrať 1 (našli jsme cestu)
4. Sečti všechny cesty z každého výstupu
5. **Backtracking:** odstraň uzel z visited po prozkoumání

**Časová složitost:** $O(V + E)$ × počet cest
- V = počet uzlů (595)
- E = počet hran (~1,696)
- **V nejhorším případě:** $O(V!)$ pokud je graf úplný
- **V průměrném případě (DAG):** $O(V \times E)$ díky memoizaci

**Prostorová složitost:** $O(V)$ pro rekurzivní zásobník a visited set

#### Přístup 2: DFS s Memoizací ✅ **OPTIMALIZOVANÉ ŘEŠENÍ**

**Klíčová optimalizace:** 
Pokud už jsme spočítali počet cest z uzlu X do `out`, můžeme si to zapamatovat!

```
memo = {}

function countPathsMemo(current, target, graph):
    if current == target:
        return 1
    
    if current in memo:
        return memo[current]
    
    totalPaths = 0
    for neighbor in graph[current]:
        totalPaths += countPathsMemo(neighbor, target, graph)
    
    memo[current] = totalPaths
    return totalPaths
```

**Výhody:**
- Každý uzel navštívíme **maximálně jednou** pro výpočet
- Časová složitost: **$O(V + E)$** - lineární vzhledem k velikosti grafu
- Prostorová složitost: **$O(V)$** pro memo cache

**Pro náš vstup:**
- 595 uzlů + ~1,696 hran = **~2,291 operací** ✅ **VELMI RYCHLÉ**

#### Přístup 3: Dynamické programování (Topological Sort) 🎯 **NEJEFEKTIVNĚJŠÍ**

Pro DAG můžeme použít topologické seřazení:

```
1. Topologicky seřaď uzly (od out k you)
2. dp[out] = 1
3. Pro každý uzel v topologickém pořadí:
   dp[node] = sum(dp[neighbor] for neighbor in graph[node])
4. Vrať dp[you]
```

**Výhody:**
- Žádná rekurze → žádný stack overflow
- Čistá $O(V + E)$ složitost
- Jednodušší implementace

**Časová složitost:** $O(V + E)$
**Prostorová složitost:** $O(V)$

#### Porovnání přístupů:

| Přístup | Časová složitost | Prostorová | Výhody | Nevýhody |
|---------|------------------|------------|--------|----------|
| DFS basic | $O(\text{počet cest})$ | $O(V)$ | Jednoduchý | Pomalý pro mnoho cest |
| DFS + memoizace | $O(V + E)$ | $O(V)$ | Rychlý, přímočarý | Rekurze |
| DP + Topo sort | $O(V + E)$ | $O(V)$ | Nejrychlejší, bez rekurze | Složitější implementace |

**Doporučení:** 
✅ **DFS s memoizací** - nejlepší kompromis mezi jednoduchostí a výkonem
- Pokud bude stack overflow, přejít na DP

---

## Requirements

### Functional Requirements

1. **RF1: Parsování vstupu**
   - Načíst seznam zařízení z textového souboru
   - Každý řádek obsahuje:
     - Název zařízení: `device_name`
     - Seznam výstupů: `output1 output2 ...`
   - Vytvořit graf reprezentovaný jako `Dictionary<string, List<string>>`

2. **RF2: Detekce cyklů (volitelné, ale doporučené)**
   - Ověřit, že graf je DAG
   - Pokud obsahuje cykly, vrátit error
   - **Podle zadání by cykly neměly existovat**

3. **RF3: Počítání cest**
   - Implementovat DFS s memoizací
   - Najít všechny cesty z `you` do `out`
   - Vrátit celkový počet různých cest

4. **RF4: Výstup výsledku**
   - Vrátit počet cest jako celé číslo

### Non-Functional Requirements

1. **NFR1: Výkon**
   - Řešení musí běžet v < 1 sekundě pro 595 uzlů
   - S memoizací: očekáváno ~1-10 ms

2. **NFR2: Paměť**
   - Maximální paměť: O(V) pro memo cache + graf
   - Pro 595 uzlů: ~10-50 KB

3. **NFR3: Přesnost**
   - Výsledek musí být **přesný počet** všech unikátních cest
   - Použít `long` nebo `ulong` pro výsledek (může být velké číslo)

---

## Edge Cases

1. **Přímá cesta z `you` do `out`**
   - Pokud `you` má `out` jako přímý výstup
   - Příklad: `you: out` → 1 cesta
   - Očekáváno: 1 + další cesty přes ostatní uzly

2. **Žádná cesta**
   - `you` není připojeno k `out`
   - Podle zadání by nemělo nastat
   - Výsledek: 0

3. **Izolované uzly**
   - Některá zařízení nejsou dosažitelná z `you`
   - Nebo nemají cestu do `out`
   - DFS je automaticky přeskočí

4. **Velký počet cest**
   - Výsledek může být velké číslo (miliony)
   - Použít `long` nebo `BigInteger` pokud přeteče

5. **`out` má vlastní výstupy**
   - Podle zadání `out` je koncový bod (reaktor)
   - Pokud by měl výstupy, ignorovat je (cesta končí v `out`)

6. **Duplicitní výstupy**
   - Zařízení má ve výstupu stejný uzel vícekrát
   - Příklad: `aaa: bbb bbb ccc`
   - Ošetřit: každý výstup počítat zvlášť (může vést k vícenásobným cestám)

7. **Self-loops**
   - Zařízení má výstup samo na sebe
   - Příklad: `aaa: aaa bbb`
   - Ošetřit v DFS pomocí visited setu (backtracking)

---

## Implementation Plan

### Story 1: Parsování a reprezentace grafu
**Acceptance Criteria:**
- Načíst všechny řádky ze souboru
- Parsovat formát `name: output1 output2 ...`
- Vytvořit `Dictionary<string, List<string>>` pro graf
- Test: Ověřit správné parsování ukázkového vstupu

**Odhad:** 30 min

---

### Story 2: DFS s memoizací
**Acceptance Criteria:**
- Implementovat rekurzivní DFS
- Použít Dictionary pro memoizaci: `Dictionary<string, long>`
- Počítat cesty z libovolného uzlu do `out`
- Test: Ověřit na ukázkovém příkladu (očekávaný výsledek: 5 cest)

**Odhad:** 45 min

---

### Story 3: Integrace a výpočet finálního výsledku
**Acceptance Criteria:**
- Načíst vstupní soubor `Inputs/day11.txt`
- Spustit DFS z uzlu `you`
- Vrátit celkový počet cest
- Test: Ověřit, že výsledek není 0 a je rozumný

**Odhad:** 15 min

---

### Story 4: Edge cases a validace
**Acceptance Criteria:**
- Ošetřit případ, kdy `you` nebo `out` neexistuje
- Ošetřit self-loops pomocí visited setu
- Použít `long` pro výsledek
- Test: Ověřit edge cases (přímá cesta, izolované uzly)

**Odhad:** 20 min

---

### Story 5: Unit testy
**Acceptance Criteria:**
- Test pro parsování vstupu
- Test pro příklad z AoC (5 cest)
- Test pro jednoduchý graf (1 cesta)
- Test pro komplexnější scénář

**Odhad:** 30 min

---

## Test Cases

### TC1: Příklad z AoC
**Input:**
```
aaa: you hhh
you: bbb ccc
bbb: ddd eee
ccc: ddd eee fff
ddd: ggg
eee: out
fff: out
ggg: out
hhh: ccc fff iii
iii: out
```

**Expected Output:** `5`

**Cesty:**
1. `you → bbb → ddd → ggg → out`
2. `you → bbb → eee → out`
3. `you → ccc → ddd → ggg → out`
4. `you → ccc → eee → out`
5. `you → ccc → fff → out`

---

### TC2: Jednoduchá přímá cesta
**Input:**
```
you: out
```

**Expected Output:** `1`

---

### TC3: Dvě paralelní cesty
**Input:**
```
you: aaa bbb
aaa: out
bbb: out
```

**Expected Output:** `2`

---

### TC4: Žádná cesta
**Input:**
```
you: aaa
aaa: bbb
bbb: ccc
```
(žádný uzel nevede do `out`)

**Expected Output:** `0`

---

### TC5: Komplexnější strom
**Input:**
```
you: a b
a: c d
b: c e
c: out
d: out
e: out
```

**Expected Output:** `5`

**Cesty:**
1. `you → a → c → out`
2. `you → a → d → out`
3. `you → b → c → out`
4. `you → b → e → out`

(Poznámka: uzel `c` je dosažitelný dvakrát, ale cesty jsou různé)

---

## Complexity Analysis

### Vstupní data:
- **V (vertices):** 595 uzlů
- **E (edges):** ~1,696 hran (2.85 průměrně na uzel)

### DFS s memoizací:
- **Časová složitost:** $O(V + E) = O(595 + 1696) = O(2291)$ ✅
- **Prostorová složitost:** $O(V) = O(595)$ ✅
- **Očekávaný čas běhu:** < 10 ms

### Bez memoizace:
- **Časová složitost:** $O(\text{počet cest})$ ⚠️
- Pokud je průměrná hloubka 20 a branching factor 3: $O(3^{20}) = O(3.5 \times 10^9)$ ❌ **NEPŘIJATELNÉ**

**Závěr:** Memoizace je **kritická** pro efektivní řešení!

---

## Notes

- **Graf je DAG:** Podle zadání data proudí pouze dopředu → žádné cykly
- **Pozor na velké číslo:** Výsledek může přesáhnout `int.MaxValue`, použít `long`
- **Alternativní přístup:** Topologické seřazení + DP (pokud DFS selže kvůli stack overflow)
- **Debugging:** Vypsat prvních 10 cest pro vizuální kontrolu správnosti

---

## References

- **AoC 2025 Day 11:** https://adventofcode.com/2025/day/11
- **All Paths in DAG:** https://en.wikipedia.org/wiki/Path_(graph_theory)
- **Topological Sort:** https://en.wikipedia.org/wiki/Topological_sorting
- **DFS Algorithm:** https://en.wikipedia.org/wiki/Depth-first_search

---

**Estimated Total Time:** ~2.5 hodiny

**Priority:** 🔥 High - Kritický algoritmus, vyžaduje správnou optimalizaci
