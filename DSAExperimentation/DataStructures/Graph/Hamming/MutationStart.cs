namespace DSAExperimentation.DataStructures.Graph.Hamming;

// The word a mutation search walks FROM - MutationDistance's first position. Its
// own type rather than a bare string because the other endpoint is a string too,
// and the two ends are not interchangeable: the start may be awaited even when
// `allowed` omits it, while the target has to be reachable through the set.
internal readonly record struct MutationStart(string Text);
