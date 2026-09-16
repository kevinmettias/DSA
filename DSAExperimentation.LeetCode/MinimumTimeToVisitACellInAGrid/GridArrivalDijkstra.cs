using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

// The grid relaxation both LC 2577 and LC 3341 reduce to: settle cells in
// nondecreasing arrival order, report the last cell's settled time, and report -1 if
// the frontier runs dry before it is reached. Every move costs exactly one second,
// so an edge has no weight of its own - what the step costs depends on the second you
// are stepping from, which is one degree more dynamic than ShortestPath.Dijkstra's
// fixed-weight IEdgeTopology can express and why this composes
// Graph.ShortestPaths' ByPriorityOrder directly instead.
//
// The relaxation is written once, in GridArrivalSearch below, and parameterized by the two
// things the two problems disagree about - each named as a contract rather than left as a
// bare callable: the arrival rule (IArrivalRule - LC 3341 just waits where it stands, LC
// 2577 has to land on the cell's own parity after bouncing with a predecessor) and the
// frontier (IArrivalFrontier - the BCL's own PriorityQueue for a baseline arm, this repo's
// own Heap for the composed one). Both arms below are therefore the identical loop with a
// different frontier plugged in, which is exactly the swap the benchmark pair measures.
internal static class GridArrivalDijkstra
{
    // Baseline frontier: the BCL's own PriorityQueue<TElement,TPriority> instead of
    // this repo's Heap - "what you'd write without this repo" (ARCHITECTURE.md 17.5).
    public static int ByBclQueue(int[][] requiredTimes, IArrivalRule arrivalRule)
    {
        var search = new GridArrivalSearch(requiredTimes, arrivalRule, new BclPriorityQueueFrontier());
        return search.MinimumArrival();
    }

    // Composed frontier: this repo's own Heap<Element,TOrder> ordered by
    // ByPriorityOrder<TNode,TWeight> - the same frontier ShortestPath.Dijkstra/AStar
    // and every other grid-Dijkstra coverage test in this repo already use.
    public static int ByHeap(int[][] requiredTimes, IArrivalRule arrivalRule)
    {
        var search = new GridArrivalSearch(requiredTimes, arrivalRule, new PriorityHeapFrontier());
        return search.MinimumArrival();
    }

    // The BCL frontier: PriorityQueue<TElement,TPriority> takes the element and its priority
    // together on the way in but hands them back through two out parameters, so the one entry
    // shape IArrivalFrontier names is put back together on the way out.
    private sealed class BclPriorityQueueFrontier : IArrivalFrontier
    {
        private readonly PriorityQueue<(int Row, int Col), int> _queue = new();

        public void Insert((int Row, int Col) node, int priority) => _queue.Enqueue(node, priority);

        public bool TryTake(out ((int Row, int Col) Node, int Priority) entry)
        {
            var taken = _queue.TryDequeue(out var node, out var priority);
            entry = (node, priority);
            return taken;
        }
    }

    // This repo's frontier: Heap<Element,TOrder> already yields the element and its priority
    // bound together, which is the entry shape IArrivalFrontier names.
    private sealed class PriorityHeapFrontier : IArrivalFrontier
    {
        private readonly Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> _heap = new();

        public void Insert((int Row, int Col) node, int priority) => _heap.Push((node, priority));

        public bool TryTake(out ((int Row, int Col) Node, int Priority) entry) => _heap.TryPop(out entry);
    }

    // One run of the relaxation, holding the state it threads through - the grid and the rule
    // it was handed, the frontier it settles from, and the best arrival known per cell. Held
    // as fields rather than passed on, so that relaxing one entry takes a single argument
    // instead of a five-argument signature, and so that both arms run one loop.
    private sealed class GridArrivalSearch(
        int[][] requiredTimes, IArrivalRule arrivalRule, IArrivalFrontier frontier)
    {
        private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

        private readonly Dictionary<(int Row, int Col), int> _distances = new() { [(0, 0)] = 0 };
        private readonly HashSet<(int Row, int Col)> _settled = [];
        private readonly (int Row, int Col) _destination = (requiredTimes.Length - 1, requiredTimes[0].Length - 1);

        public int MinimumArrival()
        {
            frontier.Insert((0, 0), 0);

            while (frontier.TryTake(out var entry))
            {
                if (!_settled.Add(entry.Node))
                {
                    continue;
                }

                if (entry.Node == _destination)
                {
                    return entry.Priority;
                }

                Relax(entry);
            }

            return LeetCodeAnswer.None;
        }

        // Relaxes every neighbor of the cell just settled: an arrival earlier than the one
        // already recorded replaces it and re-enters the frontier. The copy of a cell that an
        // earlier, worse arrival left pending is harmless - the settle test above discards it.
        private void Relax(((int Row, int Col) Node, int Priority) entry)
        {
            foreach (var neighbor in Neighbors(entry.Node))
            {
                var arrival = arrivalRule.Arrive(
                    entry.Priority, requiredTimes[neighbor.Row][neighbor.Col]);

                if (!_distances.TryGetValue(neighbor, out var known) || arrival < known)
                {
                    _distances[neighbor] = arrival;
                    frontier.Insert(neighbor, arrival);
                }
            }
        }

        // Every orthogonal neighbor still on the grid, in the order the four directions are
        // listed above.
        private IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) node)
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var next = (Row: node.Row + dRow, Col: node.Col + dCol);

                if (IsInsideGrid(next.Row, next.Col))
                {
                    yield return next;
                }
            }
        }

        // Both coordinates within the grid is one idea, tested as a pair on all four edges.
        private bool IsInsideGrid(int row, int col) =>
            row >= 0 && row <= _destination.Row && col >= 0 && col <= _destination.Col;
    }
}
