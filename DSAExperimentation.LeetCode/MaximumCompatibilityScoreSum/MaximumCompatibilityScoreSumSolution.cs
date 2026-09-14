using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximumCompatibilityScoreSum;

// LeetCode 1947. Maximum Compatibility Score Sum: pair every student with a distinct
// mentor so the summed per-pair compatibility score is as large as possible.
//
// Both strategies are the same recursion over the state (next student to place, mask
// of mentors already taken); they differ only in whether that state is remembered.
// The brute force re-explores every state from scratch down every branch - many
// different pick orders reach the identical "these mentors are taken" state, so the
// work is factorial - while the composed strategy routes the identical recursion
// through this repo's own Memoizer, the same (int, int) tuple-state shape LC 1595's
// (index, mask) recursion uses, collapsing it to one visit per (student, mask) pair.
//
// The score table itself is CompatibilityScoreMatrix; each strategy has a
// LeetCode-shaped overload that builds it and a prepared-input overload (§17.4) so a
// benchmark can charge that construction to [GlobalSetup] instead of to the search.
internal static class MaximumCompatibilityScoreSumSolution
{
    // The textbook answer: plain recursion over (student, usedMask) with no cache at
    // all, deliberately written without this repo's primitives - it is the arm the
    // memoized strategy below has to justify itself against.
    public static int MaxCompatibilitySumByBruteForceRecursion(int[][] students, int[][] mentors)
    {
        var scores = CompatibilityScoreMatrix.Build(students, mentors);

        return MaxCompatibilitySumByBruteForceRecursion(scores);
    }

    public static int MaxCompatibilitySumByBruteForceRecursion(CompatibilityScoreMatrix scores) =>
        BestAssignmentFrom(scores, 0, 0);

    private static int BestAssignmentFrom(CompatibilityScoreMatrix scores, int student, int usedMask)
    {
        if (student == scores.GroupSize)
        {
            return 0;
        }

        var top = int.MinValue;

        for (var mentor = 0; mentor < scores.GroupSize; mentor++)
        {
            if ((usedMask & (1 << mentor)) != 0)
            {
                continue;
            }

            var rest = BestAssignmentFrom(scores, student + 1, usedMask | (1 << mentor));
            top = Math.Max(top, scores.Score(student, mentor) + rest);
        }

        return top;
    }

    // The same recursion, with Memoizer owning the (student, usedMask) cache: at most
    // one evaluation per reachable state instead of one per distinct pick order.
    public static int MaxCompatibilitySumByMemoizedBitmask(int[][] students, int[][] mentors)
    {
        var scores = CompatibilityScoreMatrix.Build(students, mentors);

        return MaxCompatibilitySumByMemoizedBitmask(scores);
    }

    public static int MaxCompatibilitySumByMemoizedBitmask(CompatibilityScoreMatrix scores) =>
        Memoizer.Memoize<(int Student, int UsedMask), int>((0, 0), (state, best) =>
        {
            var (student, usedMask) = state;

            if (student == scores.GroupSize)
            {
                return 0;
            }

            var top = int.MinValue;

            for (var mentor = 0; mentor < scores.GroupSize; mentor++)
            {
                if ((usedMask & (1 << mentor)) != 0)
                {
                    continue;
                }

                var rest = best((student + 1, usedMask | (1 << mentor)));
                top = Math.Max(top, scores.Score(student, mentor) + rest);
            }

            return top;
        });
}
