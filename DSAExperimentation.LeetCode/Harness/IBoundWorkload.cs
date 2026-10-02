namespace DSAExperimentation.LeetCode.Harness;

/// <summary>
/// One strategy already paired with one already-built input: the thing a benchmark's
/// <c>[GlobalSetup]</c> resolves once and then calls inside its timed region.
/// </summary>
public interface IBoundWorkload
{
    /// <summary>
    /// Runs the bound strategy once against the input this workload was built with.
    /// </summary>
    /// <returns>
    /// The strategy's own answer, or <see langword="null"/> when the strategy is
    /// void-shaped and produces none. Throws whatever the strategy throws.
    /// </returns>
    object? Run();
}
