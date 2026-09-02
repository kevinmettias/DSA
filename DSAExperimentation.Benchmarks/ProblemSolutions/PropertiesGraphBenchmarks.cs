using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PropertiesGraph;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PropertiesGraphSolution's, the same methods
// PropertiesGraphTests proves correct. Both are O(n^2 * m) - LC's own n, m <= 100
// constraints make that the intended order - so RowCount stays at LC's own
// ceiling rather than growing past it.
[MemoryDiagnoser]
public class PropertiesGraphBenchmarks
{
    private const int Seed = 3493; // LC problem number
    private const int ColumnCount = 20;
    private const int K = 6;

    [Params(20, 100)]
    public int RowCount;

    private int[][] _properties = null!;

    [GlobalSetup]
    public void Setup() => _properties = PropertiesGraphWorkloads.BuildProperties(RowCount, ColumnCount, Seed);

    [Benchmark(Baseline = true)]
    public int BruteForce() => PropertiesGraphSolution.NumberOfComponentsByBruteForce(_properties, K);

    [Benchmark]
    public int DisjointSet() => PropertiesGraphSolution.NumberOfComponentsByDisjointSet(_properties, K);
}
