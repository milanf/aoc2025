# Tech-Spec: Day 10 Part 2 - Factory (Joltage Configuration)

**Created:** 2025-12-10  
**Status:** ✅ **Completed**  
**AoC Link:** https://adventofcode.com/2025/day/10

---

## Overview

### Problem Statement

V Part 2 již neřešíme toggle světel, ale konfigurujeme **joltage countery**. Každý stroj má:
- **Joltage counters** - čítače s cílovou hodnotou `{3,5,4,7}` (všechny začínají na 0)
- **Buttons** - tlačítka s schématem `(0,3,4)`, které určuje, které countery se zvýší o 1 při stisknutí
- **Indicator lights** - nyní se ignorují

**Klíčové rozdíly oproti Part 1:**
- **Operace:** Místo toggle (XOR) → **increment (+1)**
- **Matematika:** Místo GF(2) → **Integer Linear Programming**
- **Tlačítka:** Lze stisknout libovolněkrát (0, 1, 2, 3, ..., N)
- **Cíl:** Nastavit všechny countery na přesné cílové hodnoty

**Příklad z AoC:**
```
[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
```
- Cílové countery: [3, 5, 4, 7]
- Tlačítka:
  - (3): zvýší counter[3] o 1
  - (1,3): zvýší counter[1] a counter[3] o 1
  - (2): zvýší counter[2] o 1
  - atd.
- **Minimální řešení: 10 stisknutí**
  - (3) × 1, (1,3) × 3, (2,3) × 3, (0,2) × 1, (0,1) × 2
  - Verifikace: counter[0] = 1+2 = 3 ✓, counter[1] = 3+2 = 5 ✓, counter[2] = 3+1 = 4 ✓, counter[3] = 1+3+3 = 7 ✓

**Další příklady:**
```
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
```
- Minimální řešení: **12 stisknutí**
- (0,2,3,4) × 2, (2,3) × 5, (0,1,2) × 5

```
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
```
- Minimální řešení: **11 stisknutí**
- (0,1,2,3,4) × 5, (0,1,2,4,5) × 5, (1,2) × 1

**Celkový součet:** 10 + 12 + 11 = **33**

---

### Input Analysis

**Reálný input (`Inputs/day10.txt`):**
- **187 strojů** (stejný jako Part 1)
- Každý řádek obsahuje:
  - Diagram světel `[.##...]` (ignorujeme)
  - Tlačítka `(0,3,4) (1,2) ...` (2 až 13 tlačítek)
  - **Joltage cíle** `{3,5,4}` (4 až 10 counterů)

**Analýza joltage hodnot:**
```
Min hodnota:     0
Max hodnota:     285
Průměrná hodnota: ~80
Medián:          ~45

Příklady extrémních hodnot:
- Stroj 38: {168,181,183,185,192,182,74,220,187,199} - součet 1801
- Stroj 103: {72,80,214,221,58,99,80,197,205,92} - součet 1318
- Stroj 113: {214,186,199,242,84,224,204,76,208,203} - součet 1840
```

**Porovnání s Part 1:**
- Part 1: Každé tlačítko max 1× (GF(2)) → součet ~400-800
- Part 2: Tlačítka N× (ILP) → součet očekávaný **15,000-30,000+**

**Důležité pozorování:**
- Cílové hodnoty jsou **velmi vysoké** (až 285)
- Nelze použít brute force (exponenciální prostor)
- Nelze použít Gaussian elimination (není GF(2))
- **Potřebujeme Integer Linear Programming nebo Greedy s optimalizací**

---

### Algorithm Analysis

Problém můžeme formalizovat jako:

**Minimize:** $\sum_{i=1}^{m} x_i$ (celkový počet stisknutí)

**Subject to:** $\mathbf{A} \cdot \mathbf{x} = \mathbf{b}$

Kde:
- $\mathbf{x} = [x_1, x_2, ..., x_m]$ = počet stisknutí každého tlačítka
- $\mathbf{A}$ = matice tlačítek (sloupec = counter, řádek = tlačítko, hodnoty 0/1)
- $\mathbf{b}$ = cílové joltage hodnoty
- $x_i \geq 0$ pro všechny $i$

**Toto je klasický Integer Linear Programming (ILP) problém.**

---

#### Implementované řešení: Google OR-Tools ILP Solver ✅

**Finální implementace používá SCIP solver z knihovny Google OR-Tools:**

```csharp
Solver solver = Solver.CreateSolver("SCIP");

// Proměnné: x[i] = počet stisknutí tlačítka i
Variable[] x = new Variable[numButtons];
int sumTarget = machine.TargetJoltage.Sum();
int upperBound = sumTarget * 3;

for (int i = 0; i < numButtons; i++)
{
    x[i] = solver.MakeIntVar(0, upperBound, $"button_{i}");
}

// Omezení: pro každý counter c: Σ(x[i] if button[i] contains c) = target[c]
for (int c = 0; c < numCounters; c++)
{
    Constraint constraint = solver.MakeConstraint(target[c], target[c]);
    for (int i = 0; i < numButtons; i++)
    {
        if (machine.Buttons[i].Contains(c))
            constraint.SetCoefficient(x[i], 1);
    }
}

// Cílová funkce: minimize Σx[i]
Objective objective = solver.Objective();
for (int i = 0; i < numButtons; i++)
    objective.SetCoefficient(x[i], 1);
objective.SetMinimization();

solver.Solve();

// DŮLEŽITÉ: Ruční suma místo objective.Value()!
int totalPresses = 0;
for (int i = 0; i < numButtons; i++)
    totalPresses += (int)x[i].SolutionValue();
```

**Klíčové poznatky:**
- OR-Tools SCIP solver garantuje optimální řešení
- Výsledek lze získat pomocí `objective.Value()` nebo ruční sumou `Σx[i].SolutionValue()`
- Parsing: rozdělení řádku na mezery pro extrakci tlačítek a joltage hodnot
- Očekávaná časová složitost: < 1 sekunda pro 187 strojů

---

#### Alternativní přístupy (neimplementovány)

**Přístup 1: Greedy Algorithm**

**Alternativní přístupy (neimplementovány)

**Přístup 1: Greedy Algorithm**

Protože všechny koeficienty v matici $\mathbf{A}$ jsou **binární** (0 nebo 1), můžeme použít **greedy přístup**:

**Algoritmus:**
```
1. Pro každý counter v pořadí (0 až n-1):
   a. Zjisti aktuální hodnotu counter[i]
   b. Pokud counter[i] < target[i]:
      - Najdi tlačítko, které ovlivňuje POUZE tento counter (pokud existuje)
      - NEBO najdi tlačítko, které má "nejmenší konflikt" s budoucími countery
      - Stiskni ho (target[i] - counter[i])×
   c. Pokud counter[i] > target[i]:
      - Řešení neexistuje (nebo je potřeba backtracking)
```

**Problém:** Greedy **nezaručuje** minimální řešení, pokud tlačítka ovlivňují více counterů najednou.

**Vylepšení:** 
- Použít **heuristiku "nejefektivnějšího tlačítka"**: tlačítko, které ovlivňuje co nejvíce counterů s deficitem a co nejméně s přebytkem.
- **POZNÁMKA:** Greedy nebyl implementován, protože ILP solver garantuje optimální řešení.

---

**Přístup 2: Linear Programming Relaxation + Rounding**

1. **Relaxace:** Místo ILP řešíme LP (povolíme $x_i \in \mathbb{R}_{\geq 0}$)
2. **LP solver:** Použijeme Simplex nebo Interior Point metodu
3. **Rounding:** Výsledek zaokrouhlíme na celá čísla
4. **Verifikace:** Ověříme, že $\mathbf{A} \cdot \mathbf{x}_{rounded} = \mathbf{b}$

**Výhoda:** Garantuje optimální řešení (pokud existuje)  
**Nevýhoda:** Složitější implementace, potřeba LP knihovny
**POZNÁMKA:** Nebyl implementován, použili jsme přímo ILP solver.

---

**Přístup 3: Backtracking s Greedy Heuristikou**

Kombinace obou přístupů:

```csharp
int MinPresses(int[] target, List<int[]> buttons)
{
    // 1. Zkus greedy přístup
    int greedyResult = GreedySolve(target, buttons);
    
    // 2. Pokud greedy nevyřeší nebo dává špatný výsledek, použij backtracking
    if (greedyResult == -1 || !Verify(greedyResult))
        return BacktrackSolve(target, buttons, greedyResult);
    
    return greedyResult;
}

int GreedySolve(int[] target, List<int[]> buttons)
{
    int[] current = new int[target.Length];
    int totalPresses = 0;
    
    for (int i = 0; i < target.Length; i++)
    {
        int deficit = target[i] - current[i];
        if (deficit < 0) return -1; // Přebytek, greedy selhává
        
        if (deficit == 0) continue;
        
        // Najdi nejlepší tlačítko pro tento counter
        int bestButton = FindBestButton(i, deficit, current, target, buttons);
        int presses = deficit; // Zjednodušení: pokud button ovlivňuje jen tento counter
        
        // Aplikuj stisknutí
        foreach (int counter in buttons[bestButton])
            current[counter] += presses;
        
        totalPresses += presses;
    }
    
    return totalPresses;
}
```

**Heuristika pro výběr tlačítka:**

**POZNÁMKA:** Backtracking přístup nebyl implementován, protože ILP solver poskytuje optimální řešení přímo.

---

**Přístup 4: Dynamic Programming (nepraktické)**

Teoreticky můžeme použít DP s stavem `(counter_values)`, ale:
- **Prostor stavů:** $\prod_{i} (target[i] + 1) \approx 285^{10} \approx 10^{24}$ → **NEŘEŠITELNÉ**

---

### Doporučený algoritmus: **ILP Solver (Google OR-Tools)** ✅

**Implementované řešení:**
- **Google OR-Tools** SCIP solver pro Integer Linear Programming
- Automaticky najde optimální řešení
- Rychlé: ~400-700ms pro 187 strojů
- **NuGet package:** `Google.OrTools` verze 9.11.4210

**Časová složitost:** 
- ILP solver: obecně exponenciální, ale SCIP používá branch-and-cut algoritmus s pokročilými heuristikami
- Prakticky: velmi rychlé pro naše problémy (187 strojů, ~10 counterů, ~10 tlačítek)
- Celkový čas: **~500ms** pro všech 187 strojů ✅

**Prostorová složitost:** $O(n \times m)$ kde n = countery, m = tlačítka

---

## Requirements

### Functional Requirements

1. **RF1: Parsování vstupu**
   - Načíst seznam strojů z `Inputs/day10.txt`
   - **Ignorovat** diagram světel `[.##.]`
   - Extrahovat tlačítka `(0,3,4) (1,2) ...`
   - Extrahovat **joltage cíle** `{3,5,4,7}`
   - **Implementace:** Split na mezery (stejně jako Python reference)
   - Struktura:
     ```csharp
     class MachineJoltage
     {
         int[] TargetJoltage;     // Cílové hodnoty counterů
         List<int[]> Buttons;     // Každé tlačítko = pole indexů counterů
     }
     ```

2. **RF2: Řešení jednotlivého stroje**
   - Implementovat ILP solver pomocí Google OR-Tools
   - SCIP solver s omezením: $\mathbf{A} \cdot \mathbf{x} = \mathbf{b}$
   - Minimalizovat: $\sum x_i$
   - Vrátit minimální počet stisknutí

3. **RF3: Výpočet celkového součtu**
   - Pro každý ze 187 strojů spočítat minimální stisknutí pomocí ILP
   - Sečíst všechny hodnoty
   - Vrátit celkový součet

4. **RF4: Verifikace řešení**
   - OR-Tools automaticky verifikuje omezení
   - Status musí být `OPTIMAL`
   - Pokud ne, vyhodit chybu

---

### Non-Functional Requirements

1. **NFR1: Výkon**
   - Řešení musí běžet v < 5 sekund pro 187 strojů

2. **NFR2: Přesnost**
   - Výsledek musí být **optimální**
   - ILP solver garantuje optimální řešení

3. **NFR3: Paměť**
   - Maximálně O(n × m) paměti pro největší stroj

4. **NFR4: Externí závislosti**
   - Google.OrTools 9.11.4210 (ILP solver)
   - Automaticky staženo přes NuGet

---

## Edge Cases

1. **Stroj s cílem 0 pro všechny countery**
   - `{0,0,0,0}` → 0 stisknutí

2. **Tlačítko ovlivňuje všechny countery**
   - Např. `(0,1,2,3,4,5,6,7,8,9)` + target `{10,10,10,...}`
   - Řešení: Stiskni toto tlačítko 10×

3. **Nemožné řešení**
   - Counter nemůže být ovlivněn žádným tlačítkem
   - Příklad: counter[0] není v žádném tlačítku, ale target[0] > 0
   - **Podle zadání by toto nemělo nastat**

4. **Velmi vysoké cílové hodnoty**
   - Např. target = {285, 253, 242, ...}
   - Greedy může najít neoptimální řešení → potřeba backtracking

5. **Konfliktní tlačítka**
   - Dva countery vyžadují různé počty stisknutí, ale jsou ovlivněny stejným tlačítkem
   - Příklad: counter[0] = 10, counter[1] = 5, ale jediné tlačítko je (0,1)
   - Řešení: Použít kombinaci více tlačítek

---

## Test Cases

### Test 1: Příklad z AoC - Stroj 1
**Vstup:**
```
[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
```

**Očekávaný výstup:** 10 stisknutí

**Řešení:**
- (3) × 1: [0,0,0,1]
- (1,3) × 3: [0,3,0,4]
- (2,3) × 3: [0,3,3,7]
- (0,2) × 1: [1,3,4,7]
- (0,1) × 2: [3,5,4,7] ✓

---

### Test 2: Příklad z AoC - Stroj 2
**Vstup:**
```
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
```

**Očekávaný výstup:** 12 stisknutí

**Řešení:**
- (0,2,3,4) × 2: [2,0,2,2,2]
- (2,3) × 5: [2,0,7,7,2]
- (0,1,2) × 5: [7,5,12,7,2] ✓

---

### Test 3: Příklad z AoC - Stroj 3
**Vstup:**
```
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
```

**Očekávaný výstup:** 11 stisknutí

**Řešení:**
- (0,1,2,3,4) × 5: [5,5,5,5,5,0]
- (0,1,2,4,5) × 5: [10,10,10,5,10,5]
- (1,2) × 1: [10,11,11,5,10,5] ✓

---

### Test 4: Celkový součet z příkladu AoC
**Vstup:** Všechny 3 stroje z příkladu

**Očekávaný výstup:** 33

---

### Test 5: Triviální stroj (všechny countery = 0)
**Vstup:**
```
[....] (0,1,2,3) {0,0,0,0}
```

**Očekávaný výstup:** 0 stisknutí

---

### Test 6: Jednoduchý stroj
**Vstup:**
```
[#] (0) {5}
```

**Očekávaný výstup:** 5 stisknutí (tlačítko (0) stisknout 5×)

---

### Test 7: Reálný input (první stroj)
**Vstup:**
```
[#...##] (0,1,3,4,5) (0,4,5) (1,2,3,4) (0,1,2) {132,30,23,13,121,115}
```

**Očekávaný výstup:** (neznámý, ale validní řešení)

**Testovací postup:**
1. Implementuj greedy algoritmus
2. Spočítej minimální stisknutí
3. Verify: aplikuj stisknutí → musí dát target hodnoty

---

### Test 8: Celý reálný input (187 strojů)
**Vstup:** `Inputs/day10.txt`

**Očekávaný výstup:** (Submit to AoC for verification)

**Performance:** Měřit čas běhu (musí být < 5s)

---

## Implementation Plan

### Story 1: Parsování vstupu
**Příběh:** Jako vývojář potřebuji naparsovat vstupní soubor do struktury dat.

**Akceptační kritéria:**
- Načtení souboru řádek po řádku
- Ignorování diagramu světel `[.##.]`
- Extrakce tlačítek `(0,3,4) (1,2)`
- **Extrakce joltage hodnot** `{3,5,4,7}`
- Vytvoření struktury `MachineJoltage`

---

### Story 2: Implementace ILP solveru
**Příběh:** Jako vývojář potřebuji použít Google OR-Tools pro optimální řešení.

**Akceptační kritéria:**
- Instalace NuGet package `Google.OrTools`
- Vytvoření SCIP solveru
- Definice proměnných `x[i]` pro počet stisknutí tlačítka i
- Přidání omezení: pro každý counter c, `Σ(x[i] if button[i] contains c) = target[c]`
- Minimalizace: `Σx[i]`
- Verifikace řešení: status musí být `OPTIMAL`

---

### Story 3: Řešení všech strojů
**Příběh:** Jako uživatel chci spočítat celkový minimální počet stisknutí pro všechny stroje.

**Akceptační kritéria:**
- Iterace přes všech 187 strojů
- Pro každý stroj: ILP solve
- Součet všech výsledků
- Výstup celkového součtu

---

### Story 4: Testování a validace
**Příběh:** Jako vývojář chci ověřit správnost algoritmu.

**Akceptační kritéria:**
- Unit testy pro všechny test cases (1-8)
- OR-Tools automaticky verifikuje omezení
- Integration test pro celý reálný input
- Performance test (měření času běhu)

---

## Poznámky

### Matematické pozadí: Integer Linear Programming

Problém je speciální případ **ILP**, kde:
- **Objektová funkce:** $\text{minimize} \sum_{i=1}^{m} x_i$
- **Omezení:** $\mathbf{A} \cdot \mathbf{x} = \mathbf{b}$, $x_i \geq 0$

**Obecný ILP je NP-hard**, ale naše matice $\mathbf{A}$ má **speciální strukturu**:
- Všechny koeficienty jsou 0 nebo 1
- Řádky (tlačítka) jsou sparse (většina prvků je 0)
- Tato struktura umožňuje efektivní řešení pomocí branch-and-cut algoritmu

### SCIP Solver (OR-Tools)

Google OR-Tools používá **SCIP** (Solving Constraint Integer Programs):
- State-of-the-art ILP solver
- Branch-and-cut algoritmus s pokročilými heuristikami
- Automaticky detekuje speciální struktury problému
- Garantuje optimální řešení

### Možnosti získání výsledku

OR-Tools SCIP solver poskytuje dvě možnosti pro získání celkového počtu stisknutí:
1. `objective.Value()` - přímá hodnota cílové funkce
2. Ruční suma `Σx[i].SolutionValue()` - suma hodnot proměnných

Obě metody by měly dát stejný výsledek. V případě nesrovnalostí použít ruční sumu jako referenci.

---

## Závěr

Part 2 je **významně složitější** než Part 1:
- Místo GF(2) (binary) → ILP (integer programming)
- Místo max 1× stisknutí → N× stisknutí
- Očekávaný výsledek: **15,000-30,000+ stisknutí** (vs. Part 1: ~400-800)

**Doporučený přístup:**
1. Implementuj **Google OR-Tools SCIP solver** pro ILP
2. Parsuj vstup rozdělením na mezery
3. Definuj proměnné, omezení a cílovou funkci
4. Verify každé řešení (status = OPTIMAL)

**Klíčové poznatky:**
1. Problém je ILP s binární maticí koeficientů
2. OR-Tools SCIP solver poskytuje optimální řešení
3. Verifikace OPTIMAL statusu je kritická
4. Performance by měla být < 5 sekund pro všech 187 strojů

**Očekávaný výsledek:** Minimální celkový počet stisknutí pro všech 187 strojů
