using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumAbsoluteDifferenceBetweenElementsWithConstraint;

// LeetCode 2817. Minimum Absolute Difference Between Elements With Constraint:
// slide j across nums, inserting nums[j - x] into this repo's own
// BinarySearchTree<int> exactly when it becomes eligible (the pair needs
// j - i >= x, so index j - x is the one that just became a valid earlier partner),
// then asking FindClosest.TryFind - the same nearest-value primitive
// (DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees)
// ClosestNodesQueriesInABinarySearchTreeTests already proves out - for the tree's
// closest value to nums[j]. Insert-as-you-slide plus a nearest-neighbor query,
// driven by an index gap instead of a fixed query list.
public sealed partial class MinimumAbsoluteDifferenceBetweenElementsWithConstraintTests
{
    [Theory]
    [InlineData(new[] { 4, 3, 2, 4 }, 2, 0)]
    [InlineData(new[] { 5, 3, 2, 10, 15 }, 1, 1)]
    [InlineData(new[] { 1, 2, 3, 4 }, 3, 3)]
    public void MinAbsoluteDifference_LeetCodeExamples_ReturnsMinimumUnderIndexConstraint(
        int[] nums, int x, int expected)
    {
        var actual = MinAbsoluteDifference(nums, x);

        Assert.Equal(expected, actual);
    }

    private static int MinAbsoluteDifference(int[] nums, int x)
    {
        var tree = new BinarySearchTree<int>();
        var minDifference = int.MaxValue;

        for (var j = 0; j < nums.Length; j++)
        {
            if (j >= x)
            {
                tree.Insert(nums[j - x]);
            }

            if (FindClosest.TryFind(tree.Root, nums[j], out var closest))
            {
                minDifference = Math.Min(minDifference, Math.Abs(closest - nums[j]));
            }
        }

        return minDifference;
    }
}
