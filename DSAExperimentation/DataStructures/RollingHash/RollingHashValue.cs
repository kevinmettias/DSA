namespace DSAExperimentation.DataStructures.RollingHash;

// The combined result of RollingHash's two independent lanes, wrapped as one
// value so a caller can only ever compare both components together via a single
// Equals/== call - never silently drop to one lane's much weaker collision
// resistance by comparing e.g. First alone, the way a bare (long,long) tuple
// would let happen unnoticed.
//
// Equality here means "probably equal," not "definitely equal": two genuinely
// equal substrings always produce equal RollingHashValues, but two genuinely
// different substrings can - with the vanishingly small joint probability
// RollingHash.cs's doc comment derives - also produce equal RollingHashValues.
// A caller needing certainty re-slices and compares the original text directly
// instead; RollingHashSearch.FindAll is the worked example of doing exactly
// that before reporting a match.
internal readonly record struct RollingHashValue(long First, long Second);
