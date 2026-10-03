using DSAExperimentation.LeetCode.SmallestSufficientTeam;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SmallestSufficientTeamSolution's, the same methods
// SmallestSufficientTeamSolutionTests proves correct, each handed the SkillMasks its hoisted
// overload takes so mask construction is charged to [GlobalSetup] rather than to the
// search being measured.
//
// Each of PeoplePerSkill people covers exactly one, dedicated skill, so at every
// level of the recursion all PeoplePerSkill branches land on the SAME child mask -
// the worst case for an unmemoized walk (true O(PeoplePerSkill^SkillCount) recursive
// calls across only SkillCount+1 actually-distinct states) and the best case for
// memoization (each of those states computed exactly once).
//
// Sizes are per arm. The brute force's PeoplePerSkill^SkillCount calls stop it at 10
// skills; the memoized arm scans every person once per state, O(SkillCount^2), and runs
// on to LC 1125's own bound of 16 skills (48 people, inside its 60). The two are
// compared at the sizes both run.
public class SmallestSufficientTeamBenchmarks
{
    private const int PeoplePerSkill = 3;

    private Dictionary<int, SkillMasks> _masksBySkillCount = [];

    public static IEnumerable<int> BruteForceSizes => [6, 10];

    public static IEnumerable<int> MemoizedSizes => [.. BruteForceSizes, 13, 16];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _masksBySkillCount = MemoizedSizes.ToDictionary(skillCount => skillCount, DedicatedPeople);

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public int[] BruteForceRecursion(int skillCount) =>
        SmallestSufficientTeamSolution.SmallestTeamByBruteForceRecursion(_masksBySkillCount[skillCount]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int[] MemoizedRecursion(int skillCount) =>
        SmallestSufficientTeamSolution.SmallestTeamByMemoizedBitmask(_masksBySkillCount[skillCount]);

    private static SkillMasks DedicatedPeople(int skillCount)
    {
        var people = new List<int>();
        for (var skill = 0; skill < skillCount; skill++)
        {
            for (var copy = 0; copy < PeoplePerSkill; copy++)
            {
                people.Add(1 << skill);
            }
        }

        return new SkillMasks([.. people], (1 << skillCount) - 1);
    }
}
