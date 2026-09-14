using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPoints;

// LeetCode 2058. Find the Minimum and Maximum Number of Nodes Between Critical
// Points: a node is critical when it is a strict local maximum or minimum of the
// list, and the answer is [closest gap, farthest gap] between critical points, or
// [-1, -1] when fewer than two exist.
//
// Both strategies are a left-to-right walk over this repo's own
// SinglyLinkedListNode<int>.Next - the same "just the representation" shape
// ConvertBinaryNumberInALinkedListToIntegerSolution walks - so no algorithm
// primitive is needed beyond the node itself. What they differ in is whether the
// critical indices are materialized before the gaps are computed.
internal static class FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsSolution
{
    // "Not seen yet" for a running index, distinct in meaning from LeetCode's
    // LeetCodeAnswer.None even though both happen to be -1.
    private const int NoCriticalIndex = -1;

    // A gap needs two endpoints; one critical point (or none) has no gap at all.
    private const int MinimumCriticalPointsForGap = 2;

    // The first critical node can only be the list's second node, so indexing
    // starts there.
    private const int FirstInteriorIndex = 1;

    // The textbook baseline: one walk collects every critical point's index into a
    // BCL List<int>, then a second pass over that list finds the smallest adjacent
    // gap and the first-to-last span. Deliberately written with nothing from this
    // repo but the node it is handed - it is the arm the single-pass walk below has
    // to justify itself against - and it pays for a list proportional to the number
    // of critical points.
    public static int[] NodesBetweenCriticalPointsByMaterializedIndices(SinglyLinkedListNode<int>? head)
    {
        var criticalIndices = CollectCriticalIndices(head);

        if (criticalIndices.Count < MinimumCriticalPointsForGap)
        {
            return NoGap();
        }

        var minDistance = int.MaxValue;

        for (var i = 1; i < criticalIndices.Count; i++)
        {
            minDistance = Math.Min(minDistance, criticalIndices[i] - criticalIndices[i - 1]);
        }

        return [minDistance, criticalIndices[^1] - criticalIndices[0]];
    }

    private static List<int> CollectCriticalIndices(SinglyLinkedListNode<int>? head)
    {
        var criticalIndices = new List<int>();

        if (head?.Next?.Next is null)
        {
            return criticalIndices;
        }

        var previousValue = head.Value;
        var index = FirstInteriorIndex;

        for (var node = head.Next; node.Next is not null; node = node.Next, index++)
        {
            if (IsLocalExtreme(previousValue, node.Value, node.Next.Value))
            {
                criticalIndices.Add(index);
            }

            previousValue = node.Value;
        }

        return criticalIndices;
    }

    // Both gaps fold into the same walk: the running minimum needs only the
    // previous critical index, and the farthest gap needs only the first and last.
    // Same O(n) time as the baseline, constant extra space - no List<int> at all.
    public static int[] NodesBetweenCriticalPointsBySinglePassScan(SinglyLinkedListNode<int>? head)
    {
        if (head?.Next?.Next is null)
        {
            return NoGap();
        }

        var scan = ScanForCriticalPoints(head);

        if (scan.FirstCriticalIndex == scan.LastCriticalIndex)
        {
            return NoGap();
        }

        return [scan.MinDistance, scan.LastCriticalIndex - scan.FirstCriticalIndex];
    }

    // The loop condition is what makes a node interior: it stops at the last node,
    // so every node it yields is guaranteed to have both a predecessor value and a
    // successor node to compare against.
    private static CriticalPointScan ScanForCriticalPoints(SinglyLinkedListNode<int> head)
    {
        var walk = new WalkState { PreviousValue = head.Value };

        for (var node = head.Next; node?.Next is not null; node = node.Next, walk.Index++)
        {
            if (IsLocalExtreme(walk.PreviousValue, node.Value, node.Next.Value))
            {
                RecordCriticalIndex(walk);
            }

            walk.PreviousValue = node.Value;
        }

        return new CriticalPointScan(walk.MinDistance, walk.FirstCriticalIndex, walk.LastCriticalIndex);
    }

    // The first critical point opens the span; every later one closes a gap against
    // its predecessor and extends the span's far end.
    private static void RecordCriticalIndex(WalkState walk)
    {
        if (walk.FirstCriticalIndex == NoCriticalIndex)
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

    // LeetCode's answer when fewer than two critical points exist.
    private static int[] NoGap() => [LeetCodeAnswer.None, LeetCodeAnswer.None];

    private readonly record struct CriticalPointScan(int MinDistance, int FirstCriticalIndex, int LastCriticalIndex);

    private sealed class WalkState
    {
        public int MinDistance { get; set; } = int.MaxValue;

        public int FirstCriticalIndex { get; set; } = NoCriticalIndex;

        public int PreviousCriticalIndex { get; set; } = NoCriticalIndex;

        public int LastCriticalIndex { get; set; } = NoCriticalIndex;

        public int PreviousValue { get; set; }

        public int Index { get; set; } = FirstInteriorIndex;
    }

    // Strictly greater than both neighbors, or strictly less than both - equal
    // neighbors never make a node critical.
    private static bool IsLocalExtreme(int previous, int current, int next)
        => (current > previous && current > next) || (current < previous && current < next);
}
