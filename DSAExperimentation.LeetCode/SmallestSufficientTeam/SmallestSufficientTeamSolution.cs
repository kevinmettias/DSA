using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.SmallestSufficientTeam;

// LeetCode 1125. Smallest Sufficient Team: pick the fewest people whose combined
// skills cover every required skill. reqSkills.Length <= 16 fits the "which skills
// are still missing" state in an int, and people.Length <= 60 fits the CHOSEN PEOPLE
// in a long, so the recursion's result IS the team itself rather than just its size.
//
// Each step covers only the LOWEST still-missing skill bit - every sufficient team
// must include some person with that skill - trying each such person and keeping
// whichever completion has the fewest people. The two strategies run that identical
// recursion; they differ only in whether a state reached by several different pick
// orders is recomputed each time or once.
internal static class SmallestSufficientTeamSolution
{
    // The textbook baseline: the same recursion with no memoization at all, so the
    // identical "still missing" subtree is re-explored once per person who could
    // have reached it. Deliberately plain recursion over BCL arrays - the arm the
    // memoized strategy below has to justify itself against.
    public static int[] SmallestTeamByBruteForceRecursion(string[] reqSkills, string[][] people)
    {
        var masks = SkillMasks.Build(reqSkills, people);

        return SmallestTeamByBruteForceRecursion(masks);
    }

    public static int[] SmallestTeamByBruteForceRecursion(SkillMasks masks)
    {
        var personSkillMask = masks.PersonSkillMask;
        var pick = new SkillCoveringPick(personSkillMask);
        var teamMask = pick.Replay(masks.FullMask, pick);

        return ExtractTeam(teamMask, personSkillMask.Length);
    }

    // This repo's own Memoizer, keyed on the missing-skills bitmask - the CanIWin
    // shape, applied to a DP whose memoized RESULT is a team rather than a bool.
    // Every reachable state is computed exactly once.
    public static int[] SmallestTeamByMemoizedBitmask(string[] reqSkills, string[][] people)
    {
        var masks = SkillMasks.Build(reqSkills, people);

        return SmallestTeamByMemoizedBitmask(masks);
    }

    public static int[] SmallestTeamByMemoizedBitmask(SkillMasks masks)
    {
        var personSkillMask = masks.PersonSkillMask;

        var teamMask = Memoizer.Memoize<int, long>(
            masks.FullMask,
            new SkillCoveringPick(personSkillMask));

        return ExtractTeam(teamMask, personSkillMask.Length);
    }

    // The recursion body both strategies share, named: nothing missing means the empty
    // team, otherwise cover the lowest missing bit. The memoized arm hands it the
    // cache-backed recursion, and the brute-force arm replays it straight back into
    // itself - the same rule either way, differing only in what `rest` is. Each person's
    // skill mask is the whole of what the rule needs from its caller, so it is the
    // constructor's only input.
    private sealed class SkillCoveringPick(int[] personSkillMask) : IRecurrence<int, long>
    {
        public long Replay(int missing, IRecurrence<int, long> rest)
        {
            if (missing == 0)
            {
                return 0L;
            }

            return BestTeamCoveringBit(missing, missing & -missing, rest);
        }

        // Every sufficient team must include some person holding the target skill, so
        // the choice is which of them, and the team is that person plus whatever they
        // leave uncovered.
        private long BestTeamCoveringBit(int missing, int targetBit, IRecurrence<int, long> rest)
        {
            var best = -1L;

            for (var p = 0; p < personSkillMask.Length; p++)
            {
                if ((personSkillMask[p] & targetBit) == 0)
                {
                    continue;
                }

                var candidate = rest.Replay(missing & ~personSkillMask[p], rest) | (1L << p);
                if (best == -1 || PopCount(candidate) < PopCount(best))
                {
                    best = candidate;
                }
            }

            return best;
        }
    }

    // LeetCode wants the people themselves, in ascending index order.
    private static int[] ExtractTeam(long teamMask, int peopleCount)
    {
        var team = new List<int>();

        for (var p = 0; p < peopleCount; p++)
        {
            if ((teamMask & (1L << p)) != 0)
            {
                team.Add(p);
            }
        }

        return [.. team];
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
