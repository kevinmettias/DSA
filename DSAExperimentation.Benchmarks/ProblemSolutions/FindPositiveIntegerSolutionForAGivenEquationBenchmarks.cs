using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindPositiveIntegerSolutionForAGivenEquation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are
// FindPositiveIntegerSolutionForAGivenEquationSolution's, the same methods
// FindPositiveIntegerSolutionForAGivenEquationTests proves correct - the O(n^2)
// check-every-pair brute force, the O(n) two-pointer walk, and the O(n log n) per-row
// BinarySearch.Find over this repo's own IRandomAccessSequence
// (ShortestPathAlgorithmBenchmarks precedent for measuring every real tier instead of
// just winner-vs-brute-force). BinarySearchPerRow is expected to lose to TwoPointer;
// the point of including it is that the reusable primitive is available and correct.
//
// Each arm takes the explicit-bound overload so the measured search space is the
// [Params] value, and .Count so the three return the same comparable measurement
// while still building LeetCode's real answer (the pre-migration arms only counted).
[MemoryDiagnoser]
public class FindPositiveIntegerSolutionForAGivenEquationBenchmarks
{
    // Doubling factor for the largest reachable sum (Bound + Bound).
    private const int MaxSumFactor = 2;

    // Hoisted so the oracle's construction is not charged to any measured method - and
    // all three arms pay the same one call-through-the-interface cost the hidden
    // CustomFunction imposes.
    private static readonly ICustomFunction Sum = new SumFunction();

    private int _z;

    [Params(300, 1_000)]
    public int Bound { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // The largest reachable sum (Bound + Bound) is never a solution here, so
        // every strategy is forced through its full worst-case walk instead of an
        // early-exit on the first/last pair making brute force look artificially
        // competitive.
        _z = (MaxSumFactor * Bound) - 1;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceEveryPair() =>
        FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByBruteForce(Sum, _z, Bound).Count;

    [Benchmark]
    public int TwoPointer() =>
        FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByTwoPointer(Sum, _z, Bound).Count;

    [Benchmark]
    public int BinarySearchPerRow() =>
        FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByBinarySearchPerRow(Sum, _z, Bound).Count;

    // LeetCode example 1's function_id, f(x, y) = x + y, as a named implementation
    // because the three arms now take an ICustomFunction and C# converts no lambda to an
    // interface - the same thing LeetCode's own CustomFunction object asks of a caller.
    private sealed class SumFunction : ICustomFunction
    {
        public int Evaluate(int x, int y) => x + y;
    }
}
