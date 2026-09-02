using DSAExperimentation.LeetCode.TransformedArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TransformedArray;

// Harness only: both strategies are TransformedArraySolution's. One test method
// per strategy over LeetCode's own examples, so a failure names the strategy that
// broke.
public sealed class TransformedArrayTests
{
    public static TheoryData<int[], int[]> Examples =>
        new()
        {
            { [3, -2, 1, 1], [1, 1, 1, 3] },
            { [-1, 4, -1], [-1, -1, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TransformByStepWalk_LeetCodeExamples_ReturnsWrappedIndirection(int[] nums, int[] expected) =>
        Assert.Equal(expected, TransformedArraySolution.TransformByStepWalk(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void TransformByModuloWalk_LeetCodeExamples_ReturnsWrappedIndirection(int[] nums, int[] expected) =>
        Assert.Equal(expected, TransformedArraySolution.TransformByModuloWalk(nums));
}
