# Tech-Spec: Day 05 Part 2 - Cafeteria

**Created:** 2025-12-05  
**Status:** ✅ Completed  
**AoC Link:** https://adventofcode.com/2025/day/5

---

## Overview

### Problem Statement

V Part 2 už elfové neřeší konkrétní dostupné ingredience - místo toho chtějí znát **celkový počet unikátních ID**, která jsou považována za čerstvá podle všech rozsahů čerstvých ingrediencí v databázi.

**Klíčová změna oproti Part 1:**
- Part 1: Kontrolovali jsme, **které z dostupných ID** jsou čerstvé (1000 konkrétních ID)
- Part 2: Hledáme **všechna unikátní ID**, která pokrývají rozsahy čerstvých ingrediencí

**Zadání Part 2:**
- Druhá sekce databáze (seznam dostupných ID) je **nyní irelevantní**
- Používáme pouze **první sekci** - rozsahy ID čerstvých ingrediencí
- Ingredience je čerstvá, pokud její ID spadá do **jakéhokoliv** rozsahu
- **Cíl: spočítat celkový počet unikátních ID**, která jsou považována za čerstvá

**Example z AoC:**
```
3-5
10-14
16-20
12-18
```

Čerstvá ID: **3, 4, 5, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20**  
Celkem: **14 čerstvých ingrediencí**

**Analýza example:**
- `3-5`: pokrývá 3, 4, 5 → **3 ID**
- `10-14`: pokrývá 10, 11, 12, 13, 14 → **5 ID**
- `16-20`: pokrývá 16, 17, 18, 19, 20 → **5 ID**
- `12-18`: pokrývá 12, 13, 14, 15, 16, 17, 18 → **7 ID**

**Problém překryvů:**
- Rozsahy `10-14` a `12-18` se **překrývají** (12, 13, 14)
- Rozsahy `16-20` a `12-18` se **překrývají** (16, 17, 18)
- Musíme počítat každé ID **pouze jednou**!

**Naivní součet:** 3 + 5 + 5 + 7 = **20** ❌  
**Správný výsledek s merge:** **14** ✅

### Input Analysis

**Reálný input (`Inputs/day05.txt`):**
- **182 rozsahů** ID čerstvých ingrediencí
- ~~1000 dostupných ID~~ (ignorujeme v Part 2)
- Rozsahy jsou **velmi velké čísla** (řádově 10¹²-10¹⁴)
  - Minimální start: `1,619,411,001,860` (~1.6 bilionů)
  - Maximální konec: `559,686,803,138,542` (~560 bilionů)
  - Celkové teoretické rozpětí: **~558 bilionů**

**Rozsah hodnot v rozsazích:**
- Součet všech rozsahů (s překryvy): **~452 bilionů**
- Některé rozsahy jsou velmi velké:
  - Např: `169486974574545-170251643963353` → rozpětí ~765 miliard
- Některé rozsahy jsou jednobodové:
  - Např: `230409669398989-230409669398989` → jen 1 ID

**Porovnání s example:**
- Example: 4 rozsahy, výsledek = 14
- Reálný vstup: 182 rozsahů, výsledek = ???

**Důsledky pro algoritmus:**
- ❌ **NELZE expandovat rozsahy do pole/HashSet**
  - Některé rozsahy mají stovky miliard hodnot!
  - Paměťová složitost by byla O(10¹⁴) → nemožné
- ❌ **NELZE iterovat přes všechny možné hodnoty**
  - Časová složitost O(558 bilionů) → nemožné
- ✅ **MUSÍME použít INTERVAL MERGING**
  - Sloučíme překrývající se rozsahy
  - Spočítáme délky sloučených rozsahů
  - Časová složitost: O(n log n) kde n = 182

### Solution

**Klíčový algoritmus: Interval Merging**

Tento problém je klasický příklad **merge intervals** algoritmu:

1. **Seřadit rozsahy** podle počátku (Start)
2. **Sloučit překrývající se/sousedící rozsahy**
3. **Spočítat součet délek** sloučených rozsahů

**Kdy sloučit dva rozsahy?**

Dva rozsahy `[a.Start, a.End]` a `[b.Start, b.End]` lze sloučit, pokud:
- `b.Start <= a.End + 1`

Po seřazení (víme že `b.Start >= a.Start`):
- Pokud `b.Start <= a.End + 1` → sloučit do `[a.Start, max(a.End, b.End)]`
- Jinak → začít nový rozsah

**Příklad merge:**
```
Vstup (seřazeno):
[3-5], [10-14], [12-18], [16-20]

Krok 1: [3-5] → merged = [[3-5]]
Krok 2: [10-14] → 10 > 5+1 → nový rozsah → merged = [[3-5], [10-14]]
Krok 3: [12-18] → 12 <= 14+1 → sloučit → merged = [[3-5], [10-18]]
Krok 4: [16-20] → 16 <= 18+1 → sloučit → merged = [[3-5], [10-20]]

Výsledné rozsahy: [3-5], [10-20]
Počet ID: (5-3+1) + (20-10+1) = 3 + 11 = 14 ✅
```

**Algoritmus krok za krokem:**

```csharp
// 1. Parse a seřaď rozsahy
List<Range> ranges = ParseRanges(input).OrderBy(r => r.Start).ToList();

// 2. Merge překrývající se rozsahy
List<Range> merged = new();
Range current = ranges[0];

foreach (var next in ranges.Skip(1))
{
    if (next.Start <= current.End + 1)
    {
        // Sloučit - rozšířit current
        current = new Range(current.Start, Math.Max(current.End, next.End));
    }
    else
    {
        // Nový rozsah - uložit current a začít nový
        merged.Add(current);
        current = next;
    }
}
merged.Add(current); // Nezapomenout přidat poslední

// 3. Spočítat celkový počet ID
long totalCount = merged.Sum(r => r.End - r.Start + 1);
```

**Edge cases:**

1. **Jednobodové rozsahy** (Start == End)
   - Např: `[5-5]` → délka = 5 - 5 + 1 = **1** ✅
   
2. **Přesně sousedící rozsahy** (End + 1 == Next.Start)
   - Např: `[3-5], [6-10]` → sloučit do `[3-10]`
   - Podmínka: `6 <= 5+1` → **ANO** ✅

3. **Překrývající se rozsahy**
   - Např: `[10-14], [12-18]` → sloučit do `[10-18]`
   - Podmínka: `12 <= 14+1` → **ANO** ✅

4. **Rozsah uvnitř jiného rozsahu**
   - Např: `[10-20], [12-15]` → sloučit do `[10-20]`
   - `max(20, 15) = 20` ✅

5. **Duplicitní rozsahy**
   - Např: `[5-10], [5-10]` → sloučit do `[5-10]`
   - Ošetřeno algoritmem ✅

**Časová složitost:**
- Parsing: O(n) kde n = 182 rozsahů
- Řazení: O(n log n) = O(182 × log 182) ≈ O(1400)
- Merge: O(n) = O(182)
- Součet: O(m) kde m = počet sloučených rozsahů (m ≤ n)
- **Celkem: O(n log n) = O(~1400) operací** → velmi efektivní!

**Prostorová složitost:**
- Uložení rozsahů: O(n) = O(182)
- Sloučené rozsahy: O(m) kde m ≤ n
- **Celkem: O(n)** → minimální paměť

### Scope

**In Scope (Part 2):**
- ✅ Parsing pouze první sekce vstupu (rozsahy)
- ✅ Reprezentace rozsahů jako `record Range(long Start, long End)`
- ✅ Řazení rozsahů podle Start
- ✅ Merge překrývajících se/sousedících rozsahů
- ✅ Počítání celkového počtu čerstvých ID
- ✅ Použití `long` pro velké hodnoty (10¹⁴)
- ✅ Ošetření všech edge cases

**Out of Scope (Part 2):**
- ❌ Druhá sekce vstupu (seznam dostupných ID) - nepoužívá se
- ❌ HashSet/Array pro všechny hodnoty (neefektivní)
- ❌ Složité datové struktury (interval trees)

**Rozdíl oproti Part 1:**
| Aspekt | Part 1 | Part 2 |
|--------|--------|--------|
| Vstup | Rozsahy + konkrétní ID | Pouze rozsahy |
| Výstup | Počet čerstvých z konkrétních ID | Počet všech čerstvých ID |
| Algoritmus | Kontrola členství (O(n × m)) | Interval merging (O(n log n)) |
| Optimalizace | Merge volitelná | Merge **povinná** |

---

## Implementation Plan

### 1. Data Structures

```csharp
// Použijeme stejný record jako v Part 1
public record Range(long Start, long End)
{
    // Helper pro výpočet délky rozsahu
    public long Count => End - Start + 1;
}
```

### 2. Parsing

```csharp
// Parse pouze první sekce (rozsahy), ignorujeme druhou sekci
var sections = input.Split("\n\n");
var rangesSection = sections[0];

var ranges = rangesSection
    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(line => {
        var parts = line.Split('-');
        return new Range(long.Parse(parts[0]), long.Parse(parts[1]));
    })
    .OrderBy(r => r.Start)  // Seřadit hned při parsování
    .ToList();
```

### 3. Interval Merging

```csharp
List<Range> MergeRanges(List<Range> sortedRanges)
{
    if (sortedRanges.Count == 0)
        return new List<Range>();

    var merged = new List<Range>();
    var current = sortedRanges[0];

    foreach (var next in sortedRanges.Skip(1))
    {
        // Kontrola, zda rozsahy lze sloučit
        if (next.Start <= current.End + 1)
        {
            // Sloučit - rozšířit current rozsah
            current = new Range(
                current.Start,
                Math.Max(current.End, next.End)
            );
        }
        else
        {
            // Rozsahy se nepřekrývají - uložit current a začít nový
            merged.Add(current);
            current = next;
        }
    }
    
    // Nezapomenout přidat poslední rozsah
    merged.Add(current);
    
    return merged;
}
```

### 4. Main Logic (Part 2)

```csharp
public long SolvePart2(string input)
{
    // 1. Parse a seřaď rozsahy
    var sections = input.Split("\n\n");
    var ranges = sections[0]
        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
        .Select(line => {
            var parts = line.Split('-');
            return new Range(long.Parse(parts[0]), long.Parse(parts[1]));
        })
        .OrderBy(r => r.Start)
        .ToList();

    // 2. Merge překrývající se rozsahy
    var merged = MergeRanges(ranges);

    // 3. Spočítat celkový počet ID
    long totalCount = merged.Sum(r => r.Count);

    return totalCount;
}
```

### 5. Testing Strategy

**Test s example:**
```csharp
[Fact]
public void Part2_Example_ReturnsCorrectCount()
{
    var input = """
        3-5
        10-14
        16-20
        12-18
        
        """;  // Druhá sekce je prázdná/ignorována

    var result = solution.SolvePart2(input);
    
    Assert.Equal(14, result);
}
```

**Test s edge cases:**
```csharp
[Theory]
[InlineData("5-5", 1)]  // Jednobodový rozsah
[InlineData("1-5\n6-10", 10)]  // Sousedící rozsahy
[InlineData("1-10\n5-15", 15)]  // Překrývající se
[InlineData("1-20\n5-10", 20)]  // Rozsah uvnitř
[InlineData("1-5\n1-5", 5)]  // Duplicitní
public void Part2_EdgeCases(string rangesInput, long expected)
{
    var input = rangesInput + "\n\n";  // Přidat prázdnou druhou sekci
    var result = solution.SolvePart2(input);
    Assert.Equal(expected, result);
}
```

**Test s reálným inputem:**
```csharp
[Fact]
public void Part2_RealInput_ComputesCorrectly()
{
    var input = File.ReadAllText("Inputs/day05.txt");
    var result = solution.SolvePart2(input);
    
    // Výsledek by měl být výrazně menší než součet všech rozsahů (452B)
    Assert.True(result > 0);
    Assert.True(result < 452125362963180L);  // Součet bez merge
}
```

### 6. Optimalizace (volitelné)

**Pro ještě rychlejší řešení:**
- Merge lze provést přímo při parsování (streamovaně)
- Ale s 182 rozsahy není potřeba optimalizovat

**Validace:**
- Kontrola správného formátu rozsahů (Start <= End)
- Ošetření prázdného vstupu

---

## Key Decisions

1. **Proč Interval Merging?**
   - Jediný efektivní způsob pro rozsahy s miliardami hodnot
   - Klasický algoritmus s O(n log n) složitostí
   - Elegantní řešení bez složitých datových struktur

2. **Proč podmínka `<= End + 1`?**
   - Sloučí jak překrývající se (`12-18` + `10-14`)
   - Tak sousedící rozsahy (`1-5` + `6-10`)
   - Dává spojité rozsahy čerstvých ID

3. **Proč `Math.Max(current.End, next.End)`?**
   - Ošetřuje případ, kdy jeden rozsah je uvnitř druhého
   - Např: `[1-20]` + `[5-10]` → `[1-20]` (ne `[1-10]`)

4. **Rozdíl od Part 1:**
   - Part 1: Membership check - O(n × m)
   - Part 2: Interval merging - O(n log n)
   - Part 1 merge byl nice-to-have, Part 2 je povinný

---

## Deliverables

- ✅ Metoda `SolvePart2(string input)` v `Solutions/Day05.cs`
- ✅ Helper metoda `MergeRanges(List<Range> ranges)`
- ✅ Rozšířený `Range` record s property `Count`
- ✅ Unit testy v `AoC2025.Tests/Day05Tests.cs`:
  - Test s example (očekávaný výsledek: 14)
  - Testy edge cases (jednobodové, sousedící, překryvy)
  - Test s reálným inputem
- ✅ Dokumentace algoritmu v komentářích

---

## Notes

- **Kritické:** Použít `long` místo `int` - hodnoty přesahují 10¹⁴
- **Pozor:** Nepočítat ID vícekrát (proto merge!)
- **Efektivita:** O(n log n) je pro 182 rozsahů triviální (~1400 operací)
- **Návaznost:** Merge implementovaný pro Part 2 můžeme použít i v Part 1 jako optimalizaci
