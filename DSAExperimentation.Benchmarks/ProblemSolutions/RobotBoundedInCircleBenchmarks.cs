using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RobotBoundedInCircle;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RobotBoundedInCircleSolution's, the same methods
// RobotBoundedInCircleTests proves correct - a hand-written switch per facing
// against this repo's own HashMap<RobotDirection,(int,int)> step-delta lookup table, the same
// contrast RobotReturnToOriginBenchmarks draws for its simpler fixed-per-character
// delta case. Instruction-string construction is charged to [GlobalSetup].
[MemoryDiagnoser]
public class RobotBoundedInCircleBenchmarks
{
    // LC problem number's last digit is not meaningful here; 1 keeps the original
    // deterministic instruction stream.
    private const int InstructionSeed = 1;
    private const string Alphabet = "GLR";

    private string _instructions = "";

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(InstructionSeed);

        _instructions = new string(
            Enumerable.Range(0, Length).Select(_ => Alphabet[random.Next(Alphabet.Length)]).ToArray());
    }

    [Benchmark(Baseline = true)]
    public bool SwitchStatement() => RobotBoundedInCircleSolution.IsRobotBoundedByDirectionSwitch(_instructions);

    [Benchmark]
    public bool HashMapLookup() => RobotBoundedInCircleSolution.IsRobotBoundedByStepDeltaMap(_instructions);
}
