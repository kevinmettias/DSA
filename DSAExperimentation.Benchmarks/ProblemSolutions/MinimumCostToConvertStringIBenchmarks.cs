using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinimumCostToConvertStringI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToConvertStringISolution's, the
// same methods MinimumCostToConvertStringITests proves correct. Each arm is
// handed the prepared input its hoisted overload takes - a raw distance
// matrix for the brute-force arm, a built LetterNetwork for the
// AllPairsShortestPaths arm - so building the 26-letter conversion graph is
// charged to [GlobalSetup] rather than to the per-position lookup being
// measured.
[MemoryDiagnoser]
public class MinimumCostToConvertStringIBenchmarks
{
    private const int RulesSeed = 2976;
    private const int StringSeed = 29760;

    private string _source = "";

    private string _target = "";
    private long[,] _distances = new long[0, 0];
    private LetterNetwork _network = null!;
    [Params(100, 5000)]
    public int StringLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var (original, changed, cost) = LetterConversionWorkloads.BuildRing(RulesSeed);
        var (source, target) = LetterConversionWorkloads.BuildStrings(StringLength, StringSeed);

        _source = source;
        _target = target;
        _distances = MinimumCostToConvertStringISolution.BuildDistanceMatrix(original, changed, cost);
        _network = LetterNetwork.Build(original, changed, cost);
    }

    [Benchmark(Baseline = true)]
    public long BruteForceFloydWarshall() =>
        MinimumCostToConvertStringISolution.MinimumCostByBruteForceFloydWarshall(
            new MinimumCostToConvertStringISolution.SourceText(_source),
            new MinimumCostToConvertStringISolution.TargetText(_target),
            _distances);

    [Benchmark]
    public long AllPairsShortestPaths() =>
        MinimumCostToConvertStringISolution.MinimumCostByAllPairsShortestPaths(
            new MinimumCostToConvertStringISolution.SourceText(_source),
            new MinimumCostToConvertStringISolution.TargetText(_target),
            _network);
}
