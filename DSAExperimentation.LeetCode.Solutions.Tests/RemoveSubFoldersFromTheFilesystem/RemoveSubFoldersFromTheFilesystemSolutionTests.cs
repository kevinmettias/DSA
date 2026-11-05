using DSAExperimentation.LeetCode.Solutions.RemoveSubFoldersFromTheFilesystem;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RemoveSubFoldersFromTheFilesystem;

// Harness only: both strategies live in RemoveSubFoldersFromTheFilesystemSolution.
// LC 1233 accepts the surviving folders in any order, and the two strategies emit
// different orders by construction, so each expectation is stated ordinal-sorted and
// each arm's result is ordered the same way before comparison.
public sealed partial class RemoveSubFoldersFromTheFilesystemSolutionTests
{
    public static TheoryData<string[], string[]> Cases =>
        new()
        {
            // Two independent trees, each collapsing to its own root.
            { ["/ghi", "/hij/klm", "/hij", "/hij/klm/nop"], ["/ghi", "/hij"] },
            // A chain given in an order where no ancestor precedes its descendant.
            { ["/a/b/c/d", "/a/b", "/a/b/c", "/a"], ["/a"] },
            // Nothing to remove, and the single-folder degenerate case.
            { ["/x/y", "/x/yz", "/xy"], ["/x/y", "/x/yz", "/xy"] },
            { ["/a"], ["/a"] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void RemoveSubfoldersByPairwisePrefixCheck_Cases_KeepsOnlyOutermostFolders(
        string[] folders, string[] expected) =>
        Assert.Equal(
            expected,
            RemoveSubFoldersFromTheFilesystemSolution
                .RemoveSubfoldersByPairwisePrefixCheck(folders)
                .Order(StringComparer.Ordinal));

    [Theory]
    [MemberData(nameof(Cases))]
    public void RemoveSubfoldersByMergeSortThenScan_Cases_KeepsOnlyOutermostFolders(
        string[] folders, string[] expected) =>
        Assert.Equal(
            expected,
            RemoveSubFoldersFromTheFilesystemSolution
                .RemoveSubfoldersByMergeSortThenScan(folders)
                .Order(StringComparer.Ordinal));
}
