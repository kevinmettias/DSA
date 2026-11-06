namespace DSAExperimentation.LeetCode.Catalog.Storage;

/// <summary>One recorded output corrected by hand: the case's inputs, the output it records, and why.</summary>
/// <param name="Inputs">The case's inputs in LeetCode notation, exactly as its source records them.</param>
/// <param name="ExpectedOutput">The output the case records, or null where none can be recorded.</param>
/// <param name="Reason">Why the source's output is replaced, and what was run to know the right one.</param>
internal sealed record CaseCorrection(IReadOnlyList<string> Inputs, string? ExpectedOutput, string Reason);
