using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BuildAMatrixWithConditions;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BuildAMatrixWithConditionsSolution's, the same
// methods BuildAMatrixWithConditionsTests proves correct. Each arm is handed the
// prepared row and column value lists its hoisted overload takes, so building the
// two condition graphs is charged to [GlobalSetup] rather than to the sorts being
// measured - the same shape CourseScheduleIIBenchmarks uses. Conditions form a
// guaranteed-acyclic DAG (every edge points from a lower value to a higher one,
// capped fan-out) so both strategies run their full real workload, two orderings
// and a full k x k placement, instead of an early cycle bailout.
[MemoryDiagnoser]
public class BuildAMatrixWithConditionsBenchmarks
{
    private const int MaxFanOut = 3;

    private List<ValueNode> _rowValues = new();

    private List<ValueNode> _colValues = new();
    [Params(50, 1_000)]
    public int K { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _rowValues = BuildChainValues(K);
        _colValues = BuildChainValues(K);
    }

    private static List<ValueNode> BuildChainValues(int k)
    {
        var values = Enumerable.Range(1, k).Select(id => new ValueNode(id)).ToList();

        for (var i = 0; i < k; i++)
        {
            var fanOut = Math.Min(MaxFanOut, k - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                values[i].After.Add(values[i + f]);
            }
        }

        return values;
    }

    // Both arms return the built matrix's row count - k when the conditions are
    // satisfiable, as this workload's are - purely so the full placement is
    // consumed rather than elided.
    [Benchmark(Baseline = true)]
    public int NaiveRescanBothOrders() =>
        BuildAMatrixWithConditionsSolution.BuildMatrixByNaiveRescan(_rowValues, _colValues).Length;

    [Benchmark]
    public int KahnsTopologicalSortBothOrders() =>
        BuildAMatrixWithConditionsSolution.BuildMatrixByKahnsTopologicalSort(_rowValues, _colValues).Length;
}
