using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.DesignHashSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignHashSetSolution's, the same classes
// DesignHashSetTests proves correct. A Design problem's whole point is a
// sequence of mutating calls against one instance, so there is no separate
// "prepare input" step to hoist into [GlobalSetup] beyond the add/probe order
// arrays themselves - [GlobalSetup] builds those (so shuffling isn't charged to
// the measured method) and each [Benchmark] arm constructs its own instance and
// replays the same add-then-probe script - O(n) per call vs. O(1) average per
// call.
[MemoryDiagnoser]
public class DesignHashSetBenchmarks
{
    private const int RandomSeed = 705; private int[] _addOrder = [];

    private int[] _containsOrder = [];
    // LC problem number

    [Params(200, 5_000)]
    public int Count { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _addOrder = Enumerable.Range(0, Count).ToArray();

        var random = new Random(RandomSeed);
        _containsOrder = _addOrder.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListScan() => Replay(new DesignHashSetSolution.MyHashSetByListScan());

    [Benchmark]
    public int SetBacked() => Replay(new DesignHashSetSolution.MyHashSetBySetBacked());

    private int Replay(DesignHashSetSolution.IMyHashSet set)
    {
        foreach (var value in _addOrder)
        {
            set.Add(value);
        }

        var hits = 0;

        foreach (var value in _containsOrder)
        {
            if (set.Contains(value))
            {
                hits++;
            }
        }

        return hits;
    }
}
