using DSAExperimentation.LeetCode.RemoveSubFoldersFromTheFilesystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveSubFoldersFromTheFilesystem;

// Harness only: both strategies live in RemoveSubFoldersFromTheFilesystemSolution.
// LC 1233 accepts the surviving folders in any order, and the two strategies emit
// different orders by construction, so each expectation is stated ordinal-sorted and
// each arm's result is ordered the same way before comparison.
public sealed class RemoveSubFoldersFromTheFilesystemTests
{
    public static TheoryData<string[], string[]> Examples =>
        new()
        {
            // LeetCode example 1.
            { ["/a", "/a/b", "/c/d", "/c/d/e", "/c/f"], ["/a", "/c/d", "/c/f"] },
            // LeetCode example 2.
            { ["/a", "/a/b/c", "/a/b/d"], ["/a"] },
            // Two independent trees, each collapsing to its own root.
            { ["/ghi", "/hij/klm", "/hij", "/hij/klm/nop"], ["/ghi", "/hij"] },
            // LeetCode example 3: "/a/b/ca" shares the raw string prefix "/a/b/c" with
            // "/a/b/c" but is not one of its subfolders - the "/" delimiter check is
            // what tells them apart.
            { ["/a/b/c", "/a/b/ca", "/a/b/d"], ["/a/b/c", "/a/b/ca", "/a/b/d"] },
            // A chain given in an order where no ancestor precedes its descendant.
            { ["/a/b/c/d", "/a/b", "/a/b/c", "/a"], ["/a"] },
            // Nothing to remove, and the single-folder degenerate case.
            { ["/x/y", "/x/yz", "/xy"], ["/x/y", "/x/yz", "/xy"] },
            { ["/a"], ["/a"] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveSubfoldersByPairwisePrefixCheck_LeetCodeExamples_KeepsOnlyOutermostFolders(
        string[] folders, string[] expected) =>
        Assert.Equal(
            expected,
            RemoveSubFoldersFromTheFilesystemSolution
                .RemoveSubfoldersByPairwisePrefixCheck(folders)
                .Order(StringComparer.Ordinal));

    [Theory]
    [MemberData(nameof(Examples))]
    public void RemoveSubfoldersByMergeSortThenScan_LeetCodeExamples_KeepsOnlyOutermostFolders(
        string[] folders, string[] expected) =>
        Assert.Equal(
            expected,
            RemoveSubFoldersFromTheFilesystemSolution
                .RemoveSubfoldersByMergeSortThenScan(folders)
                .Order(StringComparer.Ordinal));
}
