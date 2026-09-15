namespace DSAExperimentation.LeetCode.FindPositiveIntegerSolutionForAGivenEquation;

// LC 1237's hidden CustomFunction, shaped the way LeetCode exposes it: an object with one
// operation rather than a raw callable. The formula is deliberately not part of this type
// - Evaluate names both coordinates and this comment is the one place the contract a
// strategy is allowed to lean on gets written down, which a bare Func<int, int, int>
// parameter had nowhere to put.
internal interface ICustomFunction
{
    // Evaluates the hidden f at (x, y): strictly increasing in both x and y, pure (the
    // same pair always yields the same value, so a strategy may probe a cell again), and
    // never throwing across the 1..1000 range this problem searches.
    int Evaluate(int x, int y);
}
