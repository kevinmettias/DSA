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
            .Workload("dense-2000", (Enumerable.Range(0, 2000).ToArray(), 3997))
            .Build();

    private static int[] Indices(TwoSumAttempt attempt, (int[] Nums, int Target) input)
        => attempt(input.Nums, input.Target, out var first, out var second) ? [first, second] : [];
}
