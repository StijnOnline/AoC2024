
internal class Day10 : Day
{

    public class V2 : Tuple<int, int>
    {
        public V2(int item1, int item2) : base(item1, item2) { }
        public int x => Item1;
        public int y => Item2;

        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
    }

    public char[] ParseInput(string input, out int width, out int height, out List<V2> startPositions)
    {
        width = input.IndexOf('\r');
        height = input.Count(c => c == '\r') + 1;
        var characters = input.ReplaceLineEndings("").ToCharArray();
        startPositions = new List<V2>();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (characters[x + y * height] is '0')
                    startPositions.Add(new V2(x, y));
            }
        }
        return characters;
    }

    public string Star1(string input, bool example = false)
    {
        var characters = ParseInput(input, out int width, out int height, out List<V2> startPositions);
        var allPaths = GetPaths(characters, width, height, startPositions);
        if(example) allPaths.Select(p => (p.First(), p.Last())).Distinct().GroupBy(p => p.Item1).Select(g => (g.Key, g.Count())).ToList().ForEach(g=>Console.WriteLine(g));
        return allPaths.Select(p=> (p.First(),p.Last())).Distinct().Count().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var characters = ParseInput(input, out int width, out int height, out List<V2> startPositions);
        var allPaths = GetPaths(characters, width, height, startPositions);
        if (example) allPaths.Select(p => (p.First(), p.Last())).GroupBy(p => p.Item1).Select(g => (g.Key, g.Count())).ToList().ForEach(g => Console.WriteLine(g));
        return allPaths.Select(p => (p.First(), p.Last())).Count().ToString();
    }

    static List<V2> directions = new List<V2>{ new V2(0, 1), new V2(0, -1), new V2(1, 0), new V2(-1, 0) };
    public List<List<V2>> GetPaths(char[] input, int width, int height, IEnumerable<V2> startPositions) { 
        var searchPaths = startPositions.Select(p=> new List<V2> { p }).ToList();
        var correctPaths = new List<List<V2>>();

        while (searchPaths.Count > 0)
        {
            foreach (var dir in directions)
            {
                var path = searchPaths[0];
                var lastPos = path.Last();
                var checkPos = lastPos + dir;
                

                if (IsOneStepUp(input, width, height, lastPos, dir))
                {
                    if (GetCharAt(input, width, height, checkPos) is '9')
                    {
                        correctPaths.Add(path.Append(checkPos).ToList());
                    }
                    else
                    {
                        searchPaths.Add(path.Append(checkPos).ToList());
                    }
                }
            }
            searchPaths.RemoveAt(0);
        }
        

        return correctPaths;
    }
    bool IsOneStepUp(char[] input, int width, int height, V2 pos, V2 dir)
    {
        V2 checkPos = pos + dir;
        if (checkPos.x < 0 || checkPos.x>=width || checkPos.y < 0 || checkPos.y >= height) return false;
            return GetCharAt(input, width, height, pos) == GetCharAt(input, width, height, pos + dir) - 1;
    }
    char GetCharAt(char[] input, int width, int height, V2 pos)
    {
        if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height) return 'E';
        return input[pos.x + pos.y*height];
    }


}

