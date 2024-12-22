
using System.Linq;
using System.Text.RegularExpressions;
using static Day17;

internal class Day17 : Day
{

    ProgramSimulation ParseInput(string input)
    {
        var split = input.Split("\r\n\r\n");
        var matches = Regex.Matches(split[0], @"\d+");
        ProgramSimulation program = new ProgramSimulation();
        program.A = int.Parse(matches[0].Value);
        program.B = int.Parse(matches[1].Value);
        program.C = int.Parse(matches[2].Value);
        program.Program = split[1].Substring(9).Split(',').Select(int.Parse).ToList();
        return program;
    }


    public string Star1(string input, bool example = false)
    {
        var program = ParseInput(input);
        program.Run();
        return string.Join(",", program.output);
    }
    public string Star2(string input, bool example = false)
    {
        var sourceProgram = ParseInput(input);
        if (example) return "Skip";

        //Some hints on reddit helped me but still was a puzzle
        //This took some digging in how the 'program' works

        //By analyzing it we learn that the size of A relates to output length
        //the output contains one value for every octal-bit (or every 3 binary bits)
        //7: 1 long
        //8: 2 long
        //63: 2 long
        //64: 3 long
        //etc
        //required output length is 16
        //so A must have a value in range [2^45, 2^48)      (15*3=45 bits)

        TestOutputLength(sourceProgram);

        //Further we learn that each 'block' of 3 binary bits results in an output value
        //Nothing outside this 'block' affects the output (expect for numbers smaller than 8)
        TestConsistentResult(sourceProgram);

        //We search by each 'block' for partial outputs that match the program
        //Then move to the next block until we match the whole thing
        List<long>? partialSolutions = new List<long>() { ((long)1 << 45) };
        List<long>? nextPartialSolutions = new List<long>();
        for (int block = 15; block >= 0; block--)
        {
            while(partialSolutions.Count > 0)
            {

                long solution = partialSolutions[0];
                partialSolutions.RemoveAt(0);
                var test = solution ^ (((solution >> (block * 3)) % 8) << (block * 3));

                for (long i = 0; i < 8; i++)
                {
                    var program = (ProgramSimulation)sourceProgram.Clone();
                    long setA = test + (i << ((block)*3));
                    program.A = setA; 
                    Console.Write($"{setA} ({Convert.ToString(setA, 8)}): ");
                    program.Run();
                    Console.Write(string.Join(",", program.output));
                    if (program.output.SequenceEqual(program.Program))
                    {
                        nextPartialSolutions.Add(setA);
                        Console.WriteLine($"    Solution!");
                        return setA.ToString();
                    }
                    if (program.output.TakeLast(16-block).SequenceEqual(program.Program.TakeLast(16-block)))
                    {
                        nextPartialSolutions.Add(setA);
                        Console.Write($"    Match!");
                    }
                    
                    Console.WriteLine();
                }
            }
            partialSolutions = nextPartialSolutions.ToList();
            nextPartialSolutions.Clear();
        }

        return "ERR";
    }


    public class ProgramSimulation : ICloneable
    {
        public List<int> Program;
        public long A;
        public long B;
        public long C;
        public int instructionPointer = 0;
        public List<int> output = new List<int>();

        public void Run()
        {
            var ops = new Action[]{ Op0, Op1, Op2, Op3, Op4, Op5, Op6, Op7 };
            while (instructionPointer < Program.Count)
            {
                var op = Program[instructionPointer];
                ops[op].Invoke();
            }
        }

        void Op0()
        {
            A = A >> (int)ComboOperand(Program[instructionPointer + 1]);
            instructionPointer += 2;
        }

        void Op1()
        {
            B = B ^ Program[instructionPointer + 1];
            instructionPointer += 2;
        }
        void Op2()
        {
            B = ComboOperand(Program[instructionPointer + 1]) % 8;
            instructionPointer += 2;
        }
        void Op3()
        {
            if (A != 0)
            {
                instructionPointer = Program[instructionPointer + 1];
                return;
            }
            instructionPointer += 2;
        }
        void Op4()
        {
            B = B ^ C;
            instructionPointer += 2;
        }
        void Op5()
        {
            output.Add((int)(ComboOperand(Program[instructionPointer + 1]) % 8));
            instructionPointer += 2;
        }
        void Op6()
        {
            B = A >> (int)ComboOperand(Program[instructionPointer + 1]);
            instructionPointer += 2;
        }
        void Op7()
        {
            C = A >> (int)ComboOperand(Program[instructionPointer+1]);
            instructionPointer += 2;
        }

        long ComboOperand(int operand)
        {
            if (operand <= 3) return operand;
            if (operand == 4) return A;
            if (operand == 5) return B;
            if (operand == 6) return C;
            throw new NotImplementedException();
        }

        public object Clone()
        {
            var clone = (ProgramSimulation) MemberwiseClone();
            clone.output = new List<int>();
            return clone;
        }
    }

    private static void TestOutputLength(ProgramSimulation sourceProgram)
    {
        Console.WriteLine("Test output length:");
        var program = (ProgramSimulation)sourceProgram.Clone();
        program.A = ((long)1 << 45) - 1;
        Console.Write($"{program.A} ({Convert.ToString(program.A, 8)}): ");
        program.Run();
        Console.WriteLine(string.Join(",", program.output) + $" Correct length: {program.output.Count == program.Program.Count}");

        program = (ProgramSimulation)sourceProgram.Clone();
        program.A = ((long)1 << 45);
        Console.Write($"{program.A} ({Convert.ToString(program.A, 8)}): ");
        program.Run();
        Console.WriteLine(string.Join(",", program.output) + $" Correct length: {program.output.Count == program.Program.Count}");

        program = (ProgramSimulation)sourceProgram.Clone();
        program.A = ((long)1 << 48) - 1;
        Console.Write($"{program.A} ({Convert.ToString(program.A, 8)}): ");
        program.Run();
        Console.WriteLine(string.Join(",", program.output) + $" Correct length: {program.output.Count == program.Program.Count}");

        program = (ProgramSimulation)sourceProgram.Clone();
        program.A = ((long)1 << 48);
        Console.Write($"{program.A} ({Convert.ToString(program.A, 8)}): ");
        program.Run();
        Console.WriteLine(string.Join(",", program.output) + $" Correct length: {program.output.Count == program.Program.Count}");
    }


    private static void TestConsistentResult(ProgramSimulation sourceProgram)
    {
        Console.WriteLine("TestConsistentResult:");
        for (long i = 0; i < 8; i++)
        {
            var program = (ProgramSimulation)sourceProgram.Clone();
            program.A = ((long)1 << 45) + i * ((long)1 << 45);
            Console.Write($"{program.A} ({Convert.ToString(program.A, 8)}): ");
            program.Run();
            Console.WriteLine(string.Join(",", program.output));
        }
        for (long i = 0; i < 8; i++)
        {
            var program = (ProgramSimulation)sourceProgram.Clone();
            program.A = ((long)1 << 45) + i * ((long)1 << 42);
            Console.Write($"{program.A} ({Convert.ToString(program.A, 8)}): ");
            program.Run();
            Console.WriteLine(string.Join(",", program.output));
        }
    }

}

