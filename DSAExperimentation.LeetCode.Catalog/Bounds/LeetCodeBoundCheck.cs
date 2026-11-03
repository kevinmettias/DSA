namespace DSAExperimentation.LeetCode.Catalog.Bounds;

// Three answers, not two: a subject the checker cannot read - a tree's Node.val, a string's
// letters - is neither inside nor outside the range, and folding it into either would either
// throw away good cases or wave through bad ones.
/// <summary>The verdict of judging one case against one <see cref="LeetCodeBound"/>.</summary>
public enum LeetCodeBoundCheck
{
    /// <summary>Every value the bound constrains was read and lies inside it.</summary>
    Satisfied,

    /// <summary>Some value the bound constrains lies outside it.</summary>
    Violated,

    /// <summary>Some subject of the bound could not be read from the case's inputs.</summary>
    NotJudged,
}
