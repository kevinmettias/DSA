using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Strange Printer II (LC 1591): once a grid's per-color bounding rectangles are
// reduced to a "must print before" dependency graph, deciding printability is
// exactly "does this graph have a valid order" - the same question
// CourseScheduleIIBenchmarks.cs already benchmarks for LC 210. NaiveRescan
// re-scans every remaining color for one with zero remaining prerequisite colors
// on each step, O(V^2 + V*E); KahnsTopologicalSort is this repo's own
// TopologicalSort.TrySort, O(V+E) via a queue of already-zero-in-degree colors.
// Colors form a guaranteed-acyclic chain (every "must print before" edge points
// from a lower color id to a higher one, capped fan-out) so both strategies run
// their full real workload instead of an early cycle bailout.
[MemoryDiagnoser]
public class StrangePrinterIIBenchmarks
{
    [Params(50, 1_000)]
    public int ColorCount;

    private List<ColorNode> _colors = null!;

    [GlobalSetup]
    public void Setup()
    {
        _colors = Enumerable.Range(0, ColorCount).Select(color => new ColorNode(color)).ToList();

        for (var i = 0; i < ColorCount; i++)
        {
            var fanOut = Math.Min(3, ColorCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                _colors[i].MustPrintBefore.Add(_colors[i + f]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRescan()
    {
        var inDegree = _colors.ToDictionary(color => color, _ => 0);
        foreach (var color in _colors)
        {
            foreach (var dependent in color.MustPrintBefore)
            {
                inDegree[dependent]++;
            }
        }

        var remaining = new List<ColorNode>(_colors);
        var order = new List<ColorNode>(_colors.Count);

        while (remaining.Count > 0)
        {
            var next = remaining.FirstOrDefault(color => inDegree[color] == 0);
            if (next is null)
            {
                break;
            }

            order.Add(next);
            remaining.Remove(next);
            foreach (var dependent in next.MustPrintBefore)
            {
                inDegree[dependent]--;
            }
        }

        return order.Count;
    }

    [Benchmark]
    public int KahnsTopologicalSort()
    {
        TopologicalSort.TrySort<
            ColorNode, ColorTopology, ListChildren<ColorNode>,
            NaturalChildOrder<ColorNode, ListChildren<ColorNode>>, ListChildren<ColorNode>>(
            _colors, out var ordering);

        return ordering.Count;
    }

    // See StrangePrinterIITests.Fixtures for the full explanation - repeated here
    // rather than shared because TwoSumBenchmarks/MedianOfTwoSortedArraysBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
    private sealed class ColorNode(int color)
    {
        public int Color { get; } = color;

        public List<ColorNode> MustPrintBefore { get; } = [];
    }

    private readonly struct ColorTopology : IGraphTopology<ColorNode, ListChildren<ColorNode>>
    {
        public static ListChildren<ColorNode> GetChildren(ColorNode node) => new(node.MustPrintBefore);
    }
}
