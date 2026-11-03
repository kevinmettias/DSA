using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindFirstAndLastPositionOfElementInSortedArrayBenchmarks (ARCHITECTURE
// 17.9): the generic arms check only compares the arms with each other, so this pins the baseline's
// closed [first, last] index range to one derived from Setup's construction rather than from any arm.
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
    public void LinearScan_TargetWithFourOccurrences_ReturnsItsClosedRun() =>
        Assert.Equal(AnswerGraphText.Of(ExpectedTargetRange()), AnswerGraphText.Of(BuildHarness().LinearScan()));

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
