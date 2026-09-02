using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Spiral Matrix IV (LC 2326): both strategies walk this repo's own
// SinglyLinkedListNode<int> exactly once (AddTwoNumbersBenchmarks' precedent for
// the primitive) to fill an n x n matrix; what differs is how the spiral path
// itself is computed. DirectionArrayWithVisitedTracking is the common
// direction-vector approach (turn whenever the next cell is out of bounds or
// already visited) - it needs a second n x n bool matrix purely to answer "have I
// been here," and a bounds/visited check on every single cell. Boundary
// ShrinkingSpiralWalk instead shrinks four cursors (top/bottom/left/right) after
// each side is walked, so a turn is decided once per side instead of once per
// cell, with no extra allocation beyond the output matrix itself.
[MemoryDiagnoser]
public class SpiralMatrixIVBenchmarks
{
    private const int RandomSeed = 2326;

    [Params(50, 200)]
    public int Size;

    private SinglyLinkedListNode<int> _head = null!;

    [GlobalSetup]
    public void Setup() => _head = BuildRandomList(Size * Size);

    [Benchmark(Baseline = true)]
    public int DirectionArrayWithVisitedTracking()
    {
        var m = Size;
        var n = Size;
        var matrix = new int[m][];
        for (var row = 0; row < m; row++)
        {
            matrix[row] = new int[n];
            Array.Fill(matrix[row], -1);
        }

        var visited = new bool[m, n];
        (int DRow, int DCol)[] directions = [(0, 1), (1, 0), (0, -1), (-1, 0)];
        int row2 = 0, col = 0, direction = 0;

        for (var node = _head; node is not null; node = node.Next)
        {
            matrix[row2][col] = node.Value;
            visited[row2, col] = true;

            var (dRow, dCol) = directions[direction];
            var nextRow = row2 + dRow;
            var nextCol = col + dCol;

            if (nextRow < 0 || nextRow >= m || nextCol < 0 || nextCol >= n || visited[nextRow, nextCol])
            {
                direction = (direction + 1) % directions.Length;
                (dRow, dCol) = directions[direction];
                nextRow = row2 + dRow;
                nextCol = col + dCol;
            }

            row2 = nextRow;
            col = nextCol;
        }

        return matrix[0][0];
    }

    [Benchmark]
    public int BoundaryShrinkingSpiralWalk()
    {
        var m = Size;
        var n = Size;
        var matrix = new int[m][];
        for (var row = 0; row < m; row++)
        {
            matrix[row] = new int[n];
            Array.Fill(matrix[row], -1);
        }

        var node = _head;
        int top = 0, bottom = m - 1, left = 0, right = n - 1;

        while (top <= bottom && left <= right && node is not null)
        {
            for (var col = left; col <= right && node is not null; col++)
            {
                matrix[top][col] = node.Value;
                node = node.Next;
            }

            top++;

            for (var row = top; row <= bottom && node is not null; row++)
            {
                matrix[row][right] = node.Value;
                node = node.Next;
            }

            right--;

            if (top <= bottom)
            {
                for (var col = right; col >= left && node is not null; col--)
                {
                    matrix[bottom][col] = node.Value;
                    node = node.Next;
                }

                bottom--;
            }

            if (left <= right)
            {
                for (var row = bottom; row >= top && node is not null; row--)
                {
                    matrix[row][left] = node.Value;
                    node = node.Next;
                }

                left++;
            }
        }

        return matrix[0][0];
    }

    private static SinglyLinkedListNode<int> BuildRandomList(int length)
    {
        var random = new Random(RandomSeed);
        var head = new SinglyLinkedListNode<int>(random.Next(0, 1_000));
        var tail = head;

        for (var i = 1; i < length; i++)
        {
            tail.Next = new SinglyLinkedListNode<int>(random.Next(0, 1_000));
            tail = tail.Next;
        }

        return head;
    }
}
