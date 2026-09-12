using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RobotReturnToOrigin;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RobotReturnToOriginSolution's, the same methods
// RobotReturnToOriginTests proves correct. The move string is built once in
// [GlobalSetup] so its random generation isn't charged to either arm.
[MemoryDiagnoser]
public class RobotReturnToOriginBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _moves = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        const string alphabet = "UDLR";
        _moves = new string(Enumerable.Range(0, Length).Select(_ => alphabet[random.Next(alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public bool SwitchStatement() => RobotReturnToOriginSolution.JudgeCircleBySwitch(_moves);

    [Benchmark]
    public bool HashMapLookup() => RobotReturnToOriginSolution.JudgeCircleByHashMapLookup(_moves);
}
