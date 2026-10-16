using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EscapeALargeMazeBenchmarks (ARCHITECTURE 17.9). Both arms run on LC 1036's
// real board, where no independent search can check them cell by cell, so this pins the answer the
// workload is built to have: Setup keeps every blocked cell off row 0 and column 0, so
// the source walks out along its own edge row and the far corner is never touched - the answer is
// true at both blocked-cell counts, and a capped search that rejected it would be wrong.
public sealed partial class EscapeALargeMazeBenchmarksTests
{
    public static TheoryData<int> BlockedCounts => new() { 50, 200 };

    [Theory]
    [MemberData(nameof(BlockedCounts))]
    public void CanEscapeByCappedBreadthFirstSearch_OpenEdgeRow_EscapesToTheFarCorner(int blockedCount) =>
        Assert.True(BuildHarness(blockedCount).CanEscapeByCappedBreadthFirstSearch());

    [Theory]
    [MemberData(nameof(BlockedCounts))]
    public void CanEscapeByCappedTraversal_OpenEdgeRow_EscapesToTheFarCorner(int blockedCount) =>
        Assert.True(BuildHarness(blockedCount).CanEscapeByCappedTraversal());

    private static EscapeALargeMazeBenchmarks BuildHarness(int blockedCount)
    {
        var harness = new EscapeALargeMazeBenchmarks { BlockedCount = blockedCount };
        harness.Setup();

        return harness;
    }
}
