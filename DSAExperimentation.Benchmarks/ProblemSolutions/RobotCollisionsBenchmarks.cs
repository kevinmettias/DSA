using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.RobotCollisions.RobotCollisionsSolution;

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

    [Params(200, 3_000)]
    public int Length;

    private int[] _positions = null!;
    private int[] _healths = null!;
    private string _directions = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);

        var shuffledPositions = Enumerable.Range(1, Length).ToArray();
        for (var i = shuffledPositions.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (shuffledPositions[i], shuffledPositions[j]) = (shuffledPositions[j], shuffledPositions[i]);
        }

        _positions = shuffledPositions;
        _healths = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxHealthExclusive)).ToArray();
        _directions = new string([.. Enumerable.Range(0, Length).Select(_ => random.Next(2) == 0 ? 'L' : 'R')]);
    }

    [Benchmark(Baseline = true)]
    public int[] RepeatedScan() => SurvivorHealthsByRepeatedScan(_positions, _healths, _directions);

    [Benchmark]
    public int[] StackSimulation() => SurvivorHealthsByStackSimulation(_positions, _healths, _directions);
}
