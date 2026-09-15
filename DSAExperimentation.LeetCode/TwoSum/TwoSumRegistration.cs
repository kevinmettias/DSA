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

    // The operation both arms implement. TwoSumSolution's Try pattern returns its
    // answer through two out parameters, and "false" leaves both unset - a contract
    // with somewhere to be written down here, and parameter names a caller reads at
    // the declaration rather than at whatever lambda happened to be bound to it.
    private interface ITwoSumAttempt
    {
        bool TryFindIndices(int[] nums, int target, out int first, out int second);
    }

    // TwoSumSolution exposes its arms as static methods, so each needs a one-method
    // adapter to satisfy the interface. Both are stateless, so one instance each is
    // shared: the strategy registered below runs inside the benchmark harness's timed
    // region, and picking an arm must not allocate there.
    private static readonly ITwoSumAttempt BruteForce = new BruteForceAttempt();
    private static readonly ITwoSumAttempt HashMap = new HashMapAttempt();

    public LeetCodeProblem Describe()
        => LeetCodeProblem.For<(int[] Nums, int Target), int[]>("two-sum")
            .Strategy("BruteForce", input => Indices(BruteForce, input))
            .Strategy("HashMap", input => Indices(HashMap, input))

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

    private static int[] Indices(ITwoSumAttempt attempt, (int[] Nums, int Target) input)
        => attempt.TryFindIndices(input.Nums, input.Target, out var first, out var second)
            ? IndexPair(first, second)
            : NoIndexPair();

    private static int[] IndexPair(int first, int second) => [first, second];

    private static int[] NoIndexPair() => [];

    private static (int[] Nums, int Target) BuildDenseValues(int length)
    {
        var random = new Random(Seed);

        return ([.. Enumerable.Range(0, length).Select(_ => random.Next(1, MaxValueExclusive))], UnreachableTarget);
    }

    private sealed class BruteForceAttempt : ITwoSumAttempt
    {
        public bool TryFindIndices(int[] nums, int target, out int first, out int second) =>
            TwoSumSolution.TryFindIndicesByBruteForce(nums, target, out first, out second);
    }

    private sealed class HashMapAttempt : ITwoSumAttempt
    {
        public bool TryFindIndices(int[] nums, int target, out int first, out int second) =>
            TwoSumSolution.TryFindIndicesByHashMap(nums, target, out first, out second);
    }
}
