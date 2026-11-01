using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveSubFoldersFromTheFilesystemBenchmarks (ARCHITECTURE 17.9). Each arm
// returns the surviving folders themselves, and BenchmarkArmsTests holds the two to the same
// folders in any order, which LC 1233 allows. What the checks here add is independent of both arms:
// an arm can neither drop every folder nor report more folders than the tree had, and since LC 1233
// promises unique folders - which Setup now draws - no folder survives twice.
public sealed partial class RemoveSubFoldersFromTheFilesystemBenchmarksTests
{
    private const int SmallestLength = 200;

    // Any non-empty folder tree keeps at least one folder - nothing prefixes the shallowest path.
    private const int FewestSurvivingFolders = 1;

    [Fact]
    public void BruteForcePairwisePrefixCheck_KeepsBetweenOneAndEveryFolder() =>
        AssertSurvivors(BuildHarness().BruteForcePairwisePrefixCheck());

    [Fact]
    public void MergeSortThenScan_KeepsBetweenOneAndEveryFolder() =>
        AssertSurvivors(BuildHarness().MergeSortThenScan());

    private static void AssertSurvivors(List<string> survivors)
    {
        Assert.InRange(survivors.Count, FewestSurvivingFolders, SmallestLength);
        Assert.Distinct(survivors);
    }

    private static RemoveSubFoldersFromTheFilesystemBenchmarks BuildHarness()
    {
        var harness = new RemoveSubFoldersFromTheFilesystemBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
