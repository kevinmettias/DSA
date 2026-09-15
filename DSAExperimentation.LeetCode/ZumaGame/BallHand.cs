namespace DSAExperimentation.LeetCode.ZumaGame;

// The balls still in hand when a Zuma move is made: the pool a solve spends from, in
// the order the strategy's own state threading keeps them. Its own type for the same
// reason BallBoard has one - so the two sides of a solve cannot be handed over the
// wrong way round without the compiler objecting.
internal readonly record struct BallHand(string Text);
