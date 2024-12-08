
internal class Day7 : Day
{
    
    List<(long, List<long>)> ParseInput(string input)
    {
        return input.Split("\r\n").Select(line => (long.Parse(line.Split(':')[0]), line.Split(": ")[1].Split(' ').Select(long.Parse).ToList())).ToList();
    }

    public string Star1(string input, bool example = false)
    {
        var equations = ParseInput(input);
        return equations.Where(e => CheckEquation(e)).Select(e => e.Item1).Sum().ToString();
    }
    public string Star2(string input, bool example = false)
    {

        return "";
    }

    bool CheckEquation((long, List<long> ) equation, int index = 0, long total =0)
    {
        (long target, List<long> values) = equation;
        if (index == values.Count) 
            return total == target;
        if (index == 0) 
            return CheckEquation(equation, index+1, values[0]);
        
        if (total * values[index] <= target)
            if (CheckEquation(equation, index + 1, total * values[index])) return true;

        if (total + values[index] <= target)
            if (CheckEquation(equation, index + 1, total + values[index])) return true;

        return false;
    }

    bool CheckEquationBackward((long, List<long>) equation, int index, long total)
    {
        (long target, List<long> values) = equation;
        if (index < 0) 
            return false;
        if (total % values[index] == 0)
        {
            if(total / values[index]==1)
                return true;
            if (CheckEquationBackward(equation, index - 1, total / values[index])) 
                return true;
        }

        if (total - values[index] == 0)
            return true;
        if (total - values[index] > 0)
        {
            if (CheckEquationBackward(equation, index - 1, total - values[index])) 
                return true;
        }

        return false;
    }
}

