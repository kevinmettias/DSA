using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumCostToMakeAtLeastOneValidPathInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumCostToMakeAtLeastOneValidPathInAGridSolution's,
// the same methods MinimumCostToMakeAtLeastOneValidPathInAGridTests proves correct.
// The baseline is the textbook O(V^2) Dijkstra (linear-scan the unsettled distance
// table for the current minimum every round, no priority queue); the other arm uses
// this repo's own Heap<Element,TOrder> ordered by ByPriorityOrder<TNode,TWeight> for
// an O(E log V) frontier. Grid cells hold a random arrow direction (1..4), built in
// [GlobalSetup] so construction is not charged to either measured method.
[MemoryDiagnoser]
public class MinimumCostToMakeAtLeastOneValidPathInAGridBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1368;

    private const int ArrowDirectionUpperBound = 5;

    [Params(15, 40)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = new int[Size][];

        for (var row = 0; row < Size; row++)
        {
            _grid[row] = new int[Size];

            for (var col = 0; col < Size; col++)
            {
                _grid[row][col] = random.Next(1, ArrowDirectionUpperBound);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveDijkstra() =>
        MinimumCostToMakeAtLeastOneValidPathInAGridSolution.MinCostByLinearScanDijkstra(_grid);

    [Benchmark]
    public int HeapDijkstra() =>
        MinimumCostToMakeAtLeastOneValidPathInAGridSolution.MinCostByHeapDijkstra(_grid);
}
