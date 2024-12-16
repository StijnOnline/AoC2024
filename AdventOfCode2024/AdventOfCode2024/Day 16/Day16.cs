
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
internal class Day16 : Day
{
    public struct ReindeerPos
    {
        public V2 pos;
        public V2 dir;
        public ReindeerPos(V2 pos, V2 dir) { this.pos = pos; this.dir = dir; }
        public int x => pos.x;
        public int y => pos.y;

        public ReindeerPos Next => new ReindeerPos(pos + dir, dir);

        public ReindeerPos Rotated()
        {
            if (dir.y == -1) return new ReindeerPos(pos, new V2(1, 0));
            if (dir.x == 1) return new ReindeerPos(pos, new V2(0, 1));
            if (dir.y == 1) return new ReindeerPos(pos, new V2(-1, 0));
            if (dir.x == -1) return new ReindeerPos(pos, new V2(0, -1));
            throw new NotImplementedException();
        }
    }

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

    static V2 startDir = V2.r;

    public string Star1(string input, bool example = false)
    {
        var obstacles = ParseInput(input, out V2 start, out V2 end);
        var nodes = PathFind(obstacles, start, end);
        return nodes[end].Item3.ToString();
    }
    public string Star2(string input, bool example = false)//takes ~10 seconds
    {
        return "";
    }


    public Dictionary<V2, (V2, V2, int)> PathFind(HashSet<V2> obstacles, V2 start, V2 end)
    {
        var closedNodes = new Dictionary<V2, (V2, V2, int)>();// pos, (dir to cheapest start,score)
        var openNodes = new PriorityQueue<(V2, V2, int), int>();//(pos,dir to cheapest start,score) score
        openNodes.Enqueue((start, V2.l, 0),0);

        while (openNodes.Count > 0)
        {
            var node = openNodes.Dequeue();
            if (closedNodes.ContainsKey(node.Item1))
            {
                if(closedNodes[node.Item1].Item3 > node.Item3)
                    closedNodes[node.Item1] = node;
            }
            else
            {
                closedNodes.Add(node.Item1, node);
            }
            if (node.Item1.Equals(end))
            {
                break;
            }

            var checkPos = node.Item1 - node.Item2;
            if (!obstacles.Contains(checkPos) && ! closedNodes.ContainsKey(checkPos))
            {
                openNodes.Enqueue((checkPos, node.Item2, node.Item3 + 1), node.Item3 + 1);
            }

            checkPos = node.Item1 - node.Item2.RotatedLeft();
            if (!obstacles.Contains(checkPos) && !closedNodes.ContainsKey(checkPos))
            {
                openNodes.Enqueue((checkPos, node.Item2.RotatedLeft(), node.Item3 + 1001), node.Item3 + 1001);
            }

            checkPos = node.Item1 - node.Item2.RotatedRight();
            if (!obstacles.Contains(checkPos) && !closedNodes.ContainsKey(checkPos))
            {
                openNodes.Enqueue((checkPos, node.Item2.RotatedRight(), node.Item3 + 1001), node.Item3 + 1001);
            }
        }

        return closedNodes;
    }
}

