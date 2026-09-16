using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LexicographicallySmallestStringAfterDeletingDuplicateCharactersBenchmarks
// (ARCHITECTURE 17.9): its two arms are competing strategies for the same question - repeatedly
// rescanning for a deletable duplicate against one monotonic-stack pass - so a harness whose arms
// disagree is timing two different problems. Setup draws the text from one fixed seed over a small
// alphabet, so the same Length must rebuild the same string; otherwise two published numbers were
// never comparable in the first place.
public sealed partial class LexicographicallySmallestStringAfterDeletingDuplicateCharactersBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameText() =>
        Assert.Equal(BuildHarness().RepeatedScan(), BuildHarness().RepeatedScan());

    [Fact]
    public void RepeatedScan_SeededSmallAlphabetText_AgreesWithMonotonicStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStack(), harness.RepeatedScan());
    }

    [Fact]
    public void MonotonicStack_SeededSmallAlphabetText_AgreesWithRepeatedScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedScan(), harness.MonotonicStack());
    }

    private static LexicographicallySmallestStringAfterDeletingDuplicateCharactersBenchmarks BuildHarness()
    {
        var harness =
            new LexicographicallySmallestStringAfterDeletingDuplicateCharactersBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
