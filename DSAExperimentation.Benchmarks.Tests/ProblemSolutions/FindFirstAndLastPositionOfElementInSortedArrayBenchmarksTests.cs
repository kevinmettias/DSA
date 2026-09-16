using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindFirstAndLastPositionOfElementInSortedArrayBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - BCL Array.IndexOf/
// LastIndexOf walking in from each end, against this repo's own LowerBound/UpperBound pair over an
// ArraySequence - so a harness whose arms disagree is delimiting two different runs of the target.
// Both answers are the closed [first, last] index range, two positional values in a fixed order, so
// they are compared as ordered sequences rather than as a set.
//
// Setup's array is each value repeated ValueDuplicationFactor times, so the target's run is exactly
// ValueDuplicationFactor indices long and its position is arithmetic on the two generator constants.
public sealed partial class FindFirstAndLastPositionOfElementInSortedArrayBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] lengths.
    private const int SmallestLength = 200;

    // Setup's own constants, restated so the expected run is derived rather than observed.
    private const int ValueDuplicationFactor = 4;
    private const int TargetDivisor = 8;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameTargetRange() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().LinearScan()),
            AnswerText.Of(BuildHarness().LinearScan()));

    [Fact]
    public void LinearScan_TargetWithFourOccurrences_ReturnsItsClosedRun()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ExpectedTargetRange()), AnswerText.Of(harness.LinearScan()));
        Assert.Equal(AnswerText.Of(harness.BinarySearchBounds()), AnswerText.Of(harness.LinearScan()));
    }

    [Fact]
    public void BinarySearchBounds_TargetWithFourOccurrences_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ExpectedTargetRange()), AnswerText.Of(harness.BinarySearchBounds()));
        Assert.Equal(AnswerText.Of(harness.LinearScan()), AnswerText.Of(harness.BinarySearchBounds()));
    }

    private static FindFirstAndLastPositionOfElementInSortedArrayBenchmarks BuildHarness()
    {
        var harness = new FindFirstAndLastPositionOfElementInSortedArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // Value v occupies indices [v * ValueDuplicationFactor, (v + 1) * ValueDuplicationFactor), so the
    // target's run starts at its own index times the duplication factor and is that many long.
    private static int[] ExpectedTargetRange()
    {
        var target = SmallestLength / TargetDivisor;
        var firstIndex = target * ValueDuplicationFactor;

        return [firstIndex, (firstIndex + ValueDuplicationFactor) - 1];
    }
}
