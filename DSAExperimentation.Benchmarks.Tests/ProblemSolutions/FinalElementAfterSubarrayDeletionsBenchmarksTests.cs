using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FinalElementAfterSubarrayDeletionsBenchmarks (ARCHITECTURE 17.9): its three
// arms are competing strategies for the same game - two exhaustive minimax replays that differ only
// in how repeated positions are cached, and the closed form the solution's own comment says they
// exist to confirm - so a harness whose arms disagree is playing three different games. All three
// return the surviving element, a plain int. Setup draws every element from [1, MaxValueExclusive),
// so whoever survives the game is one of those drawn values and cannot fall outside the drawn range;
// the same Length must rebuild the same array.
public sealed partial class FinalElementAfterSubarrayDeletionsBenchmarksTests
{
    private const int SmallestLength = 8;
    private const int MaxValueExclusive = 100_000;

    private const int MinSurvivingElement = 1;
    private const int MaxSurvivingElement = MaxValueExclusive - 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameNumbers()
    {
        Assert.InRange(
            BuildHarness().DictionaryMinimax(), MinSurvivingElement, MaxSurvivingElement);

        Assert.Equal(BuildHarness().DictionaryMinimax(), BuildHarness().DictionaryMinimax());
    }

    [Fact]
    public void DictionaryMinimax_SeededNumbers_AgreesWithMemoizedMinimax()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedMinimax(), harness.DictionaryMinimax());
    }

    [Fact]
    public void MemoizedMinimax_SeededNumbers_AgreesWithEndpointComparison()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.EndpointComparison(), harness.MemoizedMinimax());
    }

    [Fact]
    public void EndpointComparison_SeededNumbers_AgreesWithDictionaryMinimax()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DictionaryMinimax(), harness.EndpointComparison());
    }

    private static FinalElementAfterSubarrayDeletionsBenchmarks BuildHarness()
    {
        var harness = new FinalElementAfterSubarrayDeletionsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
