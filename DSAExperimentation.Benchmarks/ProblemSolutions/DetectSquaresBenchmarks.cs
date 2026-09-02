using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Detect Squares (LC 2013): a dense lattice of points is the case where "which
// points share the query's x-coordinate" actually matters - the naive approach
// rescans every stored point for every candidate corner (List.Count(predicate),
// O(n) per corner check) while this repo's HashMap<int,HashMap<int,int>>
// grouped-by-x map (the same nested-HashMap composition EvaluateDivisionTests
// already uses) only ever looks at points that could possibly share the query's
// x-coordinate, then checks the other two corners with O(1) lookups.
[MemoryDiagnoser]
public class DetectSquaresBenchmarks
{
    [Params(5, 10)]
    public int GridDimension;

    private (int X, int Y)[] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        _points = (
            from x in Enumerable.Range(0, GridDimension)
            from y in Enumerable.Range(0, GridDimension)
            select (x, y)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ListBased()
    {
        var stored = new List<(int X, int Y)>(_points);
        long total = 0;

        foreach (var query in _points)
        {
            total += CountSquaresInList(stored, query.X, query.Y);
        }

        return total;
    }

    [Benchmark]
    public long HashMapGroupedByX()
    {
        var countsByX = new HashMap<int, HashMap<int, int>>();

        foreach (var point in _points)
        {
            Add(countsByX, point.X, point.Y);
        }

        long total = 0;
        foreach (var query in _points)
        {
            total += CountSquaresInMap(countsByX, query.X, query.Y);
        }

        return total;
    }

    private static long CountSquaresInList(List<(int X, int Y)> stored, int x, int y)
    {
        long total = 0;

        foreach (var p1 in stored)
        {
            if (p1.X != x || p1.Y == y)
            {
                continue;
            }

            var side = Math.Abs(p1.Y - y);

            total += CountCornerInList(stored, x + side, y, p1.Y);
            total += CountCornerInList(stored, x - side, y, p1.Y);
        }

        return total;
    }

    private static long CountCornerInList(List<(int X, int Y)> stored, int otherX, int y, int otherY)
    {
        var cornerA = stored.Count(p => p.X == otherX && p.Y == y);
        var cornerB = stored.Count(p => p.X == otherX && p.Y == otherY);
        return (long)cornerA * cornerB;
    }

    private static void Add(HashMap<int, HashMap<int, int>> countsByX, int x, int y)
    {
        if (!countsByX.TryGetValue(x, out var byY))
        {
            byY = new HashMap<int, int>();
            countsByX.Set(x, byY);
        }

        byY.TryGetValue(y, out var existing);
        byY.Set(y, existing + 1);
    }

    private static long CountSquaresInMap(HashMap<int, HashMap<int, int>> countsByX, int x, int y)
    {
        if (!countsByX.TryGetValue(x, out var sameX))
        {
            return 0;
        }

        long total = 0;

        foreach (var y2 in sameX.Keys)
        {
            if (y2 == y)
            {
                continue;
            }

            total += CountCornersForY2(countsByX, (x, y), y2, sameX);
        }

        return total;
    }

    private static long CountCornersForY2(
        HashMap<int, HashMap<int, int>> countsByX, (int X, int Y) query, int y2, HashMap<int, int> sameX)
    {
        sameX.TryGetValue(y2, out var countY2);
        var side = Math.Abs(y2 - query.Y);
        var cornerQuery = new CornerQuery(query.Y, y2, countY2);

        return CountCornerInMap(countsByX, query.X + side, cornerQuery) + CountCornerInMap(countsByX, query.X - side, cornerQuery);
    }

    private static long CountCornerInMap(HashMap<int, HashMap<int, int>> countsByX, int otherX, CornerQuery query)
    {
        if (!countsByX.TryGetValue(otherX, out var byY))
        {
            return 0;
        }

        byY.TryGetValue(query.Y, out var countXY);
        byY.TryGetValue(query.Y2, out var countXY2);

        return (long)countXY * countXY2 * query.CountY2;
    }

    private readonly record struct CornerQuery(int Y, int Y2, int CountY2);
}
