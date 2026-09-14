using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2058 - everything about the strategies
// themselves now lives in
// LeetCode.FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPoints; what stays
// here is only how long a chain to build and how to seed it.
//
// Strict sign alternation forces almost every interior node to be a genuine local
// max or min (a positive value is always greater than its negative neighbors, and
// vice versa), so both strategies do comparable real work instead of the list
// happening to be monotonic in places - which would let the baseline's List<int>
// stay nearly empty and hide the allocation the comparison is about.
internal static class CriticalPointsWorkloads
{
    private const int MaxMagnitudeExclusive = 1_000;
    private const int AlternatingSignModulus = 2;
    private const int PositiveSign = 1;
    private const int NegativeSign = -1;

    public static SinglyLinkedListNode<int> BuildZigzagList(int length, int seed)
    {
        var random = new Random(seed);
        var head = new SinglyLinkedListNode<int>(random.Next(1, MaxMagnitudeExclusive));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            var magnitude = random.Next(1, MaxMagnitudeExclusive);
            var isPeakPosition = i % AlternatingSignModulus == 0;
            var sign = isPeakPosition ? PositiveSign : NegativeSign;
            tail.Next = new SinglyLinkedListNode<int>(sign * magnitude);
            tail = tail.Next;
        }

        return head;
    }
}
