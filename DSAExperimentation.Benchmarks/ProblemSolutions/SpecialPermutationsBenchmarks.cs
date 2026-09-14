using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.SpecialPermutations.SpecialPermutationsSolution;

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

    [Params(8, 10)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var distinct = new HashSet<int>();

        while (distinct.Count < Length)
        {
            distinct.Add(random.Next(1, MaxValueExclusive));
        }

        _nums = [.. distinct];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceBacktracking() => CountByBruteForceBacktracking(_nums);

    [Benchmark]
    public int BitmaskMemo() => CountByBitmaskMemo(_nums);
}
