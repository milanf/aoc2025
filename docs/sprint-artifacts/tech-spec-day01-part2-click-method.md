# Tech-Spec: Day 01 Part 2 - CLICK Method (0x434C49434B)

**Created:** 2025-12-02  
**Status:** ✅ Completed  
**AoC Link:** https://adventofcode.com/2025/day/1

---

## Overview

### Problem Statement

Po zadání hesla z Part 1 se dveře neotevřou. Nový bezpečnostní dokument říká: **"Use password method 0x434C49434B"** - což znamená počítat VŠECHNY průchody nulou, včetně těch **během rotace**, ne jen na konci.

**Klíčové změny oproti Part 1:**
- ✅ Part 1: Počítá pouze finální pozice == 0 po rotaci
- 🆕 Part 2: Počítá KAŽDÝ průchod nulou během rotace (včetně finálních)

**Example z AoC (stejné instrukce jako Part 1):**
```
Start: 50

L68  -> 50 - 68 = 82 (přes 0 dolů: 50→49→...→1→0→99→...→82) ✓ 1x průchod
L30  -> 82 - 30 = 52 (82→81→...→52, žádný průchod 0)
R48  -> 52 + 48 = 0 (52→53→...→99→0) ✓ 1x průchod
L5   -> 0 - 5 = 95 (0→99→...→95, žádný průchod 0 - start už je 0)
R60  -> 95 + 60 = 55 (95→96→...→99→0→1→...→55) ✓ 1x průchod
L55  -> 55 - 55 = 0 (55→54→...→1→0) ✓ 1x průchod
L1   -> 0 - 1 = 99 (0→99, žádný průchod 0 - start už je 0)
L99  -> 99 - 99 = 0 (99→98→...→1→0) ✓ 1x průchod
R14  -> 0 + 14 = 14 (0→1→...→14, žádný průchod 0 - start už je 0)
L82  -> 14 - 82 = 32 (14→13→...→1→0→99→...→32) ✓ 1x průchod

Celkem: 6 průchodů nulou (3 konečné pozice + 3 během rotace)
```

**Důležité edge cases:**
- Pokud start pozice == 0, neplatí jako průchod (už tam jsme)
- Pokud end pozice == 0, PLATÍ jako průchod (dorazili jsme tam)
- Rotace `R1000` z pozice 50 → projde nulou 10x (každých 100 kliknutí = 1 průchod)

### Solution

Modifikace Part 1 algoritmu - místo kontroly finální pozice počítat, kolikrát během rotace dial projde přes 0.

**Algoritmus:**
1. Pro každou rotaci spočítat, kolikrát se během ní dostaneme přes 0
2. **Klíčový vzorec:**
   - Počet průchodů = `|distance| / 100` (celočíselné dělení)
   - + 1 pokud rotace končí na 0 a ještě nebyla započítána

**Matematika:**
- Rotace L68 z 50 → projde 50, 49, ..., 1, **0**, 99, ..., 82
  - Start 50, End 82, Distance 68
  - Přejde hranici 0/99 → 1 průchod
- Rotace R1000 z 50 → 10 průchodů přes 0
  - 1000 / 100 = 10 průchodů
  
**Obecný vzorec pro počítání průchodů:**
```csharp
int CountZeroCrossings(int start, int end, int distance)
{
    // Pro L (doleva): start > end může znamenat průchod 0
    // Pro R (doprava): start < end může znamenat průchod 0
    
    // Jednodušší: počítat kolikrát přešla hranice 0/99
    if (distance >= 100)
    {
        // Velké rotace - více průchodů
        return distance / 100 + (end == 0 ? 1 : 0);
    }
    else
    {
        // Malé rotace - max 1 průchod
        if (direction == 'L')
            return start < distance ? 1 : (end == 0 ? 1 : 0);
        else // 'R'
            return (start + distance >= 100) ? 1 : 0;
    }
}
```

### Scope

**In Scope (Part 2):**
- ✅ Modifikace rotační logiky pro počítání průchodů během rotace
- ✅ Ošetření edge case: rotace začínající na 0
- ✅ Ošetření edge case: velké rotace (> 100)
- ✅ Unit test s example inputem (expected: `"6"`)

**Out of Scope:**
- ❌ Optimalizace (není potřeba, algoritmus je stále O(n))
- ❌ Vizualizace rotací

---

## Context for Development

### Codebase Patterns

**Soubory k modifikaci:**
- `Solutions/Day01.cs` - implementovat `SolvePart2(string input)`
- `AoC2025.Tests/Day01Tests.cs` - update Part2 expected result na `"6"`

**Přístup:**
- Sdílet `ParseInstruction()` mezi Part1 a Part2
- Vytvořit novou metodu `CountZeroCrossings(int start, char dir, int dist)`
- `SolvePart2()` bude podobné Part1, ale použije novou logiku počítání

### Files to Reference

| File | Purpose |
|------|---------|
| `Solutions/Day01.cs` | Modifikovat `SolvePart2()` a přidat helper metody |
| `AoC2025.Tests/Day01Tests.cs` | Update Part2 expected result: `"6"` |

### Technical Decisions

**1. Počítání průchodů během rotace:**

**Varianta A - Explicitní simulace (jednodušší, ale pomalejší):**
```csharp
private static int CountZeroCrossings(int start, char dir, int dist)
{
    int count = 0;
    int pos = start;
    int step = dir == 'L' ? -1 : 1;
    
    for (int i = 0; i < dist; i++)
    {
        pos = ((pos + step) % 100 + 100) % 100;
        if (pos == 0) count++;
    }
    return count;
}
```
✅ Jasná logika  
✅ Funguje pro všechny edge cases  
❌ O(distance) - může být pomalé pro velké rotace  

**Varianta B - Matematický výpočet (rychlejší, ale komplikovanější):**
```csharp
private static int CountZeroCrossings(int start, char dir, int dist)
{
    int end = RotateDial(start, dir, dist);
    
    if (dir == 'L')
    {
        // Doleva: počet průchodů = kolikrát přejdeme přes 0 dolů
        if (start >= dist)
            return 0; // Nepřekročíme 0
        else
            return (dist - start - 1) / 100 + 1;
    }
    else // 'R'
    {
        // Doprava: počet průchodů = kolikrát přejdeme přes 99→0
        int sum = start + dist;
        return sum / 100;
    }
}
```
✅ O(1) výkon  
❌ Složitější debug  
❌ Více edge cases k ošetření  

**Doporučení:** Použít **Variantu A** (explicitní simulace) - AoC inputy mají max distance ~100, taktakže výkon není problém a kód je mnohem čitelnější.

**2. Integrace s Part 1:**
```csharp
public string SolvePart2(string input)
{
    int position = 50;
    int zeroCount = 0;
    var lines = input.Split('\n');
    
    foreach (var rawLine in lines)
    {
        var instr = ParseInstruction(rawLine);
        if (instr == null) continue;
        
        // Počítat průchody během rotace
        zeroCount += CountZeroCrossings(position, instr.Value.direction, instr.Value.distance);
        
        // Update pozice
        position = RotateDial(position, instr.Value.direction, instr.Value.distance);
    }
    
    return zeroCount.ToString();
}
```

---

## Implementation Plan

### Tasks

- [ ] **Task 1:** Implementovat `CountZeroCrossings(int start, char dir, int dist)` helper metodu
  - Použít explicitní simulaci (krok po kroku)
  - Pro každý krok kontrolovat `pos == 0`
  - Return celkový počet průchodů

- [ ] **Task 2:** Implementovat `SolvePart2(string input)`
  - Kopírovat strukturu z Part1
  - Nahradit `if (position == 0) zeroCount++` za `zeroCount += CountZeroCrossings(...)`
  - Stále používat `RotateDial()` pro update pozice

- [ ] **Task 3:** Aktualizovat `Day01Tests.cs`
  - Part2 expected result: `"6"`

- [ ] **Task 4:** Testovat edge cases manuálně:
  - Velká rotace (např. R1000)
  - Rotace začínající na 0
  - Rotace končící na 0

### Acceptance Criteria

- [ ] **AC1:** Given example input z AoC  
       When `SolvePart2()` is called  
       Then result is `"6"`

- [ ] **AC2:** Given edge case: start pozice je 0, rotace L5  
       When `CountZeroCrossings(0, 'L', 5)` is called  
       Then result je 0 (neplatí průchod z 0→99, protože už jsme na 0)

- [ ] **AC3:** Given edge case: start pozice je 0, rotace R5  
       When rotace probíhá  
       Then žádný průchod 0 (jdeme 0→1→...→5)

- [ ] **AC4:** Given velká rotace: start 50, R1000  
       When `CountZeroCrossings(50, 'R', 1000)` is called  
       Then result je 10 (každých 100 kliknutí = 1 průchod)

- [ ] **AC5:** Given reálný input z `day01.txt`  
       When `SolvePart2()` is executed  
       Then result je validní číslo > 3 (musí být větší než Part 1)

---

## Additional Context

### Dependencies

**Žádné nové dependencies** - používáme stejné jako Part 1.

### Testing Strategy

**Unit Tests:**
1. Example input test → expected `"6"`
2. Edge case testy:
   - `CountZeroCrossings(50, 'L', 68)` → 1
   - `CountZeroCrossings(0, 'L', 5)` → 0 (start na 0)
   - `CountZeroCrossings(50, 'R', 1000)` → 10 (velká rotace)
   - `CountZeroCrossings(55, 'L', 55)` → 1 (končí na 0)

**Manual Testing:**
- Spustit solution na reálný input
- Očekávaný výsledek > Part 1 result (musí být alespoň 3)
- Submit na AoC pro validaci

### Notes

**Časté chyby k zamezení:**
- ⚠️ Počítat průchod, když start == 0 (neplatí! Už tam jsme)
- ⚠️ Nezapočítat finální pozici == 0 (platí! Dorazili jsme tam)
- ⚠️ Špatná logika pro velké rotace (musíme dělit vzdálenost 100)
- ⚠️ Zapomenout update position po počítání průchodů

**Vztah k Part 1:**
- Part 1 je subset Part 2 (počítá jen finální průchody)
- Part 2 by měl vždy dát result >= Part 1 result
- Pro example: Part 1 = 3, Part 2 = 6 (2x více)

**AoC Specific:**
- Part 2 je již odhalena, můžeme implementovat
- Input je stejný jako Part 1
- Expected result pro reálný input zjistíme při submitu

---

## Ready for Development

Tato specifikace obsahuje vše potřebné pro autonomní implementaci:
✅ Kompletní pochopení rozdílu oproti Part 1  
✅ Example s detailním rozborem všech průchodů  
✅ Edge cases identifikovány a ošetřeny  
✅ Code patterns definovány  
✅ Acceptance criteria jasná  

**Next Step:** Implementace pomocí `*quick-dev` workflow nebo ruční dev podle této spec.
