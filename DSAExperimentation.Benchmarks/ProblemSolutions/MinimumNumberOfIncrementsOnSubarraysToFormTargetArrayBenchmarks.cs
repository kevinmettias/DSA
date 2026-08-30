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
    [Params(200, 2_000)]
    public int Length;

    private int[] _target = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1526);
        _target = Enumerable.Range(0, Length).Select(_ => random.Next(0, 50)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int LayerByLayerSimulation()
    {
        var current = new int[_target.Length];
        var operations = 0;
        var madeProgress = true;

        while (madeProgress)
        {
            madeProgress = false;
            var i = 0;

            while (i < current.Length)
            {
                if (current[i] >= _target[i])
                {
                    i++;
                    continue;
                }

                madeProgress = true;

                while (i < current.Length && current[i] < _target[i])
                {
                    current[i]++;
                    i++;
                }

                operations++;
            }
        }

        return operations;
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
