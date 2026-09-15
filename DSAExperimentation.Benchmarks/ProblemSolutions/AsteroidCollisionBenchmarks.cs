using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AsteroidCollision;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AsteroidCollisionSolution's. Asteroid Collision
// (LC 735) - a naive repeated-scan baseline that restarts from the beginning
// of a plain List<int> after every single collision it resolves (a fresh
// O(n) scan per collision, so O(n^2) overall once cascades happen) vs. this
// repo's own Stack<int> doing the textbook single left-to-right pass,
// resolving every collision, including cascades, in one O(n) sweep.
[MemoryDiagnoser]
public class AsteroidCollisionBenchmarks
{
    // LC problem number, used as the deterministic seed for asteroid generation.
    private const int RandomSeed = 735;

    // Exclusive upper bound for an asteroid's magnitude.
    private const int MaxMagnitude = 1_000;

    // Coin-flip range: half the asteroids move left (negative), half move right.
    private const int SignCoinFlipRange = 2;

    private int[] _asteroids = [];

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _asteroids = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var magnitude = random.Next(1, MaxMagnitude);
                var movesRight = random.Next(SignCoinFlipRange) == 0;
                return movesRight ? magnitude : -magnitude;
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] RepeatedScan() => AsteroidCollisionSolution.SimulateByRepeatedScan(_asteroids);

    [Benchmark]
    public int[] StackPass() => AsteroidCollisionSolution.SimulateByStackPass(_asteroids);
}
