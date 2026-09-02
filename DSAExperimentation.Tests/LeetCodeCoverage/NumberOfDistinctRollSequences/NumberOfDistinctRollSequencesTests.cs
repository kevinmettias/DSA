using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfDistinctRollSequences;

// LeetCode 2318. Number of Distinct Roll Sequences: state (day, secondLastRoll,
// lastRoll) recursion - the next roll must be coprime with lastRoll (gcd == 1) and
// differ from both lastRoll and secondLastRoll (the "gap of 2" rule collapses to
// exactly that, since any farther-apart repeat is unconstrained) - memoized via
// this repo's own Memoizer<TState,TResult> over the 3-tuple state
// (NumberOfMusicPlaylistsTests/SuperEggDropBenchmarks precedent for a multi-field
// tuple state). 0 is used as the "no roll yet" sentinel for the first two
// positions - valid dice values are 1-6, so it never collides with a real roll,
// and the gcd check is skipped entirely while lastRoll is still the sentinel.
public sealed partial class NumberOfDistinctRollSequencesTests
{
    private const long Modulus = 1_000_000_007;

    [Theory]
    [InlineData(1, 6)]
    [InlineData(2, 22)]
    [InlineData(3, 66)]
    [InlineData(4, 184)]
    public void DistinctSequences_KnownRollCounts_ReturnsExpectedSequenceCount(int n, long expected)
        => Assert.Equal(expected, DistinctSequences(n));

    private static long DistinctSequences(int n)
    {
        return Memoizer.Memoize<(int Day, int Prev2, int Prev1), long>((1, 0, 0), Ways);

        long Ways((int Day, int Prev2, int Prev1) state, Func<(int, int, int), long> ways)
        {
            var (day, prev2, prev1) = state;

            if (day > n)
            {
                return 1;
            }

            var total = 0L;

            for (var value = 1; value <= 6; value++)
            {
                if (value == prev1 || value == prev2)
                {
                    continue;
                }

                if (prev1 != 0 && Gcd(value, prev1) != 1)
                {
                    continue;
                }

                total = (total + ways((day + 1, prev1, value))) % Modulus;
            }

            return total;
        }
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
