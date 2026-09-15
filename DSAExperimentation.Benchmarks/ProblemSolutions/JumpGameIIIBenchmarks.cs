using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.JumpGameIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are JumpGameIIISolution's, the same methods
// JumpGameIIITests proves correct. _arr deliberately never contains a 0 (values
// drawn from [1, Length)), the same "force the real worst case" intent
// TwoSumBenchmarks' _target uses: neither strategy can short-circuit on an early
// hit, so both are forced through the full reachable component from _start.
[MemoryDiagnoser]
public class JumpGameIIIBenchmarks
{
    // LC problem number, reused as the deterministic seed.
    private const int RandomSeed = 1306;

    private int[] _arr = [];

    private int _start;
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _arr[i] = random.Next(1, Length);
        }

        _start = 0;
    }

    [Benchmark(Baseline = true)]
    public bool HandRolledStackWalk() => JumpGameIIISolution.CanReachByStackWalk(_arr, _start);

    [Benchmark]
    public bool DepthFirstSearchTraverse() =>
        JumpGameIIISolution.CanReachByDepthFirstSearch(_arr, _start);
}
