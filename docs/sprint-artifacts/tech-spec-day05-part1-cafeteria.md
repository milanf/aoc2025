# Tech-Spec: Day 05 Part 1 - Cafeteria

**Created:** 2025-12-05  
**Status:** ✅ Completed  
**AoC Link:** https://adventofcode.com/2025/day/5

---

## Overview

### Problem Statement

V kuchyni byl instalován nový systém pro správu inventáře, ale elfové nyní nedokážou rozpoznat, které ingredience jsou čerstvé a které prošlé. Databáze obsahuje rozsahy ID čerstvých ingrediencí a seznam dostupných ID ingrediencí.

**Klíčové body:**
- Databáze má dvě sekce oddělené prázdným řádkem
- První sekce: **rozsahy ID čerstvých ingrediencí** (formát: `start-end`, včetně obou mezí)
- Druhá sekce: **seznam dostupných ID ingrediencí** (po jednom na řádek)
- Rozsahy se **mohou překrývat** - ingredience je čerstvá, pokud spadá do **jakéhokoliv** rozsahu
- **Cíl: spočítat kolik z dostupných ingrediencí je čerstvých**

**Example z AoC:**
```
3-5
10-14
16-20
12-18

1
5
8
11
17
32
```

Výsledek: **3 čerstvé ingredience** (5, 11, 17)
- ID 1: prošlé (nespadá do žádného rozsahu)
- ID 5: čerstvé (spadá do 3-5)
- ID 8: prošlé
- ID 11: čerstvé (spadá do 10-14)
- ID 17: čerstvé (spadá do 16-20 i 12-18)
- ID 32: prošlé

### Input Analysis

**Reálný input (`Inputs/day05.txt`):**
- **182 rozsahů** ID čerstvých ingrediencí
- **1000 dostupných ID ingrediencí** k ověření
- Rozsahy jsou **velmi velké čísla** (řádově 10¹⁴-10¹⁵)
  - Např: `169486974574545-170251643963353`
  - Rozdíl v rozsahu: ~765 miliard
- Některé rozsahy jsou jednobodové (start == end)
  - Např: `230409669398989-230409669398989`

**Porovnání s example:**
- Example: 4 rozsahy, 6 ID k ověření
- Reálný vstup: 182 rozsahů, 1000 ID k ověření → **výrazně větší!**

**Důsledky pro algoritmus:**
- ❌ **Nemůžeme expandovat rozsahy** do HashSet/Array (některé rozsahy mají stovky miliard hodnot!)
- ✅ **Musíme použít interval checking** - pro každé ID kontrolovat, zda spadá do nějakého rozsahu
- ✅ Časová složitost: O(n × m) kde n = počet ID (1000), m = počet rozsahů (182)
  - 1000 × 182 = 182 000 operací (velmi efektivní)
- ✅ Možná optimalizace: **merge překrývajících se rozsahů** pro rychlejší lookup
  - Ale s 182 rozsahy není nutné

### Solution

**Algoritmus:**

1. **Parse vstup:**
   - Rozdělení na dvě sekce podle prázdného řádku
   - První sekce: parse rozsahy (formát `start-end`)
   - Druhá sekce: parse jednotlivá ID

2. **Reprezentace rozsahů:**
   ```csharp
   record Range(long Start, long End);
   ```

3. **Pro každé dostupné ID:**
   - Zkontroluj, zda spadá do alespoň jednoho rozsahu
   - `id >= range.Start && id <= range.End`
   - Pokud ANO → počitadlo++

4. **Vrať celkový počet čerstvých ingrediencí**

**Optimalizace (volitelná):**
- **Merge překrývajících se rozsahů** před kontrolou:
  1. Seřaď rozsahy podle `Start`
  2. Slouč sousední/překrývající se rozsahy
  3. Výsledek: menší počet rozsahů k prohledání
  
  Např: `[3-5, 10-14, 12-18, 16-20]` → `[3-5, 10-20]`

**Edge cases:**
- ✅ **Jednobodové rozsahy:** kde start == end (správně pokryto podmínkou `>=` a `<=`)
- ✅ **Překrývající se rozsahy:** ID může spadat do více rozsahů (počítáme jen jednou)
- ✅ **Velké hodnoty:** použít `long` (64-bit) místo `int`
- ✅ **Duplicitní ID v seznamu:** každé ID zpracovat samostatně (podle zadání by neměly být, ale nepředpokládáme)

**Časová složitost:**
- Bez optimalizace: O(n × m) = O(1000 × 182) = O(182 000) - triviální
- S merge rozsahů: O(m log m + n × k) kde k < m - stále velmi rychlé

**Prostorová složitost:**
- O(m + n) pro uložení rozsahů a ID

### Scope

**In Scope (Part 1):**
- ✅ Parsing vstupu (rozsahy a ID)
- ✅ Reprezentace rozsahů jako struktury/records
- ✅ Kontrola, zda ID spadá do rozsahu
- ✅ Počítání čerstvých ingrediencí
- ✅ Použití `long` pro velké hodnoty
- ✅ Ošetření edge cases (jednobodové rozsahy, překryvy)

**Out of Scope (Part 1):**
- ❌ Merge překrývajících se rozsahů (není nutné pro tento rozsah dat)
- ❌ Binární vyhledávání (O(n × m) je dostatečně rychlé)
- ❌ Složité datové struktury (interval trees apod.)

**Nice to Have:**
- 💡 Implementace merge rozsahů jako příprava na Part 2
- 💡 Unit testy s příkladem z AoC
- 💡 Validace vstupu (správný formát rozsahů)

---

## Implementation Plan

### 1. Data Structures

```csharp
public record Range(long Start, long End);

// Případně s helper metodou
public record Range(long Start, long End)
{
    public bool Contains(long value) => value >= Start && value <= End;
}
```

### 2. Parsing

```csharp
// Rozdělit vstup na dvě sekce
var sections = input.Split("\n\n");

// Parse ranges
var ranges = sections[0]
    .Split('\n')
    .Select(line => {
        var parts = line.Split('-');
        return new Range(long.Parse(parts[0]), long.Parse(parts[1]));
    })
    .ToList();

// Parse IDs
var ids = sections[1]
    .Split('\n')
    .Select(long.Parse)
    .ToList();
```

### 3. Main Logic

```csharp
int freshCount = 0;

foreach (var id in ids)
{
    bool isFresh = ranges.Any(range => id >= range.Start && id <= range.End);
    if (isFresh)
        freshCount++;
}

return freshCount;
```

### 4. Testing Strategy

**Test s example:**
```
Input:
3-5
10-14
16-20
12-18

1
5
8
11
17
32

Expected: 3
```

**Edge cases testy:**
- Jednobodový rozsah: `5-5` obsahuje pouze 5
- Překrývající se rozsahy: `10-20` a `15-25` správně pokrývají 10-25
- ID na hranicích: `3` a `5` v rozsahu `3-5` jsou oba čerstvé
- Velmi velká čísla: testovat s reálnými hodnotami z inputu

---

## Technical Decisions

### Proč ne HashSet expansion?
- Některé rozsahy mají **stovky miliard** hodnot
- Expandování by vyžadovalo gigabajty paměti
- Interval checking je O(m) operací vs O(1) lookup, ale s m=182 je to zanedbatelné

### Proč ne merge rozsahů v Part 1?
- S 182 rozsahy je přímá kontrola dostatečně rychlá
- Merge přidává komplexitu bez výrazného přínosu
- Implementujeme jednoduše, optimalizujeme až když je to nutné (Part 2)

### Použití `long` vs `int`
- Reálné hodnoty přesahují `int.MaxValue` (2.1 × 10⁹)
- Hodnoty v inputu jsou řádově 10¹⁴-10¹⁵
- **Musíme použít `long`** (max 9.2 × 10¹⁸)

---

## Acceptance Criteria

1. ✅ Správně sparsovat vstup na rozsahy a ID
2. ✅ Vrátit počet čerstvých ingrediencí
3. ✅ Example z AoC vrací **3**
4. ✅ Reálný input vrací správný výsledek
5. ✅ Edge cases jsou ošetřeny
6. ✅ Výkon: vyřešeno < 100ms

---

## Dependencies

- .NET 10.0
- Žádné externí knihovny

## Risks & Mitigations

| Risk | Mitigation |
|------|------------|
| Přetečení při parsování velkých čísel | Použít `long` místo `int` |
| Špatné parsování formátu `start-end` | Validace před parsováním |
| Pomalý výkon s velkým počtem rozsahů | Current approach je O(n×m), pro dané velikosti dostatečně rychlé |
| Part 2 může vyžadovat optimalizaci | Připravit kód tak, aby bylo snadné přidat merge rozsahů |

---

## Notes

- Part 2 pravděpodobně přidá další požadavky (např. operace nad rozsahy)
- Doporučuji implementovat `Range.Contains()` helper metodu pro čitelnější kód
- Zvážit vytvoření helper metody pro merge rozsahů jako příprava na Part 2
