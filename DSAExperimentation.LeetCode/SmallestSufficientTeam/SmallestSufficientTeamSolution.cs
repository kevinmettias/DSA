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
    public static int[] SmallestTeamByBruteForceRecursion(string[] reqSkills, string[][] people) =>
        SmallestTeamByBruteForceRecursion(SkillMasks.Build(reqSkills, people));

    public static int[] SmallestTeamByBruteForceRecursion(SkillMasks masks)
    {
        var personSkillMask = masks.PersonSkillMask;

        long SmallestTeamFor(int missing) => BestTeam(personSkillMask, missing, SmallestTeamFor);

        return ExtractTeam(SmallestTeamFor(masks.FullMask), personSkillMask.Length);
    }

    // This repo's own Memoizer, keyed on the missing-skills bitmask - the CanIWin
    // shape, applied to a DP whose memoized RESULT is a team rather than a bool.
    // Every reachable state is computed exactly once.
    public static int[] SmallestTeamByMemoizedBitmask(string[] reqSkills, string[][] people) =>
        SmallestTeamByMemoizedBitmask(SkillMasks.Build(reqSkills, people));

    public static int[] SmallestTeamByMemoizedBitmask(SkillMasks masks)
    {
        var personSkillMask = masks.PersonSkillMask;

        var teamMask = Memoizer.Memoize<int, long>(
            masks.FullMask,
            (missing, smallestTeamFor) => BestTeam(personSkillMask, missing, smallestTeamFor));

        return ExtractTeam(teamMask, personSkillMask.Length);
    }

    // The recursion body both strategies share: nothing missing means the empty
    // team, otherwise cover the lowest missing bit.
    private static long BestTeam(int[] personSkillMask, int missing, Func<int, long> smallestTeamFor)
    {
        if (missing == 0)
        {
            return 0L;
        }

        return BestTeamCoveringBit(personSkillMask, missing, missing & -missing, smallestTeamFor);
    }

    private static long BestTeamCoveringBit(
        int[] personSkillMask, int missing, int targetBit, Func<int, long> smallestTeamFor)
    {
        var best = -1L;

        for (var p = 0; p < personSkillMask.Length; p++)
        {
            if ((personSkillMask[p] & targetBit) == 0)
            {
                continue;
            }

            var candidate = smallestTeamFor(missing & ~personSkillMask[p]) | (1L << p);
            if (best == -1 || PopCount(candidate) < PopCount(best))
            {
                best = candidate;
            }
        }

        return best;
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
