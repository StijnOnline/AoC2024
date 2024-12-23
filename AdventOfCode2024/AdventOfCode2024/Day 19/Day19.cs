
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using static Day19;

internal class Day19 : Day
{
    
    void ParseInput(string input, out List<string> options, out List<string> patterns)
    {
        options = input.Split("\r\n\r\n")[0].Split(",", StringSplitOptions.TrimEntries).ToList();
        patterns = input.Split("\r\n\r\n")[1].Split("\r\n").ToList();
    }

    Dictionary<string, bool> knownPatterns;
    public string Star1(string input, bool example = false)
    {
        ParseInput(input, out List<string> availablePatterns, out List<string> patterns);
        //Is this anti-regex puzzle? This is has catastrophic backtracking:
        //var RegexPattern = $"^({string.Join('|', options)})+$";
        //return patterns.Count(p => Regex.IsMatch(p, RegexPattern)).ToString();

        availablePatterns = availablePatterns.OrderBy(s => s.Length).ToList();
        knownPatterns = new Dictionary<string, bool>();

        int possible = 0;
        for (int i = 0; i < patterns.Count; i++)
        {
            bool isPossible = IsPossible(patterns[i], availablePatterns);
            if (isPossible)
                possible++;
            Console.WriteLine($"{i}: {isPossible}");
        }

        return possible.ToString();
    }

    public string Star2(string input, bool example = false)
    {
        return "";
    }

    public bool IsPossible(string input, List<string> availablePatterns)
    {
        if (knownPatterns.TryGetValue(input, out bool possible))
        {
            return possible;
        }

        if (availablePatterns.Contains(input))
        {
            knownPatterns.TryAdd(input, true);
            return true;
        }

        for (int i = 1; i < input.Length; i++)
        {
            if(IsPossible(input.Substring(0,i), availablePatterns) && IsPossible(input.Substring(i), availablePatterns))
            {
                knownPatterns.TryAdd(input, true);
                return true;
            }
        }

        knownPatterns.TryAdd(input, false);
        return false;
    }

}

