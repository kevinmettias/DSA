using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DecodeWaysII;

// LeetCode 639. Decode Ways II: DecodeWays' own single/pair recurrence, widened so
// '*' stands for "any digit 1-9" at a single position and the matching range of
// valid two-digit combinations at a pair, memoized via this repo's own Memoizer
// (DecodeWays' own precedent for the recurrence shape) with every running total
// reduced mod 1e9+7 as LeetCode requires.
public sealed partial class DecodeWaysIITests
{
    private const long Mod = 1_000_000_007;

    [Theory]
    [InlineData("*", 9)]
    [InlineData("1*", 18)]
    [InlineData("2*", 15)]
    [InlineData("**", 96)]
    [InlineData("*6*", 99)]
    public void CountDecodings_LeetCodeExamples_ReturnsCount(string s, long expected)
        => Assert.Equal(expected, Count(s));

    private static long Count(string s)
    {
        return Memoizer.Memoize<int, long>(0, DecodeFrom);

        long DecodeFrom(int index, Func<int, long> decode)
        {
            if (index == s.Length)
            {
                return 1;
            }

            if (s[index] == '0')
            {
                return 0;
            }

            var total = SingleWays(s[index]) * decode(index + 1) % Mod;

            if (index + 1 < s.Length)
            {
                total = (total + PairWays(s[index], s[index + 1]) * decode(index + 2)) % Mod;
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
