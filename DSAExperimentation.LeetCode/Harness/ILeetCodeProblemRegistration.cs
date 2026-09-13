namespace DSAExperimentation.LeetCode.Harness;

// What a problem folder contributes to the two harnesses. One implementation per
// problem, sitting beside that problem's own solution class, so everything about
// a problem - its strategies, its examples, its benchmark workloads and its
// notion of a correct answer - is in the folder named after it.
//
// An interface discovered by reflection rather than an explicit registry list
// because the alternative, at this repo's scale, is an eleven-hundred-line file
// that every new problem has to remember to edit and that merges badly between
// concurrent sessions.
internal interface ILeetCodeProblemRegistration
{
    LeetCodeProblem Describe();
}
