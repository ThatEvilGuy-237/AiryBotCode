using AiryBotCode.Api.Controllers;
using Xunit;

namespace AiryBotCode.Tests;

public class DbExplorerScopeTests
{
    [Theory]
    [InlineData(null, true)]
    [InlineData("", true)]
    [InlineData("airy_db", true)]
    [InlineData("AIRY_DB", false)]
    [InlineData("evil_users", false)]
    [InlineData("evil_users_dev", false)]
    [InlineData("evil_dash_db", false)]
    [InlineData("hive_directory", false)]
    [InlineData("postgres", false)]
    public void OnlyTheOwnDatabase(string requested, bool allowed) =>
        Assert.Equal(allowed, DbExplorerScope.AllowsDatabase("airy_db", requested));

    [Theory]
    [InlineData("public", true)]
    [InlineData("Public", false)]
    [InlineData("evil", false)]
    [InlineData("information_schema", false)]
    [InlineData("pg_catalog", false)]
    [InlineData(null, false)]
    public void OnlyThePublicSchema(string requested, bool allowed) =>
        Assert.Equal(allowed, DbExplorerScope.AllowsSchema(requested));
}
