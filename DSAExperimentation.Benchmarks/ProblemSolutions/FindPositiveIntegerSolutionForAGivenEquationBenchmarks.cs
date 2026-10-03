using DSAExperimentation.LeetCode.FindPositiveIntegerSolutionForAGivenEquation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: all three arms are
// FindPositiveIntegerSolutionForAGivenEquationSolution's, the same methods
// FindPositiveIntegerSolutionForAGivenEquationSolutionTests proves correct - the O(n^2)
// check-every-pair brute force, the O(n) two-pointer walk, and the O(n log n) per-row
// BinarySearch.Find over this repo's own IRandomAccessSequence
// (ShortestPathAlgorithmBenchmarks precedent for measuring every real tier instead of
// just winner-vs-brute-force). BinarySearchPerRow is expected to lose to TwoPointer;
// the point of including it is that the reusable primitive is available and correct.
//
// Each arm takes the explicit-bound overload so the measured search space is the
// [Params] value, and returns LeetCode's real answer, the solving pairs (the
// pre-migration arms only counted).
//
// The target is LC 1237's largest z, 100, which the 99 pairs (x, 100 - x) solve. None
// of the walks can stop early at it: the brute force tries every pair, the two-pointer
// walk lowers y from Bound before it reaches them, and every row is binary-searched.
public class FindPositiveIntegerSolutionForAGivenEquationBenchmarks
{
    private const int TargetValue = 100;

    // Hoisted so the oracle's construction is not charged to any measured method - and
    // all three arms pay the same one call-through-the-interface cost the hidden
    // CustomFunction imposes.
    private static readonly ICustomFunction Sum = new SumFunction();

    [Params(300, 1_000)]
    public int Bound { get; set; }

    [Benchmark(Baseline = true)]
    public List<(int X, int Y)> BruteForceEveryPair() =>
        FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByBruteForce(Sum, TargetValue, Bound);

    [Benchmark]
    public List<(int X, int Y)> TwoPointer() =>
        FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByTwoPointer(Sum, TargetValue, Bound);

    [Benchmark]
    public List<(int X, int Y)> BinarySearchPerRow() =>
        FindPositiveIntegerSolutionForAGivenEquationSolution
            .FindSolutionsByBinarySearchPerRow(Sum, TargetValue, Bound);

    // LeetCode example 1's function_id, f(x, y) = x + y, as a named implementation
    // because the three arms now take an ICustomFunction and C# converts no lambda to an
    // interface - the same thing LeetCode's own CustomFunction object asks of a caller.
    private sealed class SumFunction : ICustomFunction
    {
        public int Evaluate(int xValue, int yValue) => xValue + yValue;
    }
}
