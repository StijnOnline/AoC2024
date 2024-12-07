
internal class Day7_NotWorking : Day
{
    
    List<(long, List<long>)> ParseInput(string input)
    {
        return input.Split("\r\n").Select(line => (long.Parse(line.Split(':')[0]), line.Split(": ")[1].Split(' ').Select(long.Parse).ToList())).ToList();
    }

    public string Star1(string input, bool example = false)
    {
        var equations = ParseInput(input);

        //debug:
        /*Console.WriteLine("potential issues:");
        var different = equations.Where(e => e.Item1 < 3245122495222 - 3245122494391);
        different.ToList().ForEach(e => {
            var result = CheckEquation(e);
            if(result.Item1)
                Console.WriteLine($"{e.Item1}= {result.Item2}");
            //else
            //    Console.WriteLine($"{e.Item1}= {string.Join(' ', e.Item2)}");
        });*/

        /*Console.WriteLine("wrong:");
        equations.ToList().ForEach(e => {
            var result = CheckEquation(e);
            if (result.Item1 && !VerifyFilledEquation(result.Item2,e.Item1))
                Console.WriteLine($"{e.Item1}= {result.Item2}");
        });*/

        return equations.Where(e => CheckEquation(e).Item1).Select(e=>e.Item1).Sum().ToString();

        //Guess1: 30406496563 too low
        //Guess2: 3245122494391 too low
        //Guess3: 3245122495222 too high
    }
    public string Star2(string input, bool example = false)
    {

        return "";
    }

    (bool, string) CheckEquation((long, List<long> ) equation, int index=0, long total =0, string log = "")
    {
        (long target, List<long> values) = equation;
        if (index >= values.Count) return (false, log);
        if (index == 0) {
            return CheckEquation(equation, index+1, values[0], values[0].ToString());
        }

        if (total * values[index] == target) return (true, log + "*" + values[index]);
        if (total * values[index] <= target)
        {
            var result = CheckEquation(equation, index + 1, total * values[index], log + "*" + values[index]);
            if (result.Item1) return (true, result.Item2);
        }


        if (total + values[index] == target) return (true, log + "+" + values[index]);
        if (total + values[index] <= target)
        {
            var result = CheckEquation(equation, index + 1, total + values[index], log + "+" + values[index]);
            if (result.Item1) return (true, result.Item2);
        }
        
        return (false, log);
    }

    bool VerifyFilledEquation(string equation, long result)
    {
        var numbers = equation.Split(new[]{ '*','+'}).Select(long.Parse).ToArray();
        var operations = equation.Where(c=> c is '*' or '+').ToArray();
        long total = numbers[0];
        for (int i = 0; i < operations.Length; i++)
        {
            if (operations[i] is '+')
            {
                total += numbers[i+1];
                continue;
            }
            if (operations[i] is '*')
            {
                total *= numbers[i + 1];
                continue;
            }
        }
        return total == result;
    }

}

