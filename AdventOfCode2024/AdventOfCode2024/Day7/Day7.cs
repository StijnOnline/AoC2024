
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
        //Guess1: 30406496563 too low
        //Guess2: 3245122494391 too low
        //Guess3: 3245122495222 too high
    }
    public string Star2(string input, bool example = false)
    {

        return "";
    }

    bool CheckEquation((long, List<long> ) equation, int index=0, long total =0)
    {
        (long target, List<long> values) = equation;
        if (index >= values.Count) return false;
        if (index == 0) {
            return CheckEquation(equation, index+1, values[0]);
        }
        if (total * values[index] == target) return true;
        if (total * values[index] <= target)
            if (CheckEquation(equation, index + 1, total * values[index])) return true;

        if (total + values[index] == target) return true;
        if (total + values[index] <= target)
            if (CheckEquation(equation, index + 1, total + values[index])) return true;

        return false;
    }
}

