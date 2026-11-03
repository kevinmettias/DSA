using System.Collections.Immutable;
using System.Runtime.InteropServices;
using DSAExperimentation.Algorithms.Searching;

namespace DSAExperimentation.Algorithms.Sampling;

// Drawing an index with probability proportional to its weight, by prefix sums: index i owns the
// tickets from the total of the weights before it up to the total through it, so a ticket drawn
// uniformly from [0, Total) lands on i with probability weight_i / Total, exactly. Finding the owner
// is a binary search for the first running total past the ticket, O(log n) per draw after O(n) to
// build. AliasSampler answers the same draw in O(1); which to use is a matter of how many draws a
// distribution serves (ARCHITECTURE §17.6: the distribution is the fact, the sampler the algorithm).
// The caller draws the ticket, so nothing here touches a random source. The running totals are fixed
// once built, so a copy of the sampler shares them unchanged; the search reads their array in place.
/// <summary>Draws an index with probability proportional to its weight, from a uniformly drawn ticket.</summary>
public readonly struct PrefixSumSampler
{
    private readonly ImmutableArray<int> _runningTotals;

    /// <summary>The number of tickets: the sum of the weights.</summary>
    public int Total => _runningTotals[^1];

    /// <summary>Builds the running totals of the weights.</summary>
    /// <param name="weights">Each index's weight, none negative, at least one positive.</param>
    /// <exception cref="ArgumentException"><paramref name="weights"/> has a negative weight or no positive one.</exception>
    public PrefixSumSampler(ReadOnlySpan<int> weights)
    {
        SamplingWeights.TotalOf(weights);
        var runningTotals = new int[weights.Length];
        var running = 0;

        for (var i = 0; i < weights.Length; i++)
        {
            running += weights[i];
            runningTotals[i] = running;
        }

        _runningTotals = ImmutableCollectionsMarshal.AsImmutableArray(runningTotals);
    }

    /// <summary>The index a ticket lands on.</summary>
    /// <param name="ticket">A ticket in [0, <see cref="Total"/>), drawn uniformly by the caller.</param>
    /// <returns>The index whose share of the tickets holds <paramref name="ticket"/>.</returns>
    public int IndexFor(int ticket)
    {
        // presumption: allow -- the constructor always builds the running totals, so they are never the
        // default ImmutableArray, whose array is null.
        var runningTotals = ImmutableCollectionsMarshal.AsArray(_runningTotals)!;

        return BinarySearch.UpperBound(runningTotals, ticket);
    }
}
