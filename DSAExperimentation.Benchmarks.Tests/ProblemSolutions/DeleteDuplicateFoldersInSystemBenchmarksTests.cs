using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteDuplicateFoldersInSystemBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests
// cannot pin: which folders survive, known from the workload's construction rather than from either arm. Setup
// builds the folder tree from FolderPathWorkloads, which gives every top-level folder the same two subtrees, so
// every non-leaf folder has a structurally identical twin and both copies of each are deleted: nothing survives.
public sealed partial class DeleteDuplicateFoldersInSystemBenchmarksTests
{
    private const int SmallestTopLevelCount = 100;

    [Fact]
    public void BruteForcePairwiseComparison_IdenticalTopLevelSubtrees_LeavesNoSurvivors() =>
        Assert.Empty(BuildHarness().BruteForcePairwiseComparison());

    [Fact]
    public void HashMapSignatureGrouping_IdenticalTopLevelSubtrees_LeavesNoSurvivors() =>
        Assert.Empty(BuildHarness().HashMapSignatureGrouping());

    private static DeleteDuplicateFoldersInSystemBenchmarks BuildHarness()
    {
        var harness = new DeleteDuplicateFoldersInSystemBenchmarks { TopLevelCount = SmallestTopLevelCount };
        harness.Setup();

        return harness;
    }
}
