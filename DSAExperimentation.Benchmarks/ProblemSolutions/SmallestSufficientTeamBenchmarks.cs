using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SmallestSufficientTeam;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestSufficientTeamSolution's, the same methods
// SmallestSufficientTeamTests proves correct, each handed the SkillMasks its hoisted
// overload takes so mask construction is charged to [GlobalSetup] rather than to the
// search being measured.
//
// Each of PeoplePerSkill people covers exactly one, dedicated skill, so at every
// level of the recursion all PeoplePerSkill branches land on the SAME child mask -
// the worst case for an unmemoized walk (true O(PeoplePerSkill^SkillCount) recursive
// calls across only SkillCount+1 actually-distinct states) and the best case for
// memoization (each of those states computed exactly once).
[MemoryDiagnoser]
public class SmallestSufficientTeamBenchmarks
{
    private const int PeoplePerSkill = 3;

    [Params(6, 10)]
    public int SkillCount;

    private SkillMasks _masks = null!;

    [GlobalSetup]
    public void Setup()
    {
        var people = new List<int>();
        for (var skill = 0; skill < SkillCount; skill++)
        {
            for (var copy = 0; copy < PeoplePerSkill; copy++)
            {
                people.Add(1 << skill);
            }
        }

        _masks = new SkillMasks([.. people], (1 << SkillCount) - 1);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        SmallestSufficientTeamSolution.SmallestTeamByBruteForceRecursion(_masks).Length;

    [Benchmark]
    public int MemoizedRecursion() =>
        SmallestSufficientTeamSolution.SmallestTeamByMemoizedBitmask(_masks).Length;
}
