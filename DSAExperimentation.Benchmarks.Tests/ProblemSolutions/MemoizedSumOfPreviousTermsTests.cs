using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FibonacciBenchmarks's nested MemoizedSumOfPreviousTerms.
//
// MemoizedSumOfPreviousTerms is a private nested type, so no test class can name it in code. Coverage
// is attributed by TYPE, so the nested unit needs a class named after it, and the only surface that
// runs its Replay is the outer benchmark's TopDownMemoized arm, which hands a fresh instance to
// Memoizer.Memoize. These tests therefore drive that arm and assert what Replay's rule is
// responsible for: the two base cases answer themselves, and every later index is the sum of the two
// before it.
//
// The expected terms are the Fibonacci numbers themselves, known independently of the arm. Replay
// declares its own base cases and adds `state - 1` to `state - RecurrenceOrder`, so a rule that
// followed the wrong offset, recursed from a base case instead of returning it, or memoized the
// wrong key could not produce both values below.
public sealed partial class MemoizedSumOfPreviousTermsTests
{
    private const int ZerothIndex = 0;
    private const int ZerothTerm = 0;
    private const int FirstIndex = 1;
    private const int FirstTerm = 1;
    private const int TwentiethIndex = 20;
    private const int TwentiethTerm = 6765;

    [Fact]
    public void Replay_BothBaseCases_AnswerThemselvesWithoutFollowingTheRecurrence()
    {
        Assert.Equal(ZerothTerm, BuildHarness(ZerothIndex).TopDownMemoized());
        Assert.Equal(FirstTerm, BuildHarness(FirstIndex).TopDownMemoized());
    }

    [Fact]
    public void Replay_TwentiethIndex_SumsTheTwoPreviousTermsToReachTheKnownTerm() =>
        Assert.Equal(TwentiethTerm, BuildHarness(TwentiethIndex).TopDownMemoized());

    // FibonacciBenchmarks has no [GlobalSetup] - the recurrence is built from the tuned index alone,
    // so the bare initializer is the whole harness.
    private static FibonacciBenchmarks BuildHarness(int termIndex) =>
        new() { TermIndex = termIndex };
}
