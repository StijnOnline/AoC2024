using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static Day4;

internal class Day4 : Day
{
    char[] ParseInput(string input, out int width, out int height)
    {
        width = input.IndexOf('\r');
        height = input.Count(f => f == '\r') + 1;
        return input.ReplaceLineEndings("").ToCharArray();
    }
    CrossWord CreateCrossWord(string input)
    {
        int width = input.IndexOf('\r');
        int height = input.Count(f => f == '\r')+1;
        CrossWord cw = new CrossWord(input.ReplaceLineEndings("").ToCharArray(), width, height);
        return cw;
    }

    public string Star1(string input, bool example = false)
    {
        var crossword = CreateCrossWord(input);
        int count = crossword.GetAllLines().Select(x => CountText(x, "XMAS")).Sum(); 

        return count.ToString();
    }
    public string Star2(string input, bool example = false)
    {
        //F*ck you star 2, I was creating a nice Crossword class
        int count = 0;
        var characters = ParseInput(input,out int width, out int height);
        for (int y = 1; y < height-1; y++)
        {
            for (int x = 1; x < width-1; x++)
            {
                if (characters[x + y * width] != 'A') continue;
                char topleft = characters[x - 1 + (y - 1) * width];
                char topright = characters[x + 1 + (y - 1) * width];
                char bottomleft = characters[x - 1 + (y + 1) * width];
                char bottomright = characters[x + 1 + (y + 1) * width];
                if (topleft is 'A' or 'X' || topright is 'A' or 'X' || bottomleft is 'A' or 'X' || bottomright is 'A' or 'X') continue;
                if (topleft == bottomright || topright == bottomleft) continue;
                //Console.WriteLine($"X-MAS at {x},{y}");
                count++;
            }
        }
        return count.ToString();
    }


    public int CountText(string line, string text, bool countReverse=true)
    {
        return Regex.Count(line, text) + (countReverse ? Regex.Count(line, Reverse(text)) : 0);
    }
    public string Reverse(string text)
    {
        if (text == null) return null;
        char[] array = text.ToCharArray();
        Array.Reverse(array);
        return new String(array);
    }

    public class CrossWord
    {
        readonly char[] characters;
        readonly int width;
        readonly int height;
        public CrossWord(char[] chars, int width, int height)
        {
            this.characters = chars;
            this.width = width;
            this.height = height;
        }

        public List<string> GetHorizontals()
        {
            List<string> horizontals = new List<string>(height);
            for (int y = 0; y < height; y++)
            {
                char[] line = new char[height];
                for (int x = 0; x < width; x++)
                {
                    line[x] = characters[x + y * width];
                }
                horizontals.Add(new string(line));
            }
            return horizontals;
        }
        public List<string> GetVerticals()
        {
            List<string> verticals = new List<string>(width);
            for (int x = 0; x < width; x++)
            {
                char[] line = new char[height];
                for (int y = 0; y < height; y++)
                {
                    line[y] = characters[x + y * width];
                }
                verticals.Add(new string(line));
            }
            return verticals;
        }
        public List<string> GetDiagonals(bool RightToLeft)
        {
            List<string> diagonals = new List<string>(width);
            //starting from side
            // ....
            // 0...
            // .1..
            // ..2.

            // ....
            // ...0
            // ..1.
            // .2..
            for (int y = height-1; y >= 0 ; y--)
            {
                char[] line = new char[ Math.Min( height-y, width)];
                for (int i = 0; i < line.Length; i++)
                {
                    if (RightToLeft)
                        line[i] = characters[width - 1 - i + (y + i) * width];
                    else
                        line[i] = characters[i + (y + i) * width];
                }
                diagonals.Add(new string(line));
            }
            //continue following along top
            // .0..
            // ..1.
            // ...2
            // ....

            // ..0.
            // .1..
            // 2...
            // ....
            for (int x = 1; x < width; x++)
            {
                char[] line = new char[ Math.Min( width - x, height)];
                for (int i = 0; i < line.Length; i++)
                {
                    if (RightToLeft)
                        line[i] = characters[width - 1 - x - i + i * width];
                    else
                        line[i] = characters[x + i + i * width];
                }
                diagonals.Add(new string(line));
            }
            return diagonals;
        }

        public List<string> GetAllLines()
        {
            return GetHorizontals().Concat(GetVerticals()).Concat(GetDiagonals(false)).Concat(GetDiagonals(true)).ToList();
        }





        //Could make it search a list and return ranges to make this a true Crossword solver
        //public Range[] FindText(string line, string text)

        //playing with iterators
        public IEnumerable<char> GetHorizontal(int y)
        {
            for (int x = 0; x < width; x++)
            {
                yield return characters[x + y * width];
            }
        }
        public IEnumerable<char> GetVertical(int x)
        {
            for (int y = 0; y < height; y++)
            {
                yield return characters[x + y * width];
            }
        }
    }
}