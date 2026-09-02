using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumMovesToCaptureTheQueen;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumMovesToCaptureTheQueenSolution's, the same
// methods MinimumMovesToCaptureTheQueenTests proves correct (TwoSumBenchmarks
// precedent). Every single query is O(1) on the fixed 8x8 board, so there is no
// LeetCode-shaped input to grow - instead [Params] sizes a BATCH of independent
// random queries per iteration (a workload-sizing decision, same role
// LockWorkloads.BuildDeadends(count, seed) plays for OpenTheLockBenchmarks), which
// is what actually gives BenchmarkDotNet something to scale.
[MemoryDiagnoser]
public class MinimumMovesToCaptureTheQueenBenchmarks
{
    private const int Seed = 3001;

    [Params(1_000, 100_000)]
    public int BatchSize;

    private (int A, int B, int C, int D, int E, int F)[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _queries = new (int, int, int, int, int, int)[BatchSize];

        for (var i = 0; i < BatchSize; i++)
        {
            _queries[i] = RandomDistinctSquares(random);
        }
    }

    [Benchmark(Baseline = true)]
    public int DestinationEnumeration()
    {
        var total = 0;

        foreach (var (a, b, c, d, e, f) in _queries)
        {
            total += MinimumMovesToCaptureTheQueenSolution.MinMovesByDestinationEnumeration(a, b, c, d, e, f);
        }

        return total;
    }

    [Benchmark]
    public int LineOfSight()
    {
        var total = 0;

        foreach (var (a, b, c, d, e, f) in _queries)
        {
            total += MinimumMovesToCaptureTheQueenSolution.MinMovesByLineOfSight(a, b, c, d, e, f);
        }

        return total;
    }

    private static (int, int, int, int, int, int) RandomDistinctSquares(Random random)
    {
        var squares = new HashSet<(int Row, int Col)>();

        while (squares.Count < 3)
        {
            squares.Add((random.Next(1, 9), random.Next(1, 9)));
        }

        var picked = squares.ToArray();
        return (picked[0].Row, picked[0].Col, picked[1].Row, picked[1].Col, picked[2].Row, picked[2].Col);
    }
}
