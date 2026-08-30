using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveSubFoldersFromTheFilesystem;

// LeetCode 1233. Remove Sub-Folders from the Filesystem: sort the folder paths with
// this repo's own MergeSort (over an ArrayIndexedSequence<string>, ordinal-compared
// so it never depends on the current culture) so every folder's ancestors land
// immediately before it, then a single linear scan keeps a folder only when it isn't
// prefixed by "<lastKept>/" - the standard sort-then-scan solution, not the O(n^2)
// pairwise-prefix-check brute force.
public sealed partial class RemoveSubFoldersFromTheFilesystemTests
{
    [Fact]
    public void RemoveSubfolders_LeetCodeExampleOne_KeepsOnlyTopLevelFolders()
    {
        string[] folders = ["/a", "/a/b", "/c/d", "/c/d/e", "/c/f"];

        var result = RemoveSubfolders(folders);

        Assert.Equal(["/a", "/c/d", "/c/f"], result);
    }

    [Fact]
    public void RemoveSubfolders_LeetCodeExampleTwo_KeepsRootWhenPresent()
    {
        string[] folders = ["/a", "/a/b/c", "/a/b/d"];

        var result = RemoveSubfolders(folders);

        Assert.Equal(["/a"], result);
    }

    [Fact]
    public void RemoveSubfolders_SiblingFoldersShareStringPrefix_KeepsAllSiblings()
    {
        // "/a/b/ca" shares the raw string prefix "/a/b/c" with "/a/b/c" but is not
        // one of its subfolders - the "/" delimiter check is what tells them apart.
        string[] folders = ["/a/b/c", "/a/b/ca", "/a/b/d"];

        var result = RemoveSubfolders(folders);

        Assert.Equal(["/a/b/c", "/a/b/ca", "/a/b/d"], result);
    }

    private static List<string> RemoveSubfolders(string[] folders)
    {
        var sequence = new ArrayIndexedSequence<string>(folders);
        MergeSort.Sort<string, ArrayIndexedSequence<string>>(sequence, StringComparer.Ordinal);

        var result = new List<string>();
        string? lastKept = null;

        foreach (var folder in folders)
        {
            if (lastKept is null || !folder.StartsWith(lastKept + "/", StringComparison.Ordinal))
            {
                result.Add(folder);
                lastKept = folder;
            }
        }

        return result;
    }
}
