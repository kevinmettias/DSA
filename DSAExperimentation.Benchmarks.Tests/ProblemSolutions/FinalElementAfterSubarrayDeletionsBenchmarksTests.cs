using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FinalElementAfterSubarrayDeletionsBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this bounds the baseline's answer by the
// workload itself. Setup draws every element from [1, MaxValueExclusive), so whoever survives the
// game is one of those drawn values and cannot fall outside the drawn range.
public sealed partial class FinalElementAfterSubarrayDeletionsBenchmarksTests
{
    private const int SmallestLength = 8;
    private const int MaxValueExclusive = 100_000;

    private const int MinSurvivingElement = 1;
    private const int MaxSurvivingElement = MaxValueExclusive - 1;

    [Fact]
    public void DictionaryMinimax_SeededNumbers_ReturnsAValueInTheDrawnRange() =>
        Assert.InRange(
            BuildHarness().DictionaryMinimax(), MinSurvivingElement, MaxSurvivingElement);

    private static FinalElementAfterSubarrayDeletionsBenchmarks BuildHarness()
    {
        var harness = new FinalElementAfterSubarrayDeletionsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
