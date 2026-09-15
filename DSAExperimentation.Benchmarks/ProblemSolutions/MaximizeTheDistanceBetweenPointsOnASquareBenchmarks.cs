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

    private ArraySequence<long> _positions;

    [Params(50, 500)]
    public int PointCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var offsets = BuildOffsets();

        _positions = ToSortedPositions(offsets);
    }

    // PointCount distinct offsets along the square's 4 * Side perimeter walk, drawn
    // from a fixed seed so every run benchmarks the same boundary points.
    private HashSet<long> BuildOffsets()
    {
        var random = new Random(RandomSeed);
        var offsets = new HashSet<long>();

        while (offsets.Count < PointCount)
        {
            offsets.Add(random.NextInt64(4L * Side));
        }

        return offsets;
    }

    private static ArraySequence<long> ToSortedPositions(HashSet<long> offsets) =>
        MaximizeTheDistanceBetweenPointsOnASquareSolution.ToSortedPerimeterPositions(
            Side,
            offsets.Select(ToPoint).ToArray());

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
