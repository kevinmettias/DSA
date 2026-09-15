using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CourseScheduleIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CourseScheduleIVSolution's, the same methods
// CourseScheduleIVTests proves correct. Each is handed the prepared CourseGraph
// its hoisted overload takes, so building the prerequisite network is charged to
// [GlobalSetup] rather than to the reachability work being measured - leaving the
// comparison where it belongs: a fresh BFS per query against one Floyd-Warshall
// call plus a dictionary lookup per query.
//
// Prerequisite edges only ever point from a lower to a higher course id, which
// keeps the generated graph acyclic (a real prerequisite DAG) without needing a
// separate cycle check.
[MemoryDiagnoser]
public class CourseScheduleIVBenchmarks
{
    private const int QueryCount = 300;

    // LC problem number, used as the deterministic Random seed.
    private const int RandomSeed = 1462;

    private const int EdgesPerCourse = 2;

    private CourseGraph _graph = null!;

    private int[][] _queries = [];
    [Params(50, 150)]
    public int CourseCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _graph = CourseGraph.Build(CourseCount, BuildPrerequisites(random));
        _queries = BuildQueries(random);
    }

    private int[][] BuildPrerequisites(Random random)
    {
        var prerequisites = new List<int[]>();

        for (var course = 0; course < CourseCount - 1; course++)
        {
            for (var edge = 0; edge < EdgesPerCourse; edge++)
            {
                var dependent = course + 1 + random.Next(CourseCount - course - 1);
                prerequisites.Add([course, dependent]);
            }
        }

        return [.. prerequisites];
    }

    private int[][] BuildQueries(Random random)
    {
        var queries = new int[QueryCount][];

        for (var query = 0; query < QueryCount; query++)
        {
            queries[query] = [random.Next(CourseCount), random.Next(CourseCount)];
        }

        return queries;
    }

    [Benchmark(Baseline = true)]
    public List<bool> BfsPerQuery() =>
        CourseScheduleIVSolution.CheckIfPrerequisiteByBreadthFirstSearchPerQuery(_graph, _queries);

    [Benchmark]
    public List<bool> FloydWarshallAllPairs() =>
        CourseScheduleIVSolution.CheckIfPrerequisiteByFloydWarshall(_graph, _queries);
}
