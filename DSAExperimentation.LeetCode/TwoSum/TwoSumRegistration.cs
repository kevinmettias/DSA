using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.LeetCode.TwoSum;

// Shape under test: a solution whose repo-idiomatic signature is NOT LeetCode's.
// TwoSumSolution exposes the Try pattern with two out parameters, because that is
// what a C# caller wants; LeetCode's answer is a two-element array. The
// registration adapts one to the other in a typed lambda, which is the general
// answer to "our signature differs from the problem's" - the harness never needs
// to know, and the solution never has to grow a LeetCode-shaped overload just to
// be testable.
internal sealed class TwoSumRegistration : ILeetCodeProblemRegistration
{
    private const int UnreachableTarget = -1;
    private const int MaxValueExclusive = 1_000;
    private const int Seed = 1;

    private delegate bool TwoSumAttempt(int[] nums, int target, out int first, out int second);

    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<(int[] Nums, int Target), int[]>("two-sum")
            .Strategy("BruteForce", input => Indices(TwoSumSolution.TryFindIndicesByBruteForce, input))
            .Strategy("HashMap", input => Indices(TwoSumSolution.TryFindIndicesByHashMap, input))

            // "You may return the answer in any order" - so the scraped example
            // output is one correct answer, not the only one.
            .MatchingAnswersWith(LeetCodeAnswers.SetEqual<int>)
            .Case("example-1", ([2, 7, 11, 15], 9), [0, 1])
            .Case("example-2", ([3, 2, 4], 6), [1, 2])
            .Case("example-3", ([3, 3], 6), [0, 1])
            .Case("no-pair-sums-to-target", ([1, 2, 3], 100), [])
            // The two lengths the retired per-problem benchmark swept, and its
            // deliberately UNREACHABLE target: every value is positive and the
            // target is negative, so neither strategy gets an early exit and brute
            // force is measured on its actual worst case rather than on how soon it
            // happened to stumble onto the answer.
            .Workload("unreachable-target-200", BuildDenseValues(200))
            .Workload("unreachable-target-5000", BuildDenseValues(5_000))
            .Build();

    private static (int[] Nums, int Target) BuildDenseValues(int length)
    {
        var random = new Random(Seed);

        return ([.. Enumerable.Range(0, length).Select(_ => random.Next(1, MaxValueExclusive))], UnreachableTarget);
    }

    private static int[] Indices(TwoSumAttempt attempt, (int[] Nums, int Target) input)
        => attempt(input.Nums, input.Target, out var first, out var second) ? [first, second] : [];
}
