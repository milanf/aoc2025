namespace AoC2025.Solutions;

/// <summary>
/// Interface pro řešení jednotlivých dnů Advent of Code.
/// Každý den implementuje toto rozhraní pro konzistentní strukturu.
/// </summary>
public interface ISolution
{
    /// <summary>
    /// Číslo dne (1-25).
    /// </summary>
    int DayNumber { get; }
    
    /// <summary>
    /// Název puzzle z Advent of Code.
    /// </summary>
    string Title { get; }
    
    /// <summary>
    /// Řeší Part 1 daného dne.
    /// </summary>
    /// <param name="input">Vstupní data jako string (načtené z textového souboru).</param>
    /// <returns>Výsledek Part 1 jako string.</returns>
    string SolvePart1(string input);
    
    /// <summary>
    /// Řeší Part 2 daného dne.
    /// </summary>
    /// <param name="input">Vstupní data jako string (načtené z textového souboru).</param>
    /// <returns>Výsledek Part 2 jako string.</returns>
    string SolvePart2(string input);
}

