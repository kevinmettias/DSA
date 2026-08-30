using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game (LC 55): the O(n^2) forward-reachability DP (mark every index
// reachable from each already-reachable index) vs. the O(n) greedy single pass
// that tracks the farthest index reached so far. No repo primitive applies -
// this is a pure reachability scan over the array itself, the same
// "no stronger reusable primitive" shape already established for GasStation.
// _values is built so every jump length reaches deep into the rest of the
// array, forcing the DP baseline through its full O(n^2) inner scan instead of
// short-circuiting on an early reachable index.
[MemoryDiagnoser]
public class JumpGameBenchmarks
{
    [Params(200, 3_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(55);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(Length / 4, Length)).ToArray();
        _values[^1] = 0;
    }

    [Benchmark(Baseline = true)]
    public bool ForwardReachabilityDP()
    {
        var reachable = new bool[_values.Length];
        reachable[0] = true;

        for (var i = 0; i < _values.Length; i++)
        {
            if (!reachable[i])
            {
                continue;
            }

            var maxStep = Math.Min(_values[i], _values.Length - 1 - i);

            for (var step = 1; step <= maxStep; step++)
            {
                reachable[i + step] = true;
            }
        }

        return reachable[^1];
    }

    [Benchmark]
    public bool GreedyFarthestReach()
    {
        var reach = 0;

        for (var i = 0; i < _values.Length && i <= reach; i++)
        {
            reach = Math.Max(reach, i + _values[i]);
        }

        return reach >= _values.Length - 1;
    }
}
