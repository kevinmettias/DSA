using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfScoresOfBuiltStrings;

// LeetCode 2223. Sum of Scores of Built Strings: t_i is the length-i suffix of s,
// and score(t_i) is the length of the longest common prefix between t_i and s
// itself. Reversing s turns every suffix into a prefix of the reversed string, and
// the LCP of t_i with s becomes exactly the Z-value at position i of reverse(s) -
// this repo's own ZFunction.Compute (the same Z-array StringMatching's own
// ZFunctionTests already exercises). Summing Z[1..n-1] and adding n (for t_n = s
// itself, whose score is trivially its own full length) gives the answer in one
// O(n) pass instead of comparing every suffix against s character by character.
public sealed class SumOfScoresOfBuiltStringsTests
{
    [Fact]
    public void SumScores_LeetCodeExample1_ReturnsExpectedSum()
    {
        const string S = "babab";

        var result = SumScores(S);

        Assert.Equal(9, result);
    }

    [Fact]
    public void SumScores_LeetCodeExample2_ReturnsExpectedSum()
    {
        const string S = "azbazbzaz";

        var result = SumScores(S);

        Assert.Equal(14, result);
    }

    [Fact]
    public void SumScores_SingleCharacter_ReturnsOne()
    {
        const string S = "a";

        var result = SumScores(S);

        Assert.Equal(1, result);
    }

    [Fact]
    public void SumScores_NoSharedPrefixesBetweenSuffixesAndWhole_ReturnsJustN()
    {
        const string S = "abcde";

        var result = SumScores(S);

        Assert.Equal(5, result);
    }

    private static long SumScores(string s)
    {
        var reversed = new string(s.Reverse().ToArray());
        var z = ZFunction.Compute(reversed);

        var total = (long)s.Length;

        for (var i = 1; i < z.Length; i++)
        {
            total += z[i];
        }

        return total;
    }
}
