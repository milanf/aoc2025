# Tech-Spec: Day 10 Part 1 - Factory

**Created:** 2025-12-10  
**Status:** ✅ **Completed**  
**AoC Link:** https://adventofcode.com/2025/day/10

---

## Overview

### Problem Statement

V továrně se nacházejí stroje, které je potřeba inicializovat. Každý stroj má:
- **Indicator lights** (indikátorová světla) - jejich cílový stav je zobrazený v diagramu `[.##.]` (`.` = vypnuto, `#` = zapnuto)
- **Buttons** (tlačítka) - každé tlačítko má schéma zapojení `(0,3,4)`, které určuje, která světla toggleuje
- **Joltage requirements** (nepoužívá se v Part 1)

Všechna světla jsou na začátku **vypnutá**. Cílem je najít **minimální počet stisknutí tlačítek**, aby všechny stroje měly správně nastavená světla.

**Klíčové body:**
- Stisknutí tlačítka **toggleuje** světla (zapne vypnuté, vypne zapnuté)
- Každé tlačítko lze stisknout **libovolněkrát** (0, 1, 2, 3, ...)
- Hledáme **celkový součet** minimálních stisknutí pro všechny stroje

**Příklad z AoC:**
```
[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
```
- Cíl: světla [vypnuto, zapnuto, zapnuto, vypnuto]
- Minimální řešení: **2 stisknutí** (např. buttony (0,2) a (0,1) jednou každý)

```
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
```
- Minimální řešení: **3 stisknutí**

```
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
```
- Minimální řešení: **2 stisknutí**

**Celkový součet:** 2 + 3 + 2 = **7**

### Input Analysis

**Reálný input (`Inputs/day10.txt`):**
- **187 strojů** (187 řádků)
- Každý řádek obsahuje:
  - Diagram světel `[.##...]` (délka 4 až 10 světel)
  - Seznam tlačítek `(0,3,4) (1,2) ...` (2 až 13 tlačítek na stroj)
  - Joltage hodnoty `{3,5,4}` (ignorujeme v Part 1)

**Rozložení vstupních dat:**
```
Stroj 1:  [#...##]     6 světel, 6 tlačítek
Stroj 2:  [##.#]       4 světla, 3 tlačítka
Stroj 3:  [.##...]     6 světel, 5 tlačítek
...
Stroj 38: [..##...##.] 10 světel, 12 tlačítek (největší stroj)
```

**Statistika:**
- **Průměrný počet světel:** ~6
- **Průměrný počet tlačítek:** ~7
- **Maximální počet světel:** 10
- **Maximální počet tlačítek:** 13

**Porovnání s příkladem:**
- Příklad: 3 stroje → součet 7
- Reálný vstup: 187 strojů → očekávaný součet ~400-800

### Algorithm Analysis

Tento problém je ekvivalentní **řešení soustavy lineárních rovnic nad Galois Field GF(2)**, kde:
- Každé světlo má stav 0 (vypnuto) nebo 1 (zapnuto)
- Stisknutí tlačítka je XOR operace (toggle)
- Sudý počet stisknutí = 0, lichý počet = 1

#### Klíčové pozorování:
**Stisknout tlačítko 2× je stejné jako ho nestisknout!** Proto stačí určit, která tlačítka stisknout **lichý počet krát** (optimálně 1×).

#### Přístup 1: Greedy Algorithm (Jednodušší, ale ne vždy optimální)
```
1. Pro každé světlo zleva doprava:
   - Pokud není v cílovém stavu, najdi tlačítko, které ho toggleuje
   - Stiskni toto tlačítko
   - Aktualizuj stav všech ovlivněných světel
2. Opakuj, dokud nejsou všechna světla správně
```

**Problém:** Greedy algoritmus **nezaručuje** minimální počet stisknutí!

#### Přístup 2: Gaussian Elimination over GF(2) ✅ **OPTIMÁLNÍ**
Reprezentujeme problém jako matici:
```
Stroj: [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1)

Matice tlačítek (řádek = tlačítko, sloupec = světlo):
       L0 L1 L2 L3
(3)    0  0  0  1
(1,3)  0  1  0  1
(2)    0  0  1  0
(2,3)  0  0  1  1
(0,2)  1  0  1  0
(0,1)  1  1  0  0

Cílový stav: [0, 1, 1, 0]
```

**Algoritmus:**
1. Vytvoř augmentovanou matici `[A | b]` kde A = tlačítka, b = cílový stav
2. Proveď Gaussian elimination nad GF(2):
   - Použij XOR místo odčítání
   - Najdi pivot řádky
   - Eliminuj ostatní řádky
3. Po row-echelon form:
   - Najdi řešení (které tlačítka stisknout)
   - Minimalizuj počet stisknutí (hledej nejméně "1" v řešení)

**Časová složitost:** $O(n \times m^2)$ kde n = počet světel, m = počet tlačítek
- Pro stroj s 10 světly a 13 tlačítky: $O(10 \times 13^2) = O(1690)$
- Pro 187 strojů: $O(187 \times 1690) \approx O(316,000)$ ✅ **PŘIJATELNÉ**

**Prostorová složitost:** $O(n \times m)$ pro matici

#### Přístup 3: Backtracking s Pruning (Brute Force)
- Pro každé tlačítko: stiskni 0× nebo 1×
- Celkem $2^m$ možností (m = počet tlačítek)
- Pro 13 tlačítek: $2^{13} = 8192$ možností → **možné, ale pomalé**

**Závěr:** 
- **✅ Gaussian Elimination** je preferovaný přístup (deterministický, rychlý)
- Backtracking lze použít jako fallback pro edge cases

---

## Requirements

### Functional Requirements

1. **RF1: Parsování vstupu**
   - Načíst seznam strojů z textového souboru
   - Formát každého řádku:
     - Diagram světel: `[.##...]`
     - Tlačítka: `(0,3,4) (1,2) ...`
     - Joltage: `{3,5,4}` (ignorovat)
   - Převést na interní strukturu (pole boolů pro světla, matici pro tlačítka)

2. **RF2: Řešení jednotlivého stroje**
   - Implementovat Gaussian elimination over GF(2)
   - Najít minimální počet stisknutí pro daný stroj
   - Pokud neexistuje řešení, vrátit error (nebo zkusit backtracking)

3. **RF3: Výpočet celkového součtu**
   - Pro každý stroj spočítat minimální stisknutí
   - Sečíst všechny hodnoty
   - Vrátit celkový součet

4. **RF4: Výstup výsledku**
   - Vrátit celkový minimální počet stisknutí jako celé číslo

### Non-Functional Requirements

1. **NFR1: Výkon**
   - Řešení musí běžet v < 1 sekundě pro 187 strojů

2. **NFR2: Přesnost**
   - Použít celočíselnou aritmetiku
   - Výsledek musí být **skutečné minimum**, ne aproximace

3. **NFR3: Paměť**
   - Maximálně O(n × m) paměti pro největší stroj

---

## Edge Cases

1. **Stroj již v cílovém stavu**
   - Všechna světla jsou vypnutá a diagram je `[....]`
   - Výsledek: 0 stisknutí

2. **Nemožné řešení**
   - Některá světla nelze ovlivnit žádným tlačítkem
   - Nebo soustava nemá řešení
   - Příklad: `[#..]` s tlačítky `(1,2)` - světlo 0 nelze zapnout
   - **Podle zadání by toto nemělo nastat** (všechny stroje jsou inicializovatelné)

3. **Redundantní tlačítka**
   - Některá tlačítka dělají totéž (lineárně závislé)
   - Gaussian elimination to automaticky vyřeší

4. **Jedno světlo, jedno tlačítko**
   - Nejjednodušší případ: `[#] (0)` → 1 stisknutí
   - Nebo `[.] (0)` → 0 stisknutí

5. **Velké stroje**
   - 10 světel, 13 tlačítek → matice 13×10
   - Může mít více řešení → vybrat to s minimálním počtem stisknutí

---

## Test Cases

### Test 1: Příklad z AoC - Stroj 1
**Vstup:**
```
[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}
```

**Očekávaný výstup:** 2 stisknutí

**Testovací postup:**
1. Parse: 4 světla [0,1,1,0], 6 tlačítek
2. Gaussian elimination → najdi minimální řešení
3. Verify: stisknutí (0,2) a (0,1) → [0,1,1,0] ✓

---

### Test 2: Příklad z AoC - Stroj 2
**Vstup:**
```
[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}
```

**Očekávaný výstup:** 3 stisknutí

---

### Test 3: Příklad z AoC - Stroj 3
**Vstup:**
```
[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}
```

**Očekávaný výstup:** 2 stisknutí

---

### Test 4: Celkový součet z příkladu AoC
**Vstup:** Všechny 3 stroje z příkladu

**Očekávaný výstup:** 7

---

### Test 5: Triviální stroj
**Vstup:**
```
[....] (0,1,2,3)
```

**Očekávaný výstup:** 0 stisknutí (již v cílovém stavu)

---

### Test 6: Jeden light, jeden button
**Vstup:**
```
[#] (0)
```

**Očekávaný výstup:** 1 stisknutí

---

### Test 7: Reálný input (první stroj)
**Vstup:**
```
[#...##] (0,1,3,4,5) (0,4,5) (1,2,3,4) (0,1,2) {132,30,23,13,121,115}
```

**Očekávaný výstup:** (neznámý, ale platné řešení)

**Testovací postup:**
1. Implementuj algoritmus
2. Spočítej minimální stisknutí pro tento stroj
3. Verify: aplikuj stisknutí na výchozí stav → musí odpovídat diagramu

---

### Test 8: Celý reálný input (187 strojů)
**Vstup:** `Inputs/day10.txt`

**Očekávaný výstup:** (Submit to AoC for verification)

**Performance:** Měřit čas běhu (musí být < 1s)

---

## Implementation Plan

### Story 1: Parsování vstupu
**Příběh:** Jako vývojář potřebuji naparsovat vstupní soubor do struktury dat.

**Akceptační kritéria:**
- Načtení souboru řádek po řádku
- Extrakce diagramu světel `[.##.]`
- Extrakce tlačítek `(0,3,4) (1,2)`
- Ignorování joltage hodnot
- Vytvoření struktury `Machine`:
  ```csharp
  class Machine
  {
      bool[] TargetLights;  // true = #, false = .
      List<int[]> Buttons;  // každé tlačítko = pole indexů světel
  }
  ```

---

### Story 2: Implementace Gaussian Elimination over GF(2)
**Příběh:** Jako vývojář potřebuji implementovat algoritmus pro řešení soustavy rovnic nad GF(2).

**Akceptační kritéria:**
- Vytvoření augmentované matice `[A | b]`
- Implementace row-echelon form pomocí XOR operací
- Back-substitution pro nalezení řešení
- Minimalizace počtu "1" v řešení (hledáme nejméně stisknutí)

**Pseudo-kód:**
```csharp
int SolveMachine(Machine machine)
{
    int n = machine.TargetLights.Length;
    int m = machine.Buttons.Count;
    
    // Vytvoř matici A (m x n) a vektor b (n)
    bool[,] A = new bool[m, n];
    bool[] b = machine.TargetLights;
    
    // Naplň matici
    for (int i = 0; i < m; i++)
        foreach (int light in machine.Buttons[i])
            A[i, light] = true;
    
    // Gaussian elimination
    int[] solution = GaussianEliminationGF2(A, b);
    
    // Spočítej počet stisknutí (počet "1" v solution)
    return solution.Count(x => x == 1);
}
```

---

### Story 3: Řešení všech strojů
**Příběh:** Jako uživatel chci spočítat celkový minimální počet stisknutí pro všechny stroje.

**Akceptační kritéria:**
- Iterace přes všechny stroje
- Spočítání minimálních stisknutí pro každý stroj
- Součet všech výsledků
- Výstup celkového součtu

---

### Story 4: Testování a validace
**Příběh:** Jako vývojář chci ověřit správnost algoritmu.

**Akceptační kritéria:**
- Unit testy pro všechny test cases (1-6)
- Manuální verifikace řešení pro malé stroje
- Integration test pro celý reálný input
- Performance test (měření času běhu)

---

## Poznámky

### Matematické pozadí: GF(2)
Galois Field 2 (GF(2)) je těleso s dvěma prvky {0, 1}, kde:
- Sčítání: 0+0=0, 0+1=1, 1+0=1, 1+1=0 (XOR)
- Násobení: 0×0=0, 0×1=0, 1×0=0, 1×1=1 (AND)

V našem kontextu:
- **Stav světla:** 0 (vypnuto) nebo 1 (zapnuto)
- **Toggle operace:** XOR (stisknutí tlačítka)
- **Lichý počet stisknutí = 1**, sudý = 0

Proto můžeme modelovat problém jako soustavu lineárních rovnic:
$$A \mathbf{x} = \mathbf{b} \pmod{2}$$

Kde:
- $A$ je matice tlačítek (řádek = tlačítko, sloupec = světlo)
- $\mathbf{x}$ je vektor "kolikrát stisknout tlačítko" (mod 2)
- $\mathbf{b}$ je cílový stav světel

### Alternativní přístup: Backtracking
Pro malé stroje (< 15 tlačítek) lze použít backtracking:
```csharp
int MinPresses(Machine machine, int buttonIndex, bool[] currentState)
{
    if (buttonIndex == machine.Buttons.Count)
        return currentState.SequenceEqual(machine.TargetLights) ? 0 : int.MaxValue;
    
    // Option 1: Don't press button
    int skip = MinPresses(machine, buttonIndex + 1, currentState);
    
    // Option 2: Press button once
    bool[] newState = ToggleLights(currentState, machine.Buttons[buttonIndex]);
    int press = 1 + MinPresses(machine, buttonIndex + 1, newState);
    
    return Math.Min(skip, press);
}
```

**Výhoda:** Jednodušší implementace  
**Nevýhoda:** Exponenciální časová složitost $O(2^m)$

---

## Závěr

Tento problém je klasickým příkladem **řešení soustavy lineárních rovnic nad GF(2)**. Gaussian elimination poskytuje deterministické a efektivní řešení s časovou složitostí $O(n \times m^2)$, což je pro náš vstup (187 strojů, max 13 tlačítek) zcela přijatelné.

**Klíčové poznatky:**
1. Toggle operace jsou XOR v GF(2)
2. Sudý počet stisknutí = nestisknout
3. Gaussian elimination najde optimální řešení
4. Backtracking lze použít jako fallback pro malé stroje

**Očekávaný výsledek:** Minimální celkový počet stisknutí pro všech 187 strojů.
