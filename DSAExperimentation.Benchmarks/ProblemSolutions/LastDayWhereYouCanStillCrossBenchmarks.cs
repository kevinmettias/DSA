using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LastDayWhereYouCanStillCross;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LastDayWhereYouCanStillCrossSolution's, the same
// methods LastDayWhereYouCanStillCrossTests proves correct, each handed the prepared
// FloodSchedule its hoisted overload takes so turning the flood order into a grid is
// charged to [GlobalSetup] rather than to the bisection being measured. Flood order
// is a seeded random permutation of every cell, matching the problem's own guarantee
// that each cell floods on exactly one distinct day.
[MemoryDiagnoser]
public class LastDayWhereYouCanStillCrossBenchmarks
{
    private const int RandomSeed = 1970; // LC problem number
    private const int OneBased = 1;

    [Params(20, 50)]
    public int Size;

    private FloodSchedule _flooding = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var cells = new List<(int Row, int Col)>();

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                cells.Add((row, col));
            }
        }

        var order = cells.OrderBy(_ => random.Next())
            .Select(cell => new[] { cell.Row + OneBased, cell.Col + OneBased })
            .ToArray();

        _flooding = FloodSchedule.Build(Size, Size, order);
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch() =>
        LastDayWhereYouCanStillCrossSolution.LatestDayToCrossByManualBisection(_flooding);

    [Benchmark]
    public int SequenceLowerBound() =>
        LastDayWhereYouCanStillCrossSolution.LatestDayToCrossBySequenceLowerBound(_flooding);
}
