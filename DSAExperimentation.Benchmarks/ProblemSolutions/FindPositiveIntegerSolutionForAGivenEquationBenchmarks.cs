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

    // Hoisted so the delegate allocation is not charged to any measured method - and
    // all three arms pay the same one call-through-delegate cost the hidden
    // CustomFunction imposes.
    private static readonly Func<int, int, int> Sum = (x, y) => x + y;

    [Params(300, 1_000)]
    public int Bound;

    private int _z;

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
}
