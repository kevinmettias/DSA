using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfMusicPlaylists;

// LeetCode 920. Number of Music Playlists: count the playlists of exactly goal
// songs drawn from n distinct songs, where every song is played at least once and
// a song may only be replayed once at least k other songs have been played since.
//
// Both strategies solve the same recurrence over the same (length, unique) state -
// f(length, unique) = playlists of that length using exactly that many distinct
// songs:
//
//   f(length-1, unique-1) * (n - unique + 1)   play a brand-new song, one of the
//                                              n - (unique - 1) not yet used
// + f(length-1, unique)   * max(unique - k, 0) replay an old song, any of the ones
//                                              already played except the k most
//                                              recent
//
// with every running total reduced mod 1e9+7 as LeetCode requires. They differ
// only in evaluation order: a bottom-up 2-D table filled in increasing length
// order, or this repo's own Memoizer driving the same recurrence top-down -
// the CoinChangeII/DecodeWaysII pairing, on a two-field tuple state.
internal static class NumberOfMusicPlaylistsSolution
{
    // The textbook answer: a BCL long[,] filled in increasing length order.
    // Deliberately written without this repo's primitives - it is the arm the
    // memoized strategy below has to justify itself against.
    public static long NumMusicPlaylistsByTabulation(int n, int goal, int k)
    {
        var dp = new long[goal + 1, n + 1];
        dp[0, 0] = 1;

        for (var length = 1; length <= goal; length++)
        {
            for (var unique = 1; unique <= n; unique++)
            {
                var total = dp[length - 1, unique - 1] * (n - unique + 1) % ModularArithmetic.Modulo;

                if (unique > k)
                {
                    total = (total + (dp[length - 1, unique] * (unique - k))) % ModularArithmetic.Modulo;
                }

                dp[length, unique] = total;
            }
        }

        return dp[goal, n];
    }

    // This repo's own top-down engine: Memoizer.Memoize caches each
    // (length, unique) state the first time it is reached, so the recurrence reads
    // as ordinary recursion with no hand-rolled cache dictionary, and only the
    // states actually reachable from (goal, n) are ever evaluated.
    public static long NumMusicPlaylistsByMemoizedRecurrence(int n, int goal, int k) =>
        Memoizer.Memoize<(int Length, int Unique), long>((goal, n), new PlaylistCounts(n, k));

    // The recurrence, as a named type: a playlist of a given length drawing on a given
    // number of distinct songs either adds a brand-new song - one of those not yet used -
    // to a shorter playlist, or replays an old one, any already played except the k most
    // recent.
    private sealed class PlaylistCounts(int songCount, int replayGap)
        : IRecurrence<(int Length, int Unique), long>
    {
        public long Replay((int Length, int Unique) state, IRecurrence<(int Length, int Unique), long> rest)
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

            var total = rest.Replay((length - 1, unique - 1), rest) * (songCount - unique + 1)
                % ModularArithmetic.Modulo;

            if (unique > replayGap)
            {
                total = (total + (rest.Replay((length - 1, unique), rest) * (unique - replayGap)))
                    % ModularArithmetic.Modulo;
            }

            return total;
        }
    }
}
