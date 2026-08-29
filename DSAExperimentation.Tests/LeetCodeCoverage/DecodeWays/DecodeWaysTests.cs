using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DecodeWays;

public sealed partial class DecodeWaysTests
{
    [Theory]
    [InlineData("12", 2)]
    [InlineData("226", 3)]
    [InlineData("06", 0)]
    public void NumDecodings_LeetCodeExamples_ReturnsCount(string s, int expected)
        => Assert.Equal(expected, Count(s));

    private static int Count(string s)
    {
        return Memoizer.Memoize<int, int>(0, DecodeFrom);

        int DecodeFrom(int index, Func<int, int> decode)
        {
            if (index == s.Length) return 1;
            if (s[index] == '0') return 0;
            var total = decode(index + 1);
            if (index + 1 < s.Length && int.Parse(s.AsSpan(index, 2)) <= 26) total += decode(index + 2);
            return total;
        }
    }
}
