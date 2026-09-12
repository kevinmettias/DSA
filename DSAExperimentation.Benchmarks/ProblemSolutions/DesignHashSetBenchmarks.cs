using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignHashSet.DesignHashSetSolution;

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
    private const int RandomSeed = 705; // LC problem number

    [Params(200, 5_000)]
    public int Count;

    private int[] _addOrder = null!;
    private int[] _containsOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        _addOrder = Enumerable.Range(0, Count).ToArray();

        var random = new Random(RandomSeed);
        _containsOrder = _addOrder.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListScan() => Replay(new MyHashSetByListScan());

    [Benchmark]
    public int SetBacked() => Replay(new MyHashSetBySetBacked());

    private int Replay(IMyHashSet set)
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
