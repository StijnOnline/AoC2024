
internal class Day9 : Day
{
    public struct Data
    {
        public Data(int ID, Range range)
        {
            this.ID = ID;
            this.range = range;
        }
        public int ID;
        public Range range;
        public int length => range.length;
        public override string ToString()
        {
            return $"{ID}: [{range.start}-{range.end}] l={length}";
        }
    }
    public struct Range
    {
        public int start;
        public int end;

        public Range(int start, int end)
        {
            this.start = start;
            this.end = end;
        }

        public int length => end-start + 1;
        public override string ToString()
        {
            return $"[{start}-{end}] l={length}";
        }
        public int SumPositions()
        {
            return (length) * (end+start)/2;
        }
    }

    List<Data> ParseInput(string input)
    {
        List<Data> data = new List<Data> (input.Length);
        int startIndex = 0;
        var ID=0;
        for (int i = 0; i < input.Length; i++)
        {
            int value = (input[i] - '0');
            if(i % 2 == 0)
            {
                data.Add(new Data(ID, new Range(startIndex, startIndex + value - 1)));
                ID++;
            }
            startIndex += value;
        }
        return data;
    }
    
    public string Star1(string input, bool example = false)
    {
        var data = ParseInput(input);
        var result = Compress(data.ToList());
        if(example)
            Console.WriteLine(string.Join("", result.Select(d => new string( d.ID.ToString()[0],d.range.length) )));
        return result.Select(d=> (long) d.ID*d.range.SumPositions()).Sum().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var data = ParseInput(input);
        var result = Compress2(data.ToList());
        if (example){
            for (int i = 0; i < result.Count; i++)
            {
                Console.Write(new string(result[i].ID.ToString()[0], result[i].range.length));
                if(i < result.Count-1)
                    Console.Write(new string('.', result[i + 1].range.start - result[i].range.end-1));
            }
            Console.WriteLine();
        }
        return result.Select(d => (long)d.ID * d.range.SumPositions()).Sum().ToString();
        //Guess1: 9863344581804 too high
    }

    public List<Data> Compress(List<Data> data)
    {
        var defragmentedData = new List<Data> (data.Count);
        while (data.Count > 0)
        {
            //Add data from left
            defragmentedData.Add(data[0]);
            if (data.Count <= 1)
            {
                break;
            }
            var gap = new Range(data[0].range.end+1, data[1].range.start-1);
            data.RemoveAt(0);

            var last = data.Last();
            while (data.Count > 0) {
                if (last.Equals(default))
                    break;

                if (gap.length <= last.range.length)
                {
                    defragmentedData.Add(new (last.ID, gap));
                    if (gap.length == last.range.length) {
                        data.Remove(last);
                    }
                    else
                        data[data.Count - 1] = new Data(last.ID, new Range(last.range.start, last.range.end - gap.length));

                    break;
                }
                else
                {
                    defragmentedData.Add(new(last.ID, new Range(gap.start, gap.start + last.range.length-1)));
                    gap = new Range(gap.start + last.range.length, gap.end);
                    data.Remove(last);
                    if (data.Count > 0)
                        last = data.Last();
                }
            }
            if (last.Equals(default))
            {
                break;
            }
        }
        return defragmentedData;
    }

    public List<Data> Compress2(List<Data> data)
    {
        for (int checkIndex = data.Count-1; checkIndex > 0; checkIndex--)
        {
            var lengthReq = data[checkIndex].length;

            for (int j = 0; j < checkIndex; j++)
            {
                var gap = data[j + 1].range.start - data[j].range.end - 1;
                if (gap < lengthReq)
                    continue;

                data.Insert(j + 1, new Data(data[checkIndex].ID, new Range(data[j].range.end + 1, data[j].range.end + lengthReq)));
                data.RemoveAt(checkIndex+1);
                checkIndex++;
                break;
            }

        }
        return data;
    }
}

