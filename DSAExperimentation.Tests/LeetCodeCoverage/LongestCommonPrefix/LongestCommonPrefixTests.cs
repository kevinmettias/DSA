using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonPrefix;

// LeetCode 14. Longest Common Prefix: "all strings share a prefix of length n"
// is monotone, so BinarySearch.LowerBound can find the first failing length.
public sealed partial class LongestCommonPrefixTests
{
    [Theory]
    [InlineData(new[] { "flower", "flow", "flight" }, "fl")]
    [InlineData(new[] { "dog", "racecar", "car" }, "")]
    [InlineData(new[] { "interspecies", "interstellar", "interstate" }, "inters")]
    [InlineData(new[] { "" }, "")]
    public void LongestCommonPrefix_VariedInputs_ReturnsSharedPrefix(string[] values, string expected)
        => Assert.Equal(expected, LongestCommonPrefix(values));

    private static string LongestCommonPrefix(string[] values)
    {
        if (values.Length == 0)
        {
            return string.Empty;
        }

        var shortest = values.Min(value => value.Length);
        var sequence = new PrefixFeasibilitySequence(values, shortest);
        var firstFailingLength = BinarySearch.LowerBound<int, PrefixFeasibilitySequence>(sequence, 1);
        var prefixLength = firstFailingLength - 1;

        return values[0][..prefixLength];
    }

    private readonly struct PrefixFeasibilitySequence(string[] values, int maxLength) : IRandomAccessSequence<int>
    {
        public int Length => maxLength + 1;

        public int Get(int length) => AllSharePrefix(length) ? 0 : 1;

        private bool AllSharePrefix(int length)
        {
            for (var i = 1; i < values.Length; i++)
            {
                if (!values[0].AsSpan(0, length).SequenceEqual(values[i].AsSpan(0, length)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
