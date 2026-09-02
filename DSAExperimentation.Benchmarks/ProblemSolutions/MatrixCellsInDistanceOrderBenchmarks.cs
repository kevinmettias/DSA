using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Grids;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Matrix Cells in Distance Order (LC 1030): the textbook closed-form baseline -
// compute each cell's Manhattan distance directly, then BCL Array.Sort - vs. this
// repo's own Grid/GridTopology/GridChildren BFS distance map (Reduce.Graph with
// DistanceMapReduceAlgebra, the same combo GridShortestPath.cs uses) followed by
// this repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence,
// the same custom-comparer pairing QueueReconstructionByHeightBenchmarks already
// uses. Both are O(rows*cols*log(rows*cols)); this demonstrates the repo's own
// grid-traversal and sorting primitives compose correctly, not an asymptotic win.
[MemoryDiagnoser]
public class MatrixCellsInDistanceOrderBenchmarks
{
    private const int MidpointDivisor = 2;

    [Params(20, 80)]
    public int Size;

    private bool[,] _passable = null!;

    [GlobalSetup]
    public void Setup() => _passable = new bool[Size, Size];

    [IterationSetup]
    public void IterationSetup()
    {
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                _passable[r, c] = true;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[][] ManhattanFormulaThenArraySort()
    {
        var rCenter = Size / MidpointDivisor;
        var cCenter = Size / MidpointDivisor;
        var cells = new (int Row, int Col, int Distance)[Size * Size];
        var i = 0;

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                cells[i++] = (r, c, Math.Abs(r - rCenter) + Math.Abs(c - cCenter));
            }
        }

        Array.Sort(cells, (a, b) => a.Distance.CompareTo(b.Distance));

        return cells.Select(cell => new[] { cell.Row, cell.Col }).ToArray();
    }

    [Benchmark]
    public int[][] GridBfsThenMergeSort()
    {
        var grid = new Grid(_passable);
        var start = new GridNode(Size / MidpointDivisor, Size / MidpointDivisor, grid);

        var distances = Reduce.Graph<
            GridNode, GridTopology, GridChildren,
            NaturalChildOrder<GridNode, GridChildren>, GridChildren,
            BreadthFirstReduceOrder<GridNode>,
            DistanceMapReduceAlgebra<GridNode>, Dictionary<GridNode, int>>(start);

        var cells = distances.Select(kv => (kv.Key.Row, kv.Key.Col, Distance: kv.Value)).ToArray();

        MergeSort.Sort<(int Row, int Col, int Distance), ArrayIndexedSequence<(int Row, int Col, int Distance)>>(
            new ArrayIndexedSequence<(int Row, int Col, int Distance)>(cells),
            Comparer<(int Row, int Col, int Distance)>.Create((a, b) => a.Distance.CompareTo(b.Distance)));

        return cells.Select(cell => new[] { cell.Row, cell.Col }).ToArray();
    }
}
