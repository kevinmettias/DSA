using BenchmarkDotNet.Attributes;
using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reverse Integer (LC 7): direct arithmetic reversal vs. this repo's Stack<T>
// used as an explicit LIFO digit-reversal primitive.
[MemoryDiagnoser]
public class ReverseIntegerBenchmarks
{
    [Params(123456789, 1534236469)]
    public int Value;

    [Benchmark(Baseline = true)]
    public int Arithmetic()
    {
        var remaining = Math.Abs((long)Value);
        long reversed = 0;

        while (remaining > 0)
        {
            reversed = (reversed * 10) + (remaining % 10);
            remaining /= 10;
        }

        if (Value < 0)
        {
            reversed = -reversed;
        }

        return reversed is < int.MinValue or > int.MaxValue ? 0 : (int)reversed;
    }

    [Benchmark]
    public int StackDigits()
    {
        var digits = new DigitStack();
        foreach (var digit in Math.Abs((long)Value).ToString())
        {
            digits.Push(digit);
        }

        long reversed = 0;
        while (digits.TryPop(out var digit))
        {
            reversed = (reversed * 10) + (digit - '0');
        }

        if (Value < 0)
        {
            reversed = -reversed;
        }

        return reversed is < int.MinValue or > int.MaxValue ? 0 : (int)reversed;
    }
}

