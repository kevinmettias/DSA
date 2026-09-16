using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveSubFoldersFromTheFilesystemBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveSubFoldersFromTheFilesystemSolution's, competing strategies for the same question - the
// pairwise "does any other folder prefix me" check against a MergeSort-then-scan - so a harness whose
// arms disagree keeps two different sets of folders. Setup builds the random folder tree from one
// seeded Random, so the same Length must rebuild the same tree.
//
// WEAK BY CONSTRUCTION, and reported as such: each arm returns only `.Count` of the surviving list,
// not the folders themselves, so the agreement is a count-only proxy - an arm that kept the right
// NUMBER of folders but the wrong ones is invisible to it. What the range check beside it does add is
// the two ways a count goes wrong on its own: an arm that drops every folder, and one that reports
// more folders than the tree had.
public sealed partial class RemoveSubFoldersFromTheFilesystemBenchmarksTests
{
    private const int SmallestLength = 200;

    // Any non-empty folder tree keeps at least one folder - nothing prefixes the shallowest path.
    private const int FewestSurvivingFolders = 1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().BruteForcePairwisePrefixCheck(),
            BuildHarness().BruteForcePairwisePrefixCheck());

    [Fact]
    public void BruteForcePairwisePrefixCheck_AgreesWithMergeSortThenScan()
    {
        var harness = BuildHarness();
        var pairwiseCount = harness.BruteForcePairwisePrefixCheck();

        Assert.InRange(pairwiseCount, FewestSurvivingFolders, SmallestLength);
        Assert.Equal(harness.MergeSortThenScan(), pairwiseCount);
    }

    [Fact]
    public void MergeSortThenScan_AgreesWithBruteForcePairwisePrefixCheck()
    {
        var harness = BuildHarness();
        var mergeSortCount = harness.MergeSortThenScan();

        Assert.InRange(mergeSortCount, FewestSurvivingFolders, SmallestLength);
        Assert.Equal(harness.BruteForcePairwisePrefixCheck(), mergeSortCount);
    }

    private static RemoveSubFoldersFromTheFilesystemBenchmarks BuildHarness()
    {
        var harness = new RemoveSubFoldersFromTheFilesystemBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
