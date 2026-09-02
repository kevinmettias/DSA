using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RepeatedStringMatch;

// LeetCode 686. Repeated String Match: the minimum repeat count is never more than
// ceil(b.Length / a.Length) + 1 (the "+1" covers a match straddling a repetition
// boundary), so this repeats `a` up to that many times and asks PrefixFunctionSearch
// (KMP) whether `b` occurs in the repeated string, returning the first repeat count
// that works.
public sealed partial class RepeatedStringMatchTests
{
    [Theory]
    [InlineData("abcd", "cdabcdab", 3)]
    [InlineData("a", "aa", 2)]
    [InlineData("abc", "wxyz", -1)]
    public void MinRepeats_LeetCodeExamples_ReturnsMinimumRepeatCount(string a, string b, int expected)
    {
        var actual = MinRepeats(a, b);
        Assert.Equal(expected, actual);
    }

    private static int MinRepeats(string a, string b)
    {
        var minRepeats = (int)Math.Ceiling((double)b.Length / a.Length);

        for (var repeats = minRepeats; repeats <= minRepeats + 1; repeats++)
        {
            var repeatedSegments = Enumerable.Repeat(a, repeats);
            var candidate = string.Concat(repeatedSegments);
            if (PrefixFunctionSearch.FindAll(candidate, b).Count > 0)
            {
                return repeats;
            }
        }

        return -1;
    }
}
