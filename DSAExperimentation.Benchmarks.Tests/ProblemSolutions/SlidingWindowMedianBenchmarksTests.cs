using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SlidingWindowMedianBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - copy and sort each window from scratch against
// the two-heap sweep with lazy deletion - so a harness whose arms disagree is sliding over two
// different arrays. Setup draws the values from one fixed seed, so the same Length must
// rebuild the same array; otherwise two published numbers were never comparable.
//
// Both arms return one median per window as a double, and window i's median belongs to window
// i, so the two sequences are compared position by position rather than through AnswerText:
// median values are measurements, and a named tolerance compares them as such instead of
// demanding an exact float equality that neither accumulator promises.
public sealed partial class SlidingWindowMedianBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // Every median here is either a whole window element or the mean of the two middle ones,
    // so both arms land on the same representable value; the tolerance is here so a last-bit
    // difference between the two accumulations cannot fail the harness for the wrong reason.
    private const double RelativeTolerance = 1e-9;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValueStream()
    {
        var first = BuildHarness().SortEachWindow();
        var second = BuildHarness().SortEachWindow();

        Assert.Equal(first.Length, second.Length);

        for (var index = 0; index < first.Length; index++)
        {
            Assert.Equal(first[index], second[index], RelativeTolerance);
        }
    }

    [Fact]
    public void SortEachWindow_FiveHundredWideWindowOverTwoThousandValues_AgreesWithTwoHeapsLazyDeletion()
    {
        var harness = BuildHarness();
        var expected = harness.TwoHeapsLazyDeletion();
        var actual = harness.SortEachWindow();

        Assert.Equal(expected.Length, actual.Length);

        for (var index = 0; index < expected.Length; index++)
        {
            Assert.Equal(expected[index], actual[index], RelativeTolerance);
        }
    }

    [Fact]
    public void TwoHeapsLazyDeletion_FiveHundredWideWindowOverTwoThousandValues_AgreesWithSortEachWindow()
    {
        var harness = BuildHarness();
        var expected = harness.SortEachWindow();
        var actual = harness.TwoHeapsLazyDeletion();

        Assert.Equal(expected.Length, actual.Length);

        for (var index = 0; index < expected.Length; index++)
        {
            Assert.Equal(expected[index], actual[index], RelativeTolerance);
        }
    }

    private static SlidingWindowMedianBenchmarks BuildHarness()
    {
        var harness = new SlidingWindowMedianBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
