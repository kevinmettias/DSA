using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for FolderPathWorkloads (ARCHITECTURE 17.7). The whole LC 1948 reading is that
// every top-level folder has the identical shape, so the duplicate hunt has real pairs to find
// instead of skipping each comparison on a structural mismatch.
public sealed partial class FolderPathWorkloadsTests
{
    private const int TopLevelCount = 8;
    private const int PathsPerTopLevelFolder = 2;
    private const string TopLevelPrefix = "t";
    private const string NestedFolderX = "x";
    private const string NestedFolderY = "y";
    private const string NestedFolderZ = "z";

    [Fact]
    public void BuildIdenticalTopLevelFolders_TopLevelCount_ReturnsTwoPathsPerFolder() =>
        Assert.Equal(
            TopLevelCount * PathsPerTopLevelFolder,
            FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount).Length);

    [Fact]
    public void BuildIdenticalTopLevelFolders_EveryTopLevelFolder_GetsTheSameNestedShape()
    {
        var paths = FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount);

        foreach (var folder in Enumerable.Range(0, TopLevelCount))
        {
            Assert.Equal(Path(folder, NestedFolderX, NestedFolderY), paths[folder * PathsPerTopLevelFolder]);
            Assert.Equal(Path(folder, NestedFolderZ), paths[(folder * PathsPerTopLevelFolder) + 1]);
        }
    }

    // The identical shape is shared between folders, never within one: the top-level names have to
    // stay distinct or the trees would be the same tree rather than duplicates of each other.
    [Fact]
    public void BuildIdenticalTopLevelFolders_TopLevelNames_AreAllDistinct()
    {
        var paths = FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount);

        Assert.Equal(TopLevelCount, paths.Select(path => path[0]).Distinct().Count());
    }

    [Fact]
    public void BuildIdenticalTopLevelFolders_SameCount_ReturnsTheSamePaths() =>
        Assert.Equal(
            AnswerText.Of(FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount)),
            AnswerText.Of(FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount)));

    private static string[] Path(int folder, params string[] nested) =>
        [TopLevelPrefix + folder, .. nested];
}
