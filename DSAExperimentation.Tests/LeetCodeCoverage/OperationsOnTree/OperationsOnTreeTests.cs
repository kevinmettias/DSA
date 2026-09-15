using DSAExperimentation.LeetCode.OperationsOnTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OperationsOnTree;

// Harness only. The tree object is LeetCode.OperationsOnTree's LockingTree and both
// strategies are OperationsOnTreeSolution's - this file replays LeetCode's published
// call sequence against each one, so a failure still names the strategy that broke
// even though the "input" here is a sequence of lock/unlock/upgrade calls rather
// than a single argument tuple, the same shape LRUCacheTests uses for its own
// instance-API problem.
//
// LockedDescendantsOf gets its own examples because it is the step the two
// strategies actually differ in and the one the benchmark measures; before this
// migration the whole-tree scan existed only as the benchmark's baseline arm, which
// nothing asserted.
public sealed class OperationsOnTreeTests
{
    public static TheoryData<int[], LockingTreeOp[], bool[]> Examples =>
        new()
        {
            {
                // LeetCode's own example tree: 0 -> 1, 2; 1 -> 3, 4; 2 -> 5, 6.
                [-1, 0, 0, 1, 1, 2, 2],
                [
                    LockingTreeOp.Lock(2, 2),
                    LockingTreeOp.Unlock(2, 3),
                    LockingTreeOp.Unlock(2, 2),
                    LockingTreeOp.Lock(4, 5),
                    LockingTreeOp.Upgrade(0, 1),
                    LockingTreeOp.Lock(0, 1),
                ],
                [true, false, true, true, true, false]
            },
            {
                // The same call sequence against a shallower tree: 0 -> 1, 2, 3; 2 -> 4, 5.
                [-1, 0, 0, 0, 2, 2],
                [
                    LockingTreeOp.Lock(2, 2),
                    LockingTreeOp.Unlock(2, 3),
                    LockingTreeOp.Unlock(2, 2),
                    LockingTreeOp.Lock(4, 5),
                    LockingTreeOp.Upgrade(0, 1),
                    LockingTreeOp.Lock(0, 1),
                ],
                [true, false, true, true, true, false]
            },
            {
                // Unlocking is only ever the holder's to do, and a locked node
                // cannot be locked a second time.
                [-1, 0],
                [
                    LockingTreeOp.Unlock(1, 7),
                    LockingTreeOp.Lock(1, 7),
                    LockingTreeOp.Lock(1, 9),
                    LockingTreeOp.Unlock(1, 9),
                    LockingTreeOp.Unlock(1, 7),
                ],
                [false, true, false, false, true]
            },
            {
                // Upgrade refuses when the node itself is already locked.
                [-1, 0, 0],
                [LockingTreeOp.Lock(0, 1), LockingTreeOp.Lock(1, 2), LockingTreeOp.Upgrade(0, 3)],
                [true, true, false]
            },
            {
                // Upgrade refuses when an ancestor is locked: 0 -> 1 -> 2.
                [-1, 0, 1],
                [LockingTreeOp.Lock(0, 1), LockingTreeOp.Lock(2, 2), LockingTreeOp.Upgrade(1, 3)],
                [true, true, false]
            },
            {
                // Upgrade refuses when nothing beneath the node is locked.
                [-1, 0, 0],
                [LockingTreeOp.Upgrade(0, 1)],
                [false]
            },
            {
                // A successful upgrade releases every locked descendant, so one of
                // them can be locked again straight afterwards: 0 -> 1, 2; 1 -> 3, 4.
                [-1, 0, 0, 1, 1],
                [
                    LockingTreeOp.Lock(3, 1),
                    LockingTreeOp.Lock(4, 2),
                    LockingTreeOp.Upgrade(1, 5),
                    LockingTreeOp.Lock(3, 9),
                    LockingTreeOp.Lock(1, 9),
                    LockingTreeOp.Unlock(1, 5),
                ],
                [true, true, true, true, false, true]
            },
        };

    public static TheoryData<int[], LockingTreeOp[], int, int[]> DescendantExamples =>
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
    public void CreateByWholeTreeScan_LeetCodeExamples_MatchesEveryOperationResult(
        int[] parent, LockingTreeOp[] operations, bool[] expected) =>
        AssertScript(OperationsOnTreeSolution.CreateByWholeTreeScan(parent), operations, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateBySubtreeDepthFirstSearch_LeetCodeExamples_MatchesEveryOperationResult(
        int[] parent, LockingTreeOp[] operations, bool[] expected) =>
        AssertScript(OperationsOnTreeSolution.CreateBySubtreeDepthFirstSearch(parent), operations, expected);

    [Theory]
    [MemberData(nameof(DescendantExamples))]
    public void CreateByWholeTreeScan_LockedSubtrees_ReturnsLockedDescendantsInAscendingOrder(
        int[] parent, LockingTreeOp[] setup, int num, int[] expected) =>
        AssertLockedDescendants(OperationsOnTreeSolution.CreateByWholeTreeScan(parent), setup, num, expected);

    [Theory]
    [MemberData(nameof(DescendantExamples))]
    public void CreateBySubtreeDepthFirstSearch_LockedSubtrees_ReturnsLockedDescendantsInAscendingOrder(
        int[] parent, LockingTreeOp[] setup, int num, int[] expected) =>
        AssertLockedDescendants(
            OperationsOnTreeSolution.CreateBySubtreeDepthFirstSearch(parent), setup, num, expected);

    private static void AssertScript(LockingTree tree, LockingTreeOp[] operations, bool[] expected)
    {
        for (var i = 0; i < operations.Length; i++)
        {
            Assert.Equal(expected[i], operations[i].Apply(tree));
        }
    }

    private static void AssertLockedDescendants(
        LockingTree tree, LockingTreeOp[] setup, int num, int[] expected)
    {
        foreach (var operation in setup)
        {
            operation.Apply(tree);
        }

        Assert.Equal(expected, tree.LockedDescendantsOf(num));
    }
}

// One call in an OperationsOnTree script: which method to invoke and with what
// arguments. Pure dispatch, built via the named factories below so a script (like
// Examples above) reads like the LeetCode call sequence it replays.
public readonly record struct LockingTreeOp(LockingTreeOp.OpKind kind, int num, int user)
{
    public static LockingTreeOp Lock(int num, int user) => new(OpKind.Lock, num, user);

    public static LockingTreeOp Unlock(int num, int user) => new(OpKind.Unlock, num, user);

    public static LockingTreeOp Upgrade(int num, int user) => new(OpKind.Upgrade, num, user);

    // Internal, not public: only this same assembly's test methods ever call Apply,
    // and LockingTree itself is internal to the solution tier.
    internal bool Apply(LockingTree tree) => kind switch
    {
        OpKind.Lock => tree.Lock(num, user),
        OpKind.Unlock => tree.Unlock(num, user),
        _ => tree.Upgrade(num, user),
    };

    public enum OpKind
    {
        Lock,
        Unlock,
        Upgrade,
    }
}
