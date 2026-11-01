namespace DSAExperimentation.DataStructures.Graph.Hamming;

// The word a mutation search walks FROM - MutationDistance's first position. Its
// own type rather than a bare string because the other endpoint is a string too,
// and the two ends are not interchangeable: the start may be awaited even when
// `allowed` omits it, while the target has to be reachable through the set.
/// <summary>The string a mutation search starts from.</summary>
/// <param name="Text">The starting string, which need not be among the allowed strings.</param>
public readonly record struct MutationStart(string Text);
