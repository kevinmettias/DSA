using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GuessNumberHigherOrLower;

// LeetCode 374. Guess Number Higher or Lower: BinarySearch.Find over a virtual
// IRandomAccessSequence of the candidates 1..n - the same "no stored array" shape
// FirstBadVersionTests uses for isBadVersion - with a custom IComparer that routes
// every comparison through the opaque guess() oracle instead of a direct value
// comparison, finding pick in O(log n) probes instead of an O(n) linear scan.
public sealed partial class GuessNumberHigherOrLowerTests
{
    [Theory]
    [InlineData(10, 6, 6)]
    [InlineData(1, 1, 1)]
    [InlineData(2, 1, 1)]
    public void GuessNumber_LeetCodeExamples_ReturnsPickedNumber(int n, int pick, int expected)
    {
        var actual = GuessNumber(n, pick);
        Assert.Equal(expected, actual);
    }

    private static int GuessNumber(int n, int pick)
    {
        int Guess(int num) => pick.CompareTo(num);

        var sequence = new NumberLineSequence(n);
        var comparer = new GuessComparer(Guess);

        return BinarySearch.Find<int, NumberLineSequence>(sequence, target: 0, comparer)!.Value + 1;
    }

    // Get(index) is the 1-based candidate itself - the sequence is never materialized,
    // n can be as large as 2^31 - 1 without allocating anything.
    private readonly struct NumberLineSequence(int length) : IRandomAccessSequence<int>
    {
        public int Length => length;

        public int Get(int index) => index + 1;
    }

    // Compare(candidate, _) must return negative when the search should move right
    // (candidate too low) and positive when it should move left (candidate too high) -
    // exactly -guess(candidate), since guess() returns 1 when candidate is too low
    // (guess higher) and -1 when too high (guess lower). The second Find parameter is
    // unused: guess() is the sole oracle, the same way FirstBadVersion never touches a
    // real target value either.
    private sealed class GuessComparer(Func<int, int> guess) : IComparer<int>
    {
        public int Compare(int candidate, int target) => -guess(candidate);
    }
}
