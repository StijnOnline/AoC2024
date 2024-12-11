using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Day1 : Day
{
    //I had some fun learning about and practicing with Linq

    (List<int> firstNumbers, List<int> secondNumbers) ParseInput(string input) =>
        input.Split('\n')
         .Select(item => item.Split(' ', StringSplitOptions.RemoveEmptyEntries))
         .Aggregate((new List<int>(), new List<int>()), (lists, numbers) =>
         {
             lists.Item1.Add(int.Parse(numbers[0]));
             lists.Item2.Add(int.Parse(numbers[1]));
             return lists;
         });


    public string Star1(string input, bool example = false)
    {
        var lists = ParseInput(input);
        return lists.firstNumbers.Order().Zip(lists.secondNumbers.Order(), (a, b) => Math.Abs(a - b)).Sum().ToString();
    }

    public string Star2(string input, bool example = false)
    {
        var lists = ParseInput(input);
        var countsB = lists.secondNumbers.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());
        return lists.firstNumbers.Select(n => n * countsB.GetValueOrDefault(n)).Sum().ToString();//23981443
    }

}