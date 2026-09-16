namespace DSAExperimentation.LeetCode.Harness;

/// <summary>
/// One strategy already paired with one already-built input: the thing a benchmark's
/// <c>[GlobalSetup]</c> resolves once and then calls inside its timed region.
/// </summary>
/// <remarks>
/// A named type rather than the bare <c>Func&lt;object?&gt;</c> it used to be, because a
/// stored callable is a collaborator - it is invoked later, by code that cannot see what
/// it closes over, and more than once per run. <see cref="IBoundWorkload.Run"/> answers
/// the three questions the delegate form had nowhere to write: it produces the strategy's
/// own answer (null for a void-shaped one) rather than null meaning "unbound", it may
/// throw whatever the strategy throws (resolution happened in Setup, so the body is the
/// strategy alone), and it repeats - every invocation is independent, which is what makes
/// it safe to time in a loop. The input it closes over is built once and read-only
/// thereafter, so a repeated <see cref="IBoundWorkload.Run"/> cannot measure construction
/// instead of the strategy.
/// </remarks>
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
