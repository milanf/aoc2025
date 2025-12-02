﻿using System.Reflection;
using AoC2025.Solutions;

namespace AoC2025.Console;

class Program
{
    static void Main(string[] args)
    {
        System.Console.WriteLine("🎄 Advent of Code 2025 - C# Runner 🎄");
        System.Console.WriteLine();

        // Dynamicky načteme všechna řešení pomocí reflection
        var solutions = LoadSolutions();

        if (solutions.Count == 0)
        {
            System.Console.WriteLine("❌ Žádná řešení nebyla nalezena.");
            System.Console.WriteLine("   Přidej třídy implementující ISolution do Solutions/");
            return;
        }

        // Pokud je zadán argument (číslo dne), spustíme přímo
        if (args.Length > 0 && int.TryParse(args[0], out int dayArg))
        {
            RunDay(dayArg, solutions);
            return;
        }

        // Jinak zobrazíme menu
        while (true)
        {
            System.Console.WriteLine("Dostupné dny:");
            foreach (var solution in solutions.OrderBy(s => s.DayNumber))
            {
                System.Console.WriteLine($"  [{solution.DayNumber}] Day {solution.DayNumber:D2} - {solution.Title}");
            }
            System.Console.WriteLine("  [0] Ukončit");
            System.Console.WriteLine();

            System.Console.Write("Vyber den (číslo): ");
            var input = System.Console.ReadLine();

            if (!int.TryParse(input, out int day))
            {
                System.Console.WriteLine("❌ Neplatný vstup. Zadej číslo dne.");
                System.Console.WriteLine();
                continue;
            }

            if (day == 0)
            {
                System.Console.WriteLine("Nashledanou! 👋");
                break;
            }

            RunDay(day, solutions);
        }
    }

    static List<ISolution> LoadSolutions()
    {
        var solutionType = typeof(ISolution);
        var assembly = Assembly.GetExecutingAssembly();

        // Najdeme všechny typy, které implementují ISolution
        var solutions = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && solutionType.IsAssignableFrom(t))
            .Select(t => (ISolution)Activator.CreateInstance(t)!)
            .ToList();

        return solutions;
    }

    static void RunDay(int day, List<ISolution> solutions)
    {
        var solution = solutions.FirstOrDefault(s => s.DayNumber == day);

        if (solution == null)
        {
            System.Console.WriteLine($"❌ Den {day} nebyl nalezen.");
            System.Console.WriteLine();
            return;
        }

        // Načteme input soubor
        // Najdeme project root (kde je .sln soubor)
        var projectRoot = FindProjectRoot();
        var inputPath = Path.Combine(projectRoot, "Inputs", $"day{day:D2}.txt");
        
        if (!File.Exists(inputPath))
        {
            System.Console.WriteLine($"❌ Input soubor nebyl nalezen: {inputPath}");
            System.Console.WriteLine($"   Vytvoř soubor Inputs/day{day:D2}.txt s daty z adventofcode.com");
            System.Console.WriteLine();
            return;
        }

        var input = File.ReadAllText(inputPath);

        System.Console.WriteLine();
        System.Console.WriteLine($"🎯 Day {day:D2}: {solution.Title}");
        System.Console.WriteLine(new string('=', 50));

        try
        {
            System.Console.WriteLine("⏳ Spouštím Part 1...");
            var part1Result = solution.SolvePart1(input);
            System.Console.WriteLine($"✅ Part 1: {part1Result}");
            System.Console.WriteLine();

            System.Console.WriteLine("⏳ Spouštím Part 2...");
            var part2Result = solution.SolvePart2(input);
            System.Console.WriteLine($"✅ Part 2: {part2Result}");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Chyba při řešení: {ex.Message}");
        }

        System.Console.WriteLine(new string('=', 50));
        System.Console.WriteLine();
    }

    static string FindProjectRoot()
    {
        var currentDir = Directory.GetCurrentDirectory();
        
        // Hledáme adresář s .sln souborem
        while (currentDir != null)
        {
            if (Directory.GetFiles(currentDir, "*.sln").Length > 0)
            {
                return currentDir;
            }
            
            var parent = Directory.GetParent(currentDir);
            if (parent == null) break;
            currentDir = parent.FullName;
        }
        
        // Fallback: předpokládáme že jsme v bin/Debug/net10.0, jdeme 4 úrovně nahoru
        return Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..");
    }
}

