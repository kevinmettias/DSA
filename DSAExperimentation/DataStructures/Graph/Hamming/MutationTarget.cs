namespace DSAExperimentation.DataStructures.Graph.Hamming;

// The word a mutation search is trying TO REACH - MutationDistance's second
// position, and the one every intermediate step has to stay inside `allowed` to
// get to. Distinct from MutationStart so a caller cannot hand the two endpoints
// over the wrong way round, which the walk answers differently (often with null).
internal readonly record struct MutationTarget(string Text);
