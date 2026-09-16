using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.RobotCollisions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RobotCollisionsSolution's, the same methods
// RobotCollisionsTests proves correct. They sort positions identically, via
// this repo's own MergeSort over an ArrayIndexedSequence, so the measurement
// is entirely about collision resolution - one adjacent pair per full rescan
// from the front, vs. a single left-to-right sweep over this repo's own
// Stack<int> that pushes and pops each index at most once.
[MemoryDiagnoser]
public class RobotCollisionsBenchmarks
{
    private const int MaxHealthExclusive = 1_000;

    // LC problem number, reused as the deterministic workload seed.
    private const int Seed = 2751;

    private int[] _positions = [];

    private int[] _healths = [];
    private string _directions = "";
    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);

        _positions = SeededSequences.ShuffledOneTo(Length, random);
        _healths = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxHealthExclusive)).ToArray();
        _directions = new string([.. Enumerable.Range(0, Length).Select(_ => IsLeftward(random) ? 'L' : 'R')]);
    }

    // The draw is the robot's direction: half move left, half move right.
    private static bool IsLeftward(Random random) => random.Next(2) == 0;

    [Benchmark(Baseline = true)]
    public int[] RepeatedScan() => RobotCollisionsSolution.SurvivorHealthsByRepeatedScan(_positions, _healths, _directions);

    [Benchmark]
    public int[] StackSimulation() => RobotCollisionsSolution.SurvivorHealthsByStackSimulation(_positions, _healths, _directions);
}
