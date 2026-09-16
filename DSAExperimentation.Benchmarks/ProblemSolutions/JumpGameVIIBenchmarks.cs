using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.JumpGameVII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are JumpGameVIISolution's, the same methods
// JumpGameVIITests proves correct. The generated string is all '0' except a blocked
// final character, so neither strategy can short-circuit on an early success - both
// must exhaust every reachable index before returning false, which is exactly what
// makes the unmemoized baseline's Fibonacci-shaped call-count blowup real
// (StringLength=28 already reaches ~500K calls, 32 ~3.5M) while
// DepthFirstSearch.Traverse's HashSet-backed visited tracking stays linear in the
// string length. StringLength is kept modest for the same reason
// StoneGameVIIBenchmarks keeps PileCount modest for its own exponential baseline.
// Both arms take LeetCode's own input shape, so [GlobalSetup] only sizes the
// workload - it builds the string, and nothing the measured methods would otherwise
// be charged for.
[MemoryDiagnoser]
public class JumpGameVIIBenchmarks
{
    private const int MinJump = 1;
    private const int MaxJump = 2;

    private string _text = "";

    [Params(28, 32)]
    public int StringLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var chars = new char[StringLength];
        Array.Fill(chars, '0');
        chars[StringLength - 1] = '1';
        _text = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public bool CanReachByUnmemoizedRecursion() =>
        JumpGameVIISolution.CanReachByUnmemoizedRecursion(_text, MinJump, MaxJump);

    [Benchmark]
    public bool CanReachByVisitedTrackingTraversal() =>
        JumpGameVIISolution.CanReachByVisitedTrackingTraversal(_text, MinJump, MaxJump);
}
