using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for FolderPathWorkloads (ARCHITECTURE 17.7). The whole LC 1948 reading is that
// every top-level folder has the identical shape, so the duplicate hunt has real pairs to find
// instead of skipping each comparison on a structural mismatch.
public sealed partial class FolderPathWorkloadsTests
{
    private const int TopLevelCount = 8;
    private const int PathsPerTopLevelFolder = 4;
    private const string TopLevelPrefix = "t";
    private const string NestedFolderX = "x";
    private const string NestedFolderY = "y";
    private const string NestedFolderZ = "z";

    [Fact]
    public void BuildIdenticalTopLevelFolders_TopLevelCount_ReturnsFourPathsPerFolder() =>
        Assert.Equal(
            TopLevelCount * PathsPerTopLevelFolder,
            FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount).Length);

    [Fact]
    public void BuildIdenticalTopLevelFolders_EveryTopLevelFolder_GetsTheSameNestedShape()
    {
        var paths = FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount);

        foreach (var folder in Enumerable.Range(0, TopLevelCount))
        {
            string[][] expected =
                [Path(folder), Path(folder, NestedFolderX), Path(folder, NestedFolderX, NestedFolderY), Path(folder, NestedFolderZ)];

            Assert.Equal(expected, paths.Skip(folder * PathsPerTopLevelFolder).Take(PathsPerTopLevelFolder));
        }
    }

    // LC 1948 guarantees that a nested folder's parent is in the input as a path of its own.
    [Fact]
    public void BuildIdenticalTopLevelFolders_EveryNestedFolder_HasItsParentListed()
    {
        var paths = FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount);
        var listed = paths.Select(Key).ToHashSet();
        var parents = paths.Where(path => path.Length > 1).Select(path => Key(path[..^1]));

        Assert.All(parents, parent => Assert.Contains(parent, listed));
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
            AnswerGraphText.Of(FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount)),
            AnswerGraphText.Of(FolderPathWorkloads.BuildIdenticalTopLevelFolders(TopLevelCount)));

    private static string[] Path(int folder, params string[] nested) =>
        [TopLevelPrefix + folder, .. nested];

    private static string Key(string[] path) => string.Join('/', path);
}
