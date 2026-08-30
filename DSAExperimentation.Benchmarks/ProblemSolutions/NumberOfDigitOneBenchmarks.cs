using BenchmarkDotNet.Attributes;
using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Digit One (LC 233): materializing and mod/div-scanning every
// integer from 1..n (O(n log n)) vs. this repo's own Stack<int>
// digit-extraction (same primitive ReverseIntegerBenchmarks uses) feeding a
// place-value tally that visits only n's own decimal digits (O(log n)).
[MemoryDiagnoser]
public class NumberOfDigitOneBenchmarks
{
    [Params(20_000, 500_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public long BruteForceScan()
    {
        long count = 0;
        for (var number = 1; number <= N; number++)
        {
            for (var remaining = number; remaining > 0; remaining /= 10)
            {
                if (remaining % 10 == 1)
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Benchmark]
    public long DigitPositionTally()
    {
        var n = N;
        var digits = new DigitStack();
        for (var remaining = n; remaining > 0; remaining /= 10)
        {
            digits.Push(remaining % 10);
        }

        var placeValue = 1L;
        for (var i = 1; i < digits.Count; i++)
        {
            placeValue *= 10;
        }

        long count = 0;
        long higherDigits = 0;

        while (digits.TryPop(out var digit))
        {
            var lowerRemainder = n % placeValue;
            count += digit switch
            {
                0 => higherDigits * placeValue,
                1 => (higherDigits * placeValue) + lowerRemainder + 1,
                _ => (higherDigits + 1) * placeValue,
            };

            higherDigits = (higherDigits * 10) + digit;
            placeValue /= 10;
        }

        return count;
    }
}
