using BenchmarkDotNet.Attributes;
using BitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Steps to Reduce a Number in Binary Representation to One
// (LC 1404): a brute-force baseline that actually performs each "+1" step
// as a full LSB-first binary addition via this repo's own Stack<char> - the
// identical technique AddBinaryBenchmarks already uses for LC 67 - rebuilding
// the whole remaining string every odd step, vs. the O(n) single-pass
// carry-propagation scan. An alternating "10" bit pattern maximizes how many
// separate full-length StackAdd calls the baseline has to make (each digit
// flips the parity again instead of one long carry chain absorbing several
// steps into a single call), so the O(n) work-per-add-call really does
// recur roughly n/2 times.
[MemoryDiagnoser]
public class NumberOfStepsToReduceANumberInBinaryRepresentationToOneBenchmarks
{
    private const string AlternatingBitUnit = "10";
    private const int AlternatingBitUnitLength = 2;
    private const string SingleBitOne = "1";
    private const int BinaryBase = 2;
    private const int StepsForOddDigit = 2;

    [Params(100, 1_000)]
    public int Length;

    private string _binary = null!;

    [GlobalSetup]
    public void Setup()
    {
        var repeatedUnits = Enumerable.Repeat(AlternatingBitUnit, Length / AlternatingBitUnitLength);
        _binary = string.Concat(repeatedUnits);
    }

    [Benchmark(Baseline = true)]
    public int StackAddSimulation()
    {
        var current = _binary;
        var steps = 0;
        while (current != SingleBitOne)
        {
            current = current[^1] == '0' ? current[..^1] : StackAddOne(current);
            steps++;
        }

        return steps;
    }

    private static string StackAddOne(string a)
    {
        var stack = new BitStack();
        var i = a.Length - 1;
        var carry = 1;
        while (i >= 0 || carry > 0)
        {
            var sum = carry + (i >= 0 ? a[i--] - '0' : 0);
            stack.Push((char)('0' + (sum % BinaryBase)));
            carry = sum / BinaryBase;
        }

        var chars = new List<char>();
        while (stack.TryPop(out var bit))
        {
            chars.Add(bit);
        }

        return new string(chars.ToArray());
    }

    [Benchmark]
    public int CarryPropagationScan()
    {
        var steps = 0;
        var carry = 0;
        for (var i = _binary.Length - 1; i >= 1; i--)
        {
            var digit = (_binary[i] - '0') + carry;
            if (digit % BinaryBase == 1)
            {
                steps += StepsForOddDigit;
                carry = 1;
            }
            else
            {
                steps += 1;
                carry = digit / BinaryBase;
            }
        }

        return steps + carry;
    }
}
