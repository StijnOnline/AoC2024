
internal class Day11 : Day
{
    List<long> ParseInput(string input)
    {
        return input.Split(' ').Select(long.Parse).ToList();
    }

    public string Star1(string input, bool example = false)
    {
        var stones = ParseInput(input);
        for (int i = 0; i < 25; i++)
        {
            stones = Blink(stones);
        }
        return stones.Count().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var stones = ParseInput(input);
        var stoneOccurrenceCount = stones.GroupBy(s => s).Select(g => (g.Key, (long) g.Count())).ToDictionary();
        for (int i = 0; i < 75; i++)
        {
            stoneOccurrenceCount = PredictBlink(stoneOccurrenceCount);
        }
        return stoneOccurrenceCount.Sum(p=>p.Value).ToString();
    }

    List<long> Blink(List<long> stones)
    {
        for (var i = 0; i < stones.Count; i++)
        {
            if(stones[i] == 0)
            {
                stones[i] = 1;
                continue;
            }
            var numberString = stones[i].ToString();
            if (numberString.Length % 2 == 0)
            {
                stones.Insert(i+1, long.Parse(numberString.Substring(numberString.Length/2)));
                stones[i] = long.Parse(numberString.Substring(0, numberString.Length / 2));
                i++;
                continue;
            }

            stones[i] *= 2024;
            continue;
        }
        return stones;
    }


    //Many of the stones are repeating their pattern
    //I need to store every stone number and its known outcome
    //And also store how many occurrences of a number i have
    //Then I just need to find the outcome of the stone number and add the outcome number to a new countDict with the count of the stone that converted
    Dictionary<long, List<long>> knownOutcomes = new Dictionary<long, List<long>>(); //result is list because of split rule

    Dictionary<long, long> PredictBlink(Dictionary<long, long> stones)
    {
        var newStones = new Dictionary<long, long>();
        List<long> outcome=null;
        foreach (var stone in stones)
        {
            if (!knownOutcomes.ContainsKey(stone.Key))
            {
                outcome = Blink(new List<long> {stone.Key});
                knownOutcomes.Add(stone.Key, outcome);
            }
            else
            {
                outcome = knownOutcomes[stone.Key];
            }

            foreach (var resultStone in outcome)
            {
                newStones.TryGetValue(resultStone, out long currentCount);
                newStones[resultStone] = currentCount + stone.Value;
            }
        }
        return newStones;
    }
}

