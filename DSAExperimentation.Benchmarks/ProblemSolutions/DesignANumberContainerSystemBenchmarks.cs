using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignANumberContainerSystem.DesignANumberContainerSystemSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignANumberContainerSystemSolution's, the same
// classes DesignANumberContainerSystemTests proves correct. [GlobalSetup] builds
// the call script - each index first gets its own distinct number (a bijection over
// [0, Count)), then ~20% of indices are reassigned to a different random number,
// just enough churn to exercise the heap strategy's lazy discarding of now-stale
// entries - so script construction is charged to setup rather than to the replay
// each arm measures.
//
// With numbers this sparse (at most a couple of indices ever share one), a matching
// index is roughly uniformly positioned across the whole assignment table, so
// LinearScan's fresh scan for the smallest matching index genuinely costs O(n) on
// average per query - unlike a small fixed number domain, where scanning tends to
// hit a match within the first few entries regardless of Count and never actually
// exercises the O(n) case. LazyDeletionHeap's per-number heap stays O(1)-sized
// here, so Find resolves in amortized O(1) instead.
[MemoryDiagnoser]
public class DesignANumberContainerSystemBenchmarks
{
    private const int RandomSeed = 2349;
    private const int ChurnDivisor = 5;

    [Params(200, 3_000)]
    public int Count;

    private int[] _changeIndices = null!;
    private int[] _changeNumbers = null!;
    private int[] _findQueries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var churnCount = Count / ChurnDivisor;

        _changeIndices = Enumerable.Range(0, Count)
            .Concat(Enumerable.Range(0, churnCount).Select(_ => random.Next(0, Count)))
            .ToArray();
        _changeNumbers = Enumerable.Range(0, Count)
            .Concat(Enumerable.Range(0, churnCount).Select(_ => random.Next(0, Count)))
            .ToArray();
        _findQueries = Enumerable.Range(0, Count).Select(_ => random.Next(0, Count)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long LinearScan() => Replay(new NumberContainersByLinearScan());

    [Benchmark]
    public long LazyDeletionHeap() => Replay(new NumberContainersByLazyDeletionHeap());

    // Sums every reported index rather than discarding it, so the JIT can't
    // eliminate the replay as dead code.
    private long Replay(INumberContainerStrategy strategy)
    {
        for (var i = 0; i < _changeIndices.Length; i++)
        {
            strategy.Change(_changeIndices[i], _changeNumbers[i]);
        }

        var foundIndexSum = 0L;

        foreach (var number in _findQueries)
        {
            foundIndexSum += strategy.Find(number);
        }

        return foundIndexSum;
    }
}
