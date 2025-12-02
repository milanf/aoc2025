namespace AoC2025.Solutions;

/// <summary>
/// Day 2: Gift Shop - Detekce nevalidních product IDs.
/// Nevalidní ID = číslo tvořené opakováním nějakého vzorce právě dvakrát (např. 55, 6464, 123123).
/// </summary>
public class Day02 : ISolution
{
    public int DayNumber => 2;
    
    public string Title => "Gift Shop";
    
    public string SolvePart1(string input)
    {
        var ranges = ParseRanges(input.Trim());
        long sum = 0;
        
        foreach (var (start, end) in ranges)
        {
            for (long id = start; id <= end; id++)
            {
                if (IsInvalidId(id))
                {
                    sum += id;
                }
            }
        }
        
        return sum.ToString();
    }
    
    /// <summary>
    /// Detekuje, zda je product ID nevalidní podle pravidel Part 2.
    /// ID je nevalidní, pokud je složeno z nějakého vzorce opakovaného alespoň 2x.
    /// </summary>
    /// <param name="number">Product ID k testování</param>
    /// <returns>True pokud je ID nevalidní (repeating pattern min. 2x)</returns>
    private bool IsInvalidIdPart2(long number)
    {
        string str = number.ToString();
        int len = str.Length;
        for (int patternLen = 1; patternLen <= len / 2; patternLen++)
        {
            if (len % patternLen != 0) continue;
            string pattern = str.Substring(0, patternLen);
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
    
    /// <summary>
    /// Parsuje vstupní string rozsahů do listu (start, end) tuplů.
    /// Formát: "11-22,95-115,998-1012"
    /// </summary>
    /// <param name="input">Vstupní string s rozsahy oddělenými čárkami</param>
    /// <returns>List tuplů obsahujících start a end každého rozsahu</returns>
    private List<(long start, long end)> ParseRanges(string input)
    {
        var ranges = new List<(long, long)>();
        var parts = input.Split(',');
        
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;
            
            var bounds = trimmed.Split('-');
            if (bounds.Length != 2) continue;
            
            long start = long.Parse(bounds[0]);
            long end = long.Parse(bounds[1]);
            ranges.Add((start, end));
        }
        
        return ranges;
    }
    
    /// <summary>
    /// Detekuje, zda je product ID nevalidní (složeno z dvou identických částí).
    /// Příklady: 55 (5+5), 6464 (64+64), 123123 (123+123).
    /// </summary>
    /// <param name="number">Product ID k testování</param>
    /// <returns>True pokud je ID nevalidní (repeating pattern dvakrát)</returns>
    private bool IsInvalidId(long number)
    {
        string str = number.ToString();
        int len = str.Length;
        
        // Musí mít sudou délku (jinak nemůže být složeno z 2x stejného vzorce)
        if (len % 2 != 0) return false;
        
        int half = len / 2;
        string firstHalf = str.Substring(0, half);
        string secondHalf = str.Substring(half);
        
        return firstHalf == secondHalf;
    }
}
