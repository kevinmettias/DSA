using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfMusicPlaylists;

// LeetCode 920. Number of Music Playlists: f(length, unique) = playlists of the
// given length using exactly that many distinct songs so far -
// f(length-1, unique-1) * (n - unique + 1) [play a brand-new song, one of the
// n - (unique - 1) not yet used] + f(length-1, unique) * max(unique - k, 0) [replay
// an old song, any of the unique already played except the k most recent], memoized
// via this repo's own Memoizer over a 2-D (length, unique) tuple state
// (CoinChangeII/DecodeWaysII precedent for both the recurrence shape and the tuple
// state) instead of the textbook unmemoized exponential recursion, with every
// running total reduced mod 1e9+7 as LeetCode requires.
public sealed partial class NumberOfMusicPlaylistsTests
{
    private const long Mod = 1_000_000_007;

    [Theory]
    [InlineData(3, 3, 1, 6)]
    [InlineData(2, 3, 0, 6)]
    [InlineData(2, 3, 1, 2)]
    public void NumMusicPlaylists_LeetCodeExamples_ReturnsPlaylistCount(int n, int goal, int k, long expected)
    {
        var actual = NumMusicPlaylists(n, goal, k);
        Assert.Equal(expected, actual);
    }

    private static long NumMusicPlaylists(int n, int goal, int k)
    {
        return Memoizer.Memoize<(int Length, int Unique), long>((goal, n), Ways);

        long Ways((int Length, int Unique) state, Func<(int Length, int Unique), long> ways)
        {
            var (length, unique) = state;

            if (length == 0)
            {
                return unique == 0 ? 1 : 0;
            }

            if (unique == 0)
            {
                return 0;
            }

            var total = ways((length - 1, unique - 1)) * (n - unique + 1) % Mod;

            if (unique > k)
            {
                total = (total + (ways((length - 1, unique)) * (unique - k))) % Mod;
            }

            return total;
        }
    }
}
