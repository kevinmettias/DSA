using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheCountOfGoodIntegersBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - enumerating palindromes and testing each against the
// divisibility rule against backtracking over distinct digit multisets - so a harness whose arms
// disagree is timing two different problems. The class has no Setup, so DigitCount is the whole
// workload: nothing is drawn or hoisted, and both arms must count the same integers from the same
// K.
public sealed partial class FindTheCountOfGoodIntegersBenchmarksTests
{
    private const int SmallestDigitCount = 6;

    [Fact]
    public void PalindromeEnumeration_SmallestDigitCount_AgreesWithBacktrackEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackEnumeration(), harness.PalindromeEnumeration());
    }

    [Fact]
    public void BacktrackEnumeration_SmallestDigitCount_AgreesWithPalindromeEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PalindromeEnumeration(), harness.BacktrackEnumeration());
    }

    private static FindTheCountOfGoodIntegersBenchmarks BuildHarness() =>
        new() { DigitCount = SmallestDigitCount };
}
