using DSAExperimentation.LeetCode.DeleteDuplicateFoldersInSystem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DeleteDuplicateFoldersInSystem;

// Harness only. Both the pairwise brute force and the signature-grouping pass are
// DeleteDuplicateFoldersInSystemSolution's - this file pins them to LeetCode's own
// examples plus the three cases the marking rule turns on: every copy of a repeated
// subtree is deleted rather than one being kept, leaves are never duplicates of each
// other, and a repeat counts even when the two copies sit at different depths.
//
// The problem states the returned paths may be in any order, so the assertion
// compares them as a set.
public sealed class DeleteDuplicateFoldersInSystemTests
{
    public static TheoryData<string[][], string[][]> Examples =>
        new()
        {
            // LeetCode example 1: "/a" and "/c" both hold a single empty "b", so both
            // go; "/d" holds "a" instead and survives with its child.
            {
                [["a"], ["c"], ["d"], ["a", "b"], ["c", "b"], ["d", "a"]],
                [["d"], ["d", "a"]]
            },

            // LeetCode example 3: nothing repeats, so nothing is deleted.
            {
                [["a", "b"], ["c", "d"], ["c"], ["a"]],
                [["a"], ["a", "b"], ["c"], ["c", "d"]]
            },

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
    [MemberData(nameof(Examples))]
    public void DeleteDuplicateFoldersByPairwiseComparison_LeetCodeExamples_ReturnsSurvivingPaths(
        string[][] paths, string[][] expected) =>
        AssertSamePaths(
            expected,
            DeleteDuplicateFoldersInSystemSolution.DeleteDuplicateFoldersByPairwiseComparison(paths));

    [Theory]
    [MemberData(nameof(Examples))]
    public void DeleteDuplicateFoldersBySignatureGrouping_LeetCodeExamples_ReturnsSurvivingPaths(
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
