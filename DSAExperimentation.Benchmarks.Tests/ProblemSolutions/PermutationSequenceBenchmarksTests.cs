using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PermutationSequenceBenchmarks (ARCHITECTURE 17.9): the baseline's permutation
// against a fixture-derived constant rather than a value read off either arm, which the generic check
// cannot supply - it holds the factoradic selection to the baseline. Setup fixes the rank at
// DigitCount!, the lexicographically last permutation.
public sealed partial class PermutationSequenceBenchmarksTests
{
    private const int SmallestDigitCount = 6;

    // The last permutation of the digits 1..DigitCount in lexicographic order, which is rank
    // DigitCount! - the descending arrangement.
    private const string LexicographicallyLastPermutation = "654321";

    [Fact]
    public void BacktrackEnumeration_LastLexicographicRank_ReturnsTheDescendingArrangement() =>
        Assert.Equal(LexicographicallyLastPermutation, BuildHarness().BacktrackEnumeration());

    private static PermutationSequenceBenchmarks BuildHarness()
    {
        var harness = new PermutationSequenceBenchmarks { DigitCount = SmallestDigitCount };
        harness.Setup();

        return harness;
    }
}
