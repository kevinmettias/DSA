using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostToConvertStringII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToConvertStringIISolution's, the
// same methods MinimumCostToConvertStringIITests proves correct. Each arm is
// handed the prepared input its hoisted overload takes - a string index plus
// raw distance matrix for the brute-force arm, a built SubstringNetwork for
// the AllPairsShortestPaths arm - so building the substring conversion graph
// is charged to [GlobalSetup] rather than to the per-position DP walk being
// measured.
[MemoryDiagnoser]
public class MinimumCostToConvertStringIIBenchmarks
{
    private const int RulesSeed = 2977;
    private const int StringSeed = 29770;

    private string _source = "";

    private string _target = "";
    private Dictionary<string, int> _index = new();
    private long[,] _distances = new long[0, 0];
    private SubstringNetwork _network = null!;
    [Params(100, 1000)]
    public int StringLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (original, changed, cost) = SubstringConversionWorkloads.BuildRules(RulesSeed);
        var (source, target) = SubstringConversionWorkloads.BuildStrings(StringLength, StringSeed);

        _source = source;
        _target = target;
        _index = MinimumCostToConvertStringIISolution.BuildIndex(original, changed);
        _distances = MinimumCostToConvertStringIISolution.BuildDistanceMatrix(_index, original, changed, cost);
        _network = SubstringNetwork.Build(original, changed, cost);
    }

    [Benchmark(Baseline = true)]
    public long BruteForceFloydWarshall() =>
        MinimumCostToConvertStringIISolution.MinimumCostByBruteForceFloydWarshall(
            new SourceText(_source), new TargetText(_target), _index, _distances);

    [Benchmark]
    public long AllPairsShortestPaths() =>
        MinimumCostToConvertStringIISolution.MinimumCostByAllPairsShortestPaths(
            new SourceText(_source), new TargetText(_target), _network);
}
