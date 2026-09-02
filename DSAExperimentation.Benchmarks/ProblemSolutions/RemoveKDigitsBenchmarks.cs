using BenchmarkDotNet.Attributes;
using DigitStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove K Digits (LC 402): the naive round-by-round baseline - k separate O(n) scans,
// each finding and removing the first strictly-descending digit - against this repo's
// own Stack<char> monotonic sweep, which removes all k digits in one O(n) pass. Both
// produce the same final digit sequence (removing the first descent, one at a time, is
// the textbook equivalent of the monotonic-stack greedy), so this is the
// LargestRectangleInHistogramBenchmarks precedent (O(n*k)-ish baseline vs. O(n)
// primitive-based sweep) applied to string-digit removal instead of histogram area.
[MemoryDiagnoser]
public class RemoveKDigitsBenchmarks
{
    private const int DigitCount = 10;
    private const int RemovalFraction = 3;
    private const string ZeroResult = "0";

    [Params(500, 5_000)]
    public int Length;

    private string _num = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var digits = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            digits[i] = (char)('0' + random.Next(0, DigitCount));
        }

        _num = new string(digits);
        _k = Length / RemovalFraction;
    }

    [Benchmark(Baseline = true)]
    public string RepeatedFirstDescentRemoval()
    {
        var current = _num;

        for (var round = 0; round < _k; round++)
        {
            var removeIndex = current.Length - 1;

            for (var i = 0; i < current.Length - 1; i++)
            {
                if (current[i] > current[i + 1])
                {
                    removeIndex = i;
                    break;
                }
            }

            current = current.Remove(removeIndex, 1);
        }

        return TrimLeadingZeros(current);
    }

    [Benchmark]
    public string MonotonicStackSweep()
    {
        var stack = new DigitStack();
        var remaining = SweepDigits(stack, _num, _k);
        RemoveTrailingExcess(stack, remaining);
        var digits = DrainStackToDigits(stack);
        return TrimLeadingZeros(new string(digits));
    }

    // Pushes each digit, popping off any larger digits still eligible for removal -
    // the monotonic-stack greedy sweep. Returns the removal budget left afterward.
    private static int SweepDigits(DigitStack stack, string num, int remaining)
    {
        foreach (var digit in num)
        {
            while (remaining > 0 && stack.TryPeek(out var top) && top > digit)
            {
                stack.TryPop(out _);
                remaining--;
            }

            stack.Push(digit);
        }

        return remaining;
    }

    // If the sweep didn't use up the full removal budget, trims the remainder off the end.
    private static void RemoveTrailingExcess(DigitStack stack, int remaining)
    {
        while (remaining > 0 && stack.TryPop(out _))
        {
            remaining--;
        }
    }

    private static char[] DrainStackToDigits(DigitStack stack)
    {
        var digits = new char[stack.Count];
        for (var i = digits.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out digits[i]);
        }

        return digits;
    }

    private static string TrimLeadingZeros(string value)
    {
        var trimmed = value.TrimStart('0');
        return trimmed.Length == 0 ? ZeroResult : trimmed;
    }
}
