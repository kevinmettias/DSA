using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Distribute Repeating Integers (LC 1655): the same PartitionToKEqualSumSubsetsBenchmarks
// shape (a hand-specialized recursion vs. this repo's generic Backtrack.TrySearch
// closed over the identical choose/explore/unchoose steps), reframed as orders
// (the items) assigned against each distinct value's stock count (a bucket
// capacity) instead of a fixed target sum. _orders is ValueCount interleaved
// copies of 1..OrdersPerValue, so every stock bucket's capacity (their shared sum)
// can always be exactly refilled by re-assembling the copy it originally came
// from - a valid distribution always exists - but the interleaving still forces a
// real search rather than an immediate match.
[MemoryDiagnoser]
public class DistributeRepeatingIntegersBenchmarks
{
    private const int OrdersPerValue = 6;

    [Params(3, 5)]
    public int ValueCount;

    private int[] _orders = null!;
    private int[] _stock = null!;

    [GlobalSetup]
    public void Setup()
    {
        var perValue = Enumerable.Range(1, OrdersPerValue).ToArray();
        var all = new List<int>();

        for (var value = 0; value < ValueCount; value++)
        {
            all.AddRange(perValue);
        }

        var random = new Random(1655);
        _orders = all.OrderBy(_ => random.Next()).ToArray();
        _stock = Enumerable.Repeat(perValue.Sum(), ValueCount).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool NaiveBacktracking()
    {
        var sorted = (int[])_orders.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        var remaining = (int[])_stock.Clone();

        return Search(0);

        bool Search(int index)
        {
            if (index == sorted.Length)
            {
                return true;
            }

            for (var value = 0; value < remaining.Length; value++)
            {
                if (remaining[value] < sorted[index])
                {
                    continue;
                }

                remaining[value] -= sorted[index];

                if (Search(index + 1))
                {
                    return true;
                }

                remaining[value] += sorted[index];
            }

            return false;
        }
    }

    [Benchmark]
    public bool BacktrackPrimitive()
    {
        var sorted = (int[])_orders.Clone();
        Array.Sort(sorted);
        Array.Reverse(sorted);
        var state = new State(sorted, _stock);

        return Backtrack.TrySearch<State, int>(state, new BacktrackingSteps<State, int>(
            IsSolution: s => s.Index == sorted.Length,
            Candidates: s => s.Index == sorted.Length ? [] : Enumerable.Range(0, _stock.Length).Where(s.CanPlace),
            Choose: (s, value) => s.Place(value),
            Unchoose: (s, value) => s.Remove(value),
            OnSolution: _ => true));
    }

    private sealed class State(int[] orders, int[] stock)
    {
        private readonly int[] _remaining = (int[])stock.Clone();

        public int Index { get; private set; }

        public bool CanPlace(int value) => _remaining[value] >= orders[Index];

        public void Place(int value)
        {
            _remaining[value] -= orders[Index];
            Index++;
        }

        public void Remove(int value)
        {
            Index--;
            _remaining[value] += orders[Index];
        }
    }
}
