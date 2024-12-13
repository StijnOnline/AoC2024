
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

internal class Day13 : Day
{
    public List<int[]> ParseInput(string input)
    {
        var matches = Regex.Matches(input, @"\d+");
        var machines = new List<int[]>();
        for (int i = 0; i < matches.Count; i += 6)
        {
            machines.Add(matches.Skip(i).Take(6).Select(m => int.Parse(m.Value)).ToArray());
        }
        return machines;
    }

    public string Star1(string input, bool example = false)
    {
        var machines = ParseInput( input);
        return machines.Sum(m=>TokensToWin(m)).ToString();

        //Guess2 23395 too low
    }
    public string Star2(string input, bool example = false)
    {
        return "";
    }

    /*
    Math ._.
    
    X = Xa*A + Xb*B  
    Y = Ya*A + Yb*B  

    B = (X - Xa*A) / Xb                     (Reformat to B=)  
    Y = Ya*A + Yb*((X - Xa*A)/Xb)           (Replace B in Y)  
    Y = Ya*A + (Yb*X)/Xb - (Yb*Xa*A)/Xb     (Distribute Yb)  
    Y = (Ya - (Yb*Xa)/Xb)*A + (Yb*X)/Xb     (Combine A terms)  
    Y - (Yb*X)/Xb = (Ya - (Yb*Xa)/Xb)*A     (Move terms)  
    A = (Y - (Yb*X)/Xb) / (Ya - (Yb*Xa)/Xb) (Reformat to A=)  
    
    numerator = Y - (Yb*X)/Xb
    denominator = Ya - (Yb*Xa)/Xb

    (for denominator != 0)
    */

    public int? SolveA(decimal Xa, decimal Ya, decimal Xb, decimal Yb, decimal X, decimal Y)
    {
        var denominator = Ya - (Yb * Xa) / Xb;
        if (denominator == 0) 
            return null;
        var numerator = Y - (Yb * X) / Xb;
        var A = (numerator / denominator);
        return decimal.IsInteger(A) ? (int) A : null;
    }
    public int SolveB(decimal A,decimal X, decimal Xa, decimal Xb)
    {
        return (int)((X - Xa * A) / Xb);
    }


    //just so i can try spread operator
    public int? SolveA_spread(params int[] l) => SolveA(l[0], l[1], l[2], l[3], l[4], l[5]);
    public int SolveB_spread(int A,params int[] l) => SolveB(A,l[4], l[0], l[2]);

    public int TokensToWin(int[] machineNumbers)
    {
        var A = SolveA_spread(machineNumbers);
        if (!A.HasValue) return 0;
        var B = SolveB_spread(A.Value, machineNumbers);
        return A.Value * 3 + B;
    }

}

