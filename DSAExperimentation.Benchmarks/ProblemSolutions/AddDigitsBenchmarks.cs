using BenchmarkDotNet.Attributes;
using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Add Digits (LC 258): direct arithmetic digit-summation (modulo/divide, no
// intermediate storage) vs. this repo's Stack<T> used as an explicit digit-holding
// primitive for each summation pass, the same "same algorithm, channeled through a
// repo Stack<T>" comparison ReverseIntegerBenchmarks.cs already makes.
[MemoryDiagnoser]
public class AddDigitsBenchmarks
{
    [Params(999_999, int.MaxValue)]
    public int Value;

    [Benchmark(Baseline = true)]
    public int Arithmetic()
    {
        var num = Value;
        while (num >= 10)
        {
            var sum = 0;
            var remaining = num;
            while (remaining > 0)
            {
                sum += remaining % 10;
                remaining /= 10;
            }

            num = sum;
        }

        return num;
    }

    [Benchmark]
    public int StackDigits()
    {
        var num = Value;
        while (num >= 10)
        {
            var digits = new DigitStack();
            var remaining = num;
            while (remaining > 0)
            {
                digits.Push(remaining % 10);
                remaining /= 10;
            }

            num = 0;
            while (digits.TryPop(out var digit))
            {
                num += digit;
            }
        }

        return num;
    }
}
