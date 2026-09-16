using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RobotReturnToOrigin;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RobotReturnToOriginSolution's, the same methods
// RobotReturnToOriginTests proves correct. The move string is built once in
// [GlobalSetup] so its random generation isn't charged to either arm.
[MemoryDiagnoser]
public class RobotReturnToOriginBenchmarks
{
    private const string Alphabet = "UDLR";

    private string _moves = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _moves = new string(Enumerable.Range(0, Length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public bool IsAtOriginBySwitch() => RobotReturnToOriginSolution.IsAtOriginBySwitch(_moves);

    [Benchmark]
    public bool IsAtOriginByHashMapLookup() => RobotReturnToOriginSolution.IsAtOriginByHashMapLookup(_moves);
}
