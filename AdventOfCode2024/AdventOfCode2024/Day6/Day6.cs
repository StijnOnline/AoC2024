using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using System.Numerics;
using static System.Runtime.InteropServices.JavaScript.JSType;

internal class Day6 : Day
{

    public class Point : Tuple<int, int>, IEquatable<Point>
    {
        public Point(int item1, int item2) : base(item1, item2) { }
        public int x => Item1;
        public int y => Item2;

        public bool Equals(Point? other) => x == other.x && y == other.y;

        public static Point operator +(Point a, Point b)  => new Point(a.x + b.x, a.y + b.y);
    }

    HashSet<Point> ParseInput(string input, out Point startPos)
    {
        // ugly and nonoptimal use of linq: need to select point+char as tuple, then filter by char, then select point
        //return input.Split("\r\n").SelectMany((line, y)=>line.Select((c,x)=> ( new Point(x,y), c)).Where(t => t.c == '#').Select(t=>t.Item1)).ToHashSet();
        
        startPos = null;
        HashSet < Point > obstacles = new HashSet<Point>();
        foreach (var (line, y) in input.Split("\r\n").Select((line, y)=> (line, y)))
        {
            foreach (var (c, x) in line.Select((c, x)=> (c, x)))
            {
                if (c == '#') obstacles.Add(new Point(x, y));
                if (c == '^') startPos = new Point(x, y);
            }
        }
        return obstacles;
    }

    //top left = (0,0)
    public string Star1(string input, bool example = false)
    {
        var obstacles = ParseInput(input, out Point pos);
        int maxX = obstacles.MaxBy(p => p.x).x;
        int maxY = obstacles.MaxBy(p => p.y).y;
        var dir = new Point(0, -1);
        HashSet<Point> passedPositions = new HashSet<Point>();
        while (pos.x >= 0 && pos.x <= maxX && pos.y >= 0 && pos.y <= maxY)
        {
            passedPositions.Add(pos);
            if (obstacles.Contains(pos + dir))
                dir = Rotate(dir);
            else
                pos += dir;
        }
        return passedPositions.Count().ToString();
    }
    public string Star2(string input, bool example = false)
    {
        return "";
    }

    public Point Rotate(Point p)
    {
        if(p.y == -1) return new Point(1,0);
        if(p.x == 1) return new Point(0,1);
        if(p.y == 1) return new Point(-1,0);
        if(p.x == -1) return new Point(0,-1);
        return null;
    }

}

