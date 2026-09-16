using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfSetsOfKNonOverlappingLineSegmentsBenchmarks (ARCHITECTURE 17.9): both
// arms count the same ways of drawing k non-overlapping segments over the points - bottom-up
// tabulation against the MemoizedPascal recursion from the same binomial identity - so a harness whose
// arms disagree is timing two different questions. The count modulo 1e9+7 is the problem's whole
// answer rather than a proxy. Setup only picks k from the points, so the same Points must pick the
// same k.
public sealed partial class NumberOfSetsOfKNonOverlappingLineSegmentsBenchmarksTests
{
    private const int SmallestPoints = 50;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().Tabulation(), BuildHarness().Tabulation());

    [Fact]
    public void Tabulation_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    [Fact]
    public void Memoized_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    private static NumberOfSetsOfKNonOverlappingLineSegmentsBenchmarks BuildHarness()
    {
        var harness = new NumberOfSetsOfKNonOverlappingLineSegmentsBenchmarks { Points = SmallestPoints };
        harness.Setup();

        return harness;
    }
}
