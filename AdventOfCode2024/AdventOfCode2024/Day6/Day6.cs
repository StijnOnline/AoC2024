using System.Data;

internal class Day6 : Day
{
    public struct GuardPos : IEquatable<GuardPos>
    {
        public V2 pos;
        public V2 dir;
        public GuardPos(V2 pos, V2 dir) { this.pos = pos; this.dir = dir; }
        public int x => pos.x;
        public int y => pos.y;

        public bool Equals(GuardPos other) => pos == other.pos && dir == other.dir;

        public GuardPos Next => new GuardPos(pos + dir, dir);

        public GuardPos Rotated()
        {
            if (dir.y == -1) return new GuardPos(pos, new V2(1, 0)) ;
            if (dir.x == 1) return new GuardPos(pos, new V2(0, 1));
            if (dir.y == 1) return new GuardPos(pos, new V2(-1, 0));
            if (dir.x == -1) return new GuardPos(pos, new V2(0, -1));
            throw new NotImplementedException();
        }
    }

    public struct V2 : IEquatable<V2>
    {
        public V2(int x, int y) { this.x = x;this.y = y; }
        public int x;
        public int y;

        public bool Equals(V2 other) => x == other.x && y == other.y;
        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
        public static bool operator ==(V2 a, V2 b) => a.Equals(b);
        public static bool operator !=(V2 a, V2 b) => !a.Equals(b);
    }

    //top left = (0,0)
    HashSet<V2> ParseInput(string input, out V2 startPos)
    {
        startPos = new V2(-1,-1);
        HashSet<V2> obstacles = new HashSet<V2>();
        foreach (var (line, y) in input.Split("\r\n").Select((line, y) => (line, y)))
        {
            foreach (var (c, x) in line.Select((c, x) => (c, x)))
            {
                if (c == '#') obstacles.Add(new V2(x, y));
                if (c == '^') startPos = new V2(x, y);
            }
        }
        return obstacles;
    }

    static V2 startDir = new V2(0, -1);

    public string Star1(string input, bool example = false)
    {
        var obstacles = ParseInput(input, out V2 pos);
        return GetPath(obstacles, new GuardPos(pos, startDir), out _).Select(p=>p.pos).Distinct().Count().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var obstacles = ParseInput(input, out V2 startPos);
        var normalPath = GetPath(obstacles, new GuardPos(startPos, startDir), out _);
        int normalLength = normalPath.Count();

        HashSet<V2> possibleObstacles = new HashSet<V2>();
        foreach (var obstaclePoint in normalPath.Select(gp => gp.pos))
        {
            if (obstaclePoint.x == startPos.x && obstaclePoint.y == startPos.y) continue;
            var testObstacles = obstacles.Append(obstaclePoint);
            var loopingPath = GetPath(testObstacles.ToHashSet(), new GuardPos(startPos, startDir), out bool loop);
            if (loop)
                possibleObstacles.Add(obstaclePoint);
        }
        return possibleObstacles.Count().ToString();
    }

    public HashSet<GuardPos> GetPath(HashSet<V2> obstacles, GuardPos pos, out bool loop)
    {
        int maxX = obstacles.MaxBy(p => p.x).x;
        int maxY = obstacles.MaxBy(p => p.y).y;
        HashSet<GuardPos> passedPositions = new HashSet<GuardPos>();
        loop = false;
        while (pos.x >= 0 && pos.x <= maxX && pos.y >= 0 && pos.y <= maxY)
        {
            passedPositions.Add(pos);
            if (obstacles.Contains(pos.Next.pos))
                pos = pos.Rotated();
            else
            {
                pos = pos.Next;
                if (passedPositions.Contains(pos))
                {
                    loop = true;
                    break;
                }
            }
        }
        return passedPositions;
    }

}

