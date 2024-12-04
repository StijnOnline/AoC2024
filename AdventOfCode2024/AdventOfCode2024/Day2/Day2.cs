using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Day2 : Day
{
    List<List<int>> ParseInput(string input) =>
    input.Split('\n')
        .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList())
        .ToList();

    public string Star1(string input, bool example = false)
    {
        var lists = ParseInput(input);
        return lists.Count(IsSafe).ToString();
    }
    static bool IsSafe(List<int> levels)
    {
        int prevLevel = levels[0];
        bool ascending = levels[1] > levels[0];
        for (int i = 1; i < levels.Count; i++)
        {
            var diff = Math.Abs(levels[i] - prevLevel);
            if (diff < 1 || diff > 3) return false;
            if (levels[i] > prevLevel != ascending) return false;
            prevLevel = levels[i];
        }
        return true;
    }


    public string Star2(string input, bool example = false)
    {
        var lists = ParseInput(input);
        return lists.Count(IsSafeOneOff).ToString();//647 too  low
    }

    static bool IsSafeOneOff(List<int> levels)
    {
        if (IsSafe(levels)) return true;
        for (int i = 0; i < levels.Count; i++)
        {
            var alternative = levels.ToList();
            alternative.RemoveAt(i);
            if (IsSafe(alternative)) return true;
        }
        return false;
    }

    //Interesting alternatives if you want to avoid list creation (if this was an issue)
    static bool IsSafe(List<int> levels, int skipIndex)
    {
        int? prevLevel = null;
        bool? ascending = null;
        for (int i = 0; i < levels.Count; i++)
        {
            if (i == skipIndex) continue;

            if (prevLevel == null)
            {
                prevLevel = levels[i];
                continue;
            }

            var diff = Math.Abs(levels[i] - prevLevel.Value);
            if (diff < 1 || diff > 3) return false;

            if (ascending == null)
                ascending = levels[i] > prevLevel;

            if (levels[i] > prevLevel != ascending) return false;
            prevLevel = levels[i];
        }
        return true;
    }

    IEnumerable<int> SkipIndex(IEnumerable<int> levels, int skipIndex)
    {
        for (int i = 0; i < levels.Count(); i++)
        {
            if (i != skipIndex)
                yield return levels.ElementAt(i);
        }
    }
}