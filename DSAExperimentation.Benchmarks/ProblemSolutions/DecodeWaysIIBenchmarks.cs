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
    private const string WildcardPair = "2*";
    private const int PairLength = 2;
    private const int SingleWildcardWays = 9;
    private const int BothWildcardPairWays = 15;
    private const int StarThenSmallDigitWays = 2;
    private const int FirstIsOneStarWays = 9;
    private const int FirstIsTwoStarWays = 6;
    private const int DecimalBase = 10;
    private const int MaxLetterCode = 26;

    [Params(20, 200)]
    public int Length;

    private string _value = null!;

    [GlobalSetup]
    public void Setup()
    {
        var pairs = Enumerable.Repeat(WildcardPair, Length / PairLength);
        _value = string.Concat(pairs);
    }

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
                dp[i] = (dp[i] + (PairWays(_value[i], _value[i + 1]) * dp[i + PairLength])) % Mod;
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
                total = (total + (PairWays(_value[index], _value[index + 1]) * decode(index + PairLength))) % Mod;
            }

            return total;
        }
    }

    private static long SingleWays(char c) => c == '*' ? SingleWildcardWays : 1;

    private static long PairWays(char first, char second)
    {
        if (first == '*' && second == '*')
        {
            return BothWildcardPairWays;
        }

        if (first == '*')
        {
            return second <= '6' ? StarThenSmallDigitWays : 1;
        }

        if (second == '*')
        {
            return first == '1' ? FirstIsOneStarWays : first == '2' ? FirstIsTwoStarWays : 0;
        }

        var value = ((first - '0') * DecimalBase) + (second - '0');
        return value is >= DecimalBase and <= MaxLetterCode ? 1 : 0;
    }
}
