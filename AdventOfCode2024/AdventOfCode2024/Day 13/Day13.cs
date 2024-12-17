
using System.Diagnostics;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;

internal class Day13 : Day
{
    public List<long[]> ParseInput(string input)
    {
        var matches = Regex.Matches(input, @"\d+");
        var machines = new List<long[]>();
        for (int i = 0; i < matches.Count; i += 6)
        {
            machines.Add(matches.Skip(i).Take(6).Select(m => long.Parse(m.Value)).ToArray());
        }
        return machines;
    }

    public string Star1(string input, bool example = false)
    {
        var machines = ParseInput( input);
        return machines.Sum(m=>TokensToWin(m)).ToString();
    }
    public string Star2(string input, bool example = false)
    {
        var machines = ParseInput(input);
        foreach (var machine in machines)
        {
            machine[4] += 10000000000000;
            machine[5] += 10000000000000;
        }
        return machines.Sum(m => TokensToWin(m)).ToString();
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

    public long? SolveA(decimal Xa, decimal Ya, decimal Xb, decimal Yb, decimal X, decimal Y)
    {
        var denominator = Ya - (Yb * Xa) / Xb;
        if (denominator==0) 
            return null;
        var numerator = Y - (Yb * X) / Xb;
        var A = (numerator / denominator);
        if (Math.Abs(A - Math.Round(A)) < 1e-10m)
            return (long)(Math.Round(A));
        else 
            return null;
    }
    public long SolveB(decimal A,decimal X, decimal Xa, decimal Xb)
    {
        return (long)Math.Round((X - Xa * A) / Xb);
    }


    //just so i can try spread operator
    public long? SolveA_spread(params long[] l) => SolveA(l[0], l[1], l[2], l[3], l[4], l[5]);
    public long SolveB_spread(long A,params long[] l) => SolveB(A,l[4], l[0], l[2]);

    public long TokensToWin(long[] machineNumbers)
    {
        var A = SolveA_spread(machineNumbers);
        if (!A.HasValue) 
            return 0;
        var B = SolveB_spread(A.Value, machineNumbers);

        var X = machineNumbers[0] * A + machineNumbers[2] * B;
        if (machineNumbers[4] != X)
            return 0;

        var Y = machineNumbers[1] * A + machineNumbers[3] * B;
        if (machineNumbers[5] != Y)
            return 0;

        return A.Value * 3 + B;
    }

}

