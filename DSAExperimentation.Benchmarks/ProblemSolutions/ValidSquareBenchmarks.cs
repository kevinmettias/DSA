using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ValidSquare;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Valid Square (LC 593): checking a single quad is inherently O(1) work (6 pairwise
// distances), so both strategies below run across a large batch of quads to make
// the comparison measurable. Half the batch is real squares (random center/
// half-side, axis-aligned), half is four independently-random points, so neither
// strategy gets to shortcut on an all-true or all-false batch.
[MemoryDiagnoser]
public class ValidSquareBenchmarks
{
    private const int RandomSeed = 593; // LC problem number
    private const int AlternationModulus = 2;
    private const int CoordinateBound = 1_000;
    private const int MaxHalfSide = 500;

    [Params(5_000, 100_000)]
    public int BatchCount;

    private int[][][] _batches = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _batches = new int[BatchCount][][];

        for (var i = 0; i < BatchCount; i++)
        {
            _batches[i] = i % AlternationModulus == 0 ? RandomSquare(random) : RandomQuad(random);
        }
    }

    private static int[][] RandomSquare(Random random)
    {
        var cx = random.Next(-CoordinateBound, CoordinateBound);
        var cy = random.Next(-CoordinateBound, CoordinateBound);
        var half = random.Next(1, MaxHalfSide);

        return
        [
            [cx - half, cy - half],
            [cx - half, cy + half],
            [cx + half, cy + half],
            [cx + half, cy - half],
        ];
    }

    private static int[][] RandomQuad(Random random) =>
    [
        [random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound)],
        [random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound)],
        [random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound)],
        [random.Next(-CoordinateBound, CoordinateBound), random.Next(-CoordinateBound, CoordinateBound)],
    ];

    [Benchmark(Baseline = true)]
    public int MinMaxScan()
    {
        var validCount = 0;

        foreach (var points in _batches)
        {
            if (ValidSquareSolution.IsValidSquareByMinMaxScan(points))
            {
                validCount++;
            }
        }

        return validCount;
    }

    [Benchmark]
    public int MergeSort()
    {
        var validCount = 0;

        foreach (var points in _batches)
        {
            if (ValidSquareSolution.IsValidSquareByMergeSort(points))
            {
                validCount++;
            }
        }

        return validCount;
    }
}
