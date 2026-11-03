using DSAExperimentation.LeetCode.Catalog.Sync.LeetCode;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Tests.LeetCode;

public sealed partial class StatementTextTests
{
    // The <pre> layout older questions use.
    private const string PreformattedExamples = """
        <pre>
        <strong>Input:</strong> nums = [2,7,11,15], target = 9
        <strong>Output:</strong> [0,1]
        <strong>Explanation:</strong> Because nums[0] + nums[1] == 9.
        </pre>
        <pre>
        <strong>Input:</strong> nums = [3,3], target = 6
        <strong>Output:</strong> [0,1]
        </pre>
        """;

    // The example-block layout newer questions use.
    private const string BlockExamples = """
        <div class="example-block">
        <p><strong>Input:</strong> <span class="example-io">s = "abc"</span></p>
        <p><strong>Output:</strong> <span class="example-io">&quot;cba&quot;</span></p>
        </div>
        """;

    // Next Greater Element II's layout: the explanation opens a line with no label of its own.
    private const string UnlabeledExplanation = """
        <pre>
        <strong>Input:</strong> nums = [1,2,1]
        <strong>Output:</strong> [2,-1,2]
        Explanation: The first 1's next greater number is 2;
        The number 2 can't find next greater number.
        </pre>
        """;

    // Combination Sum II's layout: a list of lists written over several lines.
    private const string MultilineOutput = """
        <pre>
        <strong>Input:</strong> candidates = [2,5,2,1,2], target = 5
        <strong>Output:</strong>
        [
        [1,2,2],
        [5]
        ]
        </pre>
        """;

    // Greatest Common Divisor of Strings' layout: zero-width spaces trail the empty string.
    private const string ZeroWidthSpaces = """
        <pre>
        <strong>Input:</strong> str1 = "LEET", str2 = "CODE"
        <strong>Output:</strong> ""&#8203;&#8203;&#8203;
        </pre>
        """;

    private const string ConstraintsList = """
        <p><strong>Constraints:</strong></p>
        <ul>
        	<li><code>2 &lt;= nums.length &lt;= 10<sup>4</sup></code></li>
        	<li><code>-10<sup>9</sup> &lt;= nums[i] &lt;=&nbsp;10<sup>9</sup></code></li>
        	<li><strong>Only one valid answer exists.</strong></li>
        </ul>
        """;

    [Fact]
    public void Outputs_PreformattedExamples_ReadsEachOutputLine()
        => Assert.Equal(["[0,1]", "[0,1]"], StatementText.Outputs(PreformattedExamples));

    [Fact]
    public void Outputs_ExampleBlocks_ReadsTheDecodedValue()
        => Assert.Equal(["\"cba\""], StatementText.Outputs(BlockExamples));

    [Fact]
    public void Inputs_PreformattedExamples_ReadsEachInputLine()
        => Assert.Equal(["nums = [2,7,11,15], target = 9", "nums = [3,3], target = 6"], StatementText.Inputs(PreformattedExamples));

    // Find the Largest Area of Square Inside Two Rectangles writes one label "Input:&nbsp;".
    [Fact]
    public void Inputs_LabelEndingInANoBreakSpace_ReadsTheInput()
        => Assert.Equal(["n = 3"], StatementText.Inputs("<p><strong>Input:&nbsp;</strong>n = 3</p>"));

    [Fact]
    public void Inputs_ExampleBlocks_ReadsTheDecodedValue()
        => Assert.Equal(["s = \"abc\""], StatementText.Inputs(BlockExamples));

    [Fact]
    public void Outputs_ExplanationWithoutALabel_EndsTheOutputBeforeIt()
        => Assert.Equal(["[2,-1,2]"], StatementText.Outputs(UnlabeledExplanation));

    [Fact]
    public void Outputs_OutputOverSeveralLines_ReadsItWhole()
        => Assert.Equal(["[ [1,2,2], [5] ]"], StatementText.Outputs(MultilineOutput));

    [Fact]
    public void Outputs_ZeroWidthSpaces_RemovesThem()
        => Assert.Equal(["\"\""], StatementText.Outputs(ZeroWidthSpaces));

    [Fact]
    public void Constraints_ListWithSuperscripts_WritesPowersWithACaret()
        => Assert.Equal(
            ["2 <= nums.length <= 10^4", "-10^9 <= nums[i] <= 10^9", "Only one valid answer exists."],
            StatementText.Constraints(ConstraintsList));

    [Fact]
    public void Constraints_StatementWithoutAConstraintsList_ReturnsNone()
        => Assert.Empty(StatementText.Constraints(PreformattedExamples));
}
