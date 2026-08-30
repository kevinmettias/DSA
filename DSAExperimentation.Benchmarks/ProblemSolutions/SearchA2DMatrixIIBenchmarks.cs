using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Search a 2D Matrix II (LC 240): three genuinely distinct algorithms answer
// the same "is target present" question over the same row-and-column-sorted
// matrix, so all three are benchmarked (ShortestPathAlgorithmBenchmarks'
// precedent for a real algorithm swap, not an apples-to-oranges comparison).
// FullScan is the textbook O(rows*cols) baseline. PerRowBinarySearch composes
// this repo's own BinarySearch.Find over an ArraySequence<int> witness per row
// - O(rows*log(cols)), a genuine repo-primitive fit. StaircaseSearch is the
// specialized O(rows+cols) corner-walk this problem is famous for - it is
// expected to win, and the point of including it is exactly that: showing
// when the general BinarySearch primitive is not the asymptotically optimal
// tool, the same lesson FloydWarshall teaches in the shortest-path cluster.
// Target is fixed below every matrix value so all three are forced through
// their full worst-case walk instead of an early exit making one look
// artificially competitive.
[MemoryDiagnoser]
public class SearchA2DMatrixIIBenchmarks
{
    private const int Target = -1;

    [Params(50, 300)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
        => _matrix = Enumerable.Range(0, Size)
            .Select(row => Enumerable.Range(0, Size).Select(col => (row * Size) + col).ToArray())
            .ToArray();

    [Benchmark(Baseline = true)]
    public bool FullScan()
    {
        foreach (var row in _matrix)
        {
            foreach (var value in row)
            {
                if (value == Target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Benchmark]
    public bool PerRowBinarySearch()
    {
        foreach (var row in _matrix)
        {
            var sequence = new ArraySequence<int>(row);
            if (BinarySearch.Find(sequence, Target) is not null)
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool StaircaseSearch()
    {
        var row = 0;
        var col = _matrix[0].Length - 1;

        while (row < _matrix.Length && col >= 0)
        {
            var current = _matrix[row][col];
            if (current == Target)
            {
                return true;
            }

            if (current > Target)
            {
                col--;
            }
            else
            {
                row++;
            }
        }

        return false;
    }
}
