# Tech-Spec: Day 09 Part 1 - Movie Theater

**Created:** 2025-12-09  
**Status:** ✅ **Completed**  
**AoC Link:** https://adventofcode.com/2025/day/9

---

## Overview

### Problem Statement

Ocitáte se v severním pólu v kinosále s velkým dlážděným podlahou zajímavého vzoru. Elfové pokládají nové dlaždice a chcou najít **největší obdélník**, který má červené dlaždice v **dvou protilehlých rozích**.

**Klíčové body:**
- Máte seznam souřadnic všech **červených dlaždic** v mřížce (vstupní data)
- Můžete si vybrat **libovolné dva body (červené dlaždice) jako opačné rohy obdélníku**
- Cílem je najít **největší plochu obdélníku** mezi všemi možnými kombinacemi
- Plocha obdélníku mezi body $(x_1, y_1)$ a $(x_2, y_2)$ je: $(|x_2 - x_1| + 1) \times (|y_2 - y_1| + 1)$

**Příklad z AoC:**
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

Největší obdélník má plochu **50** (např. mezi body (2,5) a (11,1)).

### Input Analysis

**Reálný input (`Inputs/day09.txt`):**
- **496 červených dlaždic** (496 řádků)
- Každý řádek obsahuje 2 čísla: `X,Y` (celočíselné souřadnice)
- **Rozsah X souřadnic:** 1 až ~98,000 (analýza: 97,554 až 98,014 jsou nejčastější)
- **Rozsah Y souřadnic:** 1 až ~98,000 (analýza: podobný rozsah)

**Vzorový vstup (prvních řádků):**
```
97554,50097
97554,51315
98014,51315
98014,52516
...
```

**Porovnání s příkladem:**
- Příklad: **8 červených dlaždic** → maximální plocha **50**
- Reálný vstup: **496 červených dlaždic** → očekávaná plocha ~4.7 miliard

**Charakteristika distribuce souřadnic:**
- Data vykazují **specifické clustery** (nejčastěji kolem souřadnic 1-50k a 90k-98k)
- Nejsou rovnoměrně rozložena
- Existují **malé a velké mezery** v souřadnicích
- ⚠️ To znamená, že největší plocha **bude tvořena body v krajních pozicích**

### Algorithm Analysis

#### Naivní přístup (Brute Force)
```
for each point i from 0 to n-1:
    for each point j from i+1 to n-1:
        area = |x[i] - x[j]| × |y[i] - y[j]|
        max_area = max(max_area, area)
```

**Časová složitost:** $O(n^2)$ kde n = 496 → **~246,000 operací** ✅ **PŘIJATELNÉ**

**Prostorová složitost:** $O(n)$ pro uložení souřadnic

#### Analýza optimálnosti:
- S 496 body máme $\binom{496}{2} = 122,760$ kombinací
- Bez další optimalizace nemůžeme dělat lépe než $O(n^2)$
- Neexistuje matematická vlastnost, která by nám umožnila přeskočit některé páry (narozdíl od např. konvexního obalu)
- **✅ Brute force přístup je v pořádku pro tuto velikost vstupu**

#### Efektivnější přístup (pokud by byl vstup větší):
1. **Seřazení bodů** podle X-souřadnice: $O(n \log n)$
2. **Procházení** pouze nadějných párů (pruning): Možné, ale komplikované
3. **Rotating Calipers:** Algoritmus pro maximální vzdálenost mezi body, ale zde hledáme plochu obdélníku, ne vzdálenost

**Závěr:** Pro 496 bodů je $O(n^2)$ zcela přijatelné řešení.

---

## Requirements

### Functional Requirements

1. **RF1: Parsování vstupu**
   - Načíst seznam souřadnic z textového souboru
   - Formát: `X,Y` na každém řádku
   - Převést na celočíselné souřadnice

2. **RF2: Výpočet všech možných ploch**
   - Pro každou kombinaci dvou bodů spočítat plochu obdélníku
   - Plocha = $|x_1 - x_2| \times |y_1 - y_2|$
   - Sledovat maximální hodnotu

3. **RF3: Výstup výsledku**
   - Vrátit maximální plochu jako celočíselnou hodnotu

### Non-Functional Requirements

1. **NFR1: Výkon**
   - Řešení musí běžet v < 1 sekundě pro vstup s 496 body

2. **NFR2: Přesnost**
   - Použít celočíselné aritmetiku (bez zaokrouhlování)
   - Výsledek bez chyb zaokrouhlování

3. **NFR3: Paměť**
   - Maximálně O(n) paměti pro uložení bodů

---

## Edge Cases

1. **Malý počet bodů**
   - Vstup s pouze 2 body → jediná možná plocha
   - Vstup s 1 bodem → plocha 0

2. **Shodné souřadnice**
   - Body se stejnými X nebo Y souřadnicemi → plocha 0
   - Úplně shodné body → plocha 0

3. **Velmi velké souřadnice**
   - X, Y až ~100,000 → plocha až 10,000,000,000 (10⁹)
   - Potřeba `long` (64-bitového celého čísla) místo `int` (32-bitového)

4. **Body v jedné linii**
   - Všechny body na jedné X-ové nebo Y-ové souřadnici → maximální plocha 0
   - Příklad: (5,1), (5,2), (5,3) → všechny plochy 0

---

## Test Cases

### Test 1: Příklad z AoC
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

**Očekávaný výstup:** `50`

**Vysvětlení:** Největší obdélník je mezi body (2,5) a (11,1):
- Šířka: |11 - 2| + 1 = 10 (sloupce 2 až 11 včetně)
- Výška: |5 - 1| + 1 = 5 (řádky 1 až 5 včetně)
- **Plocha = 10 × 5 = 50**

Klíčová změna: Vzorec pro plochu je **(|x2-x1| + 1) × (|y2-y1| + 1)**, ne právě součin vzdáleností!

### Test 2: Dva body
**Vstup:**
```
0,0
10,5
```

**Očekávaný výstup:** `50`

**Vysvětlení:** Jediný obdélník má plochu |10-0| × |5-0| = 10 × 5 = 50

### Test 3: Body se stejnou souřadnicí X
**Vstup:**
```
5,0
5,10
5,20
```

**Očekávaný výstup:** `0`

**Vysvětlení:** Všechny body mají stejné X → všechny obdélníky mají šířku 0 → plocha 0

### Test 4: Body se stejnou souřadnicí Y
**Vstup:**
```
0,5
10,5
20,5
```

**Očekávaný výstup:** `0`

**Vysvětlení:** Všechny body mají stejné Y → všechny obdélníky mají výšku 0 → plocha 0

### Test 5: Větší dataset
**Vstup:** Prvních 10 bodů z `day09.txt`

**Očekávaný výstup:** Reálný input (496 bodů) vrací **4,763,040,296**

---

## Implementation Strategy

### Algoritmus
1. Parsovat vstupní soubor a načíst všechny souřadnice do seznamu
2. Pro každou kombinaci dvou bodů (i < j):
   - Spočítat plochu: `area = |x[i] - x[j]| × |y[i] - y[j]|`
   - Aktualizovat maximální plochu
3. Vrátit maximální plochu

### Pseudokód
```
max_area = 0
for i = 0 to n-1:
    for j = i+1 to n-1:
        width = abs(points[i].x - points[j].x) + 1
        height = abs(points[i].y - points[j].y) + 1
        area = width × height
        max_area = max(max_area, area)
return max_area
```

### Datové struktury
- **Pole bodů:** `Point[] points` nebo `List<Point>`
- **Každý bod:** X a Y souřadnice (`long` pro bezpečnost proti přetečení)

### Dělení úloh
1. **ParseInput()** - Parsování vstupního souboru
2. **CalculateMaxArea(points)** - Výpočet maximální plochy
3. **Main()** - Orchestrace a výstup

---

## Complexity Analysis

### Časová složitost
- Parsování: $O(n)$
- Výpočet: $O(n^2)$ - každý pár bodů
- Celkem: $O(n^2)$ kde n = 496
- **Odhadovaný čas:** < 100ms

### Prostorová složitost
- Uložení bodů: $O(n)$
- Pomocné proměnné: $O(1)$
- Celkem: $O(n)$

---

## Success Criteria

1. ✅ Program správně parsuje vstupní soubor
2. ✅ Program korektně spočítá maximální plochu pro příklad
3. ✅ Program vrátí správnou odpověď pro `day09.txt`
4. ✅ Program běží v < 1 sekundě
5. ✅ Test pokrývají edge cases (rovnoběžné čáry, dva body, apod.)
