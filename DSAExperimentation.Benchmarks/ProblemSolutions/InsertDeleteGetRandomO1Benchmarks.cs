using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.InsertDeleteGetRandomO1.InsertDeleteGetRandomO1Solution;

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
    [Params(200, 20_000)]
    public int Count;

    private int[] _insertOrder = null!;
    private int[] _removalOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        _insertOrder = Enumerable.Range(0, Count).ToArray();

        var random = new Random(1);
        _removalOrder = _insertOrder.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListScan() => Replay(new RandomizedSetByListScan());

    [Benchmark]
    public int HashMapSwapRemove() => Replay(new RandomizedSetByHashMapSwapRemove());

    private int Replay(IRandomizedSet set)
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
