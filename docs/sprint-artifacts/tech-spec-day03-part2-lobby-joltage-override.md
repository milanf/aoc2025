# Tech-Spec: Day 03 Part 2 - Lobby Joltage Override

**Created:** 2025-12-03  
**Status:** ✅ Completed  
**AoC Link:** https://adventofcode.com/2025/day/3

---

## Overview

### Problem Statement

Po stisknutí "joltage limit safety override" tlačítka potřebujeme generovat mnohem vyšší joltage. Nyní musíme v každé bance zapnout **přesně 12 baterií** místo 2.

**Klíčové změny oproti Part 1:**
- **⚠️ CRITICAL: Každá banka má ~100 číslic v reálném inputu** (example má pouze 15!)
- Musíme zapnout **přesně 12 baterií** (místo 2)
- Vypneme tedy **přesně 3 baterie**
- Joltage = 12-ciferné číslo ze zapnutých baterií (v původním pořadí!)
- **Cíl: najít maximální možný joltage pro každou banku**
- **Výsledek: součet maximálních joltagů ze všech bank**

**Example z AoC:**
```
987654321111111 -> vypneme tři 1 na konci -> 987654321111
811111111111119 -> vypneme tři 1 uvnitř -> 811111111119  
234234234234278 -> vypneme 2,3,2 na začátku -> 434234234278
818181911112111 -> vypneme tři 1 vpředu -> 888911112111

Total = 987654321111 + 811111111119 + 434234234278 + 888911112111 = 3121910778619
```

### Solution

**⚠️ KRITICKÁ REALITA:**
- Example input: 15 číslic → C(15, 3) = 455 kombinací
- **Reálný input: ~100 číslic → C(100, 3) = 161,700 kombinací**
- **Pro 200 řádků = 32,340,000 operací**
- Brute force je stále OK (desítky milionů operací), ale na hraně

**Algoritmus pro maximální joltage:**

**Doporučené řešení: Greedy přístup od zleva**

Chceme získat **největší 12-ciferné číslo** → potřebujeme co největší číslice co nejvíce vlevo.

**Myšlenka:**
- Chceme **ponechat** 12 největších číslic v co nejlevější pozici
- = Chceme **odstranit** 3 číslice tak, aby výsledek byl maximální

**Greedy algoritmus (optimalní):**
```
1. Procházíme zleva doprava
2. Pro každou pozici se ptáme: "Měli bychom tuto číslici odstranit?"
3. Odstranit JI, POKUD:
   - Ještě máme "kredit" na odstranění (< 3 odstraněno)
   - A existuje větší číslice vpravo v dosahu
   
Dosah = musíme zajistit, že zbyde alespoň 12 číslic
```

**Efektivnější implementace:**
```
Stack-based approach:
- Udržujeme stack výsledných číslic
- Pro každou číslici rozhodujeme: přidat nebo odstranit předchozí?
- Pravidlo: Pokud aktuální číslice > top stacku A ještě můžeme odstraňovat
  → Pop ze stacku (= odstraníme tu menší)
- Nakonec ořežeme na přesně 12 číslic
```

**Fallback: Brute force (pokud greedy je složitý):**
- Vygenerujeme všechny kombinace 3 pozic k vypnutí
- C(100, 3) = 161,700 kombinací × 200 řádků = 32M operací
- V C# to zabere ~1-2 sekundy (stále přijatelné)

### Scope

**In Scope (Part 2):**
- ✅ Modifikace `SolvePart2()` metody v `Day03.cs`
- ✅ Generování všech kombinací 3 pozic k vypnutí (z 15 celkových)
- ✅ Pro každou kombinaci vytvoření výsledného 12-ciferného čísla
- ✅ Nalezení maxima pro každou banku
- ✅ Součet všech maxim (výsledek je `long`, ne `int`!)
- ✅ Unit test s example inputem

**Out of Scope:**
- ❌ Optimalizace na greedy algoritmus (brute force je dostatečný)
- ❌ Změna Part 1 kódu
- ❌ Validace inputu (předpokládáme validní data)

---

## Context for Development

### Codebase Patterns

**Existující struktura:**
- `Solutions/Day03.cs` - už obsahuje Part 1 implementaci
- `AoC2025.Tests/Day03Tests.cs` - existují testy pro Part 1
- `AoC2025.Tests/TestData/day03_example.txt` - existuje example pro Part 1

**Co upravit:**
1. `Day03.cs` → změnit `SolvePart2()` z placeholderu na skutečnou implementaci
2. `Day03Tests.cs` → přidat test pro Part 2

### Files to Modify

| File | Changes |
|------|---------|
| `Solutions/Day03.cs` | Implementovat `SolvePart2()` a helper metodu `FindMaxJoltageWithTwelveBatteries()` |
| `AoC2025.Tests/Day03Tests.cs` | Přidat `Part2_ExampleInput_ReturnsExpectedResult()` test |

### Technical Decisions

**1A. Preferované řešení: Greedy Stack-Based Algorithm**
```csharp
private long FindMaxJoltageGreedy(string bank)
{
    int toRemove = 3;  // Kolik číslic chceme odstranit
    int toKeep = 12;   // Kolik chceme ponechat
    var stack = new Stack<char>();
    
    for (int i = 0; i < bank.Length; i++)
    {
        char current = bank[i];
        
        // Pokud aktuální číslice je větší než top stacku
        // A ještě máme "kredit" na odstranění
        // A máme dost číslic zbývajících
        while (stack.Count > 0 && 
               toRemove > 0 && 
               stack.Peek() < current &&
               (bank.Length - i + stack.Count - 1) >= toKeep)
        {
            stack.Pop();
            toRemove--;
        }
        
        stack.Push(current);
**3. Nalezení maxima pro banku (výběr přístupu):**

**Doporučeno: Greedy přístup (O(n) složitost)**
```csharp
private long FindMaxJoltageWithTwelveBatteries(string bank)
{
    // Použít stack-based greedy algoritmus výše
    return FindMaxJoltageGreedy(bank);
}
```

**Alternativa: Brute Force (O(C(n,3) × n) složitost)**
```csharp
private long FindMaxJoltageWithTwelveBatteries(string bank)
{
    long maxJoltage = 0;
    
    // Pro všechny kombinace 3 pozic k vypnutí
    foreach (var positions in GetCombinations(bank.Length, 3))
    {
        long joltage = CreateNumberWithoutPositions(bank, positions);
        maxJoltage = Math.Max(maxJoltage, joltage);
    }
    
    return maxJoltage;
}
```

**Poznámka:** Greedy je preferovaný - O(n) vs O(n³), ale brute force stále funguje (1-2s runtime).
**1B. Fallback řešení: Brute Force (pokud greedy nefunguje):**
### Tasks

**Doporučený přístup: Greedy Algorithm**

- [ ] **Task 1:** Implementovat greedy helper metodu `FindMaxJoltageGreedy(string bank)`
  - Stack-based algoritmus pro nalezení maxima v O(n)
  - Logika: Odstraňuj menší číslice, pokud za nimi následuje větší
  - Return: `long` reprezentující maximální joltage

- [ ] **Task 2:** Implementovat `FindMaxJoltageWithTwelveBatteries(string bank)`
  - Wrapper, který volá `FindMaxJoltageGreedy()`
  - Input: jeden řádek číslic (~100 znaků)
  - Output: maximální joltage jako `long`

- [ ] **Task 3:** Upravit `SolvePart2(string input)`
  - ⚠️ **ODSTRANIT HACK**: `bank.Substring(0, 15)` - musí se zpracovat celý řádek!
  - Split input na řádky
  - Pro každý řádek volat `FindMaxJoltageWithTwelveBatteries()`
  - Sečíst všechny hodnoty (jako `long`)
  - Return součet jako `string`

**Fallback přístup: Brute Force (pokud greedy selže)**

- [ ] **Task 1B:** Implementovat `GetCombinations(int n, int k)` (už existuje)
- [ ] **Task 2B:** Implementovat `CreateNumberWithoutPositions()` (už existuje)
- [ ] **Task 3B:** `FindMaxJoltageWithTwelveBatteries()` použije kombinace
  - ⚠️ Pozor: C(100, 3) = 161,700 iterací na řádek!
  - Runtime: ~1-2 sekundy**
```csharp
private long FindMaxJoltageWithTwelveBatteries(string bank)
{
    long maxJoltage = 0;
    
    // Pro všechny kombinace 3 pozic k vypnutí
    foreach (var positions in GetCombinations(bank.Length, 3))
    {
        long joltage = CreateNumberWithoutPositions(bank, positions);
        maxJoltage = Math.Max(maxJoltage, joltage);
    }
    
    return maxJoltage;
}
```

**4. Datové typy:**
- Part 1 používal `int` (max 99)
- Part 2 potřebuje `long` (12-ciferná čísla)
- Výsledek je `long`, ale vrací se jako `string`

**5. Edge cases:**
- Všechny číslice stejné: `"111111111111111"` → výsledek je `111111111111` (libovolné 3 pozice)
- Maximum na začátku: chceme vypnout co nejmenší číslice co nejdřív zleva

---

## Implementation Plan

### Tasks

- [ ] **Task 1:** Implementovat helper metodu `GetCombinations(int n, int k)`
  - Generuje všechny k-tice z {0, 1, ..., n-1}
  - Pro náš případ: n=15 (délka řádku), k=3 (počet pozic k vypnutí)
  - Return `IEnumerable<int[]>`

- [ ] **Task 2:** Implementovat helper metodu `CreateNumberWithoutPositions(string bank, int[] positionsToRemove)`
  - Input: původní string a pozice k vypnutí
  - Output: `long` reprezentující číslo BEZ vypnutých pozic
  - Použít `StringBuilder` pro sestavení výsledku

- [ ] **Task 3:** Implementovat helper metodu `FindMaxJoltageWithTwelveBatteries(string bank)`
  - Input: jeden řádek číslic (např. `"987654321111111"`)
  - Algoritmus:
    - Pro všechny kombinace 3 pozic
    - Vytvoř číslo bez těch pozic
    - Sleduj maximum
  - Output: maximální joltage jako `long`

- [ ] **Task 4:** Upravit `SolvePart2(string input)`
  - Split input na řádky (stejně jako Part 1)
  - Pro každý řádek volat `FindMaxJoltageWithTwelveBatteries()`
  - Sečíst všechny hodnoty (jako `long`)
  - Return součet jako `string`

- [ ] **Task 5:** Přidat test do `Day03Tests.cs`
  - Použít stejný `day03_example.txt` jako Part 1
  - Expected result: `"3121910778619"`

### Test Cases

**Example test:**
```
Input (stejný jako Part 1):
987654321111111
811111111111119
234234234234278
818181911112111

Expected Output: 3121910778619

Breakdown:
- Bank 1: 987654321111 (vypnout pozice 12,13,14)
- Bank 2: 811111111119 (vypnout nějaké 1 uvnitř)
- Bank 3: 434234234278 (vypnout 2,3,2 na začátku)
- Bank 4: 888911112111 (vypnout 1 na začátku)

Total: 987654321111 + 811111111119 + 434234234278 + 888911112111 = 3121910778619
```

**Edge cases k otestování (volitelné):**
- Všechny stejné číslice: `"111111111111111"` → max = 111111111111
- Sestupná sekvence: `"987654321012345"` → max = 987654321345 (vypnout 0,1,2)

---

## Acceptance Criteria

- [ ] **AC1:** Given example input z AoC, When `SolvePart2()` is called, Then vrátí `"3121910778619"`
- [ ] **AC2:** Given první řádek `"987654321111111"`, When zpracován, Then vrátí maximální joltage `987654321111`
- [ ] **AC3:** Given reálný input z `Inputs/day03.txt`, When `SolvePart2()` is called, Then vrátí správný výsledek (ověřit na AoC webu)
- [ ] **AC4:** Unit test `Part2_ExampleInput_ReturnsExpectedResult()` prochází

---

### Notes

**⚠️ REALITA INPUTU:**
- **Example:** 15 číslic → C(15, 3) = 455 kombinací
- **Reálný input:** ~100 číslic → C(100, 3) = 161,700 kombinací
- **200 řádků:** 200 × 161,700 = 32,340,000 operací
- **Brute force runtime:** ~1-2 sekundy (hraničně přijatelné)

**Proč preferovat greedy:**
- **Greedy složitost:** O(n) = O(100) na řádek → 20,000 operací celkem
- **Brute force složitost:** O(C(n,3) × n) = O(161,700 × 100) = 16M operací na řádek
- **Rozdíl:** ~1,600× rychlejší!
- **Greedy runtime:** <0.1 sekundy

**Greedy algoritmus je OPTIMÁLNÍ pro tento problém:**
- Vždy najde správnou odpověď (nejedná se o heuristiku)
- Důkaz: Chceme největší číslo → největší číslice co nejvíc vlevo
- Stack algoritmus garantuje, že vždy odstraníme "nejhorší" číslice

**Kdy použít brute force:**
- Pokud greedy implementace selže na edge cases
- Pro debugging (lze snadno ověřit výsledky)
- Runtime 1-2s je stále akceptovatelný pro AoC

**Doporučení:** Implementuj greedy. Pokud nefunguje, fallback na brute force.

### Notes

**Proč brute force stačí:**
- C(15, 3) = 455 kombinací na řádek
- 200 řádků = 91,000 operací
- Každá operace je O(15) pro sestavení čísla
- Celková složitost: O(200 × 455 × 15) ≈ 1.4M operací → triviální

**Alternativní greedy přístup** (pokud by výkon byl problém):
- Procházet zleva doprava
- Na každé pozici rozhodnout: "vypnout nebo nechat?"
- Rozhodnutí: porovnat lexikograficky zbývající substring s/bez aktuální číslice
- Složitost: O(n) = O(15) na řádek → 200× rychlejší

**Pro tento problém doporučuji brute force** - je jednodušší na implementaci, čitelnější a výkon je naprosto dostačující.

---

## Ready for Development

✅ Problem je jasný  
✅ Algoritmus je definovaný  
✅ Implementační kroky jsou rozepsané  
✅ Test cases jsou připravené  

**Doporučený next step:**  
Spusť Quick-Dev workflow s tímto spec souborem v čerstvém kontextu pro nejlepší výsledky.
