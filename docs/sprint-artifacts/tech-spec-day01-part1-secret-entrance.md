# Tech-Spec: Day 01 Part 1 - Secret Entrance

**Created:** 2025-12-02  
**Status:** ✅ Completed  
**AoC Link:** https://adventofcode.com/2025/day/1

---

## Overview

### Problem Statement

Elves potřebují otevřít trezor u tajného vchodu na Severní pól. Trezor má číselník s hodnotami 0-99, který lze otáčet doleva (L) nebo doprava (R). Instrukce jsou ve formátu `L68` (otočit doleva o 68 kliknutí) nebo `R48` (otočit doprava o 48 kliknutí).

**Klíčové body:**
- Číselník startuje na hodnotě **50**
- Hodnoty jsou 0-99 (cyklické - z 0 doleva jde na 99, z 99 doprava na 0)
- L = doleva (lower numbers), R = doprava (higher numbers)
- **Heslo = počet průchodů hodnotou 0** po jakékoliv rotaci

**Example z AoC:**
```
L68  -> 50 - 68 = -18 -> 82 (přes 0: 50->49->...->0->99->...->82)
L30  -> 82 - 30 = 52
R48  -> 52 + 48 = 100 -> 0 ✓ (přes 100: wrap to 0)
L5   -> 0 - 5 = -5 -> 95
R60  -> 95 + 60 = 155 -> 55 (155 % 100)
L55  -> 55 - 55 = 0 ✓
L1   -> 0 - 1 = -1 -> 99
L99  -> 99 - 99 = 0 ✓
R14  -> 0 + 14 = 14
L82  -> 14 - 82 = -68 -> 32

Výsledek: 3 průchody nulou
```

### Solution

Implementace simulátoru číselníku s modulo aritmetikou pro cyklický rozsah 0-99.

**Algoritmus:**
1. Parse každého řádku: směr (L/R) + distance (int)
2. Simulace rotace s modulem 100 a ošetřením záporných čísel
3. Po každé rotaci kontrola, zda `currentPosition == 0`
4. Return počet průchodů nulou

### Scope

**In Scope (Part 1):**
- ✅ Parsing instrukcí (L/R + číslo)
- ✅ Simulace číselníku 0-99 s cyklickým wrapováním
- ✅ Počítání průchodů nulou
- ✅ Unit test s example inputem

**Out of Scope:**
- ❌ Part 2 (bude řešeno samostatně po odemčení)
- ❌ Optimalizace pro obrovské inputy (není potřeba pro AoC)
- ❌ Vizualizace rotací

---

## Context for Development

### Codebase Patterns

**Struktura projektu:**
- `Solutions/Day01.cs` - implementace řešení (implementuje `ISolution`)
- `Inputs/day01.txt` - reálný input z AoC (4542 řádků instrukcí)
- `AoC2025.Tests/Day01Tests.cs` - xUnit testy
- `AoC2025.Tests/TestData/day01_example.txt` - example input z AoC zadání

**ISolution Interface:**
```csharp
public interface ISolution
{
    int DayNumber { get; }
    string Title { get; }
    string SolvePart1(string input);
    string SolvePart2(string input);
}
```

**Konvence:**
- Parsování: `input.Split('\n')` nebo `input.Split(Environment.NewLine)`
- Return type: vždy `string` (i když výsledek je číslo)
- XML dokumentační komentáře pro public API
- Private pomocné metody pro čitelnost

### Files to Reference

| File | Purpose |
|------|---------|
| `Solutions/Day01.cs` | Hlavní implementační soubor - nahradit template kód |
| `AoC2025.Tests/Day01Tests.cs` | Test - aktualizovat expected result na `"3"` |
| `AoC2025.Tests/TestData/day01_example.txt` | Doplnit example input z AoC |
| `Inputs/day01.txt` | Reálný input (už obsahuje data) |

### Technical Decisions

**1. Modulo aritmetika pro cyklický rozsah:**
```csharp
// Správné ošetření záporných čísel v C#:
int newPosition = ((currentPosition + offset) % 100 + 100) % 100;
```
Proč? C# modulo vrací záporná čísla pro záporné operandy.

**2. Parsing instrukcí:**
```csharp
char direction = line[0];              // 'L' nebo 'R'
int distance = int.Parse(line[1..]);   // Range operator C# 8+
```

**3. Podmínka pro počítání nul:**
```csharp
if (currentPosition == 0) zeroCount++;
```
Počítá **po** každé rotaci, ne před.

---

## Implementation Plan

### Tasks

- [ ] **Task 1:** Implementovat `ParseInstruction(string line)` helper metodu
  - Input: `"L68"` → Output: `(direction: 'L', distance: 68)`
  - Edge case: trimovat whitespace, kontrola prázdných řádků

- [ ] **Task 2:** Implementovat `RotateDial(int current, char dir, int dist)` helper metodu
  - Aplikovat modulo 100 aritmetiku
  - L = odečíst, R = přičíst
  - Return novou pozici

- [ ] **Task 3:** Implementovat `SolvePart1(string input)`
  - Inicializovat `position = 50`, `zeroCount = 0`
  - Loop přes všechny instrukce
  - Po každé rotaci kontrola `position == 0`
  - Return `zeroCount.ToString()`

- [ ] **Task 4:** Aktualizovat `Title` property na `"Secret Entrance"`

- [ ] **Task 5:** Doplnit example input do `day01_example.txt`:
  ```
  L68
  L30
  R48
  L5
  R60
  L55
  L1
  L99
  R14
  L82
  ```

- [ ] **Task 6:** Aktualizovat `Day01Tests.cs`:
  - Part1 expected result: `"3"`
  - (Part2 test zatím ponechat s "EXPECTED_RESULT")

### Acceptance Criteria

- [ ] **AC1:** Given example input z AoC  
       When `SolvePart1()` is called  
       Then result is `"3"`

- [ ] **AC2:** Given reálný input z `day01.txt`  
       When `SolvePart1()` is executed  
       Then result je validní číslo (submit na AoC pro ověření)

- [ ] **AC3:** Given edge case: instrukce která začíná na 0  
       When rotace provede další průchod 0  
       Then zeroCount se zvýší

- [ ] **AC4:** Given negativní rotace (např. L70 z pozice 20)  
       When rotace přes 0 dolů  
       Then výsledek je správný (20 - 70 = -50 → 50)

- [ ] **AC5:** Given pozitivní rotace přes 99  
       When rotace přesáhne 99  
       Then správný wrap (např. 95 + 10 = 105 → 5)

---

## Additional Context

### Dependencies

**NuGet Packages:**
- xUnit (již nainstalováno v `AoC2025.Tests`)
- Žádné další závislosti nutné

**C# Features:**
- Range operator `[1..]` (C# 8+)
- Pattern matching (optional, pro eleganci)
- File-scoped namespaces (C# 10+)

### Testing Strategy

**Unit Tests:**
1. Example input test → expected `"3"`
2. Edge case testy (optional, ale doporučené):
   - Rotace začínající na 0
   - Velké distance > 100
   - Pouze L instrukce
   - Pouze R instrukce

**Manual Testing:**
- Spustit solution na reálný input
- Submit na AoC pro validaci

### Notes

**Časté chyby k zamezení:**
- ⚠️ Špatný modulo pro záporná čísla: `-50 % 100 = -50` v C# (ne 50!)
  - Řešení: `((x % 100) + 100) % 100`
- ⚠️ Počítání nul PŘED první rotací (start je 50, ne 0)
- ⚠️ Off-by-one: počítat jen když `position == 0`, ne před rotací

**AoC Specific:**
- Part 2 bude odhalena až po správném vyřešení Part 1
- Input je unikátní pro každého uživatele
- Expected result pro reálný input není v tech-spec (zjistíme při submitu)

**Performance:**
- Input má ~4500 řádků → O(n) algoritmus je dostatečně rychlý
- Žádná potřeba memoizace nebo optimalizace

---

## Ready for Development

Tato specifikace obsahuje vše potřebné pro autonomní implementaci:
✅ Kompletní pochopení problému  
✅ Example s detailním rozborem  
✅ Edge cases identifikovány  
✅ Code patterns definovány  
✅ Acceptance criteria jasná  

**Next Step:** Implementace pomocí `*quick-dev` workflow nebo ruční dev podle této spec.
