using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SpecialPermutations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpecialPermutationsSolution's, the same methods
// SpecialPermutationsTests proves correct - enumerate all n! orderings with
// this repo's own Backtrack.Search and check the adjacency rule once each one
// is complete (so Length has to stay small enough for this arm to finish) vs.
// threading (Remaining, Last) through this repo's own Memoizer, visiting each
// reachable state at most once regardless of how many orderings are legal.
[MemoryDiagnoser]
public class SpecialPermutationsBenchmarks
{
    private const int MaxValueExclusive = 60;

    // LC problem number, reused as the deterministic element seed.
    private const int Seed = 2741;

    private int[] _nums = [];

    [Params(8, 10)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var distinct = new HashSet<int>();

        while (distinct.Count < Length)
        {
            var value = random.Next(1, MaxValueExclusive);
            distinct.Add(value);
        }

        _nums = [.. distinct];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceBacktracking() => SpecialPermutationsSolution.CountByBruteForceBacktracking(_nums);

    [Benchmark]
    public int BitmaskMemo() => SpecialPermutationsSolution.CountByBitmaskMemo(_nums);
}
