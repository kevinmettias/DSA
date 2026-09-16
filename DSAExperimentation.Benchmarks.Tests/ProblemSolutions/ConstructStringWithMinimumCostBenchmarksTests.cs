using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ConstructStringWithMinimumCostBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - a brute-force DP over the word list against the
// Aho-Corasick automaton's shared-prefix walk - so a harness whose arms disagree is timing two
// different problems. Setup draws the target from the same fixed seed, so the same TargetLength must
// rebuild the same target; otherwise two published numbers were never comparable.
//
// The target is private, so the workload's documented shape is asserted through the arms' answer.
// The word list carries all three single letters, so any target over that alphabet is constructible
// and the answer is a real cost rather than the "impossible" sentinel; and no character can cost
// more than the costliest single-letter word, so TargetLength of them cannot exceed that bound.
public sealed partial class ConstructStringWithMinimumCostBenchmarksTests
{
    private const int SmallestTargetLength = 1_000;
    private const int CostliestSingleLetterWord = 5;

    [Fact]
    public void Setup_SameTargetLength_RebuildsTheSameTarget()
    {
        Assert.InRange(
            BuildHarness().BruteForceDp(),
            0,
            SmallestTargetLength * CostliestSingleLetterWord);

        Assert.Equal(BuildHarness().BruteForceDp(), BuildHarness().BruteForceDp());
    }

    [Fact]
    public void BruteForceDp_SeededTargetOverTheSmallWordSet_AgreesWithAhoCorasickDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.AhoCorasickDp(), harness.BruteForceDp());
    }

    [Fact]
    public void AhoCorasickDp_SeededTargetOverTheSmallWordSet_AgreesWithBruteForceDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDp(), harness.AhoCorasickDp());
    }

    private static ConstructStringWithMinimumCostBenchmarks BuildHarness()
    {
        var harness = new ConstructStringWithMinimumCostBenchmarks { TargetLength = SmallestTargetLength };
        harness.Setup();

        return harness;
    }
}
