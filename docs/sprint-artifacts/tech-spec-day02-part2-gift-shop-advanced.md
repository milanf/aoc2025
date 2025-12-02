# Tech-Spec: Day 02 Part 2 - Gift Shop (Advanced Pattern Detection)

**Created:** 2025-12-02  
**Status:** Ready for Development  
**AoC Link:** https://adventofcode.com/2025/day/2

---

## Overview

### Problem Statement

Clerk zjistil, že pravidla z Part 1 stále nezachycují všechny nevalidní IDs. Mladý Elf dělal i další hloupé vzorce!

**Nová pravidla pro Part 2:**
- **Nevalidní ID = číslo složené z nějakého vzorce opakovaného ALESPOŇ 2x** (ne jen přesně dvakrát)
- Příklady nevalidních IDs:
  - `12341234` - vzorec "1234" opakován 2x
  - `123123123` - vzorec "123" opakován 3x
  - `1212121212` - vzorec "12" opakován 5x
  - `1111111` - vzorec "1" opakován 7x

**Rozdíl oproti Part 1:**
- Part 1: pouze **přesně 2x opakování** celého vzorce → `55`, `6464`, `123123`
- Part 2: **libovolně krát opakování** (min. 2x) → `55`, `555`, `5555`, `12121212`

**Example z AoC (Part 2):**
```
11-22,95-115,998-1012,1188511880-1188511890,222220-222224,
1698522-1698528,446443-446449,38593856-38593862,565653-565659,
824824821-824824827,2121212118-2121212124
```

**Analýza rozsahů (Part 2 vs Part 1):**
- `11-22`: **11, 22** (oba 2x opakování) - **BEZE ZMĚNY**
- `95-115`: **99, 111** - **ZMĚNA!** `111` je nově nevalidní (1 opakováno 3x)
- `998-1012`: **999, 1010** - **ZMĚNA!** `999` je nově nevalidní (9 opakováno 3x)
- `1188511880-1188511890`: **1188511885** (11885 2x) - **BEZE ZMĚNY**
- `222220-222224`: **222222** (222 2x nebo 22 3x nebo 2 6x) - **BEZE ZMĚNY**
- `1698522-1698528`: žádné nevalidní - **BEZE ZMĚNY**
- `446443-446449`: **446446** (446 2x) - **BEZE ZMĚNY**
- `38593856-38593862`: **38593859** (3859 2x) - **BEZE ZMĚNY**
- `565653-565659`: **565656** - **ZMĚNA!** (56 opakováno 3x)
- `824824821-824824827`: **824824824** - **ZMĚNA!** (824 opakováno 3x)
- `2121212118-2121212124`: **2121212121** - **ZMĚNA!** (21 opakováno 5x)

**Výsledek pro example (Part 2):** `4174379265`

### Solution

Rozšíření detektoru z Part 1 o kontrolu všech možných délek vzorců (délka musí být dělitelem celkové délky čísla).

**Algoritmus:**
1. **Reuse Part 1 infrastructure**: parser rozsahů, iterace čísel, sumace
2. **Nový detector `IsInvalidIdPart2(long number)`:**
   - Převést číslo na string
   - **Pro každou možnou délku vzorce** `patternLen` od 1 do `str.Length / 2`:
     - Zkontrolovat, zda délka stringu je **dělitelná** `patternLen` (bez zbytku)
     - Rozdělit string na části délky `patternLen`
     - Pokud jsou **všechny části identické** → nevalidní ID
   - Return true pokud existuje alespoň jeden opakující se vzorec

**Pattern Detection Logic:**
```csharp
bool IsInvalidIdPart2(long number)
{
    string str = number.ToString();
    int len = str.Length;
    
    // Testovat všechny možné délky vzorců (od 1 do len/2)
    for (int patternLen = 1; patternLen <= len / 2; patternLen++)
    {
        // Délka stringu musí být násobkem délky vzorce
        if (len % patternLen != 0) continue;
        
        // Získat první vzorec
        string pattern = str.Substring(0, patternLen);
        
        // Zkontrolovat, zda celé číslo = opakovaný vzorec
        bool isRepeating = true;
        for (int i = patternLen; i < len; i += patternLen)
        {
            string segment = str.Substring(i, patternLen);
            if (segment != pattern)
            {
                isRepeating = false;
                break;
            }
        }
        
        if (isRepeating) return true;
    }
    
    return false;
}
```

**Alternativní přístup (string reconstruction):**
```csharp
bool IsInvalidIdPart2(long number)
{
    string str = number.ToString();
    int len = str.Length;
    
    for (int patternLen = 1; patternLen <= len / 2; patternLen++)
    {
        if (len % patternLen != 0) continue;
        
        string pattern = str.Substring(0, patternLen);
        int repeatCount = len / patternLen;
        string reconstructed = string.Concat(Enumerable.Repeat(pattern, repeatCount));
        
        if (reconstructed == str) return true;
    }
    
    return false;
}
```

### Scope

**In Scope (Part 2):**
- ✅ Rozšíření detekce vzorců na libovolný počet opakování (min. 2x)
- ✅ Testování všech možných délek vzorců (1 až length/2)
- ✅ Implementace `SolvePart2` pomocí nové logiky
- ✅ Unit test s example inputem (expected: `4174379265`)
- ✅ Verifikace, že Part 1 stále funguje (regression test)

**Out of Scope:**
- ❌ Optimalizace pro huge rozsahy (AoC je vždy brute-force friendly)
- ❌ Matematické generování kandidátů (zbytečně složité)

---

## Context for Development

### Codebase Patterns

**Existující implementace (Part 1):**
- `Solutions/Day02.cs` - už existuje s funkčním Part 1
- `IsInvalidId(long number)` - detekuje pouze 2x opakování (sudá délka + split na polovinu)
- `SolvePart1` - funguje správně

**Co se bude měnit:**
- Přidat novou metodu `IsInvalidIdPart2(long number)`
- Implementovat `SolvePart2` (téměř identické s Part 1, jen jiný detector)
- **NEZMĚNIT** `IsInvalidId` (Part 1 musí zůstat funkční!)

### Files to Reference

| File | Purpose |
|------|---------|
| `Solutions/Day02.cs` | Přidat metodu `IsInvalidIdPart2` a implementovat `SolvePart2` |
| `AoC2025.Tests/Day02Tests.cs` | Přidat test pro Part 2 |
| `AoC2025.Tests/TestData/day02_example.txt` | Reuse existující (stejný example input) |
| `Inputs/day02.txt` | Reuse existující (stejný reálný input) |

### Technical Decisions

**1. Nová metoda vs. úprava existující:**
- **Rozhodnutí**: Vytvořit **novou metodu** `IsInvalidIdPart2`
- **Důvod**: Part 1 musí zůstat funkční (možné regression testy), nechceme riskovat breaking changes

**2. Algoritmus pro pattern detection:**
- **Zvolený přístup**: Loop přes všechny možné délky vzorců (1 až len/2)
- **Performance**: O(n²) worst case, ale pro číslice v řádu 10-12 číslic je to naprosto OK
- **Alternativa (zamítnuto)**: Regex `^(.+?)\1+$` - elegantnější, ale méně explicitní a může být pomalejší

**3. Pattern length optimization:**
- Testujeme pouze délky, které jsou **děliteli** celkové délky → `len % patternLen == 0`
- Příklad: pro `123456` (délka 6) testujeme pouze vzorce délek 1, 2, 3 (ne 4, 5)

**4. Edge cases:**
- Čísla s jednou číslicí (např. `5`) → nejkratší vzorec má délku 1, ale pak nemůžeme mít 2x opakování → musí být pattern len <= len/2
- Jednociferné číslo `5` nemůže být nevalidní (není opakovaný vzorec)

---

## Implementation Stories

### Story 1: Implementace Pattern Detectoru pro Part 2

**Acceptance Criteria:**
- [ ] Nová private metoda `bool IsInvalidIdPart2(long number)`
- [ ] Loop přes všechny možné délky vzorců (1 až `str.Length / 2`)
- [ ] Filtrování pouze délek, které jsou děliteli celkové délky
- [ ] Pro každou validní délku: extrakce vzorce a kontrola všech segmentů
- [ ] Return `true` pokud existuje alespoň jeden opakující se vzorec

**Implementation Notes:**

```csharp
/// <summary>
/// Detekuje, zda je product ID nevalidní podle pravidel Part 2.
/// ID je nevalidní, pokud je složeno z nějakého vzorce opakovaného alespoň 2x.
/// Příklady: 555 (5 třikrát), 12341234 (1234 dvakrát), 1212121212 (12 pětkrát).
/// </summary>
/// <param name="number">Product ID k testování</param>
/// <returns>True pokud je ID nevalidní (repeating pattern min. 2x)</returns>
private bool IsInvalidIdPart2(long number)
{
    string str = number.ToString();
    int len = str.Length;
    
    // Testovat všechny možné délky vzorců (od 1 do len/2)
    // Pattern nesmí být delší než polovina čísla (jinak není opakován)
    for (int patternLen = 1; patternLen <= len / 2; patternLen++)
    {
        // Délka stringu musí být násobkem délky vzorce
        if (len % patternLen != 0) continue;
        
        // Získat první vzorec
        string pattern = str.Substring(0, patternLen);
        
        // Zkontrolovat, zda celé číslo = opakovaný vzorec
        bool isRepeating = true;
        for (int i = patternLen; i < len; i += patternLen)
        {
            string segment = str.Substring(i, patternLen);
            if (segment != pattern)
            {
                isRepeating = false;
                break;
            }
        }
        
        if (isRepeating) return true;
    }
    
    return false;
}
```

**Test Cases (inline unit test):**
- `55` → true (5 opakováno 2x)
- `555` → true (5 opakováno 3x)
- `1111111` → true (1 opakováno 7x)
- `12341234` → true (1234 opakováno 2x)
- `123123123` → true (123 opakováno 3x)
- `1212121212` → true (12 opakováno 5x)
- `1234` → false (žádný opakující se vzorec)
- `101` → false (odd length, žádný pattern)
- `565656` → true (56 opakováno 3x) ← **důležité pro example!**

---

### Story 2: Implementace SolvePart2

**Acceptance Criteria:**
- [ ] Metoda `SolvePart2` reuse `ParseRanges` z Part 1
- [ ] Iterace všech čísel v rozsazích
- [ ] Volání `IsInvalidIdPart2` pro každé číslo
- [ ] Sumace nevalidních IDs
- [ ] Return součtu jako string

**Implementation Notes:**

```csharp
public string SolvePart2(string input)
{
    var ranges = ParseRanges(input.Trim());
    long sum = 0;
    
    foreach (var (start, end) in ranges)
    {
        for (long id = start; id <= end; id++)
        {
            if (IsInvalidIdPart2(id))
            {
                sum += id;
            }
        }
    }
    
    return sum.ToString();
}
```

**Notes:**
- Téměř identický s `SolvePart1`, jediný rozdíl je volání `IsInvalidIdPart2` místo `IsInvalidId`
- Možná by šlo refactorovat do společné metody, ale pro jednoduchost ponecháme duplicitu

---

### Story 3: Unit Test pro Part 2

**Acceptance Criteria:**
- [ ] Test `Part2_WithExampleInput_ReturnsExpectedResult` v `Day02Tests.cs`
- [ ] Reuse existující `day02_example.txt`
- [ ] Expected result: `"4174379265"`
- [ ] Test prochází zeleně

**Implementation Notes:**

**`AoC2025.Tests/Day02Tests.cs` (doplnění):**

```csharp
[Fact]
public void Part2_WithExampleInput_ReturnsExpectedResult()
{
    // Arrange
    var solution = new Day02();
    string exampleInput = File.ReadAllText("TestData/day02_example.txt");
    
    // Act
    string result = solution.SolvePart2(exampleInput);
    
    // Assert
    Assert.Equal("4174379265", result);
}
```

**Debugging pomůcka** (pokud test failne):
```csharp
[Fact]
public void Part2_DebugIndividualRanges()
{
    var solution = new Day02();
    
    // Test konkrétních změn oproti Part 1
    Assert.Contains("111", GetInvalidIdsInRange(solution, 95, 115)); // NEW!
    Assert.Contains("999", GetInvalidIdsInRange(solution, 998, 1012)); // NEW!
    Assert.Contains("565656", GetInvalidIdsInRange(solution, 565653, 565659)); // NEW!
    Assert.Contains("824824824", GetInvalidIdsInRange(solution, 824824821, 824824827)); // NEW!
    Assert.Contains("2121212121", GetInvalidIdsInRange(solution, 2121212118, 2121212124)); // NEW!
}
```

---

### Story 4: Verifikace Pattern Detectoru

**Acceptance Criteria:**
- [ ] Unit testy pro jednotlivé edge cases `IsInvalidIdPart2`
- [ ] Test pokrývá všechny příklady z AoC zadání
- [ ] Regression test: Part 1 stále funguje

**Implementation Notes:**

```csharp
[Theory]
[InlineData(55, true)]           // 5 opakováno 2x
[InlineData(555, true)]          // 5 opakováno 3x
[InlineData(1111111, true)]      // 1 opakováno 7x
[InlineData(12341234, true)]     // 1234 opakováno 2x
[InlineData(123123123, true)]    // 123 opakováno 3x
[InlineData(1212121212, true)]   // 12 opakováno 5x
[InlineData(565656, true)]       // 56 opakováno 3x (z example!)
[InlineData(824824824, true)]    // 824 opakováno 3x (z example!)
[InlineData(2121212121, true)]   // 21 opakováno 5x (z example!)
[InlineData(99, true)]           // 9 opakováno 2x
[InlineData(111, true)]          // 1 opakováno 3x
[InlineData(999, true)]          // 9 opakováno 3x
[InlineData(1010, true)]         // 10 opakováno 2x
[InlineData(1234, false)]        // Žádný pattern
[InlineData(101, false)]         // Odd length, žádný pattern
[InlineData(1, false)]           // Single digit
[InlineData(12, false)]          // 1 != 2
public void IsInvalidIdPart2_DetectsPatterns(long number, bool expected)
{
    // Pro testing potřebujeme přístup k private metodě
    // Alternativa: vytvořit wrapper public metodu pro testování
    // nebo použít reflection (ne doporučeno)
    
    var solution = new Day02();
    string result = solution.SolvePart2(number.ToString());
    bool isInvalid = result != "0";
    
    Assert.Equal(expected, isInvalid);
}
```

**Note:** Pokud nechceme testovat private metody, použijeme `SolvePart2` s single-number inputem jako workaround.

**Regression Test:**
```csharp
[Fact]
public void Part1_StillWorksAfterPart2Implementation()
{
    // Arrange
    var solution = new Day02();
    string exampleInput = File.ReadAllText("TestData/day02_example.txt");
    
    // Act
    string result = solution.SolvePart1(exampleInput);
    
    // Assert
    Assert.Equal("1227775554", result); // Part 1 expected result
}
```

---

## Verification Checklist

**Pre-Implementation:**
- [ ] Part 1 je implementován a testy prochází
- [ ] Example input existuje v `TestData/day02_example.txt`
- [ ] Reálný input je stažen v `Inputs/day02.txt`

**Implementation:**
- [ ] `IsInvalidIdPart2` správně detekuje všechny vzorce z example testů
- [ ] `SolvePart2` reuse existující parser a iteraci
- [ ] Žádné breaking changes v Part 1 (`IsInvalidId` nezměněna)

**Post-Implementation:**
- [ ] Part 2 test s example inputem vrací `4174379265` ✅
- [ ] Part 1 regression test stále prochází ✅
- [ ] Edge case testy pokrývají: jednociferné, lichá délka, žádný pattern

---

## Technical Notes

### Matematické pozorování

**Pattern Length Constraints:**
- Pro číslo délky `n`, možné délky vzorců jsou **dělitelé** čísla `n`
- Příklad: délka 12 → možné vzorce délek 1, 2, 3, 4, 6 (ne 5, 7, 8, 9, 10, 11)

**Optimalizace (budoucí, pokud by bylo potřeba):**
Místo brute-force iterace by šlo generovat kandidáty:
```csharp
// Pseudo-code: generovat pouze repeating patterns
for (int patternLen = 1; patternLen <= maxDigits / 2; patternLen++)
{
    for (int repeatCount = 2; repeatCount <= maxDigits / patternLen; repeatCount++)
    {
        // Generovat všechny možné vzorce délky patternLen
        // Opakovat repeatCount krát
        // Filtrovat, zda je v rozsahu
    }
}
```
Ale pro AoC je brute-force vždy dostatečný!

### Performance Estimation

**Example input:**
- Největší rozsah: `2121212118-2121212124` (pouze 7 čísel)
- Celkem čísel k otestování: cca 100-200
- Čas: < 1ms

**Reálný input (odhad):**
- Pravděpodobně podobně malé rozsahy (AoC není o performance)
- Očekávaný čas: < 100ms

**Worst case scenario:**
- Rozsah `1-1000000000` (1 miliarda čísel)
- Čas: několik sekund až minut (ale AoC tohle nedělá)

---

## Implementation Artifacts

**Soubory k úpravě:**
1. `Solutions/Day02.cs` - přidat `IsInvalidIdPart2`, implementovat `SolvePart2`

**Soubory k vytvoření:**
- Žádné nové soubory (reuse existující infrastruktura z Part 1)

**Soubory k rozšíření:**
2. `AoC2025.Tests/Day02Tests.cs` - přidat Part 2 testy

---

## Summary for Developer

**Co děláš:**
Rozšiřuješ detekci nevalidních IDs z Part 1 (pouze 2x opakování) na Part 2 (libovolně krát opakování, min. 2x).

**Klíčové změny:**
- Nová metoda `IsInvalidIdPart2` testující všechny možné délky vzorců
- `SolvePart2` téměř identické s Part 1, jen jiný detector
- **NEZMĚNIT** Part 1 (regression!)

**Expected Results:**
- Example input Part 2: `4174379265`
- Změny oproti Part 1: přidání `111`, `999`, `565656`, `824824824`, `2121212121`

**Estimated Time:**
- Implementation: 10-15 minut
- Testing: 5 minut
- **Total: ~20 minut**

---

**Ready to implement! 🚀**
