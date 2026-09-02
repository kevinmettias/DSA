using DSAExperimentation.LeetCode.MinimumOperationsToMakeBinaryArrayElementsEqualToOneI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumOperationsToMakeBinaryArrayElementsEqualToOneI;

// Harness only. Both strategies are
// MinimumOperationsToMakeBinaryArrayElementsEqualToOneISolution's - this file
// just pins them to LeetCode's published examples, including the unreachable
// case where the trailing zero has no room left for a flip.
public sealed class MinimumOperationsToMakeBinaryArrayElementsEqualToOneITests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [0, 1, 1, 1, 0, 0], 3 },
            { [0, 1, 1, 1], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByArrayMutation_LeetCodeExamples_ReturnsFewestFlips(int[] nums, int expected) =>
        Assert.Equal(expected, MinimumOperationsToMakeBinaryArrayElementsEqualToOneISolution.MinOperationsByArrayMutation(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByFlipParityWindow_LeetCodeExamples_ReturnsFewestFlips(int[] nums, int expected) =>
        Assert.Equal(expected, MinimumOperationsToMakeBinaryArrayElementsEqualToOneISolution.MinOperationsByFlipParityWindow(nums));
}
