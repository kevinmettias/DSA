using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LoudAndRich;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are LoudAndRichSolution's, the same methods
// LoudAndRichTests proves correct. Each arm is handed the prepared
// List<PersonNode> its hoisted overload takes, so graph construction is charged
// to [GlobalSetup] rather than to the pass being measured. Richer edges point
// from a lower id to a higher one (capped fan-out) so the relation is a
// guaranteed-acyclic DAG, the same generation shape CourseScheduleIIBenchmarks
// already uses.
[MemoryDiagnoser]
public class LoudAndRichBenchmarks
{
    private const int RandomSeed = 5;
    private const int MaxFanOut = 3;

    [Params(50, 1_000)]
    public int PersonCount;

    private int[] _quiet = null!;
    private List<PersonNode> _people = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _quiet = Enumerable.Range(0, PersonCount).Select(_ => random.Next(0, PersonCount)).ToArray();
        _people = Enumerable.Range(0, PersonCount).Select(id => new PersonNode(id)).ToList();

        for (var i = 0; i < PersonCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, PersonCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                // person i is richer than person i + f.
                _people[i].Poorer.Add(_people[i + f]);
                _people[i + f].Richer.Add(_people[i]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int[] NaivePerPersonWalk() => LoudAndRichSolution.QuietestByPerPersonWalk(_people, _quiet);

    [Benchmark]
    public int[] TopologicalDpPass() => LoudAndRichSolution.QuietestByTopologicalDpPass(_people, _quiet);
}
