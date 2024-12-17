
internal class Day16 : Day
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


    public string Star1(string input, bool example = false)
    {
        var obstacles = ParseInput(input, out V2 start, out V2 end);
        var nodes = PathFind(obstacles, start, end);
        return nodes.Where(p => p.Key.Item1.Equals(end)).MinBy(p => p.Value.Item1).Value.Item1.ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var obstacles = ParseInput(input, out V2 start, out V2 end);
        var nodes = PathFind(obstacles, start, end);

        var bestPositions = new HashSet<V2>();
        var endNode = nodes.Where(p => p.Key.Item1.Equals(end));
        endNode = endNode.Where(p => p.Value.Item1 == endNode.Min(p2=>p2.Value.Item1)).ToList();
        bestPositions.Add(end);
        var checkPositions = endNode.SelectMany(pair=>pair.Value.Item2.Select(posDir=>(posDir.Item1,posDir.Item2))).ToList();
        while (checkPositions.Count > 0)
        {
            var position = checkPositions[0];

            checkPositions.RemoveAt(0);

            bestPositions.Add(position.Item1);
            if (position.Item1.Equals(start))
                break;
            var previousNodes = nodes[position].Item2.ToList();
            var newnodes = previousNodes.Select(n=>(n.Item1,n.Item2)).Distinct().ToList();
            checkPositions.AddRange(newnodes);
        }

        if(example)
        for (int y = 0; y < 20; y++)
        {
            for (int x = 0; x < 20; x++)
            {
                if(obstacles.Contains(new V2(x,y))){
                    Console.Write('#');
                    continue;
                }

                if (bestPositions.Contains(new V2(x, y)))
                {
                    Console.Write('O');
                    continue;
                }

                Console.Write('.');
            }
            Console.WriteLine();
        }

        return bestPositions.Count().ToString();
    }

    //tried doing path finding algorithm from memory
    //but this case with unique orientations and storing data to deduce all best paths made it hard
    //also used many tuples, not ideal for clarity

    public Dictionary<(V2, V2), (int, List<(V2, V2, int)>)> PathFind(HashSet<V2> obstacles, V2 start, V2 end)
    {
        var closedNodes = new Dictionary<(V2, V2),(int, List<(V2, V2,int)>)>();// (pos,dir) => (score, list of previous(pos, dir))
        var openNodes = new PriorityQueue<(V2, V2, int, (V2, V2, int)), int>();//(pos,dir to cheapest start,score, prevPosDir) score
        openNodes.Enqueue((start, V2.r, 0, (V2.z,V2.z,0)), 0);

        while (openNodes.Count > 0)
        {
            var node = openNodes.Dequeue();
            var posDir = (node.Item1, node.Item2);
            if (closedNodes.Any(p=> p.Key.Item1.Equals(node.Item1) && p.Key.Item2.Equals(node.Item2 * -1) && p.Value.Item1 < node.Item3))//going against existing dir with already lower score
            {
                continue;
            }
            if (closedNodes.ContainsKey(posDir))
            {
                if (closedNodes[posDir].Item1 == node.Item3)//equal, add previous position
                {
                    if (!closedNodes[posDir].Item2.Any(posDir => posDir.Equals(node.Item4)))
                    {
                        closedNodes[posDir].Item2.Add(node.Item4);
                    }
                }
                else if (node.Item3 < closedNodes[posDir].Item1)//better, overwrite previous positions
                {
                    closedNodes[posDir] = (node.Item3, new List<(V2,V2, int)>() { node.Item4 });
                }
                continue;
            }
            closedNodes.Add(posDir, (node.Item3, new List<(V2, V2, int)>() { node.Item4 }));

            var checkPos = node.Item1 + node.Item2;
            if (!obstacles.Contains(checkPos) && !closedNodes.ContainsKey((checkPos, node.Item2)))
            {
                openNodes.Enqueue((checkPos, node.Item2, node.Item3 + 1, (node.Item1,node.Item2, node.Item3)), node.Item3 + 1);
            }

            checkPos = node.Item1 + node.Item2.RotatedLeft();
            if (!obstacles.Contains(checkPos))
            {
                openNodes.Enqueue((node.Item1, node.Item2.RotatedLeft(), node.Item3 + 1000, (node.Item1, node.Item2, node.Item3)), node.Item3 + 1000);
            }

            checkPos = node.Item1 + node.Item2.RotatedRight();
            if (!obstacles.Contains(checkPos))
            {
                openNodes.Enqueue((node.Item1, node.Item2.RotatedRight(), node.Item3 + 1000, (node.Item1, node.Item2, node.Item3)), node.Item3 + 1000);
            }
        }

        return closedNodes;
    }
}

