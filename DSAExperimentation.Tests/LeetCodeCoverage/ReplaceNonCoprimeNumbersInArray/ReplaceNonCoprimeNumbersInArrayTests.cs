using DSAExperimentation.LeetCode.ReplaceNonCoprimeNumbersInArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ReplaceNonCoprimeNumbersInArray;

// Harness only. Both merge strategies are
// ReplaceNonCoprimeNumbersInArraySolution's - this file just pins them to
// LeetCode's published examples plus the cascade cases that distinguish a
// backward-merging stack from a naive left-to-right pass.
public sealed partial class ReplaceNonCoprimeNumbersInArrayTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            // LeetCode example 1: 6 and 4 merge to 12, then 3 and 2 merge to 6,
            // which merges into the 12 already sitting behind it.
            { [6, 4, 3, 2, 7, 6, 2], [12, 7, 6] },

            // LeetCode example 2: 1 is coprime with everything, so the runs of
            // ones survive untouched between the merged runs.
            { [2, 2, 1, 1, 3, 3, 3], [2, 1, 1, 3] },

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
    [MemberData(nameof(Examples))]
    public void ReplaceByRepeatedRescan_LeetCodeExamples_MergesEveryNonCoprimePair(int[] nums, int[] expected) =>
        Assert.Equal(expected, ReplaceNonCoprimeNumbersInArraySolution.ReplaceByRepeatedRescan(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ReplaceByStackCascade_LeetCodeExamples_MergesEveryNonCoprimePair(int[] nums, int[] expected) =>
        Assert.Equal(expected, ReplaceNonCoprimeNumbersInArraySolution.ReplaceByStackCascade(nums));
}
