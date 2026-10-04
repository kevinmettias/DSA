using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.DataStructures;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PopulatingNextRightPointersInEachNodeBenchmarks (ARCHITECTURE 17.9). Arm
// agreement is BenchmarkArmsTests' job; this pins the full readout from the fixture's construction.
// Setup builds the perfect tree whose level-order values are index mod 1001, so depth d holds the
// indices 2^d - 1 through 2^(d+1) - 2 left to right, and LeetCode's readout is each depth's values
// followed by '#' (null).
public sealed partial class PopulatingNextRightPointersInEachNodeBenchmarksTests
{
    private const int SmallestNodeCount = 63;
    private const int ValueSpan = 1_001;

    [Fact]
    public void ManualQueueBfs_PerfectTree_ReadsEveryDepthAlongItsNextPointers() =>
        Assert.Equal(ExpectedReadout(), BuildHarness().ManualQueueBfs());

    [Fact]
    public void LevelGroupedTraversal_PerfectTree_ReadsEveryDepthAlongItsNextPointers() =>
        Assert.Equal(ExpectedReadout(), BuildHarness().LevelGroupedTraversal());

    private static int?[] ExpectedReadout()
    {
        var readout = new List<int?>();

        for (var levelStart = 0; levelStart < SmallestNodeCount; levelStart = (AlgorithmConstants.BranchingFactor * levelStart) + 1)
        {
            for (var index = levelStart; index <= AlgorithmConstants.BranchingFactor * levelStart; index++)
            {
                readout.Add(index % ValueSpan);
            }

            readout.Add(null);
        }

        return [.. readout];
    }

    private static PopulatingNextRightPointersInEachNodeBenchmarks BuildHarness()
    {
        var harness = new PopulatingNextRightPointersInEachNodeBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
