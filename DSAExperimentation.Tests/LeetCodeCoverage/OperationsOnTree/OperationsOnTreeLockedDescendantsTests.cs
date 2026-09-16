using DSAExperimentation.LeetCode.OperationsOnTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OperationsOnTree;

// Harness only: the LockedDescendantsOf half of the OperationsOnTree problem, against
// both of OperationsOnTreeSolution's strategies. It gets its own examples, and its own
// file, because it is the step the two strategies actually differ in and the one the
// benchmark measures - before the tiered migration the whole-tree scan existed only as
// the benchmark's baseline arm, which nothing asserted. The lock/unlock/upgrade script
// both strategies have to agree on lives in OperationsOnTreeTests.
public sealed partial class OperationsOnTreeLockedDescendantsTests
{
    public static TheoryData<int[], LockingTreeOp[], int, int[]> Examples =>
        new()
        {
            // 0 -> 1, 2; 1 -> 3, 4.
            { [-1, 0, 0, 1, 1], [LockingTreeOp.Lock(3, 1), LockingTreeOp.Lock(4, 2)], 1, [3, 4] },
            { [-1, 0, 0, 1, 1], [LockingTreeOp.Lock(3, 1), LockingTreeOp.Lock(4, 2)], 0, [3, 4] },

            // A locked node is never its own descendant.
            { [-1, 0, 0, 1, 1], [LockingTreeOp.Lock(1, 1)], 1, [] },

            // A lock in a sibling subtree is not beneath node 1.
            { [-1, 0, 0, 1, 1], [LockingTreeOp.Lock(2, 1)], 1, [] },

            // Nothing locked anywhere.
            { [-1, 0, 0], [], 0, [] },

            // A chain 0 -> 1 -> 2 with only the deepest node locked.
            { [-1, 0, 1], [LockingTreeOp.Lock(2, 2)], 0, [2] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByWholeTreeScan_LockedSubtrees_ReturnsLockedDescendantsInAscendingOrder(
        int[] parent, LockingTreeOp[] setup, int num, int[] expected) =>
        AssertLockedDescendants(OperationsOnTreeSolution.CreateByWholeTreeScan(parent), setup, num, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySubtreeDepthFirstSearch_LockedSubtrees_ReturnsLockedDescendantsInAscendingOrder(
        int[] parent, LockingTreeOp[] setup, int num, int[] expected) =>
        AssertLockedDescendants(
            OperationsOnTreeSolution.CreateBySubtreeDepthFirstSearch(parent), setup, num, expected);

    private static void AssertLockedDescendants(
        LockingTree tree, LockingTreeOp[] setup, int num, int[] expected)
    {
        foreach (var operation in setup)
        {
            operation.TryApply(tree);
        }

        Assert.Equal(expected, tree.LockedDescendantsOf(num));
    }
}
