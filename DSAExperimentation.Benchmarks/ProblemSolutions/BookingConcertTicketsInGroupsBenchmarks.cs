using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Booking Concert Tickets in Groups (LC 2286): a raw-array baseline (Gather/Scatter
// each linear-scan every row up to maxRow, O(n) per call) vs. this repo's own
// SegmentTree<int,MaxOperation<int>>/SegmentTree<int,SumOperation<int>> pair plus
// BinarySearch.LowerBound (RangeSumQueryMutableBenchmarks' fresh-rebuild-per-run
// precedent, so mutation from one run never leaks into the next). The leftmost
// qualifying-row search collapses from an O(n) scan to O(log^2 n) via the same
// "binary search on the answer" idiom KokoEatingBananasTests already uses, and
// Scatter's feasibility check collapses to one O(log n) sum query instead of an
// O(n) row-by-row total. Both variants replay the same fixed stream of random
// Gather/Scatter calls.
[MemoryDiagnoser]
public class BookingConcertTicketsInGroupsBenchmarks
{
    private const int SeatsPerRow = 50;
    private const int OperationCount = 300;
    private const int RandomSeed = 2286; // LC problem number
    private const int OperationTypeCount = 2;

    [Params(200, 2_000)]
    public int RowCount;

    private (bool IsGather, int K, int MaxRow)[] _operations = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _operations = new (bool IsGather, int K, int MaxRow)[OperationCount];

        for (var i = 0; i < OperationCount; i++)
        {
            var isGather = random.Next(OperationTypeCount) == 0;
            var k = random.Next(1, SeatsPerRow + 1);
            var maxRow = random.Next(0, RowCount);
            _operations[i] = (isGather, k, maxRow);
        }
    }

    [Benchmark(Baseline = true)]
    public int ArrayScan()
    {
        var available = Enumerable.Repeat(SeatsPerRow, RowCount).ToArray();
        var bookings = 0;

        foreach (var (isGather, k, maxRow) in _operations)
        {
            bookings += isGather ? GatherLinear(available, k, maxRow) : ScatterLinear(available, k, maxRow);
        }

        return bookings;
    }

    [Benchmark]
    public int SegmentTreeBinarySearch()
    {
        var initial = Enumerable.Repeat(SeatsPerRow, RowCount).ToArray();
        var maxTree = new SegmentTree<int, MaxOperation<int>>(initial);
        var sumTree = new SegmentTree<int, SumOperation<int>>((int[])initial.Clone());
        var bookings = 0;

        foreach (var (isGather, k, maxRow) in _operations)
        {
            bookings += isGather ? GatherTree(maxTree, sumTree, k, maxRow) : ScatterTree(maxTree, sumTree, k, maxRow);
        }

        return bookings;
    }

    private static int GatherLinear(int[] available, int k, int maxRow)
    {
        for (var row = 0; row <= maxRow; row++)
        {
            if (available[row] >= k)
            {
                available[row] -= k;
                return 1;
            }
        }

        return 0;
    }

    private static int ScatterLinear(int[] available, int k, int maxRow)
    {
        var total = 0;

        for (var row = 0; row <= maxRow; row++)
        {
            total += available[row];
        }

        if (total < k)
        {
            return 0;
        }

        for (var row = 0; row <= maxRow && k > 0; row++)
        {
            var take = Math.Min(available[row], k);
            available[row] -= take;
            k -= take;
        }

        return 1;
    }

    private static int GatherTree(
        SegmentTree<int, MaxOperation<int>> maxTree, SegmentTree<int, SumOperation<int>> sumTree, int k, int maxRow)
    {
        var row = FindLeftmostRowWithCapacity(maxTree, fromRow: 0, maxRow, threshold: k);

        if (row is null)
        {
            return 0;
        }

        var available = maxTree.Query(row.Value, row.Value);
        SetAvailable(maxTree, sumTree, row.Value, available - k);

        return 1;
    }

    private static int ScatterTree(
        SegmentTree<int, MaxOperation<int>> maxTree, SegmentTree<int, SumOperation<int>> sumTree, int k, int maxRow)
    {
        if (sumTree.Query(0, maxRow) < k)
        {
            return 0;
        }

        var trees = new SeatTrees(maxTree, sumTree);
        var row = 0;

        while (k > 0)
        {
            (row, k) = ScatterStep(trees, row, maxRow, k);
        }

        return 1;
    }

    private static (int Row, int K) ScatterStep(SeatTrees trees, int row, int maxRow, int k)
    {
        row = FindLeftmostRowWithCapacity(trees.MaxTree, row, maxRow, threshold: 1)!.Value;

        var available = trees.MaxTree.Query(row, row);
        var take = Math.Min(available, k);
        SetAvailable(trees.MaxTree, trees.SumTree, row, available - take);
        k -= take;

        if (take == available)
        {
            row++;
        }

        return (row, k);
    }

    private static int? FindLeftmostRowWithCapacity(
        SegmentTree<int, MaxOperation<int>> maxTree, int fromRow, int maxRow, int threshold)
    {
        var sequence = new HasCapacitySequence(maxTree, fromRow, maxRow, threshold);
        var offset = BinarySearch.LowerBound(sequence, true);

        return offset >= sequence.Length ? null : fromRow + offset;
    }

    private static void SetAvailable(
        SegmentTree<int, MaxOperation<int>> maxTree, SegmentTree<int, SumOperation<int>> sumTree, int row, int available)
    {
        maxTree.Update(row, available);
        sumTree.Update(row, available);
    }

    private readonly struct HasCapacitySequence(
        SegmentTree<int, MaxOperation<int>> availableSeats, int fromRow, int maxRow, int threshold)
        : IRandomAccessSequence<bool>
    {
        public int Length => maxRow - fromRow + 1;

        public bool Get(int index) => availableSeats.Query(fromRow, fromRow + index) >= threshold;
    }

    private readonly record struct SeatTrees(
        SegmentTree<int, MaxOperation<int>> MaxTree, SegmentTree<int, SumOperation<int>> SumTree);
}
