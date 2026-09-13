namespace DSAExperimentation.LeetCode.OddEvenJump;

// Which of LeetCode 975's two jump rules a target lookup is resolving. The parity of
// the jump number is the whole difference between them: an odd-numbered jump lands on
// the smallest later value >= the current one, an even-numbered jump on the largest
// later value <= it. Both break ties toward the smaller index.
internal enum JumpDirection
{
    // Odd jumps: search upward, for the smallest qualifying value.
    Odd,

    // Even jumps: search downward, for the largest qualifying value.
    Even,
}
