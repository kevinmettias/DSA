using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StringTransformation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StringTransformationSolution's, the same methods
// StringTransformationTests proves correct. They share the closed-form modular
// combine step (O(log k) regardless of k, so k stays a large fixed constant here
// rather than a varying axis) - the isolated variable is how the single
// rotation-match count it needs gets computed: an O(n^2) brute-force window
// compare vs. this repo's O(n) ZFunction.FindAll.
[MemoryDiagnoser]
public class StringTransformationBenchmarks
{
    private const int Seed = 1;
    private const int AlphabetSize = 26;
    private const long Operations = 1_000_000_000_007;

    // The target is the source rotated by half its length - a genuine rotation
    // rather than the source itself, so both strategies find a real, non-trivial
    // match instead of the degenerate zero-match case.
    private const int HalfwayDivisor = 2;

    private string _source = "";
    private string _target = "";

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var characters = Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray();
        var source = new string(characters);
        var rotation = Length / HalfwayDivisor;

        _source = source;
        _target = source[rotation..] + source[..rotation];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRotationCompare() =>
        StringTransformationSolution.NumberOfWaysByBruteForceRotationCompare(_source, _target, Operations);

    [Benchmark]
    public int ZFunctionSearch() =>
        StringTransformationSolution.NumberOfWaysByZFunction(_source, _target, Operations);
}
