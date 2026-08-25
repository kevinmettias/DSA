namespace DSAExperimentation.Algorithms.Graph.Engines.Walking;

// Purely an implementation detail of the shared walk engines below - the result
// type for a step that only runs for its side effects. Never appears in any public
// signature; IDepthFirstHooks/IBreadthFirstHooks stay void.
internal readonly record struct Unit;
