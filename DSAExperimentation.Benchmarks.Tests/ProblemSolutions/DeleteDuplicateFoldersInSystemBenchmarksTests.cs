using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteDuplicateFoldersInSystemBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - comparing every pair of non-leaf subtrees directly
// against one post-order pass that groups equal subtree signatures - so a harness whose arms disagree
// is timing two different problems. Setup builds the folder tree from FolderPathWorkloads, which gives
// every top-level folder the same two subtrees, so every non-leaf folder has a structurally identical
// twin and both copies of each are deleted: nothing survives, which is what makes the reported survivor
// count a decisive value rather than an arbitrary one. The same TopLevelCount must rebuild the same
// paths and with it the same survivors.
public sealed partial class DeleteDuplicateFoldersInSystemBenchmarksTests
{
    private const int SmallestTopLevelCount = 100;

    // TopLevelCount * 2 paths - FolderPathWorkloads gives every top-level folder two subtrees.
    private const int ExpectedPathCount = SmallestTopLevelCount * 2;

    private const int ExpectedSurvivorCount = 0;

    [Fact]
    public void Setup_SameTopLevelCount_RebuildsTheSameWorkload()
    {
        Assert.InRange(BuildHarness().BruteForcePairwiseComparison(), ExpectedSurvivorCount, ExpectedPathCount);
        Assert.Equal(
            BuildHarness().BruteForcePairwiseComparison(),
            BuildHarness().BruteForcePairwiseComparison());
    }

    [Fact]
    public void BruteForcePairwiseComparison_IdenticalTopLevelSubtrees_AgreesWithHashMapSignatureGrouping()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSurvivorCount, harness.BruteForcePairwiseComparison());
        Assert.Equal(harness.HashMapSignatureGrouping(), harness.BruteForcePairwiseComparison());
    }

    [Fact]
    public void HashMapSignatureGrouping_IdenticalTopLevelSubtrees_AgreesWithBruteForcePairwiseComparison()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedSurvivorCount, harness.HashMapSignatureGrouping());
        Assert.Equal(harness.BruteForcePairwiseComparison(), harness.HashMapSignatureGrouping());
    }

    private static DeleteDuplicateFoldersInSystemBenchmarks BuildHarness()
    {
        var harness = new DeleteDuplicateFoldersInSystemBenchmarks { TopLevelCount = SmallestTopLevelCount };
        harness.Setup();

        return harness;
    }
}
