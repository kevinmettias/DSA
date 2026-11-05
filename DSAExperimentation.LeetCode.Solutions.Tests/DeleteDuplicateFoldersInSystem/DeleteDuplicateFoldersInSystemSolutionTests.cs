using DSAExperimentation.LeetCode.Solutions.DeleteDuplicateFoldersInSystem;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DeleteDuplicateFoldersInSystem;

// Harness only. Both the pairwise brute force and the signature-grouping pass are
// DeleteDuplicateFoldersInSystemSolution's - this file pins them to the three cases
// the marking rule turns on: every copy of a repeated subtree is deleted rather than
// one being kept, leaves are never duplicates of each other, and a repeat counts even
// when the two copies sit at different depths. LeetCode's published examples run in
// CatalogCaseTests.
//
// The problem states the returned paths may be in any order, so the assertion
// compares them as a set.
public sealed partial class DeleteDuplicateFoldersInSystemSolutionTests
{
    public static TheoryData<string[][], string[][]> Cases =>
        new()
        {
            // "a" and "b" are exact duplicates of each other two levels deep (each has
            // children x -> {y} and z), so BOTH are deleted entirely, not just one -
            // this problem deletes every copy, unlike "keep one" duplicate-subtree
            // problems. "c" has a differently-shaped single child and survives.
            {
                [
                    ["a"], ["a", "x"], ["a", "x", "y"], ["a", "z"],
                    ["b"], ["b", "x"], ["b", "x", "y"], ["b", "z"],
                    ["c"], ["c", "x"],
                ],
                [["c"], ["c", "x"]]
            },

            // Three unrelated empty folders are not duplicates of one another: the
            // problem's identity rule needs a NON-EMPTY set of subfolders.
            {
                [["a"], ["b"], ["c"]],
                [["a"], ["b"], ["c"]]
            },

            // Identical subtrees need not sit at the same level: "/a" and "/b/p" both
            // hold a single empty "x", so both are deleted and only "/b" is left.
            {
                [["a"], ["a", "x"], ["b"], ["b", "p"], ["b", "p", "x"]],
                [["b"]]
            },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void DeleteDuplicateFoldersByPairwiseComparison_Cases_ReturnsSurvivingPaths(
        string[][] paths, string[][] expected) =>
        AssertSamePaths(
            expected,
            DeleteDuplicateFoldersInSystemSolution.DeleteDuplicateFoldersByPairwiseComparison(paths));

    [Theory]
    [MemberData(nameof(Cases))]
    public void DeleteDuplicateFoldersBySignatureGrouping_Cases_ReturnsSurvivingPaths(
        string[][] paths, string[][] expected) =>
        AssertSamePaths(
            expected,
            DeleteDuplicateFoldersInSystemSolution.DeleteDuplicateFoldersBySignatureGrouping(paths));

    private static void AssertSamePaths(string[][] expected, List<string[]> actual)
    {
        var expectedSet = expected.Select(path => string.Join('/', path)).ToHashSet();
        var actualSet = actual.Select(path => string.Join('/', path)).ToHashSet();

        Assert.Equal(expectedSet, actualSet);
    }
}
