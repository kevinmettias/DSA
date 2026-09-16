using DSAExperimentation.LeetCode.KthLargestElementInAStream;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KthLargestElementInAStream;

// Harness only: both strategies are KthLargestElementInAStreamSolution's. Each
// script is a sequence of Add values with the expected running kth-largest checked
// after every insert, so a failure still names both the strategy and the exact
// insert that produced the wrong value.
public sealed partial class KthLargestElementInAStreamTests
{
    public static TheoryData<int, int[], int[], int[]> Examples =>
        new()
        {
            { 3, [4, 5, 8, 2], [3, 5, 10, 9, 4], [4, 5, 5, 8, 8] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySizeKMinHeap_LeetCodeExampleSequence_ReturnsRunningKthLargest(
        int kthRank, int[] nums, int[] adds, int[] expected) =>
        Assert.Equal(expected, RunScript(KthLargestElementInAStreamSolution.CreateBySizeKMinHeap(kthRank, nums), adds));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySortOnEveryAdd_LeetCodeExampleSequence_ReturnsRunningKthLargest(
        int kthRank, int[] nums, int[] adds, int[] expected) =>
        Assert.Equal(expected, RunScript(KthLargestElementInAStreamSolution.CreateBySortOnEveryAdd(kthRank, nums), adds));

    private static int[] RunScript(IKthLargestStream stream, int[] adds) =>
        [.. adds.Select(add => stream.Add(add))];
}
