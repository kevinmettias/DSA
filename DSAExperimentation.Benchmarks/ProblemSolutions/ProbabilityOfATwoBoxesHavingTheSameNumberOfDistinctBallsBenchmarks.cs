using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBalls;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsSolution's, the same
// methods its test proves correct - a hand-rolled recursive enumeration against
// Backtrack.Search driving the identical choose/unchoose walk. Both visit the
// exact same search tree and do the exact same binomial arithmetic per node, so
// the comparison isolates Backtrack's own call/delegate overhead rather than a
// difference in what is computed. Only the ball counts are decided here, which is
// a workload-sizing decision (ARCHITECTURE.md 17.7).
[MemoryDiagnoser]
public class ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsBenchmarks
{
    // LC problem number, used as the deterministic seed for ball-count generation.
    private const int RandomSeed = 1467;

    // Random.Next(1, BallCountUpperBound) yields each colour's ball count in [1, BallCountUpperBound - 1].
    private const int BallCountUpperBound = 7;

    // The balls are dealt into exactly two boxes, so the total has to be even.
    private const int BoxCount = 2;

    [Params(4, 6)]
    public int TypeCount;

    private int[] _balls = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _balls = [.. Enumerable.Range(0, TypeCount).Select(_ => random.Next(1, BallCountUpperBound))];

        if (_balls.Sum() % BoxCount != 0)
        {
            _balls[^1]++;
        }
    }

    [Benchmark(Baseline = true)]
    public double HandRolledRecursion() =>
        ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsSolution
            .GetProbabilityByHandRolledRecursion(_balls);

    [Benchmark]
    public double BacktrackPrimitive() =>
        ProbabilityOfATwoBoxesHavingTheSameNumberOfDistinctBallsSolution
            .GetProbabilityByBacktracking(_balls);
}
