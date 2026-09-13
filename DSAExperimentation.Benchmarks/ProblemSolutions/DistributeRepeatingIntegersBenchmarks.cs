using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.LeetCode.DistributeRepeatingIntegers;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DistributeRepeatingIntegersSolution's, the same
// methods DistributeRepeatingIntegersTests proves correct - a hand-written
// recursion against this repo's generic Backtrack.TrySearch closed over the
// identical choose/explore/unchoose steps. Each arm is handed the prepared stock
// counts its hoisted overload takes, so collapsing nums into per-value counts is
// charged to [GlobalSetup] rather than to the search being measured.
//
// _orders is ValueCount interleaved copies of 1..OrdersPerValue and every stock
// bucket holds their shared sum, so a valid distribution always exists - each
// bucket can be exactly refilled by re-assembling the copy it came from - but the
// shuffled ordering still forces a real search rather than an immediate match.
[MemoryDiagnoser]
public class DistributeRepeatingIntegersBenchmarks
{
    private const int OrdersPerValue = 6;

    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 1655;

    [Params(3, 5)]
    public int ValueCount;

    private int[] _orders = null!;
    private DynamicArray<int> _stock = null!;

    [GlobalSetup]
    public void Setup()
    {
        var perValue = Enumerable.Range(1, OrdersPerValue).ToArray();
        var all = new List<int>();

        for (var value = 0; value < ValueCount; value++)
        {
            all.AddRange(perValue);
        }

        var random = new Random(RandomSeed);
        _orders = all.OrderBy(_ => random.Next()).ToArray();
        _stock = new DynamicArray<int>();
        var capacity = perValue.Sum();

        for (var value = 0; value < ValueCount; value++)
        {
            _stock.Add(capacity);
        }
    }

    [Benchmark(Baseline = true)]
    public bool NaiveBacktracking() =>
        DistributeRepeatingIntegersSolution.CanDistributeByNaiveBacktracking(_stock, _orders);

    [Benchmark]
    public bool BacktrackPrimitive() =>
        DistributeRepeatingIntegersSolution.CanDistributeByGenericBacktrack(_stock, _orders);
}
