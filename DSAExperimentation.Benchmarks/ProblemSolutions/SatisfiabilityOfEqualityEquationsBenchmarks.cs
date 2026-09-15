using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SatisfiabilityOfEqualityEquations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SatisfiabilityOfEqualityEquationsSolution's, the same
// methods SatisfiabilityOfEqualityEquationsTests proves correct. The baseline pays a
// freshly allocated HashSet<char>+Queue<char> BFS per "!=" equation over an adjacency
// list rebuilt from the "==" equations; the composed arm is this repo's own
// DisjointSet(26) - O(1) Union per equality and O(a(26)) IsConnected per inequality,
// with no per-query allocation.
[MemoryDiagnoser]
public class SatisfiabilityOfEqualityEquationsBenchmarks
{
    private const int RandomSeed = 1;

    private string[] _equations = [];

    [Params(100, 500)]
    public int EquationCount { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _equations = EqualityEquationWorkloads.BuildEquations(EquationCount, seed: RandomSeed);

    [Benchmark(Baseline = true)]
    public bool AdjacencyListBfs() =>
        SatisfiabilityOfEqualityEquationsSolution.EquationsPossibleByAdjacencyBfs(_equations);

    [Benchmark]
    public bool DisjointSetUnionFind() =>
        SatisfiabilityOfEqualityEquationsSolution.EquationsPossibleByDisjointSet(_equations);
}
