
using System;

internal class Day7 : Day
{
    
    List<(long, List<long>)> ParseInput(string input)
    {
        return input.Split("\r\n").Select(line => (long.Parse(line.Split(':')[0]), line.Split(": ")[1].Split(' ').Select(long.Parse).ToList())).ToList();
    }

    public string Star1(string input, bool example = false)
    {
        var equations = ParseInput(input);
        Console.WriteLine($"BackwardsSolution: {equations.Where(e => CheckEquationBackward(e, e.Item2.Count-1, e.Item1)).Select(e => e.Item1).Sum()}");
        return equations.Where(e => CheckEquation(e)).Select(e => e.Item1).Sum().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var equations = ParseInput(input);
        return equations.Where(e => CheckEquation(e,star2:true)).Select(e => e.Item1).Sum().ToString();
    }

    bool CheckEquation((long, List<long> ) equation, int index = 0, long total =0, bool star2=false)
    {
        (long target, List<long> values) = equation;
        if (index == values.Count) 
            return total == target;
        if (index == 0) 
            return CheckEquation(equation, index+1, values[0], star2);
        
        if (total * values[index] <= target)
            if (CheckEquation(equation, index + 1, total * values[index], star2)) 
                return true;

        if (total + values[index] <= target)
            if (CheckEquation(equation, index + 1, total + values[index], star2)) 
                return true;

        var concat = long.Parse("" + total + values[index]);
        if (star2 && concat <= target)
            if (CheckEquation(equation, index + 1, concat, star2)) 
                return true;

        return false;
    }

    bool CheckEquationBackward((long, List<long>) equation, int index, long total, bool star2=false)
    {
        (long target, List<long> values) = equation;
        if (index < 0) 
            return false;
        if (total % values[index] == 0)
        {
            if(total / values[index]==1)
                return true;
            if (CheckEquationBackward(equation, index - 1, total / values[index], star2)) 
                return true;
        }

        if (total - values[index] == 0)
            return true;
        if (total - values[index] > 0)
        {
            if (CheckEquationBackward(equation, index - 1, total - values[index], star2)) 
                return true;
        }


        //no working star2 for this
        /*if (star2 && total.ToString().EndsWith(values[index].ToString()))/// 1 23  => 123
        {
            var stringMatch = total.ToString().IndexOf(values[index].ToString());
            if (stringMatch==0)
                return index == 0;
            if (CheckEquationBackward(equation, index - 1, long.Parse(total.ToString().Substring(0, stringMatch)), star2))
                return true;
        }*/

        return false;
    }
}

