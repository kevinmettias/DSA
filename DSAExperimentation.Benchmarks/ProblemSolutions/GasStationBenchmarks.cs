using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Gas Station (LC 134): the O(n^2) brute force (simulate a full lap from every
// candidate start) vs. the O(n) greedy single pass that resets the candidate start
// whenever the running tank goes negative (GasStationTests' own algorithm). No repo
// primitive applies - this is a pure prefix-sum/greedy scan over the two arrays
// themselves, the same "no stronger reusable primitive" shape already established
// for JumpGame. _gas/_cost are built so a solution always exists but sits at the
// very end of the array, forcing the brute-force baseline through nearly all of its
// O(n^2) simulated laps instead of succeeding on an early candidate start.
[MemoryDiagnoser]
public class GasStationBenchmarks
{
    [Params(200, 3_000)]
    public int Length;

    private int[] _gas = null!;
    private int[] _cost = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(134);
        _gas = new int[Length];
        _cost = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _gas[i] = random.Next(1, 10);
            _cost[i] = random.Next(1, 10);
        }

        // Guarantee a solution exists (total gas >= total cost) without moving it to
        // index 0, so brute force can't short-circuit on its very first candidate.
        var deficit = _cost.Sum() - _gas.Sum();
        _gas[^1] += Math.Max(deficit, 0) + 1;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceSimulateEveryStart()
    {
        for (var start = 0; start < _gas.Length; start++)
        {
            var tank = 0;
            var completed = true;

            for (var step = 0; step < _gas.Length; step++)
            {
                var i = (start + step) % _gas.Length;
                tank += _gas[i] - _cost[i];

                if (tank < 0)
                {
                    completed = false;
                    break;
                }
            }

            if (completed)
            {
                return start;
            }
        }

        return -1;
    }

    [Benchmark]
    public int GreedyDebtReset()
    {
        var total = 0;
        var tank = 0;
        var start = 0;

        for (var i = 0; i < _gas.Length; i++)
        {
            var delta = _gas[i] - _cost[i];
            total += delta;
            tank += delta;

            if (tank < 0)
            {
                start = i + 1;
                tank = 0;
            }
        }

        return total < 0 ? -1 : start;
    }
}
