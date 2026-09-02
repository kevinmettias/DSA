using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPoints;

// LeetCode 2058. Find the Minimum and Maximum Number of Nodes Between Critical
// Points: a single left-to-right walk over this repo's own
// SinglyLinkedListNode<int>.Next, the same "just the representation" shape
// ConvertBinaryNumberInALinkedListToInteger's walk already uses - no algorithm
// primitive needed beyond the node representation itself. A running previous
// value plus the first/previous/last critical indices are enough to compute both
// the minimum consecutive gap and the first-to-last gap in one pass.
public sealed partial class FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsTests
{
    [Fact]
    public void NodesBetweenCriticalPoints_FewerThanThreeNodes_ReturnsMinusOneMinusOne()
        => Assert.Equal([-1, -1], NodesBetweenCriticalPoints(Build([3, 1])));

    [Fact]
    public void NodesBetweenCriticalPoints_MonotonicList_ReturnsMinusOneMinusOne()
        => Assert.Equal([-1, -1], NodesBetweenCriticalPoints(Build([1, 2, 3, 4, 5])));

    [Fact]
    public void NodesBetweenCriticalPoints_ThreeCriticalPoints_ReturnsClosestAndFarthestGap()
        => Assert.Equal([1, 3], NodesBetweenCriticalPoints(Build([5, 3, 1, 2, 5, 1, 2])));

    [Fact]
    public void NodesBetweenCriticalPoints_ExactlyTwoCriticalPoints_MinEqualsMaxGap()
        => Assert.Equal([3, 3], NodesBetweenCriticalPoints(Build([1, 3, 2, 2, 3, 2, 2, 2, 7])));

    private static int[] NodesBetweenCriticalPoints(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next?.Next is null)
        {
            return [-1, -1];
        }

        var scan = ScanForCriticalPoints(head);

        return scan.FirstCriticalIndex == scan.LastCriticalIndex
            ? [-1, -1]
            : [scan.MinDistance, scan.LastCriticalIndex - scan.FirstCriticalIndex];
    }

    private readonly record struct CriticalPointScan(int MinDistance, int FirstCriticalIndex, int LastCriticalIndex);

    private static CriticalPointScan ScanForCriticalPoints(SinglyLinkedListNode<int> head)
    {
        var walk = CreateWalkState(head);

        for (var node = head.Next!; node.Next is not null; node = node.Next, walk.Index++)
        {
            VisitNode(node, walk);
        }

        return new CriticalPointScan(walk.MinDistance, walk.FirstCriticalIndex, walk.LastCriticalIndex);
    }

    private static WalkState CreateWalkState(SinglyLinkedListNode<int> head)
        => new() { PreviousValue = head.Value };

    private static void VisitNode(SinglyLinkedListNode<int> node, WalkState walk)
    {
        if (IsLocalExtreme(walk.PreviousValue, node.Value, node.Next!.Value))
        {
            if (walk.FirstCriticalIndex == -1)
            {
                walk.FirstCriticalIndex = walk.Index;
            }
            else
            {
                walk.MinDistance = Math.Min(walk.MinDistance, walk.Index - walk.PreviousCriticalIndex);
            }

            walk.PreviousCriticalIndex = walk.Index;
            walk.LastCriticalIndex = walk.Index;
        }

        walk.PreviousValue = node.Value;
    }

    private sealed class WalkState
    {
        public int MinDistance = int.MaxValue;
        public int FirstCriticalIndex = -1;
        public int PreviousCriticalIndex = -1;
        public int LastCriticalIndex = -1;
        public int PreviousValue;
        public int Index = 1;
    }

    private static bool IsLocalExtreme(int previous, int current, int next)
        => (current > previous && current > next) || (current < previous && current < next);

    private static SinglyLinkedListNode<int> Build(int[] values)
    {
        var dummy = new SinglyLinkedListNode<int>(0);
        var tail = dummy;

        foreach (var value in values)
        {
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return dummy.Next!;
    }
}
