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
    // Base of the positional numeral system both benchmarks decompose N into.
    private const int DecimalBase = 10;

    [Params(20_000, 500_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public long BruteForceScan()
    {
        long count = 0;
        for (var number = 1; number <= N; number++)
        {
            for (var remaining = number; remaining > 0; remaining /= DecimalBase)
            {
                if (remaining % DecimalBase == 1)
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
        for (var remaining = n; remaining > 0; remaining /= DecimalBase)
        {
            digits.Push(remaining % DecimalBase);
        }

        var placeValue = 1L;
        for (var i = 1; i < digits.Count; i++)
        {
            placeValue *= DecimalBase;
        }

        long count = 0;
        long higherDigits = 0;

        while (digits.TryPop(out var digit))
        {
            count += AccumulateDigitOnes(digit, n, ref placeValue, ref higherDigits);
        }

        return count;
    }

    // Folds one popped digit into the running ones-count, then advances placeValue
    // and higherDigits to the next (more significant) digit position - the
    // self-contained per-digit step of the place-value tally above.
    private static long AccumulateDigitOnes(int digit, long n, ref long placeValue, ref long higherDigits)
    {
        var lowerRemainder = n % placeValue;
        var delta = digit switch
        {
            0 => higherDigits * placeValue,
            1 => (higherDigits * placeValue) + lowerRemainder + 1,
            _ => (higherDigits + 1) * placeValue,
        };

        higherDigits = (higherDigits * DecimalBase) + digit;
        placeValue /= DecimalBase;
        return delta;
    }
}
