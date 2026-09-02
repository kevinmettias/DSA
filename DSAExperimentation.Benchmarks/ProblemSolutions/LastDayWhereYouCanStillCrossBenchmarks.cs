using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Last Day Where You Can Still Cross (LC 1970): a hand-rolled int lo/hi bisection
// loop vs. this repo's own BinarySearch.LowerBound over an on-demand
// IsBlockedSequence (LastDayWhereYouCanStillCrossTests precedent, the same
// "binary search on the answer" shape SplitArrayLargestSumBenchmarks/
// KokoEatingBananasBenchmarks already use) - both binary search the same
// monotone "is day d blocked" predicate, each probe a single
// DepthFirstSearch.Traverse (NumberOfIslandsTests' grid-neighbors idiom) from a
// virtual node wired to every still-dry top-row cell. Flood order is a random
// permutation of every cell, matching the problem's own guarantee that each
// cell floods on exactly one distinct day.
[MemoryDiagnoser]
public class LastDayWhereYouCanStillCrossBenchmarks
{
    private const int RandomSeed = 1970; // LC problem number
    private const int MidpointDivisor = 2;

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Params(20, 50)]
    public int Size;

    private int[,] _floodDay = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var cells = new List<(int Row, int Col)>();

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                cells.Add((r, c));
            }
        }

        var order = cells.OrderBy(_ => random.Next()).ToArray();
        _floodDay = new int[Size, Size];

        for (var i = 0; i < order.Length; i++)
        {
            _floodDay[order[i].Row, order[i].Col] = i + 1;
        }
    }

    [Benchmark(Baseline = true)]
    public int ManualBinarySearch()
    {
        var low = 0;
        var high = Size * Size;

        while (low < high)
        {
            var mid = low + ((high - low) / MidpointDivisor);

            if (CanCross(Size, _floodDay, mid))
            {
                low = mid + 1;
            }
            else
            {
                high = mid;
            }
        }

        return low - 1;
    }

    [Benchmark]
    public int SequenceLowerBound()
    {
        var sequence = new IsBlockedSequence(Size, _floodDay);
        return BinarySearch.LowerBound(sequence, true) - 1;
    }

    // (-1, -1) is a virtual node above the grid, wired to every top-row cell still dry
    // on this day - one Traverse call instead of one per dry top-row column.
    private static bool CanCross(int size, int[,] floodDay, int day)
        => DepthFirstSearch.Traverse((Row: -1, Col: -1), cell => Neighbors(cell, size, floodDay, day))
            .Any(cell => cell.Row == size - 1);

    private static IEnumerable<(int Row, int Col)> Neighbors(
        (int Row, int Col) cell, int size, int[,] floodDay, int day)
    {
        if (cell.Row == -1)
        {
            for (var c = 0; c < size; c++)
            {
                if (floodDay[0, c] > day)
                {
                    yield return (0, c);
                }
            }

            yield break;
        }

        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (next.Row >= 0 && next.Row < size && next.Col >= 0 && next.Col < size && floodDay[next.Row, next.Col] > day)
            {
                yield return next;
            }
        }
    }

    private readonly struct IsBlockedSequence(int size, int[,] floodDay) : IRandomAccessSequence<bool>
    {
        public int Length => (size * size) + 1;

        public bool Get(int index) => !CanCross(size, floodDay, index);
    }
}
