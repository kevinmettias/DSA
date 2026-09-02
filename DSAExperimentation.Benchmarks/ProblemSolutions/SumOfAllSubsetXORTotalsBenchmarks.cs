using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sum of All Subset XOR Totals (LC 1863): BruteForceBitmask recomputes each subset's
// XOR total from scratch by rescanning every element for each of the 2^n bitmasks -
// O(2^n * n). Backtracking instead walks this repo's own Backtrack.Search
// choose/explore/unchoose recursion, maintaining one running XOR toggled by a single
// XOR on Choose and untoggled by the same XOR on Unchoose - O(2^n) - the same
// exhaustive-enumeration-via-Backtrack.Search shape SubsetsIIBenchmarks.cs already
// uses.
[MemoryDiagnoser]
public class SumOfAllSubsetXORTotalsBenchmarks
{
    private const int RandomSeed = 1863; // LC problem number
    private const int MaxValueBitWidth = 20; // random values are drawn from [1, 2^20)

    [Params(10, 18)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1 << MaxValueBitWidth)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceBitmask()
    {
        var total = 0;
        var subsetCount = 1 << _values.Length;

        for (var mask = 0; mask < subsetCount; mask++)
        {
            var xorTotal = 0;
            for (var bit = 0; bit < _values.Length; bit++)
            {
                if ((mask & (1 << bit)) != 0)
                {
                    xorTotal ^= _values[bit];
                }
            }

            total += xorTotal;
        }

        return total;
    }

    [Benchmark]
    public int Backtracking()
    {
        var total = 0;
        var state = new XorState();

        Backtrack.Search<XorState, int>(
            state,
            isSolution: static _ => true,
            candidates: s => Enumerable.Range(s.NextIndex, _values.Length - s.NextIndex),
            choose: (s, index) =>
            {
                s.RunningXor ^= _values[index];
                s.NextIndex = index + 1;
            },
            unchoose: (s, index) => s.RunningXor ^= _values[index],
            onSolution: s => total += s.RunningXor);

        return total;
    }

    private sealed class XorState
    {
        public int RunningXor { get; set; }

        public int NextIndex { get; set; }
    }
}
