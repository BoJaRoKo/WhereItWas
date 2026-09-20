using WhereItWas.Core;

namespace WhereItWas.Tests;

public sealed class ExecuteTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", true)]
    [InlineData("   ", true)]
    [InlineData("\t", false)]
    [InlineData("\r\n", false)]
    [InlineData("a", true)]
    [InlineData("0", true)]
    [InlineData("false", true)]
    [InlineData(" abc ", false)]
    [InlineData("abc ", true)]
    [InlineData("SELECT 1", false)]
    [InlineData("\twartość\t", true)]
    [InlineData("ala/ma/kota", true)]
    [InlineData("/ala/ma/kota", true)]
    [InlineData("/ala/:ma/kota", true)]
    [InlineData("/:ala/:ma/kota", false)]
    public void SimpleVeryfication_ReturnsExpectedResult(string? query, bool expected)
    {
        var actual = Execute.SimpleVeryfication(query!);

        Assert.Equal(expected, actual);
    }
}
