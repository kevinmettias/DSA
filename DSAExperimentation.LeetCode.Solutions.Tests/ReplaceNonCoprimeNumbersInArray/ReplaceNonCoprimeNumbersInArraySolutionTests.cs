using DSAExperimentation.LeetCode.Solutions.ReplaceNonCoprimeNumbersInArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ReplaceNonCoprimeNumbersInArray;

// Harness only. Both merge strategies are
// ReplaceNonCoprimeNumbersInArraySolution's - this file just pins them to
// the cascade cases that distinguish a
// backward-merging stack from a naive left-to-right pass.
public sealed partial class ReplaceNonCoprimeNumbersInArraySolutionTests
{
    public static TheoryData<int[], int[]> Cases =>
        new()
        {
            // A merge that only becomes possible after the one to its right has
            // happened - 3 and 6 merge first, and the result then merges with the
            // 2 in front of it.
            { [2, 3, 6], [6] },

            // Every merge cascades: the whole array collapses to one value.
            { [2, 4, 8], [8] },

            // Nothing shares a factor, so the array is returned unchanged.
            { [2, 3, 5, 7], [2, 3, 5, 7] },

            // A single element has no adjacent pair at all.
            { [7], [7] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReplaceByRepeatedRescan_Cases_MergesEveryNonCoprimePair(int[] nums, int[] expected) =>
        Assert.Equal(expected, ReplaceNonCoprimeNumbersInArraySolution.ReplaceByRepeatedRescan(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void ReplaceByStackCascade_Cases_MergesEveryNonCoprimePair(int[] nums, int[] expected) =>
        Assert.Equal(expected, ReplaceNonCoprimeNumbersInArraySolution.ReplaceByStackCascade(nums));
}
