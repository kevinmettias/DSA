using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheShortestSuperstring;

// LeetCode 943. Find the Shortest Superstring: overlap[i][j] (the longest suffix of
// word i that is also a prefix of word j) turns picking a word order into a
// maximum-weight Hamiltonian path over the words - bitmask TSP. State (Mask, Last) is
// exactly the (visited-set, current) shape ShortestPathVisitingAllNodesTests already
// proves out for this repo, just as Memoizer's TState instead of a graph-topology
// node, and maximizing overlap instead of minimizing hop count. TResult additionally
// carries which predecessor produced the optimum, since (unlike a scalar min/max) the
// final answer needs a reconstructible word order, not just its length - the same
// "carry enough to reconstruct, not just the metric" need StickersToSpellWordTests'
// own ApplySticker state transition satisfies for its answer.
public sealed partial class FindTheShortestSuperstringTests
{
    [Fact]
    public void ShortestSuperstring_ThreeDisjointWords_ConcatenatesAllWithNoOverlap()
    {
        string[] words = ["alex", "loves", "leetcode"];

        var result = ShortestSuperstring(words);

        Assert.Equal(17, result.Length);
        Assert.All(words, word => Assert.Contains(word, result));
    }

    [Fact]
    public void ShortestSuperstring_OverlappingWords_FindsMinimalLengthUsingOverlaps()
    {
        string[] words = ["catg", "ctaagt", "gcta", "ttca", "atgcatc"];

        var result = ShortestSuperstring(words);

        Assert.Equal(16, result.Length);
        Assert.All(words, word => Assert.Contains(word, result));
    }

    private static string ShortestSuperstring(string[] words)
    {
        if (words.Length == 1)
        {
            return words[0];
        }

        var overlap = BuildOverlaps(words);
        var order = ReconstructBestOrder(words.Length, overlap);

        var superstring = words[order[0]];
        for (var i = 1; i < order.Length; i++)
        {
            superstring += words[order[i]][overlap[order[i - 1], order[i]]..];
        }

        return superstring;
    }

    private static int[] ReconstructBestOrder(int wordCount, int[,] overlap)
    {
        var fullMask = (1 << wordCount) - 1;
        (int Best, int Prev) recurrence((int Mask, int Last) state, Func<(int Mask, int Last), (int Best, int Prev)> best)
            => Recurrence(state, best, wordCount, overlap);
        var (last, prev) = FindBestFinalState(wordCount, fullMask, recurrence);
        return BacktrackOrder(wordCount, last, prev, recurrence);
    }

    // Tries every word as the tour's final word (each a different Memoizer start
    // state) and keeps whichever achieves the highest total overlap.
    private static (int Last, int Prev) FindBestFinalState(
        int wordCount,
        int fullMask,
        Func<(int Mask, int Last), Func<(int Mask, int Last), (int Best, int Prev)>, (int Best, int Prev)> recurrence)
    {
        var last = 0;
        var prev = -1;
        var bestTotal = -1;

        for (var candidate = 0; candidate < wordCount; candidate++)
        {
            var (total, candidatePrev) = Memoizer.Memoize<(int Mask, int Last), (int Best, int Prev)>(
                (fullMask, candidate), recurrence);

            if (total > bestTotal)
            {
                bestTotal = total;
                last = candidate;
                prev = candidatePrev;
            }
        }

        return (last, prev);
    }

    // Bitmask TSP recurrence over Memoizer: state (Mask, Last) = "these words are
    // placed, Last is the rightmost one"; result = (best total overlap achievable,
    // the predecessor that achieves it). Each top-level call fixes a different
    // candidate final word, since Memoizer.Memoize only ever returns the result
    // for the one start state it's given.
    private static (int Best, int Prev) Recurrence(
        (int Mask, int Last) state,
        Func<(int Mask, int Last), (int Best, int Prev)> best,
        int wordCount,
        int[,] overlap)
    {
        var remaining = state.Mask & ~(1 << state.Last);
        if (remaining == 0)
        {
            return (0, -1);
        }

        return Enumerable.Range(0, wordCount)
            .Where(candidate => (remaining & (1 << candidate)) != 0)
            .Aggregate((Best: -1, Prev: -1), (currentBest, candidate) =>
            {
                var (subBest, _) = best((remaining, candidate));
                var total = subBest + overlap[candidate, state.Last];
                return total > currentBest.Best ? (total, candidate) : currentBest;
            });
    }

    // Walks the (Mask, Last) -> Prev chain backwards from the best final state,
    // rebuilding the word order the memoized recurrence discovered.
    private static int[] BacktrackOrder(
        int wordCount,
        int last,
        int prev,
        Func<(int Mask, int Last), Func<(int Mask, int Last), (int Best, int Prev)>, (int Best, int Prev)> recurrence)
    {
        var order = new int[wordCount];
        var mask = (1 << wordCount) - 1;

        for (var i = wordCount - 1; i >= 0; i--)
        {
            order[i] = last;
            mask &= ~(1 << last);

            if (mask == 0)
            {
                break;
            }

            last = prev;
            var (_, nextPrev) = Memoizer.Memoize<(int Mask, int Last), (int Best, int Prev)>((mask, last), recurrence);
            prev = nextPrev;
        }

        return order;
    }

    private static int[,] BuildOverlaps(string[] words)
    {
        var n = words.Length;
        var overlap = new int[n, n];

        for (var i = 0; i < n; i++)
        {
            for (var j = 0; j < n; j++)
            {
                if (i != j)
                {
                    overlap[i, j] = OverlapLength(words[i], words[j]);
                }
            }
        }

        return overlap;
    }

    // Longest suffix of `left` that is also a prefix of `right` - how much of
    // `right` can be dropped when it is appended immediately after `left`.
    private static int OverlapLength(string left, string right)
    {
        var max = Math.Min(left.Length, right.Length);

        for (var len = max; len > 0; len--)
        {
            var suffixOfLeft = left.AsSpan(left.Length - len);
            var prefixOfRight = right.AsSpan(0, len);
            if (suffixOfLeft.SequenceEqual(prefixOfRight))
            {
                return len;
            }
        }

        return 0;
    }
}
