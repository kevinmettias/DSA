namespace DSAExperimentation.LeetCode.WordLadder;

// The word a ladder has to reach. Its own type for the same reason BeginWord has one,
// and it is named for what it is at the call site rather than for a position among two
// interchangeable strings - the chain is only ever read in one direction, and the
// "is the end word even in the dictionary" guard on it is not a property the begin word
// shares.
internal readonly record struct EndWord(string Text);
