namespace DSAExperimentation.Algorithms.Walking;

// The state type of a reduce that carries nothing - one run only for what the walk
// itself does, as ConnectedComponents' reduce runs only to mark a component's nodes in
// its visited set. Never appears in any public signature.
internal readonly record struct Unit;
