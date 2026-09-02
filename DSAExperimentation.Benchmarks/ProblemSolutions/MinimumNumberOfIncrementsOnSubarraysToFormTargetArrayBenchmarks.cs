using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Increments on Subarrays to Form a Target Array (LC 1526): a literal
// layer-by-layer simulation of the increment operations (each pass extends one stroke as far
// right as it can before starting the next, O(n * max(target)) total) vs. the O(n) single pass
// that sums positive rises between consecutive elements. No repo primitive applies to either
// side - both are pure array scans, the same category MaximumSubarrayBenchmarks already
// established for this kind of greedy problem.
[MemoryDiagnoser]
public class MinimumNumberOfIncrementsOnSubarraysToFormTargetArrayBenchmarks
{
    private const int RandomSeed = 1526; // LC 1526
    private const int MaxTargetHeight = 50;

    [Params(200, 2_000)]
    public int Length;

    private int[] _target = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _target = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxTargetHeight)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LayerByLayerSimulation()
    {
        var current = new int[_target.Length];
        var operations = 0;
        var madeProgress = true;

        while (madeProgress)
        {
            (madeProgress, operations) = RunSweep(current, _target, operations);
        }

        return operations;
    }

    private static (bool MadeProgress, int Operations) RunSweep(int[] current, int[] target, int operations)
    {
        var madeProgress = false;
        var i = 0;

        while (i < current.Length)
        {
            var (nextIndex, advancedStroke) = AdvanceFromIndex(current, target, i);
            i = nextIndex;

            if (advancedStroke)
            {
                madeProgress = true;
                operations++;
            }
        }

        return (madeProgress, operations);
    }

    private static (int Index, bool AdvancedStroke) AdvanceFromIndex(int[] current, int[] target, int index)
    {
        if (current[index] >= target[index])
        {
            return (index + 1, false);
        }

        while (index < current.Length && current[index] < target[index])
        {
            current[index]++;
            index++;
        }

        return (index, true);
    }

    [Benchmark]
    public int RunningDiffScan()
    {
        var operations = 0;
        var previous = 0;

        for (var i = 0; i < _target.Length; i++)
        {
            if (_target[i] > previous)
            {
                operations += _target[i] - previous;
            }

            previous = _target[i];
        }

        return operations;
    }
}
