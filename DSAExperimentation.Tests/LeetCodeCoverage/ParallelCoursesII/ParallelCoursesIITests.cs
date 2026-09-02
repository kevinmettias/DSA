using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ParallelCoursesII;

// LeetCode 1494. Parallel Courses II: bitmask DP over "which courses are already
// completed," via this repo's own Memoizer - the same CanIWin/PartitionToKEqualSumSubsets
// shape, keyed on an int bitmask (one bit per course) instead of a bare integer. At each
// state, "ready" is every not-yet-taken course whose prerequisites are already all
// completed; every subset of at most k ready courses is a candidate next semester, and
// the recurrence picks whichever subset leads to the fewest remaining semesters -
// submask enumeration (`sub = (sub - 1) & ready`) is the one piece of glue code this
// composition needs, the same bit trick CanIWin's own SumChosen/bit-scan already uses.
public sealed class ParallelCoursesIITests
{
    [Fact]
    public void MinNumberOfSemesters_DiamondPrerequisites_ReturnsThree()
    {
        int[][] relations = [[2, 1], [3, 1], [1, 4]];

        var actual = MinNumberOfSemesters(n: 4, relations, k: 2);
        Assert.Equal(3, actual);
    }

    [Fact]
    public void MinNumberOfSemesters_ThreeCoursesGateOneFollowUp_ReturnsFour()
    {
        int[][] relations = [[2, 1], [3, 1], [4, 1], [1, 5]];

        var actual = MinNumberOfSemesters(n: 5, relations, k: 2);
        Assert.Equal(4, actual);
    }

    [Fact]
    public void MinNumberOfSemesters_NoPrerequisites_PacksExactlyKPerSemester()
    {
        var actual = MinNumberOfSemesters(n: 11, relations: [], k: 2);
        Assert.Equal(6, actual);
    }

    private static int MinNumberOfSemesters(int n, int[][] relations, int k)
    {
        var prereqMask = BuildPrereqMasks(n, relations);
        var fullMask = (1 << n) - 1;

        return Memoizer.Memoize<int, int>(0, (completedMask, semestersFrom) =>
        {
            if (completedMask == fullMask)
            {
                return 0;
            }

            var ready = ComputeReadyMask(prereqMask, completedMask, n);
            var best = BestSemestersOverSubsets(ready, k, completedMask, semestersFrom);

            return 1 + best;
        });
    }

    private static int[] BuildPrereqMasks(int n, int[][] relations)
    {
        var prereqMask = new int[n];
        foreach (var relation in relations)
        {
            var next = relation[1] - 1;
            prereqMask[next] |= 1 << (relation[0] - 1);
        }

        return prereqMask;
    }

    private static int ComputeReadyMask(int[] prereqMask, int completedMask, int n)
    {
        var ready = 0;
        for (var course = 0; course < n; course++)
        {
            var bit = 1 << course;
            if ((completedMask & bit) == 0 && (prereqMask[course] & completedMask) == prereqMask[course])
            {
                ready |= bit;
            }
        }

        return ready;
    }

    private static int BestSemestersOverSubsets(int ready, int k, int completedMask, Func<int, int> semestersFrom)
    {
        var best = int.MaxValue;
        for (var subset = ready; subset > 0; subset = (subset - 1) & ready)
        {
            if (PopCount(subset) > k)
            {
                continue;
            }

            best = Math.Min(best, semestersFrom(completedMask | subset));
        }

        return best;
    }

    private static int PopCount(int value)
    {
        var count = 0;
        while (value != 0)
        {
            value &= value - 1;
            count++;
        }

        return count;
    }
}
