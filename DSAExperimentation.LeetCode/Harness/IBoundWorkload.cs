namespace DSAExperimentation.LeetCode.Harness;

// One strategy already paired with one already-built input: the thing a
// benchmark's [GlobalSetup] resolves once and then calls inside its timed region.
//
// A named type rather than the bare `Func<object?>` it used to be, because a
// stored callable is a collaborator - it is invoked later, by code that cannot
// see what it closes over, and more than once per run. Run() answers the three
// questions the delegate form had nowhere to write: it produces the strategy's
// own answer (null for a void-shaped one) rather than null meaning "unbound", it
// may throw whatever the strategy throws (resolution happened in Setup, so the
// body is the strategy alone), and it repeats - every invocation is independent,
// which is what makes it safe to time in a loop. The input it closes over is
// built once and read-only thereafter, so a repeated Run() cannot measure
// construction instead of the strategy.
public interface IBoundWorkload
{
    object? Run();
}
