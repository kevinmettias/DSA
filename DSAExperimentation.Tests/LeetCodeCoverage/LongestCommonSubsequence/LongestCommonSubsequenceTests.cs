using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonSubsequence;

// LeetCode 1143. Longest Common Subsequence: this repo's Memoizer over suffix-pair
// states (i, j), the same two-string-DP shape EditDistanceTests/
// MaximumLengthOfRepeatedSubarrayTests already use - here the recurrence keeps the
// longer of "skip a character from either string" instead of counting a
// contiguous match run.
public sealed partial class LongestCommonSubsequenceTests
{
    [Theory]
    [InlineData("abcde", "ace", 3)]
    [InlineData("abc", "abc", 3)]
    [InlineData("abc", "def", 0)]
    public void LongestCommonSubsequenceLength_LeetCodeExamples_ReturnsLcsLength(string text1, string text2, int expected)
        => Assert.Equal(expected, LongestCommonSubsequenceLength(text1, text2));

    private static int LongestCommonSubsequenceLength(string text1, string text2)
    {
        return Memoizer.Memoize<(int First, int Second), int>((0, 0), LcsFrom);

        int LcsFrom((int First, int Second) state, Func<(int First, int Second), int> lcs)
        {
            var (i, j) = state;

            if (i == text1.Length || j == text2.Length)
            {
                return 0;
            }

            if (text1[i] == text2[j])
            {
                return 1 + lcs((i + 1, j + 1));
            }

            return Math.Max(lcs((i + 1, j)), lcs((i, j + 1)));
        }
    }
}
