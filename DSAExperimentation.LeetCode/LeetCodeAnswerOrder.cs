namespace DSAExperimentation.LeetCode;

// Whether LeetCode accepts an answer's elements in any order, which only a question's statement says.
// A question states it once, on LeetCodeQuestionAttribute, and every harness that judges an answer
// reads it from there - the explicit choice LeetCodeAnswers asks for, never a default inherited by
// saying nothing.
internal enum LeetCodeAnswerOrder
{
    // Element for element, as written: a number, a string, or a sequence whose order is part of the
    // answer.
    AsWritten,

    // "Return the answer in any order": the answer's elements may come back in any order, each still
    // compared as written - the rows-in-any-order shape of LeetCodeAnswers.IsRowSetEqual.
    AnyOrder,

    // Rows in their places, each row's own elements in any order: LC 1489's critical edges then its
    // pseudo-critical ones, each list in any order. The ordered-rows, unordered-contents shape
    // LeetCodeAnswers names beside IsRowSetEqual.
    EachInAnyOrder,

    // Any order at both depths: the rows in any order, and each row's own elements in any order too -
    // LC 49's anagram groups, and LC 1192's connections, each an unordered pair of servers.
    AnyOrderThroughout,
}
