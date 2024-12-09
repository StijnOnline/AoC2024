
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
        public override string ToString()
        {
            return $"{ID}: [{range.start}-{range.end}] l={range.length}";
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
        for (int i = 0; i < input.Length; i++)
        {
            int value = (input[i] - '0');
            var ID = i % 2 == 0 ? i / 2 : -1;
            data.Add(new Data(ID, new Range(startIndex, startIndex + value-1)));
            startIndex += value;
        }
        return data;
    }
    
    public string Star1(string input, bool example = false)
    {
        var data = ParseInput(input);
        var result = Defrag(data.ToList());
        if(example)
            Console.WriteLine(string.Join("", result.Select(d => new string( d.ID.ToString()[0],d.range.length) )));
        return result.Select(d=> (long) d.ID*d.range.SumPositions()).Sum().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        return "";
    }

    public List<Data> Defrag(List<Data> data)
    {
        var defragmentedData = new List<Data> (data.Count);
        while (data.Count > 0)
        {
            //Add data from left
            defragmentedData.Add(data[0]);
            data.RemoveAt(0);

            //get gap
            if(data.Count == 0)
                break;
            var gap = data[0].range;
            data.RemoveAt(0);

            if (data.Count == 0){
                break;
            }
            var last = data.Last();
            while (data.Count > 0) {
                if (gap.length <= last.range.length)
                {
                    defragmentedData.Add(new (last.ID, gap));
                    if (gap.length == last.range.length) {
                        data.RemoveAt(data.Count - 1);
                        if(data.Count>0) 
                            data.RemoveAt(data.Count - 1);
                    }
                    else
                        data[data.Count - 1] = new Data(last.ID, new Range(last.range.start, last.range.end - gap.length));

                    break;
                }
                else
                {
                    defragmentedData.Add(new(last.ID, new Range(gap.start, gap.start + last.range.length-1)));
                    gap = new Range(gap.start + last.range.length, gap.end);
                    data.RemoveAt(data.Count - 1);
                    if (data.Count > 0)
                        data.RemoveAt(data.Count - 1);
                    last = data.Last();
                }
            }
        }
        return defragmentedData;
    }
}

