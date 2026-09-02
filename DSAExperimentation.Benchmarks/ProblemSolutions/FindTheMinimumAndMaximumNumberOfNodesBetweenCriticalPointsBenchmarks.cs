using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find the Minimum and Maximum Number of Nodes Between Critical Points (LC 2058):
// MaterializeIndicesThenScan is the natural first-instinct solution - collect every
// critical point's index into a List<int> during one walk, then a second pass over
// that list computes the min/max gaps. SinglePassConstantSpace instead folds both
// passes into the same walk over this repo's own SinglyLinkedListNode<int>.Next,
// tracking only the first/previous/last critical index and the running minimum -
// same O(n) time, no List<int> allocation.
[MemoryDiagnoser]
public class FindTheMinimumAndMaximumNumberOfNodesBetweenCriticalPointsBenchmarks
{
    private const int RandomSeed = 7;
    private const int MaxMagnitudeExclusive = 1_000;
    private const int MinimumCriticalPointsForDistance = 2;
    private const int AlternatingSignModulus = 2;

    [Params(200, 5_000)]
    public int Length;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup() => _head = BuildZigzagList(Length);

    [Benchmark(Baseline = true)]
    public int[] MaterializeIndicesThenScan()
    {
        var criticalIndices = CollectCriticalIndices();

        if (criticalIndices.Count < MinimumCriticalPointsForDistance)
        {
            return [-1, -1];
        }

        var minDistance = ComputeMinDistance(criticalIndices);

        return [minDistance, criticalIndices[^1] - criticalIndices[0]];
    }

    private List<int> CollectCriticalIndices()
    {
        var criticalIndices = new List<int>();
        var previousValue = _head.Value;
        var index = 1;

        for (var node = _head.Next; node!.Next is not null; node = node.Next, index++)
        {
            if (IsLocalExtreme(previousValue, node.Value, node.Next.Value))
            {
                criticalIndices.Add(index);
            }

            previousValue = node.Value;
        }

        return criticalIndices;
    }

    private static int ComputeMinDistance(List<int> criticalIndices)
    {
        var minDistance = int.MaxValue;
        for (var i = 1; i < criticalIndices.Count; i++)
        {
            minDistance = Math.Min(minDistance, criticalIndices[i] - criticalIndices[i - 1]);
        }

        return minDistance;
    }

    [Benchmark]
    public int[] SinglePassConstantSpace()
    {
        var (minDistance, firstCriticalIndex, lastCriticalIndex) = ScanForCriticalPoints();

        return firstCriticalIndex == lastCriticalIndex
            ? [-1, -1]
            : [minDistance, lastCriticalIndex - firstCriticalIndex];
    }

    private (int MinDistance, int FirstCriticalIndex, int LastCriticalIndex) ScanForCriticalPoints()
    {
        var minDistance = int.MaxValue;
        var firstCriticalIndex = -1;
        var previousCriticalIndex = -1;
        var lastCriticalIndex = -1;
        var previousValue = _head.Value;
        var index = 1;

        for (var node = _head.Next; node!.Next is not null; node = node.Next, index++)
        {
            if (IsLocalExtreme(previousValue, node.Value, node.Next.Value))
            {
                (minDistance, firstCriticalIndex, previousCriticalIndex, lastCriticalIndex) =
                    TrackCriticalIndex(index, minDistance, firstCriticalIndex, previousCriticalIndex);
            }

            previousValue = node.Value;
        }

        return (minDistance, firstCriticalIndex, lastCriticalIndex);
    }

    private static (int MinDistance, int FirstCriticalIndex, int PreviousCriticalIndex, int LastCriticalIndex) TrackCriticalIndex(
        int index, int minDistance, int firstCriticalIndex, int previousCriticalIndex)
    {
        if (firstCriticalIndex == -1)
        {
            firstCriticalIndex = index;
        }
        else
        {
            minDistance = Math.Min(minDistance, index - previousCriticalIndex);
        }

        return (minDistance, firstCriticalIndex, index, index);
    }

    private static bool IsLocalExtreme(int previous, int current, int next)
        => (current > previous && current > next) || (current < previous && current < next);

    // Strict sign alternation forces almost every interior node to be a genuine
    // local max or min (a positive value is always greater than its negative
    // neighbors, and vice versa) - both strategies then do comparable real work
    // instead of the list happening to be monotonic in places.
    private static SinglyLinkedListNode<int> BuildZigzagList(int length)
    {
        var random = new Random(RandomSeed);
        var head = new SinglyLinkedListNode<int>(random.Next(1, MaxMagnitudeExclusive));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            var magnitude = random.Next(1, MaxMagnitudeExclusive);
            var value = i % AlternatingSignModulus == 0 ? magnitude : -magnitude;
            tail.Next = new SinglyLinkedListNode<int>(value);
            tail = tail.Next;
        }

        return head;
    }
}
