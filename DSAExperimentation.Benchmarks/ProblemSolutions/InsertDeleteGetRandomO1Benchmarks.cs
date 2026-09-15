using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.InsertDeleteGetRandomO1;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are InsertDeleteGetRandomO1Solution's, the same classes
// InsertDeleteGetRandomO1Tests proves correct. A Design problem's whole point is a
// sequence of mutating calls against one instance, so there is no separate "prepare
// input" step to hoist into [GlobalSetup] beyond the insert/removal order arrays
// themselves - [GlobalSetup] builds those (so shuffling isn't charged to the measured
// method) and each [Benchmark] arm constructs its own instance and replays the same
// insert-then-remove script, returning the surviving Count so the JIT can't eliminate
// the replay as dead code.
[MemoryDiagnoser]
public class InsertDeleteGetRandomO1Benchmarks
{
    private int[] _insertOrder = [];

    private int[] _removalOrder = [];
    [Params(200, 20_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _insertOrder = Enumerable.Range(0, Count).ToArray();

        var random = new Random(1);
        _removalOrder = _insertOrder.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListScan() => Replay(new InsertDeleteGetRandomO1Solution.RandomizedSetByListScan());

    [Benchmark]
    public int HashMapSwapRemove() => Replay(new InsertDeleteGetRandomO1Solution.RandomizedSetByHashMapSwapRemove());

    private int Replay(InsertDeleteGetRandomO1Solution.IRandomizedSet set)
    {
        foreach (var value in _insertOrder)
        {
            set.Insert(value);
        }

        foreach (var value in _removalOrder)
        {
            set.Remove(value);
        }

        return set.Count;
    }
}
