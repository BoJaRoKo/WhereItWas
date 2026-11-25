using WhereItWas.Core;

namespace WhereItWas.Tests;

public sealed class ExecuteTests
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", true)]
    [InlineData("   ", true)]
    [InlineData("\t", true)]
    [InlineData("\r\n", true)]
    [InlineData("a", true)]
    [InlineData("0", true)]
    [InlineData("false", true)]
    [InlineData(" abc ", true)]
    [InlineData("abc ", true)]
    [InlineData("SELECT 1", true)]
    [InlineData("\twartość\t", true)]
    [InlineData("ala/ma/kota", true)]
    [InlineData("/ala/ma/kota", true)]
    [InlineData("/ala/:ma/kota", true)]
    [InlineData("/:ala/:ma/kota", false)]
    public void GetQueries_ValidatesQuery(string? query, bool expected)
    {
        if (expected)
        {
            var queries = Execute.GetQueries(query!);
            Assert.Equal(Execute.QueryNames().Select(info => info.Id),
                queries.Select(definition => definition.Id));
            Assert.All(queries, definition =>
                Assert.False(string.IsNullOrWhiteSpace(definition.Sql)));
        }
        else
        {
            Assert.ThrowsAny<ArgumentException>(() => Execute.GetQueries(query!));
        }
    }
}
