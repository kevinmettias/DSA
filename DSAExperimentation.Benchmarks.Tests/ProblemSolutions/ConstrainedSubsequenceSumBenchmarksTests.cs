using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstrainedSubsequenceSumBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning the whole window per position against the
// monotonic deque - so a harness whose arms disagree is timing two different problems. Setup draws
// the array from one fixed seed, so the same Length must rebuild the same array; otherwise two
// published numbers were never comparable in the first place.
//
// The array is private, so its documented shape is asserted through the arm's own answer: values are
// drawn from [-SmallestValueBound, SmallestValueBound) and the answer is the sum of a subsequence of
// at most Length of them, so it lands inside [0, Length * SmallestValueBound] for a seeded draw that
// is not entirely negative - which a 2000-draw run from this seed is not.
public sealed partial class ConstrainedSubsequenceSumBenchmarksTests
{
    private const int SmallestLength = 2_000;
    private const int SmallestValueBound = 1_000;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValueArray()
    {
        Assert.InRange(
            BuildHarness().RescanWindowEachPosition(),
            0,
            SmallestLength * SmallestValueBound);
        Assert.Equal(BuildHarness().RescanWindowEachPosition(), BuildHarness().RescanWindowEachPosition());
    }

    [Fact]
    public void RescanWindowEachPosition_WindowOfFifty_AgreesWithMonotonicDequeDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicDequeDp(), harness.RescanWindowEachPosition());
    }

    [Fact]
    public void MonotonicDequeDp_WindowOfFifty_AgreesWithRescanWindowEachPosition()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RescanWindowEachPosition(), harness.MonotonicDequeDp());
    }

    private static ConstrainedSubsequenceSumBenchmarks BuildHarness()
    {
        var harness = new ConstrainedSubsequenceSumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
