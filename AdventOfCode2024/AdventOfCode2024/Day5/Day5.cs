using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;

internal class Day5 : Day
{

    void ParseInput(string input, out List<Tuple<int,int>> orderingRules, out List<List<int>> pageList)
    {
        var split = input.Split("\r\n\r\n");
        orderingRules = new List<Tuple<int, int>>();
        foreach (string rule in split[0].Split("\r\n"))
        {
            var ruleSplit = rule.Split('|');
            orderingRules.Add(new Tuple<int, int>(int.Parse(ruleSplit[0]), int.Parse(ruleSplit[1])));
        }
        pageList = new List<List<int>>();
        foreach (string page in split[1].Split("\r\n"))
        {
            pageList.Add(page.Split(',').Select(p=>int.Parse(p)).ToList());
        }
    }

    public string Star1(string input, bool example = false)
    {
        ParseInput(input, out List<Tuple<int, int>> orderingRules, out List<List<int>> pageList);
        return pageList.Where(p => OrderPageCorrect(orderingRules, p)).Select(p => GetMiddleNumber(p)).Sum().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        ParseInput(input, out List<Tuple<int, int>> orderingRules, out List<List<int>> pageList);
        var incorrectPageLists = pageList.Where(p => !OrderPageCorrect(orderingRules, p));
        var correctPageLists = incorrectPageLists.Select(l=>CreateSortedList(orderingRules, l));
        var comp = new PageComparer(orderingRules);
        //test alternative:
        correctPageLists = incorrectPageLists.Select(l => l.OrderBy(p=>p,comp).ToList());
        //incorrectPageLists.Select(l=>string.Join(",", l)).Zip(correctPageLists.Select(l => string.Join(",", l))).ToList().ForEach(l => Console.WriteLine( l.First + " => " + l.Second));
        var count = correctPageLists.Select(p => GetMiddleNumber(p)).Sum();
        return count.ToString();
    }

    public List<int> CreateSortedList(List<Tuple<int, int>> orderingRules, List<int> pageList)
    {
        List<int> newList = new List<int>();
        for (int i = 0; i < pageList.Count; i++)
        {
            var numberToAdd = pageList[i];

            for (int insertIndex = 0; insertIndex <= newList.Count; insertIndex++)
            {
                var testList = newList.ToList();
                testList.Insert(insertIndex, numberToAdd);
                if (OrderPageCorrect(orderingRules, testList))
                {
                    newList.Insert(insertIndex, numberToAdd);
                    break;
                }
            }
        }
        return newList;
    }

    public bool OrderPageCorrect(List<Tuple<int, int>> orderingRules, List<int> pageList)
    {
        for (int i = 0; i < pageList.Count; i++)
        {
            var failedRule = orderingRules.Where(r => r.Item2 == pageList[i]).FirstOrDefault(r => pageList.Skip(i + 1).Contains(r.Item1));//check if there is a page after this page that should be before
            if (failedRule!=null) 
                return false;
            failedRule = orderingRules.Where(r => r.Item1 == pageList[i]).FirstOrDefault(r => pageList.Take(i).Contains(r.Item2)); //check if there is a page before this page that should be after
            if (failedRule != null) 
                return false;
        }
        return true;
    }

    public int GetMiddleNumber(List<int> numbers)
    {
        return numbers[numbers.Count / 2];//assume uneven count
    }
}

public class PageComparer : IComparer<int>
{
    List<Tuple<int, int>> orderingRules;

    public PageComparer(List<Tuple<int, int>> orderingRules)
    {
        this.orderingRules = orderingRules;
    }

    public int Compare(int x, int y)
    {
        if (orderingRules.Any(r=>r.Item1==x && r.Item2==y))
        {
            return -1;
        }
        else if (orderingRules.Any(r => r.Item1 == y && r.Item2 == x))
        {
            return 1;
        }

        return 0;
    }
}

