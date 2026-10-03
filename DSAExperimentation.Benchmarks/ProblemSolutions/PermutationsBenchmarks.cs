using DSAExperimentation.LeetCode.Permutations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PermutationsSolution's, now returning the same
// permutations the test proves correct instead of merely counting them.
public class PermutationsBenchmarks
{
    private int[] _values = [];

    [Params(3, 6)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public List<List<int>> SpecializedRecursive() => PermutationsSolution.PermuteBySpecializedRecursion(_values);

    [Benchmark]
    public List<List<int>> Backtracking() => PermutationsSolution.PermuteByBacktracking(_values);
}
