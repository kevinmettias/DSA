using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find All Good Strings (LC 1397): enumerating every candidate string in [s1, s2]
// and substring-checking each one with the BCL vs. this repo's own KMP
// prefix/failure function (PrefixFunctionSearch.ComputeFailureFunction) driving a
// Memoizer-memoized digit DP that counts good strings by walking the automaton
// state space directly, without ever materializing a candidate string. s1/s2 span
// the full alphabet at every position so the enumeration baseline pays its full
// 26^Length cost.
[MemoryDiagnoser]
public class FindAllGoodStringsBenchmarks
{
    private const int Modulus = 1_000_000_007;
    private const int AlphabetSize = 26;
    private const string EvilSubstring = "ab";

    [Params(3, 4)]
    public int Length;

    private string _s1 = null!;
    private string _s2 = null!;
    private string _evil = null!;

    [GlobalSetup]
    public void Setup()
    {
        _s1 = new string('a', Length);
        _s2 = new string('z', Length);
        _evil = EvilSubstring;
    }

    [Benchmark(Baseline = true)]
    public int EnumerationScan()
    {
        var total = (int)Math.Pow(AlphabetSize, Length);
        var current = _s1.ToCharArray();
        var count = 0;

        for (var i = 0; i < total; i++)
        {
            if (new string(current).IndexOf(_evil, StringComparison.Ordinal) < 0)
            {
                count++;
            }

            Increment(current);
        }

        return count;
    }

    private static void Increment(char[] value)
    {
        for (var i = value.Length - 1; i >= 0; i--)
        {
            if (value[i] < 'z')
            {
                value[i]++;
                return;
            }

            value[i] = 'a';
        }
    }

    [Benchmark]
    public int AutomatonDigitDp()
    {
        var context = new DigitDpContext(
            _s1.Length, _s1, _s2, _evil, PrefixFunctionSearch.ComputeFailureFunction(_evil));

        long CountFromState(
            (int Position, int Matched, bool TightLow, bool TightHigh) state,
            Func<(int Position, int Matched, bool TightLow, bool TightHigh), long> count)
            => CountFrom(state, count, context);

        var result = Memoizer.Memoize<(int Position, int Matched, bool TightLow, bool TightHigh), long>(
            (0, 0, true, true), CountFromState);

        return (int)result;
    }

    private static long CountFrom(
        (int Position, int Matched, bool TightLow, bool TightHigh) state,
        Func<(int Position, int Matched, bool TightLow, bool TightHigh), long> count,
        DigitDpContext context)
    {
        if (state.Matched == context.Evil.Length)
        {
            return 0;
        }

        if (state.Position == context.N)
        {
            return 1;
        }

        return AccumulateOverRange(state, count, context);
    }

    private static long AccumulateOverRange(
        (int Position, int Matched, bool TightLow, bool TightHigh) state,
        Func<(int Position, int Matched, bool TightLow, bool TightHigh), long> count,
        DigitDpContext context)
    {
        var low = state.TightLow ? context.S1[state.Position] : 'a';
        var high = state.TightHigh ? context.S2[state.Position] : 'z';
        var total = 0L;

        for (var c = low; c <= high; c++)
        {
            var matched = AdvanceAutomaton(context.Evil, context.Failure, state.Matched, c);
            if (matched == context.Evil.Length)
            {
                continue;
            }

            var next = (state.Position + 1, matched, state.TightLow && c == low, state.TightHigh && c == high);
            total = (total + count(next)) % Modulus;
        }

        return total;
    }

    private readonly record struct DigitDpContext(int N, string S1, string S2, string Evil, int[] Failure);

    private static int AdvanceAutomaton(string evil, int[] failure, int matched, char next)
    {
        while (matched > 0 && evil[matched] != next)
        {
            matched = failure[matched - 1];
        }

        return evil[matched] == next ? matched + 1 : matched;
    }
}
