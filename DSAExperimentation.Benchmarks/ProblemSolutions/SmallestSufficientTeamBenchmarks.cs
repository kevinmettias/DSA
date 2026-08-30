using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Smallest Sufficient Team (LC 1125): the textbook unmemoized bitmask recursion
// (re-explores the identical "which skills are still missing" subtree once per
// distinct person who could have reached it) vs. the same recursion routed through
// this repo's own Memoizer, keyed on the missing-skills bitmask - the CanIWinBenchmarks
// precedent, applied to a DP whose memoized RESULT (not just a bool) is the team
// itself, packed into a long bitmask of chosen people (people.Length stays well under
// 64). Each of PeoplePerSkill people covers exactly one, dedicated skill, so at every
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

    private int[] _personSkillMask = null!;
    private int _fullMask;

    [GlobalSetup]
    public void Setup()
    {
        _fullMask = (1 << SkillCount) - 1;

        var people = new List<int>();
        for (var skill = 0; skill < SkillCount; skill++)
        {
            for (var copy = 0; copy < PeoplePerSkill; copy++)
            {
                people.Add(1 << skill);
            }
        }

        _personSkillMask = [.. people];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() => PopCount(SmallestTeamBruteForce(_fullMask));

    private long SmallestTeamBruteForce(int missing)
    {
        if (missing == 0)
        {
            return 0L;
        }

        var targetBit = missing & -missing;
        var best = -1L;

        for (var p = 0; p < _personSkillMask.Length; p++)
        {
            if ((_personSkillMask[p] & targetBit) == 0)
            {
                continue;
            }

            var candidate = SmallestTeamBruteForce(missing & ~_personSkillMask[p]) | (1L << p);
            if (best == -1 || PopCount(candidate) < PopCount(best))
            {
                best = candidate;
            }
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion()
    {
        var teamMask = Memoizer.Memoize<int, long>(_fullMask, (missing, smallestTeamFor) =>
        {
            if (missing == 0)
            {
                return 0L;
            }

            var targetBit = missing & -missing;
            var best = -1L;

            for (var p = 0; p < _personSkillMask.Length; p++)
            {
                if ((_personSkillMask[p] & targetBit) == 0)
                {
                    continue;
                }

                var candidate = smallestTeamFor(missing & ~_personSkillMask[p]) | (1L << p);
                if (best == -1 || PopCount(candidate) < PopCount(best))
                {
                    best = candidate;
                }
            }

            return best;
        });

        return PopCount(teamMask);
    }

    private static int PopCount(long mask)
    {
        var count = 0;
        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }
}
