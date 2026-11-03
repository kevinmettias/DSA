namespace DSAExperimentation.Algorithms.Sampling;

// The one rule every sampler's weights obey: none negative, and at least one positive, so there is a
// ticket to draw. Each sampler checks it the same way and reports the same failure.
internal static class SamplingWeights
{
    private const string NoWeightMessage = "At least one weight must be positive, and none negative.";

    // The weights' total, after checking them.
    public static int TotalOf(ReadOnlySpan<int> weights)
    {
        var total = 0;

        foreach (var weight in weights)
        {
            if (weight < 0)
            {
                throw new ArgumentException(NoWeightMessage, nameof(weights));
            }

            total += weight;
        }

        if (total == 0)
        {
            throw new ArgumentException(NoWeightMessage, nameof(weights));
        }

        return total;
    }
}
