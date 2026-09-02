using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Detonate the Maximum Bombs (LC 2101): both benchmarks run the identical O(n) DFS
// per candidate bomb over the identical directed "i's blast reaches j's center"
// edges - no smarter algorithm applies here (LeetCode's own constraints assume the
// try-every-source O(n^3) shape, the same one MinimizeMalwareSpreadBenchmarks'
// PerCandidateFloodFill baseline reruns once per infected candidate). ManualRecursive
// Visit hand-rolls the walk with a bool[] visited array and a private recursive
// method; RepoDepthFirstSearch instead hands the identical per-node successor
// closure to this repo's own DepthFirstSearch.Traverse (the same
// Func&lt;TNode,IEnumerable&lt;TNode&gt;&gt;-closed walk JumpGameIIITests already proves).
// The comparison is therefore about each visited-set's container overhead (array
// indexing vs. HashSet hashing, plain recursion vs. an explicit repo Stack), not an
// asymptotic difference.
[MemoryDiagnoser]
public class DetonateTheMaximumBombsBenchmarks
{
    private const int GridSize = 1_000;
    private const int MaxRadius = 60;
    private const int RandomSeed = 2101;
    private const int RadiusIndex = 2; // index of the radius value within a _bombs[i] triple

    [Params(50, 300)]
    public int BombCount;

    private int[][] _bombs = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _bombs = new int[BombCount][];

        for (var i = 0; i < BombCount; i++)
        {
            _bombs[i] = [random.Next(0, GridSize), random.Next(0, GridSize), random.Next(1, MaxRadius)];
        }
    }

    [Benchmark(Baseline = true)]
    public int ManualRecursiveVisit()
    {
        var maxDetonated = 0;

        for (var i = 0; i < _bombs.Length; i++)
        {
            var visited = new bool[_bombs.Length];
            VisitManual(i, visited);
            maxDetonated = Math.Max(maxDetonated, CountVisited(visited));
        }

        return maxDetonated;
    }

    private void VisitManual(int index, bool[] visited)
    {
        if (visited[index])
        {
            return;
        }

        visited[index] = true;

        var x = _bombs[index][0];
        var y = _bombs[index][1];
        var radius = _bombs[index][RadiusIndex];

        for (var j = 0; j < _bombs.Length; j++)
        {
            if (!visited[j] && IsWithinBlastRadius(x, y, radius, j))
            {
                VisitManual(j, visited);
            }
        }
    }

    private static int CountVisited(bool[] visited)
    {
        var count = 0;

        foreach (var wasVisited in visited)
        {
            if (wasVisited)
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int RepoDepthFirstSearch()
    {
        var maxDetonated = 0;

        for (var i = 0; i < _bombs.Length; i++)
        {
            var reached = DepthFirstSearch.Traverse(i, Reachable);
            maxDetonated = Math.Max(maxDetonated, reached.Count);
        }

        return maxDetonated;
    }

    private IEnumerable<int> Reachable(int index)
    {
        var x = _bombs[index][0];
        var y = _bombs[index][1];
        var radius = _bombs[index][RadiusIndex];

        for (var j = 0; j < _bombs.Length; j++)
        {
            if (j != index && IsWithinBlastRadius(x, y, radius, j))
            {
                yield return j;
            }
        }
    }

    private bool IsWithinBlastRadius(int x, int y, int radius, int targetIndex)
    {
        var dx = (long)(_bombs[targetIndex][0] - x);
        var dy = (long)(_bombs[targetIndex][1] - y);
        return dx * dx + dy * dy <= (long)radius * radius;
    }
}
