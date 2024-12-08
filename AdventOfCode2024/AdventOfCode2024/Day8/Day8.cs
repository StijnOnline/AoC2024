
using System.Collections.Generic;

internal class Day8 : Day
{
    public class V2 : Tuple<int, int>
    {
        public V2(int item1, int item2) : base(item1, item2) { }
        public int x => Item1;
        public int y => Item2;

        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
        public static V2 operator -(V2 a, V2 b) => new V2(a.x - b.x, a.y - b.y);
        public static V2 operator *(V2 a, int b) => new V2(a.x * b, a.y * b);
        public override string ToString()
        {
            return $"({x},{y})";
        }
    }

    Dictionary<char, List<V2>> ParseInput(string input, out int width, out int height)
    {
        width = input.IndexOf('\r');
        height = input.Count(f => f == '\r') + 1;
        Dictionary<char,List<V2>> antennas = new Dictionary<char, List<V2>>();
        foreach (var (line, y) in input.Split("\r\n").Select((line, y) => (line, y)))
        {
            foreach (var (c, x) in line.Select((c, x) => (c, x)))
            {
                if (c is '.' or '#') continue;
                if(antennas.ContainsKey(c))
                    antennas[c].Add(new V2(x, y));
                else
                    antennas.Add(c,new List <V2>{ new V2(x, y)});
            }
        }
        return antennas;
    }

    public string Star1(string input, bool example = false)
    {
        var antennas = ParseInput(input, out int width, out int height);
        var antiNodes = antennas.SelectMany(a => GetAntiNodes(a.Value, width,height)).Distinct();
        return antiNodes.Count().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var antennas = ParseInput(input, out int width, out int height);
        var antiNodes = antennas.SelectMany(a => GetAntiNodes(a.Value, width, height, true)).Distinct();
        return antiNodes.Count().ToString();
    }

    public List<V2> GetAntiNodes(List<V2> antennas, int width,int height, bool star2 = false)
    {
        List<V2> antiNodes = new List<V2>();
        for (int i = 0; i < antennas.Count; i++)
        {
            for (int j = 0; j < antennas.Count; j++)
            {
                if (i == j) continue;
                var delta = antennas[j] - antennas[i];
                int k = star2 ? 0 : 1;
                V2 p;
                do
                {
                    p = antennas[j] + delta * k;
                    if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) break;
                    antiNodes.Add(p);
                    k++;
                } while (star2);
            }
        }
        return antiNodes;
    }
}

