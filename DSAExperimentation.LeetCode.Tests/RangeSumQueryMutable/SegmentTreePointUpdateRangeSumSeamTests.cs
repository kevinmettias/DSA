using DSAExperimentation.LeetCode.RangeSumQueryMutable;

namespace DSAExperimentation.LeetCode.Tests.RangeSumQueryMutable;

// The seam between RangeSumQueryMutableSolution's two NumArray implementations.
// CreateByArrayRescan clones the array and rescans it per SumRange;
// CreateBySegmentTreeQuery builds one DataStructures.SegmentTree<int,
// SumOperation<int>> and answers every later Update and SumRange from it.
//
// The two arms agree on any single call, so nothing in a one-shot test can tell
// them apart - what the segment tree has to get right is that Update replaces a
// leaf's value and then repairs exactly the ancestors of that leaf, and that a
// later range query reads the repaired internal nodes rather than a stale one. A
// range query crossing the boundary between two internal nodes is where a partial
// repair shows up, so every script below interleaves updates with overlapping
// queries rather than checking a state once.
public sealed partial class SegmentTreePointUpdateRangeSumSeamTests
{
    [Fact]
    public void SumRange_AfterPointUpdates_MatchesArrayRescan()
    {
        int[] nums = [1, 3, 5];
        NumArrayOperation[] script =
        [
            NumArrayOperation.Sum(0, 2),
            NumArrayOperation.Update(1, 2),
            NumArrayOperation.Sum(0, 2),
        ];

        Assert.Equal(["9", "8"], Replay(nums, script, useSegmentTree: true));
        AssertSameAnswers(nums, script);
    }

    // Updates that walk the tree's rightmost path and its leftmost path, with a
    // query spanning both afterwards.
    [Fact]
    public void Update_AtBothEndsOfTheArray_MatchesArrayRescan()
    {
        int[] nums = [4, 1, 7, 3, 9, 2, 8, 5];
        NumArrayOperation[] script =
        [
            NumArrayOperation.Update(0, 100),
            NumArrayOperation.Update(7, 200),
            NumArrayOperation.Sum(0, 7),
            NumArrayOperation.Sum(0, 3),
            NumArrayOperation.Sum(4, 7),
        ];

        AssertSameAnswers(nums, script);
    }

    // A single-element array: every query is the whole array and every update
    // replaces the only leaf there is, so the tree has no internal node to repair.
    [Fact]
    public void SumRange_SingleElementArray_MatchesArrayRescan()
    {
        int[] nums = [42];
        NumArrayOperation[] script =
        [
            NumArrayOperation.Sum(0, 0),
            NumArrayOperation.Update(0, 7),
            NumArrayOperation.Sum(0, 0),
        ];

        AssertSameAnswers(nums, script);
    }

    // Setting a leaf to the value it already holds must leave every later query
    // where it was - the case where a repair that recomputes from an unread leaf
    // still has to produce the same stored answer.
    [Fact]
    public void Update_ToTheValueAlreadyHeld_LeavesLaterQueriesUnchanged()
    {
        int[] nums = [2, 5, 4, 9];
        NumArrayOperation[] script =
        [
            NumArrayOperation.Sum(0, 3),
            NumArrayOperation.Update(2, 4),
            NumArrayOperation.Sum(0, 3),
            NumArrayOperation.Sum(2, 2),
        ];

        AssertSameAnswers(nums, script);
    }

    // Array lengths that are not powers of two leave the tree with a partly filled
    // last level, so a query's decomposition crosses one fewer split than a
    // full tree's would.
    [Fact]
    public void SumRange_NonPowerOfTwoLength_MatchesArrayRescan()
    {
        int[] nums = [6, 1, 8, 3, 5, 2, 7];
        NumArrayOperation[] script =
        [
            NumArrayOperation.Update(6, 70),
            NumArrayOperation.Update(3, 30),
            NumArrayOperation.Sum(0, 6),
            NumArrayOperation.Sum(3, 6),
            NumArrayOperation.Sum(0, 0),
            NumArrayOperation.Sum(0, 2),
            NumArrayOperation.Update(0, 0),
            NumArrayOperation.Sum(1, 5),
        ];

        AssertSameAnswers(nums, script);
    }

    [Fact]
    public void Update_NegativeValues_MatchesArrayRescan()
    {
        int[] nums = [5, -3, 8, -1, 0, 4];
        NumArrayOperation[] script =
        [
            NumArrayOperation.Update(1, -20),
            NumArrayOperation.Sum(0, 5),
            NumArrayOperation.Update(4, 9),
            NumArrayOperation.Sum(2, 4),
        ];

        AssertSameAnswers(nums, script);
    }

    private static void AssertSameAnswers(int[] nums, NumArrayOperation[] script)
        => Assert.Equal(
            Replay(nums, script, useSegmentTree: false),
            Replay(nums, script, useSegmentTree: true));

    private static List<string> Replay(int[] nums, NumArrayOperation[] script, bool useSegmentTree)
    {
        var array = useSegmentTree
            ? RangeSumQueryMutableSolution.CreateBySegmentTreeQuery(nums)
            : RangeSumQueryMutableSolution.CreateByArrayRescan(nums);
        var transcript = new List<string>();

        foreach (var operation in script)
        {
            if (operation.IsUpdate)
            {
                array.Update(operation.Left, operation.Right);
                continue;
            }

            transcript.Add(array.SumRange(operation.Left, operation.Right).ToString());
        }

        return transcript;
    }

    private readonly record struct NumArrayOperation(bool IsUpdate, int Left, int Right)
    {
        public static NumArrayOperation Update(int index, int value) => new(IsUpdate: true, index, value);

        public static NumArrayOperation Sum(int left, int right) => new(IsUpdate: false, left, right);
    }
}
