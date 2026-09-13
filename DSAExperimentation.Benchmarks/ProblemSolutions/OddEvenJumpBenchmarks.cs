using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.OddEvenJump;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are OddEvenJumpSolution's, the same methods OddEvenJumpTests
// proves correct. The array is generated once in [GlobalSetup] and is already
// LeetCode's own input shape, so each arm is handed it directly - the quadratic
// per-index forward scan for each jump target against MergeSort plus a monotonic
// Stack<int> sweep, O(n^2) against O(n log n), with the same backward reachability
// pass on both sides.
[MemoryDiagnoser]
public class OddEvenJumpBenchmarks
{
    // LC problem number, used as the deterministic setup seed.
    private const int RandomSeed = 975;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, Length)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceScan() =>
        OddEvenJumpSolution.OddEvenJumpsByBruteForceScan(_values);

    [Benchmark]
    public int MergeSortStackSweep() =>
        OddEvenJumpSolution.OddEvenJumpsByMergeSortStackSweep(_values);
}
