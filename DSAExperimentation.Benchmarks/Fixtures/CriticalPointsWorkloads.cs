using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2058 - everything about the strategies
// themselves now lives in
// LeetCode.FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPoints; what stays
// here is only how long a chain to build and how to seed it.
//
// Strict high/low alternation forces almost every interior node to be a genuine local
// max or min (a peak is drawn from a band entirely above the valleys', so it is always
// greater than its neighbors, and vice versa), so both strategies do comparable real
// work instead of the list happening to be monotonic in places - which would let the
// baseline's List<int> stay nearly empty and hide the allocation the comparison is
// about. Both bands stay inside LC 2058's 1..10^5.
internal static class CriticalPointsWorkloads
{
    private const int MaxMagnitudeExclusive = 1_000;
    private const int PeakPositionModulus = 2;

    // How far the peak band sits above the valley band: peaks run 1001..1999, valleys 1..999.
    private const int PeakOffset = MaxMagnitudeExclusive;

    public static SinglyLinkedListNode<int> BuildZigzagList(int length, int seed)
    {
        var random = new Random(seed);
        var headMagnitude = random.Next(1, MaxMagnitudeExclusive);
        var head = new SinglyLinkedListNode<int>(PeakOffset + headMagnitude);
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            var magnitude = random.Next(1, MaxMagnitudeExclusive);
            var isPeakPosition = i % PeakPositionModulus == 0;
            var offset = isPeakPosition ? PeakOffset : 0;
            tail.Next = new SinglyLinkedListNode<int>(offset + magnitude);
            tail = tail.Next;
        }

        return head;
    }
}
