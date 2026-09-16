using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SpecialPermutationsBenchmarks (ARCHITECTURE 17.9): both arms count the
// same legal orderings of the same distinct values - one by enumerating every permutation and
// checking it once complete, the other by memoizing (Remaining, Last) - so a harness whose arms
// disagree is timing two different problems. Setup draws distinct values with a fixed seed, so
// the same Length must rebuild the same value set.
//
// Both arms report the count itself, which is the answer LeetCode 2741 asks for, so agreement
// compares the whole result.
public sealed partial class SpecialPermutationsBenchmarksTests
{
    private const int SmallestLength = 8;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValues() =>
        Assert.Equal(BuildHarness().BruteForceBacktracking(), BuildHarness().BruteForceBacktracking());

    [Fact]
    public void BruteForceBacktracking_EightDistinctValues_AgreesWithBitmaskMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskMemo(), harness.BruteForceBacktracking());
    }

    [Fact]
    public void BitmaskMemo_EightDistinctValues_AgreesWithBruteForceBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceBacktracking(), harness.BitmaskMemo());
    }

    private static SpecialPermutationsBenchmarks BuildHarness()
    {
        var harness = new SpecialPermutationsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
