using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace DSAExperimentation.Algorithms.Sampling;

// Drawing an index with probability proportional to its weight in O(1), by Vose's alias method: n
// columns, each holding Total tickets, column i keeping the tickets below its threshold for itself
// and giving the rest to its alias. A column is drawn uniformly, then a ticket within it, so index i
// comes up weight_i * n times among the n * Total (column, ticket) pairs: exactly its weight's share.
// The shares are built in integers, scaled by n, so the table is exact rather than rounded, and
// exactness is what lets the thresholds be read off the shares once pairing ends: a column paired as
// the short one keeps the share it had then, and one never paired ends with exactly Total, since the
// shares always sum to n * Total. PrefixSumSampler answers the same draw with a binary search; the
// distribution is the fact, the sampler the algorithm (ARCHITECTURE §17.6). The caller draws the
// column and the ticket. The table is fixed once built, so a copy of the sampler shares it unchanged.
/// <summary>Draws an index with probability proportional to its weight in constant time, from a drawn column and ticket.</summary>
public readonly struct AliasSampler
{
    private readonly ImmutableArray<int> _thresholds;
    private readonly ImmutableArray<int> _aliases;

    /// <summary>The number of columns, one per weight.</summary>
    public int Columns => _thresholds.Length;

    /// <summary>The number of tickets in each column: the sum of the weights.</summary>
    public int Total { get; }

    /// <summary>Builds the alias table of the weights.</summary>
    /// <param name="weights">Each index's weight, none negative, at least one positive.</param>
    /// <exception cref="ArgumentException"><paramref name="weights"/> has a negative weight or no positive one.</exception>
    public AliasSampler(ReadOnlySpan<int> weights)
    {
        Total = SamplingWeights.TotalOf(weights);
        var shares = ScaledByColumnCount(weights);
        _aliases = ImmutableCollectionsMarshal.AsImmutableArray(PairColumns(shares));
        _thresholds = ThresholdsOf(shares);
    }

    // Each weight times the column count, so a column's fair share is exactly Total.
    private static long[] ScaledByColumnCount(ReadOnlySpan<int> weights)
    {
        var scaled = new long[weights.Length];

        for (var i = 0; i < weights.Length; i++)
        {
            scaled[i] = (long)weights[i] * weights.Length;
        }

        return scaled;
    }

    // Vose's pairing: a column short of its share keeps what it has and takes the rest from one with
    // more, which pays for it and may then fall short itself. Every column starts as its own alias,
    // which a column never paired as the short one stays.
    private int[] PairColumns(long[] shares)
    {
        var aliases = new int[shares.Length];
        var small = new Stack<int>();
        var large = new Stack<int>();

        for (var i = 0; i < shares.Length; i++)
        {
            aliases[i] = i;
            ShelveByShare(i, shares, small, large);
        }

        while (small.Count > 0 && large.Count > 0)
        {
            var poor = small.Pop();
            var rich = large.Pop();
            aliases[poor] = rich;
            shares[rich] -= Total - shares[poor];
            ShelveByShare(rich, shares, small, large);
        }

        return aliases;
    }

    // A column short of its share waits to be paired; one at or over it waits to pay.
    private void ShelveByShare(int column, long[] shares, Stack<int> small, Stack<int> large)
    {
        if (shares[column] < Total)
        {
            small.Push(column);
            return;
        }

        large.Push(column);
    }

    // Each column's share once pairing ends is its threshold: at most Total, so it fits an int.
    private static ImmutableArray<int> ThresholdsOf(long[] shares)
    {
        var thresholds = new int[shares.Length];

        for (var i = 0; i < shares.Length; i++)
        {
            thresholds[i] = (int)shares[i];
        }

        return ImmutableCollectionsMarshal.AsImmutableArray(thresholds);
    }

    /// <summary>The index a column and a ticket within it land on.</summary>
    /// <param name="column">A column in [0, <see cref="Columns"/>), drawn uniformly by the caller.</param>
    /// <param name="ticket">A ticket in [0, <see cref="Total"/>), drawn uniformly by the caller.</param>
    /// <returns>The column itself when the ticket is below its threshold; otherwise its alias.</returns>
    public int IndexFor(int column, int ticket)
    {
        if (ticket < _thresholds[column])
        {
            return column;
        }

        return _aliases[column];
    }
}
