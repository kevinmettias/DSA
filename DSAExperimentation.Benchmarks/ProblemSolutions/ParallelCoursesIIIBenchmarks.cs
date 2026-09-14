using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.ParallelCoursesIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ParallelCoursesIIISolution's, the same methods
// ParallelCoursesIIITests proves correct. Each arm is handed the prepared
// List<CourseTimeNode> its hoisted overload takes, so graph construction is charged
// to [GlobalSetup] rather than to the DP being measured. Nodes form a
// guaranteed-acyclic DAG (every edge points from a lower id to a higher one, capped
// fan-out) so both strategies do their full real workload instead of an early cycle
// bailout.
[MemoryDiagnoser]
public class ParallelCoursesIIIBenchmarks
{
    private const int MaxDuration = 100;

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 2050;

    private const int MaxFanOut = 3;

    [Params(50, 1_000)]
    public int NodeCount;

    private List<CourseTimeNode> _nodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nodes = Enumerable.Range(0, NodeCount)
            .Select(id => new CourseTimeNode(id, random.Next(1, MaxDuration)))
            .ToList();

        for (var i = 0; i < NodeCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, NodeCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                _nodes[i].Successors.Add(_nodes[i + f]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int RepeatedRelaxation() => ParallelCoursesIIISolution.MinimumTimeByRepeatedRelaxation(_nodes);

    [Benchmark]
    public int KahnsTopologicalSortDp() => ParallelCoursesIIISolution.MinimumTimeByKahnsTopologicalSortDp(_nodes);
}
