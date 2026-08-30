using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Decode Ways II (LC 639): the same tabulation-vs-Memoizer comparison
// DecodeWaysBenchmarks already makes for plain Decode Ways, widened so '*'
// contributes the single/pair way-counts a wildcard digit allows, with every
// running total reduced mod 1e9+7 as LeetCode requires.
[MemoryDiagnoser]
public class DecodeWaysIIBenchmarks
{
    private const long Mod = 1_000_000_007;

    [Params(20, 200)]
    public int Length;

    private string _value = null!;

    [GlobalSetup]
    public void Setup() => _value = string.Concat(Enumerable.Repeat("2*", Length / 2));

    [Benchmark(Baseline = true)]
    public long Tabulation()
    {
        var dp = new long[_value.Length + 1];
        dp[_value.Length] = 1;

        for (var i = _value.Length - 1; i >= 0; i--)
        {
            if (_value[i] == '0')
            {
                continue;
            }

            dp[i] = SingleWays(_value[i]) * dp[i + 1] % Mod;

            if (i + 1 < _value.Length)
            {
                dp[i] = (dp[i] + (PairWays(_value[i], _value[i + 1]) * dp[i + 2])) % Mod;
            }
        }

        return dp[0];
    }

    [Benchmark]
    public long Memoized()
    {
        return Memoizer.Memoize<int, long>(0, DecodeFrom);

        long DecodeFrom(int index, Func<int, long> decode)
        {
            if (index == _value.Length)
            {
                return 1;
            }

            if (_value[index] == '0')
            {
                return 0;
            }

            var total = SingleWays(_value[index]) * decode(index + 1) % Mod;

            if (index + 1 < _value.Length)
            {
                total = (total + (PairWays(_value[index], _value[index + 1]) * decode(index + 2))) % Mod;
            }

            return total;
        }
    }

    private static long SingleWays(char c) => c == '*' ? 9 : 1;

    private static long PairWays(char first, char second)
    {
        if (first == '*' && second == '*')
        {
            return 15;
        }

        if (first == '*')
        {
            return second <= '6' ? 2 : 1;
        }

        if (second == '*')
        {
            return first == '1' ? 9 : first == '2' ? 6 : 0;
        }

        var value = ((first - '0') * 10) + (second - '0');
        return value is >= 10 and <= 26 ? 1 : 0;
    }
}
