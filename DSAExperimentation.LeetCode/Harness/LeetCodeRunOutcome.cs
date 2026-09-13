namespace DSAExperimentation.LeetCode.Harness;

// What one (strategy, case) pair produced. Deliberately a REPORT rather than an
// assertion, the same choice LeetCodeSolutionValidator already made: the single
// xUnit harness turns this into an Assert, but a REPL or a future sync tool can
// read it without an xUnit dependency, and the rendered Expected/Actual text is
// produced here - where TOutput is still statically known - rather than in the
// harness, where it has already been erased to object.
internal sealed record LeetCodeRunOutcome
{
    public required bool Matched { get; init; }

    public required string Expected { get; init; }

    public required string Actual { get; init; }

    public string FailureReason => $"expected {Expected}, got {Actual}";
}
