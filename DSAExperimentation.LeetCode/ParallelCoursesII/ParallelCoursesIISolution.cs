using System.Numerics;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.ParallelCoursesII;

// LeetCode 1494. Parallel Courses II: the fewest semesters needed to take all n
// courses, where a course may only be taken once every one of its prerequisites
// has been taken and at most k courses may be taken in any one semester.
//
// Both strategies are the same bitmask recursion over "which courses are already
// completed" (one bit per course): at each state "ready" is every not-yet-taken
// course whose prerequisites are all completed, every subset of at most k ready
// courses is a candidate next semester, and the recurrence picks whichever subset
// leads to the fewest remaining semesters. Submask enumeration
// (`subset = (subset - 1) & ready`) walks those candidates.
//
// They differ only in whether the recursion remembers states it has already
// solved: MinNumberOfSemestersByBruteForceRecursion re-explores a completed-course
// mask once per order that can reach it, while MinNumberOfSemestersByMemoizedRecursion
// routes the same recurrence through this repo's own Memoizer keyed on that mask,
// so each of the 2^n states is solved exactly once.
internal static class ParallelCoursesIISolution
{
    // The textbook answer: the same recurrence with no memoization at all, so a
    // state reachable by many different semester orders is re-solved once per
    // order. Deliberately BCL-only - it is the arm the memoized strategy below
    // has to justify itself against.
    public static int MinNumberOfSemestersByBruteForceRecursion(int n, int[][] relations, int k)
    {
        var courses = BuildCourseLoad(n, relations, k);

        return SemestersFrom(0);

        int SemestersFrom(int completedMask) => BestSemesters(courses, completedMask, SemestersFrom);
    }

    // This repo's own Memoizer, keyed on the completed-course bitmask - the same
    // shape CanIWin and PartitionToKEqualSumSubsets use, an int bitmask as the memo
    // state rather than a bare counter.
    public static int MinNumberOfSemestersByMemoizedRecursion(int n, int[][] relations, int k)
    {
        var courses = BuildCourseLoad(n, relations, k);

        return Memoizer.Memoize<int, int>(
            0,
            (completedMask, semestersFrom) => BestSemesters(courses, completedMask, semestersFrom));
    }

    // The immutable part of the problem (the prerequisite masks, the all-courses-taken
    // mask and the per-semester cap), as opposed to the completed-course mask that
    // changes at every step.
    private readonly record struct CourseLoad(int[] PrerequisiteMasks, int FullMask, int MaxPerSemester);

    private static CourseLoad BuildCourseLoad(int n, int[][] relations, int k)
    {
        var prerequisiteMasks = new int[n];

        foreach (var relation in relations)
        {
            var next = relation[1] - 1;
            prerequisiteMasks[next] |= 1 << (relation[0] - 1);
        }

        return new CourseLoad(prerequisiteMasks, (1 << n) - 1, k);
    }

    // One step of the recurrence, shared by both strategies so the only thing they
    // differ in is how semestersFrom resolves a recursive call.
    private static int BestSemesters(CourseLoad courses, int completedMask, Func<int, int> semestersFrom)
    {
        if (completedMask == courses.FullMask)
        {
            return 0;
        }

        var ready = ReadyMask(courses, completedMask);
        var best = int.MaxValue;

        for (var subset = ready; subset > 0; subset = (subset - 1) & ready)
        {
            if (BitOperations.PopCount((uint)subset) > courses.MaxPerSemester)
            {
                continue;
            }

            var remaining = semestersFrom(completedMask | subset);
            best = Math.Min(best, remaining);
        }

        return 1 + best;
    }

    // Every course not yet taken whose prerequisites are all already completed.
    private static int ReadyMask(CourseLoad courses, int completedMask)
    {
        var ready = 0;

        for (var course = 0; course < courses.PrerequisiteMasks.Length; course++)
        {
            var bit = 1 << course;
            var prerequisites = courses.PrerequisiteMasks[course];

            if ((completedMask & bit) == 0 && (prerequisites & completedMask) == prerequisites)
            {
                ready |= bit;
            }
        }

        return ready;
    }
}
