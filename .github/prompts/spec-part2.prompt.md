---
agent: 'bmd-custom-bmm-quick-flow-solo-dev'
description: 'Vytvoř technickou specifikaci pro řešení Part 2 daného dne Advent of Code 2025'
tools: ["changes","edit","fetch","githubRepo","problems","runCommands","runTasks","runTests","search","runSubagent","testFailure","todos","usages"]
---

# Dříve než začneš!
- Vždy se ujisti, že máš načtené všechny své instrukce a konfigrace pro použitého agenta (bmd-custom-bmm-quick-flow-solo-dev), musíš mj. vědět jakým jazykem máš komunikovat a vytvářet výstupy.
- Pokud si nejsi jistý, nebo chybí číslo dne v `${input:DayNumber}`, pak se ho nesnaž vymyslet, ale raději se zeptej uživatele.
- Ujisti se, že je vyřešen Part 1 daného dne před tím, než začneš vytvářet specifikaci pro Part 2.

# Vytvoření technické specifikace pro Advent of Code 2025 - Part 2

- Vytvoř specifikaci pro řešní `${input:DayNumber}` Part 2.
- Nejdříve se zeptej užívatele na vstup pro Part 2, ten si neumíš stáhnout sám z internetu!
- Výstup ukládej ve stejném formátu a vzoru pro pojmenování jako předchozí dny.

# DÚLEŽITÉ:
- NIKDY NEPŘEDPOKLÁDEJ ROZSAH VSTUPU JEN NA ZÁKLADĚ VZOROVÉHO PŘÍKLADU. VŽDY SI ZKONTROLUJ VSTUP V SOUBORU day`${input:DayNumber}`.txt A URČI ROZSAH A LIMITY VSTUPNÍCH HODNOT!
- NIKDY JEN NA ZÁKLADĚ VZOROVÉHO PŘÍKLADU NEURČUJ ČASOVOU A PROSTOROVOU SLOŽITOST. VŽDY PROVEĎ ANALÝZU NA ZÁKLADĚ SKUTEČNÉHO VSTUPU!
- BRUTE FORCE ŘEŠENÍ JSOU VŽDY AŽ POSLEDNÍ MOŽNOST, REÁLNÁ DATA V ZADÁNÍ TÉMĚŘ VŽDY VYŽADUJÍ EFEKTIVNÍ ALGORITMY!
- Vždy měj na paměti, že Advent of Code je soutěž zaměřená na algoritmy. Málokdy funguje bruteforce řešení, je třeba přijít na efektivní algoritmus.
- Mezi důležité algomitmy patří např. prohledávání grafů, dynamické programování, greedy algoritmy; DFS (Depth-First Search) a BFS (Breadth-First Search) – pro procházení grafů, mřížek, labyrintů; Dijkstra / A* – pro hledání nejkratší cesty s váhami; Flood fill – pro označování oblastí v matici; apod.
- algoritmy si můžeš vyhledat a prostudovat, pokud si nejsi jistý, který použít.
- jako vodítko pro stanovení správného algoritmu se podívej i na input pro daný den. Nespoléhej pouze na vzorové řešení, které je vždy triviální a obsahuje pár kroků.
- pamatuj i na určení různých edge case scénářů, které je třeba ošetřit. Opět podívej se i na input, vzorové řešení často edge case neřeší.