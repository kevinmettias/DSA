namespace DSAExperimentation.Tests.LeetCodeCoverage.DecodeString;

// One LeetCode example: the encoded input and the expanded string its judge output
// shows for it. The two values are named fields rather than two adjacent `string`
// parameters, so a row is written `new DecodeCase(Encoded: ..., Expected: ...)` and
// an encoded/expected swap has to be typed out by name instead of falling out of a
// position the compiler would have accepted either way.
public readonly record struct DecodeCase(string Encoded, string Expected);
