
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

internal class Day14 : Day
{
    
    public class Bot
    {
        public V2 pos;
        public V2 vel;
        public int x { get { return pos.x; } set { pos = new V2(value,pos.y); } }
        public int y { get { return pos.y; } set { pos = new V2(pos.x, value); } }
    }
    public class V2 : Tuple<int, int>
    {
        public V2(int item1, int item2) : base(item1, item2) { }
        public int x => Item1;
        public int y => Item2;

        public static V2 operator +(V2 a, V2 b) => new V2(a.x + b.x, a.y + b.y);
    }
    public List<Bot> ParseInput(string input)
    {
        List<Bot> bots = new List<Bot>();
        foreach (var line in input.Split("\r\n"))
        {
            var split = line.Split(new[] {'=',' ',','});
            bots.Add(new Bot() {pos = new V2(int.Parse(split[1]), int.Parse(split[2])), vel = new V2(int.Parse(split[4]), int.Parse(split[5])) });
        };
        return bots;
    }

    public string Star1(string input, bool example = false)
    {
        var bots = ParseInput(input);
        var size = new V2(bots.Max(b=>b.x)+1, bots.Max(b => b.y)+1);
        bots = SimulateBots(bots, size, 100);
        DebugBots(bots, size);
        return SafetyFactor(bots, size).ToString();
    }
    public string Star2(string input, bool example = false)
    {
        return "";
    }
    public List<Bot> SimulateBots(List<Bot> bots, V2 size, int seconds = 1)
    {
        for (int i = 1; i <= seconds; i++)
        {
            foreach (var bot in bots)
            {
                bot.pos += bot.vel;
                if(bot.x >= size.x-1 || bot.x < 0)
                {
                    bot.x = (bot.x + size.x) % size.x;
                }
                if (bot.y >= size.y-1 || bot.y < 0)
                {
                    bot.y = (bot.y + size.y) % size.y;
                }
            }
        }
        return bots;
    }

    void DebugBots(List<Bot> bots, V2 size)
    {
        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                var count = bots.Where(b => b.x == x && b.y == y).Count();
                Console.Write(count == 0 ? '.' : count.ToString());
            }
            Console.WriteLine();
        }
    }
    int SafetyFactor(List<Bot> bots, V2 size)
    {
        int midX = size.x / 2;
        int midY = size.y / 2;
        var Q1 = bots.Where(b => b.x < midX && b.y < midY).Count();  // Top-left
        var Q2 = bots.Where(b => b.x > midX && b.y < midY).Count();  // Top-right
        var Q3 = bots.Where(b => b.x < midX && b.y > midY).Count();  // Bottom-left
        var Q4 = bots.Where(b => b.x > midX && b.y > midY).Count();  // Bottom-right

        return Q1* Q2 * Q3 * Q4;
    }
}

