
using System.Diagnostics;
using System.Linq;
using static Day12;

internal class Day12 : Day
{
    public class Region
    {
        public string guid = Guid.NewGuid().ToString();
        public char character;
        public List<V2> positions = new List<V2>();
        public Region(char character, V2 startPos)
        {
            this.character = character;
            positions = new List<V2>() { startPos };
        }
        public override string ToString() => $"Region [{character}]: {positions.Count} positions guid:{guid}";
    
        public int Area = 1;
        public int Perimeter = 4;
        public int Price => Area * Perimeter;
        public int Price2 => Area * Sides;

        //Do a series of 'Raycasts' that stores the 'depth' each time it steps inside/outide the shape
        //Then add the amount of 'depths' that are different from last raycast
        public int Sides
        {
            get
            {
                int sides = 0;
                var min = new V2(positions.Min(p => p.x), positions.Min(p => p.y));
                var max = new V2(positions.Max(p => p.x), positions.Max(p => p.y));
                HashSet<int> StepInside = new HashSet<int>();//only count unique 'depths'
                HashSet<int> StepOutside = new HashSet<int>();
                HashSet<int> LastStepInside = new HashSet<int>();
                HashSet<int> LastStepOutside = new HashSet<int>();
                bool inside = false;
                //Horizontal Raycasts
                for (int y = 0; y <= max.y - min.y; y++)
                {
                    sides += StepInside.Except(LastStepInside).Count() + StepOutside.Except(LastStepOutside).Count();

                    LastStepInside = StepInside;
                    LastStepOutside = StepOutside;
                    StepInside = new HashSet<int>();
                    StepOutside = new HashSet<int>();
                    inside = false;
                    for (int x = 0; x <= max.x - min.x; x++)
                    {
                        var pos = new V2(min.x + x, min.y + y);
                        if (positions.Contains(pos))
                        {
                            if (!inside) StepInside.Add(x);
                            inside = true;
                        }
                        else
                        {
                            if (inside) StepOutside.Add(x);
                            inside = false;
                        }
                    }
                    if (inside) StepOutside.Add(max.x - min.x + 1);
                }
                sides += StepInside.Except(LastStepInside).Count() + StepOutside.Except(LastStepOutside).Count();
                StepInside = new HashSet<int>();
                StepOutside = new HashSet<int>();

                //Vertical Raycasts
                for (int x = 0; x <= max.x - min.x; x++)
                {
                    sides += StepInside.Except(LastStepInside).Count() + StepOutside.Except(LastStepOutside).Count();

                    LastStepInside = StepInside;
                    LastStepOutside = StepOutside;
                    StepInside = new HashSet<int>();
                    StepOutside = new HashSet<int>();
                    inside = false;
                    for (int y = 0; y <= max.y - min.y; y++)
                    {
                        var pos = new V2(min.x + x, min.y + y);
                        if (positions.Contains(pos))
                        {
                            if (!inside) StepInside.Add(y);
                            inside = true;
                        }
                        else
                        {
                            if (inside) StepOutside.Add(y);
                            inside = false;
                        }
                    }
                    if (inside) StepOutside.Add(max.y - min.y + 1);
                }
                sides += StepInside.Except(LastStepInside).Count() + StepOutside.Except(LastStepOutside).Count();
                return sides;
            }
        }
    }
    public class V2 : Tuple<int, int>
    {
        public V2(int item1, int item2) : base(item1, item2) { }
        public int x => Item1;
        public int y => Item2;

        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
    }


    public string Star1(string input, bool example = false)
    {
        MapRegions(input, out List<Region> regions, out Dictionary<V2, Region> regionDict);

        var wrong = regions.Where(r=>r.Area!=r.positions.Count);
        //regions.ForEach(DebugRegion);

        return regions.Sum(r => r.Price).ToString();
    }
    public string Star2(string input, bool example = false)
    {
        MapRegions(input, out List<Region> regions, out Dictionary<V2, Region> regionDict);
        return regions.Sum(r => r.Price2).ToString();
    }

    public void AddPos(Region region, V2 pos, Dictionary<V2, Region> regionDict)
    {
        region.positions.Add(pos);
        regionDict.Add(pos, region);

        region.Area++;
        regionDict.TryGetValue(pos+new V2(-1,0), out Region regionLeft);
        regionDict.TryGetValue(pos+new V2(0,-1), out Region regionUp);
        if (region == regionLeft && region == regionUp)
        {
            return;
        }
        region.Perimeter += 2;
    }

    void MapRegions(string input, out List<Region> regions, out Dictionary<V2, Region> regionDict)
    {
        regions = new List<Region>();
        regionDict = new Dictionary<V2, Region>();

        int width = input.IndexOf('\r');
        int height = input.Count(c => c == '\r') + 1;
        var characters = input.ReplaceLineEndings("").ToCharArray();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pos = new V2(x, y);
                var c = characters[x + y * width];

                regionDict.TryGetValue(pos + new V2(-1, 0), out Region regionLeft);
                regionDict.TryGetValue(pos + new V2(0, -1), out Region regionUp);

                //merge regions
                if (regionLeft!=null && regionUp!=null && regionLeft!=regionUp && regionLeft.character == c && regionLeft.character == regionUp.character)
                {
                    regionUp.positions.AddRange(regionLeft.positions);
                    foreach (var p in regionLeft.positions)
                    {
                        regionDict[p] = regionUp;
                    }
                    regionUp.Area += regionLeft.Area;
                    regionUp.Perimeter += regionLeft.Perimeter;
                    regions.Remove(regionLeft);
                    AddPos(regionUp, pos, regionDict);
                    continue;
                }

                if (regionLeft != null && regionLeft.character == c)
                {
                    AddPos(regionLeft, pos, regionDict);
                    continue;
                }

                if (regionUp != null && regionUp.character == c)
                {
                    AddPos(regionUp, pos, regionDict);
                    continue;
                }

                var region = new Region(c,pos);
                regionDict.Add(pos, region);
                regions.Add(region);

            }
        }
    }

    void DebugRegion(Region region)
    {
        var min = new V2(region.positions.Min(p => p.x), region.positions.Min(p => p.y));
        var max = new V2(region.positions.Max(p => p.x), region.positions.Max(p => p.y));
        for (int y = 0; y <= max.y - min.y; y++)
        {
            for (int x = 0; x <= max.x-min.x; x++)
            {
                var pos = new V2(min.x + x, min.y + y);
                if (region.positions.Contains(pos))
                {
                    Console.Write(region.character);
                }
                else
                {
                    Console.Write(' ');
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine($"Area {region.Area} Permeter: {region.Perimeter}");
    }
}

