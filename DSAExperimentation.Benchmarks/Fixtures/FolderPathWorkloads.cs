namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1948 - how many top-level folders to hand the
// duplicate hunt. Every top-level folder is given the identical two-level shape
// x -> {y} and z, so both strategies must actually find topLevelCount-choose-2 worth
// of duplicate pairs rather than skipping the comparison on an early structural
// mismatch. What a folder tree IS, and what makes two of them duplicates, is the
// problem's own model (LeetCode.DeleteDuplicateFoldersInSystem).
internal static class FolderPathWorkloads
{
    private const int PathsPerTopLevelFolder = 2;
    private const string TopLevelPrefix = "t";
    private const string NestedFolderX = "x";
    private const string NestedFolderY = "y";
    private const string NestedFolderZ = "z";

    public static string[][] BuildIdenticalTopLevelFolders(int topLevelCount)
    {
        var paths = new string[topLevelCount * PathsPerTopLevelFolder][];

        for (var i = 0; i < topLevelCount; i++)
        {
            var top = TopLevelPrefix + i;
            paths[i * PathsPerTopLevelFolder] = [top, NestedFolderX, NestedFolderY];
            paths[(i * PathsPerTopLevelFolder) + 1] = [top, NestedFolderZ];
        }

        return paths;
    }
}
