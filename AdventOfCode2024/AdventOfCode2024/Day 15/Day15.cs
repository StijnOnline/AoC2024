
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
internal class Day15 : Day
{
    public class V2 : Tuple<int, int>
    {
        public V2(int item1, int item2) : base(item1, item2) { }
        public int x => Item1;
        public int y => Item2;

        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
        public static V2 operator *(V2 a, int b) => new V2(a.x * b, a.y * b);
        public static V2 l => new V2(-1,0);
        public static V2 r => new V2(1,0);
        public static V2 u => new V2(0,-1);
        public static V2 d => new V2(0,1);
        public static V2 z => new V2(0,0);
    }

    void ParseInput(string input, out List<V2> walls, out List<V2> boxes, out V2 startPos, out List<V2> moves)
    {
        var split = input.Split("\r\n\r\n");
        var mapLines = split[0].Split("\r\n");
        walls = new List<V2>();
        boxes = new List<V2>();
        startPos = null;
        for (int y = 0; y < mapLines.Length; y++)
        {
            for (int x = 0; x < mapLines[y].Length; x++)
            {
                if(mapLines[y][x] == '#')
                    walls.Add(new V2(x, y));

                if (mapLines[y][x] == 'O')
                    boxes.Add(new V2(x, y));

                if (mapLines[y][x] == '@')
                    startPos = new V2(x, y);
            }
        }

        V2 dir(char c)
        {
            if (c is '^') return V2.u;
            if (c is 'v') return V2.d;
            if (c is '>') return V2.r;
            if (c is '<') return V2.l;
            return null;
        }
        moves = split[1].ReplaceLineEndings("").Select(dir).ToList();
    }

    public string Star1(string input, bool example = false)
    {
        ParseInput(input, out List<V2> walls, out List<V2> boxes, out V2 startPos, out List<V2> moves);
        SimRobot(walls, boxes, startPos, moves);
        return boxes.Select(b=> b.x + b.y * 100).Sum().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        ParseInput(input, out List<V2> walls, out List<V2> boxes, out V2 startPos, out List<V2> moves);
        walls = walls.SelectMany(b => new[]{ new V2(b.x * 2, b.y), new V2(b.x * 2+1, b.y) }).ToList();
        boxes = boxes.Select(b => new V2(b.x * 2, b.y)).ToList();
        startPos = new V2(startPos.x * 2, startPos.y);
        SimRobot2(walls, boxes, startPos, moves);
        return boxes.Select(b => b.x + b.y * 100).Sum().ToString();
    }


    void SimRobot(List<V2> walls, List<V2> boxes, V2 pos, List<V2> moves)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            int j = 1;
            var nextPos = pos + moves[i] * j;

            for (; boxes.Contains(nextPos); j++)
            {
                nextPos = pos + moves[i] * j;
            }

            if (walls.Contains(nextPos)) 
                continue;

            if (j > 1)
            {
                boxes.Add(nextPos);
                boxes.Remove(pos + moves[i]);
            }

            pos += moves[i];
        }
    }

    void SimRobot2(List<V2> walls, List<V2> boxes, V2 pos, List<V2> moves)
    {
        for (int i = 0; i < moves.Count; i++)
        {
            var pushBoxes = new List<V2>();
            if (AttemptStep(walls, boxes, pos, moves[i], pushBoxes))
            {
                pos += moves[i];
                foreach (var box in pushBoxes)
                {
                    boxes.Remove(box);
                    boxes.Add(box + moves[i]);
                }
            }
        }
    }

    bool AttemptStep(List<V2> walls, List<V2> boxes, V2 checkPos, V2 dir, List<V2> pushBoxes)
    {
        var nextPos = checkPos+dir;

        bool isBox = IsBox(boxes, nextPos, out V2 boxPos);
        bool isWall = walls.Contains(nextPos);
        if (isWall)
            return false;
        if(!isBox) 
            return true;

        if (pushBoxes.Contains(boxPos))//because we always check left and right side of box, ignore already checked boxes
            return true;

        pushBoxes.Add(boxPos);
        bool success = AttemptStep(walls, boxes,  boxPos + (dir.x == 1 ? V2.r : V2.z), dir, pushBoxes);
        success &= (dir.y==0 || AttemptStep(walls, boxes, boxPos + V2.r, dir, pushBoxes));
        return success;
    }

    bool IsBox(List<V2> boxes, V2 p, out V2 boxPos)
    {
        boxPos = null;
        if (boxes.Contains(p))
        {
            boxPos = p;
            return true;
        }
        if (boxes.Contains(p + V2.l))
        {
            boxPos = p + V2.l;
            return true;
        }
        return false;
    }
}

