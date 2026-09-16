using DSAExperimentation.LeetCode.RemoveKDigits;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveKDigits;

// Harness only. Both strategies are RemoveKDigitsSolution's - this file pins them to
// LeetCode's published examples.
public sealed partial class RemoveKDigitsTests
{
    public static TheoryData<string, int, string> Examples =>
        new()
        {
            { "1432219", 3, "1219" },
            { "10200", 1, "200" },
            { "10", 2, "0" },
            { "112", 1, "11" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByRepeatedFirstDescentRemoval_ClassicExamples_ReturnsSmallestPossibleNumber(
        string num, int removalCount, string expected)
    {
        var actual = RemoveKDigitsSolution.RemoveByRepeatedFirstDescentRemoval(num, removalCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveByMonotonicStackSweep_ClassicExamples_ReturnsSmallestPossibleNumber(
        string num, int removalCount, string expected)
    {
        var actual = RemoveKDigitsSolution.RemoveByMonotonicStackSweep(num, removalCount);

        Assert.Equal(expected, actual);
    }
}
