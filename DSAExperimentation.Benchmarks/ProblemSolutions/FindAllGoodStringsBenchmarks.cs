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
        _evil = "ab";
    }

    [Benchmark(Baseline = true)]
    public int EnumerationScan()
    {
        var total = (int)Math.Pow(26, Length);
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
        var n = _s1.Length;
        var s1 = _s1;
        var s2 = _s2;
        var evil = _evil;
        var failure = PrefixFunctionSearch.ComputeFailureFunction(evil);

        long CountFrom(
            (int Position, int Matched, bool TightLow, bool TightHigh) state,
            Func<(int Position, int Matched, bool TightLow, bool TightHigh), long> count)
        {
            if (state.Matched == evil.Length)
            {
                return 0;
            }

            if (state.Position == n)
            {
                return 1;
            }

            var low = state.TightLow ? s1[state.Position] : 'a';
            var high = state.TightHigh ? s2[state.Position] : 'z';
            var total = 0L;

            for (var c = low; c <= high; c++)
            {
                var matched = AdvanceAutomaton(evil, failure, state.Matched, c);
                if (matched == evil.Length)
                {
                    continue;
                }

                var next = (state.Position + 1, matched, state.TightLow && c == low, state.TightHigh && c == high);
                total = (total + count(next)) % Modulus;
            }

            return total;
        }

        var result = Memoizer.Memoize<(int Position, int Matched, bool TightLow, bool TightHigh), long>(
            (0, 0, true, true), CountFrom);

        return (int)result;
    }

    private static int AdvanceAutomaton(string evil, int[] failure, int matched, char next)
    {
        while (matched > 0 && evil[matched] != next)
        {
            matched = failure[matched - 1];
        }

        return evil[matched] == next ? matched + 1 : matched;
    }
}
