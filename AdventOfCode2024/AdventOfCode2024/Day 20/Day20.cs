
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;

internal class Day20 : Day
{
    public class V2 : Tuple<int, int>
    {
        public V2(int item1, int item2) : base(item1, item2) { }
        public int x => Item1;
        public int y => Item2;

        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
        public static V2 operator -(V2 a, V2 b) => a + b * -1;
        public static V2 operator *(V2 a, int b) => new V2(a.x * b, a.y * b);
        public static V2 l => new V2(-1, 0);
        public static V2 r => new V2(1, 0);
        public static V2 u => new V2(0, -1);
        public static V2 d => new V2(0, 1);
        public static V2 z => new V2(0, 0);
        public V2 RotatedLeft()
        {
            if (this.Equals(l)) return d;
            if (this.Equals(d)) return r;
            if (this.Equals(r)) return u;
            if (this.Equals(u)) return l;
            return null;
        }
        public V2 RotatedRight()
        {
            if (this.Equals(l)) return u;
            if (this.Equals(d)) return l;
            if (this.Equals(r)) return d;
            if (this.Equals(u)) return r;
            return null;
        }
    }

    //top left = (0,0)
    HashSet<V2> ParseInput(string input, out V2 startPos, out V2 endPos)
    {
        startPos = new V2(-1, -1);
        endPos = new V2(-1, -1);
        HashSet<V2> obstacles = new HashSet<V2>();
        foreach (var (line, y) in input.Split("\r\n").Select((line, y) => (line, y)))
        {
            foreach (var (c, x) in line.Select((c, x) => (c, x)))
            {
                if (c == '#') obstacles.Add(new V2(x, y));
                if (c == 'S') startPos = new V2(x, y);
                if (c == 'E') endPos = new V2(x, y);
            }
        }
        return obstacles;
    }


    static List<V2> directions = new List<V2> { new V2(0, 1), new V2(0, -1), new V2(1, 0), new V2(-1, 0) };
    public string Star1(string input, bool example = false)
    {
        var obstacles = ParseInput(input, out V2 startPos, out V2 endPos);
        List<(V2, int, (V2, int))> NewNodes((V2, int, (V2, int)) node)
        {
            List<(V2, int, (V2, int))> newNodes = new List<(V2, int, (V2, int))>();
            foreach (var dir in directions)
            {
                var checkPos = node.Item1 + dir;
                if (obstacles.Contains(checkPos))
                    continue;
                newNodes.Add((checkPos, node.Item2 + 1, (node.Item1, node.Item2)));

            }
            return newNodes;
        }
        var nodes = PathFind((startPos, 0, (startPos, 0)), NewNodes, endPos);
        var normalPath = GetPath(nodes, startPos, endPos);
        normalPath.Reverse();

        var cheats = new Dictionary<int, int>();//picoseconds saved, count of how many cheats save that much
        foreach (var pos in normalPath)
        {
            foreach (var dir in directions)
            {
                var checkPos = pos + dir * 2;
                if (!nodes.ContainsKey(checkPos))
                    continue;

                var diff = nodes[checkPos].Item1 - nodes[pos].Item1 - 2;
                if (diff > 0)
                {
                    cheats.TryGetValue(diff, out int count);
                    cheats[diff] = count + 1;
                }
            }
        }


        int cheatsAboveHundred = 0;
        foreach (var item in cheats.OrderBy(p=>p.Key))
        {
            if(example)
                Console.WriteLine($"{item.Value} cheats save {item.Key}");
            if (item.Key >= 100)
            {
                cheatsAboveHundred += item.Value;
            }
        }

        return cheatsAboveHundred.ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var obstacles = ParseInput(input, out V2 startPos, out V2 endPos);
        List<(V2, int, (V2, int))> NewNodes((V2, int, (V2, int)) node)
        {
            List<(V2, int, (V2, int))> newNodes = new List<(V2, int, (V2, int))>();
            foreach (var dir in directions)
            {
                var checkPos = node.Item1 + dir;
                if (obstacles.Contains(checkPos))
                    continue;
                newNodes.Add((checkPos, node.Item2 + 1, (node.Item1, node.Item2)));

            }
            return newNodes;
        }
        var nodes = PathFind((startPos, 0, (startPos, 0)), NewNodes, endPos);
        var normalPath = GetPath(nodes, startPos, endPos);
        normalPath.Reverse();

        var cheats = new Dictionary<int, int>();//picoseconds saved, count of how many cheats save that much
        for (int i = 0; i < normalPath.Count; i++)
        {
            var pos = normalPath[i];
            for (int x = -20; x <= 20; x++)
            {
                for (int y = -20; y <= 20; y++)
                {
                    var checkPos = pos + new V2(x,y);
                    var d = Dist(pos, checkPos);
                    if (d > 20)
                        continue;
                    if (!nodes.ContainsKey(checkPos))
                        continue;

                    var diff = nodes[checkPos].Item1 - nodes[pos].Item1 - d;
                    if (diff > 0)
                    {
                        cheats.TryGetValue(diff, out int count);
                        cheats[diff] = count + 1;
                    }
                }
            }
        }

        int cheatsAboveHundred = 0;
        foreach (var item in cheats.OrderBy(p => p.Key))
        {
            if (example)
                Console.WriteLine($"{item.Value} cheats save {item.Key}");
            if (item.Key >= 100)
            {
                cheatsAboveHundred += item.Value;
            }
        }

        return cheatsAboveHundred.ToString();
    }

    public int Dist(V2 from, V2 to)
    {
        return Math.Abs(from.x - to.x) + Math.Abs(from.y-to.y);
    }

    public Dictionary<K, (int, List<(K, int)>)> PathFind<K>((K, int, (K, int)) startNode, Func<(K, int, (K, int)), List<(K, int, (K, int))>> GetNewNodes, K target)
    {
        var closedNodes = new Dictionary<K, (int, List<(K, int)>)>();// (pos,dir) => (score, list of previous(pos, dir))
        var openNodes = new PriorityQueue<(K, int, (K, int)), int>();//(pos,dir to cheapest start,score, prevnode.Item1) score
        openNodes.Enqueue(startNode, 0);

        while (openNodes.Count > 0)
        {
            var node = openNodes.Dequeue();
            if (node.Item1.Equals(target))
            {
                closedNodes.Add(node.Item1, (node.Item2, new List<(K, int)>() { node.Item3 }));
                break;
            }

            if (closedNodes.ContainsKey(node.Item1))
            {
                if (closedNodes[node.Item1].Item1 == node.Item2)//equal, add previous position
                {
                    if (!closedNodes[node.Item1].Item2.Any(node => node.Item1.Equals(node.Item1)))
                    {
                        closedNodes[node.Item1].Item2.Add(node.Item3);
                    }
                }
                else if (node.Item2 < closedNodes[node.Item1].Item1)//better, overwrite previous positions
                {
                    closedNodes[node.Item1] = (node.Item2, new List<(K, int)>() { node.Item3 });
                }
                continue;
            }
            closedNodes.Add(node.Item1, (node.Item2, new List<(K, int)>() { node.Item3 }));
            var newNodes = GetNewNodes(node).Where(n=> !closedNodes.ContainsKey(n.Item1));
            newNodes = newNodes.Where(n=> !closedNodes.ContainsKey(n.Item1));
            openNodes.EnqueueRange(newNodes.Select(n=>(n,n.Item2)));
        }

        return closedNodes;
    }

    public List<K> GetPath<K>(Dictionary<K, (int, List<(K, int)>)> closedNodes, K start, K target)
    {
        List < K > path = new List < K >();
        K checkPos = target;
        while (!checkPos.Equals(start))
        {
            path.Add(checkPos);
            checkPos = closedNodes[checkPos].Item2.First().Item1;
        }
        path.Add(checkPos);
        return path;
    }
}

