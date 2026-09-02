using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.LeetCode.MaximizeTheDistanceBetweenPointsOnASquare;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeTheDistanceBetweenPointsOnASquareSolution's,
// the same methods MaximizeTheDistanceBetweenPointsOnASquareTests proves correct.
// Point-to-offset mapping and sorting is charged to [GlobalSetup] via the
// solution's own ToSortedPerimeterPositions, so only the binary search itself is
// measured.
[MemoryDiagnoser]
public class MaximizeTheDistanceBetweenPointsOnASquareBenchmarks
{
    private const int Side = 1_000_000;
    private const int K = 10;
    private const int RandomSeed = 3464;

    [Params(50, 500)]
    public int PointCount;

    private ArraySequence<long> _positions;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var offsets = new HashSet<long>();

        while (offsets.Count < PointCount)
        {
            offsets.Add(random.NextInt64(4L * Side));
        }

        var points = offsets.Select(ToPoint).ToArray();
        _positions = MaximizeTheDistanceBetweenPointsOnASquareSolution.ToSortedPerimeterPositions(Side, points);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan() =>
        MaximizeTheDistanceBetweenPointsOnASquareSolution.MaxDistanceByLinearScan(Side, _positions, K);

    [Benchmark]
    public int SortedGreedy() =>
        MaximizeTheDistanceBetweenPointsOnASquareSolution.MaxDistanceBySortedGreedy(Side, _positions, K);

    // Inverse of the solution's own ToPerimeterPosition mapping, used only to turn
    // a random offset into a boundary point for the workload - the same mapping,
    // read backwards, so distinct offsets always land on distinct points.
    private static int[] ToPoint(long offset)
    {
        if (offset <= Side)
        {
            return [0, (int)offset];
        }

        if (offset <= 2L * Side)
        {
            return [(int)(offset - Side), Side];
        }

        if (offset <= 3L * Side)
        {
            return [Side, (int)((3L * Side) - offset)];
        }

        return [(int)((4L * Side) - offset), 0];
    }
}
