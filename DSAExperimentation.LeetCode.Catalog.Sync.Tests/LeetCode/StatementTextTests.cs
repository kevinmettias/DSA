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
    public void Constraints_ListWithSuperscripts_WritesPowersWithACaret()
        => Assert.Equal(
            ["2 <= nums.length <= 10^4", "-10^9 <= nums[i] <= 10^9", "Only one valid answer exists."],
            StatementText.Constraints(ConstraintsList));

    [Fact]
    public void Constraints_StatementWithoutAConstraintsList_ReturnsNone()
        => Assert.Empty(StatementText.Constraints(PreformattedExamples));
}
