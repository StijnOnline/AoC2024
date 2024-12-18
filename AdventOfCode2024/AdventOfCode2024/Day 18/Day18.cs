
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Drawing;

internal class Day18 : Day
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
    List<V2> ParseInput(string[] lines, out V2 size)
    {
        List<V2> obstacles = new List<V2>();
        foreach (var (line, y) in lines.Select((line, y) => (line, y)))
        {
            var split = line.Split(',').Select(int.Parse).ToList();
            obstacles.Add(new V2(split[0], split[1]));
        }
        size = new V2(obstacles.Max(p=>p.x)+1, obstacles.Max(p => p.y)+1);
        return obstacles;
    }


    static List<V2> directions = new List<V2> { new V2(0, 1), new V2(0, -1), new V2(1, 0), new V2(-1, 0) };
    public string Star1(string input, bool example = false)
    {
        var bytesToUse = example ? 12 : 1024;
        var obstacles = ParseInput(input.Split("\r\n").Take(bytesToUse).ToArray(), out V2 size);
        List<(V2, int, (V2, int))> NewNodes((V2, int, (V2, int)) node)
        {
            List<(V2, int, (V2, int))> newNodes = new List<(V2, int, (V2, int))>();
            foreach (var dir in directions)
            {
                var checkPos = node.Item1 + dir;
                if (checkPos.x < 0 || checkPos.x >= size.x || checkPos.y < 0 || checkPos.y >= size.y)
                    continue;
                if (obstacles.Contains(checkPos))
                    continue;
                newNodes.Add((checkPos, node.Item2 + 1, (node.Item1, node.Item2)));

            }
            return newNodes;
        }
        var test = PathFindAll((V2.z, 0, (V2.z, 0)), NewNodes);
        return test[size-new V2(1,1)].Item1.ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var allObstacles = ParseInput(input.Split("\r\n").ToArray(), out V2 size);
        var bytesToUse = example ? 12 : 1024;
        List<V2> obstacles = allObstacles.Take(bytesToUse).ToList();
        List<(V2, int, (V2, int))> NewNodes((V2, int, (V2, int)) node)
        {
            List<(V2, int, (V2, int))> newNodes = new List<(V2, int, (V2, int))>();
            foreach (var dir in directions)
            {
                var checkPos = node.Item1 + dir;
                if (checkPos.x < 0 || checkPos.x >= size.x || checkPos.y < 0 || checkPos.y >= size.y)
                    continue;
                if (obstacles.Contains(checkPos))
                    continue;
                newNodes.Add((checkPos, node.Item2 + 1, (node.Item1, node.Item2)));

            }
            return newNodes;
        }

        V2 target = size - new V2(1, 1);
        var closedNodes = PathFindAll((V2.z, 0, (V2.z, 0)), NewNodes);
        var path = GetPath(closedNodes, V2.z, target);
        int i = bytesToUse;
        for (; i < allObstacles.Count; i++)
        {
            obstacles.Add(allObstacles[i]);
            if (!path.Contains(allObstacles[i])) 
                continue;

            closedNodes = PathFindAll((V2.z, 0, (V2.z, 0)), NewNodes);
            if (!closedNodes.ContainsKey(target))
                break;
            path = GetPath(closedNodes, V2.z, target);
        }

        Console.WriteLine("Grid Blocked at O");
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                if (allObstacles[i].Equals(new V2(x, y)))
                {
                    Console.Write('X');
                    continue;
                }

                if (path.Contains(new V2(x, y)))
                {
                    Console.Write('O');
                    continue;
                }

                if (obstacles.Contains(new V2(x, y)))
                {
                    Console.Write('#');
                    continue;
                }

                Console.Write('.');
            }
            Console.WriteLine();
        }
        

        return allObstacles[i].ToString();
        //lol don't forget to format as 'X,Y' without spaces
    }


    public Dictionary<K, (int, List<(K, int)>)> PathFindAll<K>((K, int, (K, int)) startNode, Func<(K, int, (K, int)), List<(K, int, (K, int))>> GetNewNodes)
    {
        var closedNodes = new Dictionary<K, (int, List<(K, int)>)>();// (pos,dir) => (score, list of previous(pos, dir))
        var openNodes = new PriorityQueue<(K, int, (K, int)), int>();//(pos,dir to cheapest start,score, prevnode.Item1) score
        openNodes.Enqueue(startNode, 0);

        while (openNodes.Count > 0)
        {
            var node = openNodes.Dequeue();
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

