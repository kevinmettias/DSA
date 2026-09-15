namespace DSAExperimentation.LeetCode.ZumaGame;

// The row of colored balls a Zuma move inserts into, before collapse. It exists so
// the two strings a solve takes read as distinct roles at the call site - a board is
// inserted into, a hand is spent on it - rather than as a pair of interchangeable
// `string` positions whose order a caller can silently swap. Distinct in type from
// BallHand because the relation is one-directional: balls leave the hand and land in
// the board, never the reverse.
internal readonly record struct BallBoard(string Text);
